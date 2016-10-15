using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityEditor.Scripting.Compilers
{
	internal abstract class CompilerOutputParserBase
	{
		protected static CompilerMessage CreateInternalCompilerErrorMessage(string[] compileroutput)
		{
			CompilerMessage result;
			result.file = string.Empty;
			result.message = string.Join("\n", compileroutput);
			result.type = CompilerMessageType.Error;
			result.line = 0;
			result.column = 0;
			result.normalizedStatus = default(NormalizedCompilerStatus);
			return result;
		}

		protected internal static CompilerMessage CreateCompilerMessageFromMatchedRegex(string line, Match m, string erroridentifier)
		{
			CompilerMessage result;
			result.file = m.Groups["filename"].Value;
			result.message = line;
			result.line = int.Parse(m.Groups["line"].Value);
			result.column = int.Parse(m.Groups["column"].Value);
			result.type = ((!(m.Groups["type"].Value == erroridentifier)) ? CompilerMessageType.Warning : CompilerMessageType.Error);
			result.normalizedStatus = default(NormalizedCompilerStatus);
			return result;
		}

		public virtual IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
		{
			return this.Parse(errorOutput, new string[0], compilationHadFailure);
		}

		public virtual IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			bool flag = false;
			List<CompilerMessage> list = new List<CompilerMessage>();
			Regex outputRegex = this.GetOutputRegex();
			for (int i = 0; i < errorOutput.Length; i++)
			{
				string text = errorOutput[i];
				string input = (text.Length <= 1000) ? text : text.Substring(0, 100);
				Match match = outputRegex.Match(input);
				if (match.Success)
				{
					CompilerMessage item = CompilerOutputParserBase.CreateCompilerMessageFromMatchedRegex(text, match, this.GetErrorIdentifier());
					item.normalizedStatus = this.NormalizedStatusFor(match);
					if (item.type == CompilerMessageType.Error)
					{
						flag = true;
					}
					list.Add(item);
				}
			}
			if (compilationHadFailure && !flag)
			{
				list.Add(CompilerOutputParserBase.CreateInternalCompilerErrorMessage(errorOutput));
			}
			return list;
		}

		protected virtual NormalizedCompilerStatus NormalizedStatusFor(Match match)
		{
			return default(NormalizedCompilerStatus);
		}

		protected abstract string GetErrorIdentifier();

		protected abstract Regex GetOutputRegex();

		protected static NormalizedCompilerStatus TryNormalizeCompilerStatus(Match match, string memberNotFoundError, Regex missingMemberRegex)
		{
			string value = match.Groups["id"].Value;
			NormalizedCompilerStatus result = default(NormalizedCompilerStatus);
			if (value != memberNotFoundError)
			{
				return result;
			}
			result.code = NormalizedCompilerStatusCode.MemberNotFound;
			Match match2 = missingMemberRegex.Match(match.Groups["message"].Value);
			result.details = match2.Groups["type_name"].Value + "%" + match2.Groups["member_name"].Value;
			return result;
		}
	}
}
