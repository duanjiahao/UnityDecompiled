using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnityEditor.Scripting.Compilers
{
	internal class GendarmeOutputParser : UnityScriptCompilerOutputParser
	{
		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
		{
			throw new ArgumentException("Gendarme Output Parser needs standard out");
		}

		[DebuggerHidden]
		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			GendarmeOutputParser.<Parse>c__IteratorB <Parse>c__IteratorB = new GendarmeOutputParser.<Parse>c__IteratorB();
			<Parse>c__IteratorB.standardOutput = standardOutput;
			<Parse>c__IteratorB.<$>standardOutput = standardOutput;
			GendarmeOutputParser.<Parse>c__IteratorB expr_15 = <Parse>c__IteratorB;
			expr_15.$PC = -2;
			return expr_15;
		}

		private static CompilerMessage CompilerErrorFor(GendarmeRuleData gendarmeRuleData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(gendarmeRuleData.Problem);
			stringBuilder.AppendLine(gendarmeRuleData.Details);
			stringBuilder.AppendLine(string.IsNullOrEmpty(gendarmeRuleData.Location) ? string.Format("{0} at line : {1}", gendarmeRuleData.Source, gendarmeRuleData.Line) : gendarmeRuleData.Location);
			string message = stringBuilder.ToString();
			return new CompilerMessage
			{
				type = CompilerMessageType.Error,
				message = message,
				file = gendarmeRuleData.File,
				line = gendarmeRuleData.Line,
				column = 1
			};
		}

		private static GendarmeRuleData GetGendarmeRuleDataFor(IList<string> output, int index)
		{
			GendarmeRuleData gendarmeRuleData = new GendarmeRuleData();
			for (int i = index; i < output.Count; i++)
			{
				string text = output[i];
				if (text.StartsWith("Problem:"))
				{
					gendarmeRuleData.Problem = text.Substring(text.LastIndexOf("Problem:", StringComparison.Ordinal) + "Problem:".Length);
				}
				else if (text.StartsWith("* Details"))
				{
					gendarmeRuleData.Details = text;
				}
				else if (text.StartsWith("* Source"))
				{
					gendarmeRuleData.IsAssemblyError = false;
					gendarmeRuleData.Source = text;
					gendarmeRuleData.Line = GendarmeOutputParser.GetLineNumberFrom(text);
					gendarmeRuleData.File = GendarmeOutputParser.GetFileNameFrome(text);
				}
				else if (text.StartsWith("* Severity"))
				{
					gendarmeRuleData.Severity = text;
				}
				else if (text.StartsWith("* Location"))
				{
					gendarmeRuleData.IsAssemblyError = true;
					gendarmeRuleData.Location = text;
				}
				else
				{
					if (!text.StartsWith("* Target"))
					{
						gendarmeRuleData.LastIndex = i;
						break;
					}
					gendarmeRuleData.Target = text;
				}
			}
			return gendarmeRuleData;
		}

		private static string GetFileNameFrome(string currentLine)
		{
			int num = currentLine.LastIndexOf("* Source:") + "* Source:".Length;
			int num2 = currentLine.IndexOf("(");
			if (num != -1 && num2 != -1)
			{
				return currentLine.Substring(num, num2 - num).Trim();
			}
			return string.Empty;
		}

		private static int GetLineNumberFrom(string currentLine)
		{
			int num = currentLine.IndexOf("(") + 2;
			int num2 = currentLine.IndexOf(")");
			if (num != -1 && num2 != -1)
			{
				return int.Parse(currentLine.Substring(num, num2 - num));
			}
			return 0;
		}
	}
}
