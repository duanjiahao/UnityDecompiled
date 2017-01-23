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
			result.file = "";
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
			Regex internalErrorOutputRegex = this.GetInternalErrorOutputRegex();
			int i = 0;
			while (i < errorOutput.Length)
			{
				string text = errorOutput[i];
				string input = (text.Length <= 1000) ? text : text.Substring(0, 100);
				Match match = outputRegex.Match(input);
				if (match.Success)
				{
					goto IL_88;
				}
				if (internalErrorOutputRegex != null)
				{
					match = internalErrorOutputRegex.Match(input);
				}
				if (match.Success)
				{
					goto IL_88;
				}
				IL_BF:
				i++;
				continue;
				IL_88:
				CompilerMessage item = CompilerOutputParserBase.CreateCompilerMessageFromMatchedRegex(text, match, this.GetErrorIdentifier());
				item.normalizedStatus = this.NormalizedStatusFor(match);
				if (item.type == CompilerMessageType.Error)
				{
					flag = true;
				}
				list.Add(item);
				goto IL_BF;
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

		protected virtual Regex GetInternalErrorOutputRegex()
		{
			return null;
		}

		protected static NormalizedCompilerStatus TryNormalizeCompilerStatus(Match match, string idToCheck, Regex messageParser, Func<string, Regex, NormalizedCompilerStatus> normalizer)
		{
			string value = match.Groups["id"].Value;
			NormalizedCompilerStatus normalizedCompilerStatus = default(NormalizedCompilerStatus);
			NormalizedCompilerStatus result;
			if (value != idToCheck)
			{
				result = normalizedCompilerStatus;
			}
			else
			{
				result = normalizer(match.Groups["message"].Value, messageParser);
			}
			return result;
		}

		protected static NormalizedCompilerStatus NormalizeMemberNotFoundError(string errorMsg, Regex messageParser)
		{
			NormalizedCompilerStatus result;
			result.code = NormalizedCompilerStatusCode.MemberNotFound;
			Match match = messageParser.Match(errorMsg);
			result.details = match.Groups["type_name"].Value + "%" + match.Groups["member_name"].Value;
			return result;
		}

		protected static NormalizedCompilerStatus NormalizeSimpleUnknownTypeOfNamespaceError(string errorMsg, Regex messageParser)
		{
			NormalizedCompilerStatus result;
			result.code = NormalizedCompilerStatusCode.UnknownTypeOrNamespace;
			Match match = messageParser.Match(errorMsg);
			result.details = match.Groups["type_name"].Value;
			return result;
		}

		protected static NormalizedCompilerStatus NormalizeUnknownTypeMemberOfNamespaceError(string errorMsg, Regex messageParser)
		{
			NormalizedCompilerStatus result;
			result.code = NormalizedCompilerStatusCode.UnknownTypeOrNamespace;
			Match match = messageParser.Match(errorMsg);
			result.details = match.Groups["namespace"].Value + "." + match.Groups["type_name"].Value;
			return result;
		}
	}
}
