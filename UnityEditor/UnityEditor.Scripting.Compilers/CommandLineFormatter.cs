using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal static class CommandLineFormatter
	{
		private static readonly Regex UnsafeCharsWindows = new Regex("[^A-Za-z0-9\\_\\-\\.\\:\\,\\/\\@\\\\]");

		private static readonly Regex UnescapeableChars = new Regex("[\\x00-\\x08\\x10-\\x1a\\x1c-\\x1f\\x7f\\xff]");

		private static readonly Regex Quotes = new Regex("\"");

		public static string EscapeCharsQuote(string input)
		{
			string result;
			if (input.IndexOf('\'') == -1)
			{
				result = "'" + input + "'";
			}
			else if (input.IndexOf('"') == -1)
			{
				result = "\"" + input + "\"";
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static string PrepareFileName(string input)
		{
			string result;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = CommandLineFormatter.EscapeCharsQuote(input);
			}
			else
			{
				result = CommandLineFormatter.EscapeCharsWindows(input);
			}
			return result;
		}

		public static string EscapeCharsWindows(string input)
		{
			string result;
			if (input.Length == 0)
			{
				result = "\"\"";
			}
			else if (CommandLineFormatter.UnescapeableChars.IsMatch(input))
			{
				Debug.LogWarning("Cannot escape control characters in string");
				result = "\"\"";
			}
			else if (CommandLineFormatter.UnsafeCharsWindows.IsMatch(input))
			{
				result = "\"" + CommandLineFormatter.Quotes.Replace(input, "\"\"") + "\"";
			}
			else
			{
				result = input;
			}
			return result;
		}

		internal static string GenerateResponseFile(IEnumerable<string> arguments)
		{
			string uniqueTempPathInProject = FileUtil.GetUniqueTempPathInProject();
			using (StreamWriter streamWriter = new StreamWriter(uniqueTempPathInProject))
			{
				foreach (string current in from a in arguments
				where a != null
				select a)
				{
					streamWriter.WriteLine(current);
				}
			}
			return uniqueTempPathInProject;
		}
	}
}
