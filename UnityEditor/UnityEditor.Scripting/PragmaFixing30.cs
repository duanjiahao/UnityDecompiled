using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class PragmaFixing30
	{
		private static void FixJavaScriptPragmas()
		{
			string[] array = PragmaFixing30.CollectBadFiles();
			if (array.Length == 0)
			{
				return;
			}
			if (!InternalEditorUtility.inBatchMode)
			{
				PragmaFixingWindow.ShowWindow(array);
			}
			else
			{
				PragmaFixing30.FixFiles(array);
			}
		}

		public static void FixFiles(string[] filesToFix)
		{
			for (int i = 0; i < filesToFix.Length; i++)
			{
				string text = filesToFix[i];
				try
				{
					PragmaFixing30.FixPragmasInFile(text);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Failed to fix pragmas in file '" + text + "'.\n" + ex.Message);
				}
			}
		}

		private static bool FileNeedsPragmaFixing(string fileName)
		{
			return PragmaFixing30.CheckOrFixPragmas(fileName, true);
		}

		private static void FixPragmasInFile(string fileName)
		{
			PragmaFixing30.CheckOrFixPragmas(fileName, false);
		}

		private static bool CheckOrFixPragmas(string fileName, bool onlyCheck)
		{
			string text = File.ReadAllText(fileName);
			StringBuilder sb = new StringBuilder(text);
			PragmaFixing30.LooseComments(sb);
			Match match = PragmaFixing30.PragmaMatch(sb, "strict");
			if (!match.Success)
			{
				return false;
			}
			bool success = PragmaFixing30.PragmaMatch(sb, "downcast").Success;
			bool success2 = PragmaFixing30.PragmaMatch(sb, "implicit").Success;
			if (success && success2)
			{
				return false;
			}
			if (!onlyCheck)
			{
				PragmaFixing30.DoFixPragmasInFile(fileName, text, match.Index + match.Length, success, success2);
			}
			return true;
		}

		private static void DoFixPragmasInFile(string fileName, string oldText, int fixPos, bool hasDowncast, bool hasImplicit)
		{
			string text = string.Empty;
			string str = (!PragmaFixing30.HasWinLineEndings(oldText)) ? "\n" : "\r\n";
			if (!hasImplicit)
			{
				text = text + str + "#pragma implicit";
			}
			if (!hasDowncast)
			{
				text = text + str + "#pragma downcast";
			}
			File.WriteAllText(fileName, oldText.Insert(fixPos, text));
		}

		private static bool HasWinLineEndings(string text)
		{
			return text.IndexOf("\r\n") != -1;
		}

		[DebuggerHidden]
		private static IEnumerable<string> SearchRecursive(string dir, string mask)
		{
			PragmaFixing30.<SearchRecursive>c__Iterator9 <SearchRecursive>c__Iterator = new PragmaFixing30.<SearchRecursive>c__Iterator9();
			<SearchRecursive>c__Iterator.dir = dir;
			<SearchRecursive>c__Iterator.mask = mask;
			<SearchRecursive>c__Iterator.<$>dir = dir;
			<SearchRecursive>c__Iterator.<$>mask = mask;
			PragmaFixing30.<SearchRecursive>c__Iterator9 expr_23 = <SearchRecursive>c__Iterator;
			expr_23.$PC = -2;
			return expr_23;
		}

		private static void LooseComments(StringBuilder sb)
		{
			Regex regex = new Regex("//");
			foreach (Match match in regex.Matches(sb.ToString()))
			{
				int index = match.Index;
				while (index < sb.Length && sb[index] != '\n' && sb[index] != '\r')
				{
					sb[index++] = ' ';
				}
			}
		}

		private static Match PragmaMatch(StringBuilder sb, string pragma)
		{
			return new Regex("#\\s*pragma\\s*" + pragma).Match(sb.ToString());
		}

		private static string[] CollectBadFiles()
		{
			List<string> list = new List<string>();
			foreach (string current in PragmaFixing30.SearchRecursive(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "*.js"))
			{
				try
				{
					if (PragmaFixing30.FileNeedsPragmaFixing(current))
					{
						list.Add(current);
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Failed to fix pragmas in file '" + current + "'.\n" + ex.Message);
				}
			}
			return list.ToArray();
		}
	}
}
