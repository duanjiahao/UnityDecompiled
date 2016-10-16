using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEditor.Scripting.Compilers
{
	internal class Il2CppOutputParser : CompilerOutputParserBase
	{
		private const string _errorIdentifier = "IL2CPP error";

		private static readonly Regex sErrorRegexWithSourceInformation = new Regex("\\s*(?<message>.*) in (?<filename>.*):(?<line>\\d+)");

		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			List<CompilerMessage> list = new List<CompilerMessage>();
			for (int i = 0; i < standardOutput.Length; i++)
			{
				string text = standardOutput[i];
				if (text.StartsWith("IL2CPP error"))
				{
					string text2 = string.Empty;
					int num = 0;
					StringBuilder stringBuilder = new StringBuilder();
					Match match = Il2CppOutputParser.sErrorRegexWithSourceInformation.Match(text);
					if (match.Success)
					{
						text2 = match.Groups["filename"].Value;
						num = int.Parse(match.Groups["line"].Value);
						stringBuilder.AppendFormat("{0} in {1}:{2}", match.Groups["message"].Value, Path.GetFileName(text2), num);
					}
					else
					{
						stringBuilder.Append(text);
					}
					if (i + 1 < standardOutput.Length && standardOutput[i + 1].StartsWith("Additional information:"))
					{
						stringBuilder.AppendFormat("{0}{1}", Environment.NewLine, standardOutput[i + 1]);
						i++;
					}
					list.Add(new CompilerMessage
					{
						file = text2,
						line = num,
						message = stringBuilder.ToString(),
						type = CompilerMessageType.Error
					});
				}
			}
			return list;
		}

		protected override string GetErrorIdentifier()
		{
			return "IL2CPP error";
		}

		protected override Regex GetOutputRegex()
		{
			return Il2CppOutputParser.sErrorRegexWithSourceInformation;
		}
	}
}
