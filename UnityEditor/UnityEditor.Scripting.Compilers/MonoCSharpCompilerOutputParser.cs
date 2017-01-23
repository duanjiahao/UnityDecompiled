using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace UnityEditor.Scripting.Compilers
{
	internal class MonoCSharpCompilerOutputParser : CompilerOutputParserBase
	{
		private static Regex sCompilerOutput = new Regex("\\s*(?<filename>.*)\\((?<line>\\d+),(?<column>\\d+)\\):\\s*(?<type>warning|error)\\s*(?<id>[^:]*):\\s*(?<message>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private static Regex sInternalErrorCompilerOutput = new Regex("\\s*(?<message>Internal compiler (?<type>error)) at\\s*(?<filename>.*)\\((?<line>\\d+),(?<column>\\d+)\\):\\s*(?<id>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private static Regex sMissingMember = new Regex("[^`]*`(?<type_name>[^']+)'[^`]+`(?<member_name>[^']+)'", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private static Regex sMissingType = new Regex("[^`]*`(?<type_name>[^']+)'[^`]+`(?<namespace>[^']+)'", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		private static Regex sUnknownTypeOrNamespace = new Regex("[^`]*`(?<type_name>[^']+)'.*", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		[CompilerGenerated]
		private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<string, Regex, NormalizedCompilerStatus> <>f__mg$cache2;

		protected override Regex GetOutputRegex()
		{
			return MonoCSharpCompilerOutputParser.sCompilerOutput;
		}

		protected override Regex GetInternalErrorOutputRegex()
		{
			return MonoCSharpCompilerOutputParser.sInternalErrorCompilerOutput;
		}

		protected override string GetErrorIdentifier()
		{
			return "error";
		}

		protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
		{
			string arg_29_1 = "CS0117";
			Regex arg_29_2 = MonoCSharpCompilerOutputParser.sMissingMember;
			if (MonoCSharpCompilerOutputParser.<>f__mg$cache0 == null)
			{
				MonoCSharpCompilerOutputParser.<>f__mg$cache0 = new Func<string, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeMemberNotFoundError);
			}
			NormalizedCompilerStatus normalizedCompilerStatus = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, arg_29_1, arg_29_2, MonoCSharpCompilerOutputParser.<>f__mg$cache0);
			NormalizedCompilerStatus result;
			if (normalizedCompilerStatus.code != NormalizedCompilerStatusCode.NotNormalized)
			{
				result = normalizedCompilerStatus;
			}
			else
			{
				string arg_6A_1 = "CS0246";
				Regex arg_6A_2 = MonoCSharpCompilerOutputParser.sUnknownTypeOrNamespace;
				if (MonoCSharpCompilerOutputParser.<>f__mg$cache1 == null)
				{
					MonoCSharpCompilerOutputParser.<>f__mg$cache1 = new Func<string, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
				}
				normalizedCompilerStatus = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, arg_6A_1, arg_6A_2, MonoCSharpCompilerOutputParser.<>f__mg$cache1);
				if (normalizedCompilerStatus.code != NormalizedCompilerStatusCode.NotNormalized)
				{
					result = normalizedCompilerStatus;
				}
				else
				{
					string arg_AB_1 = "CS0234";
					Regex arg_AB_2 = MonoCSharpCompilerOutputParser.sMissingType;
					if (MonoCSharpCompilerOutputParser.<>f__mg$cache2 == null)
					{
						MonoCSharpCompilerOutputParser.<>f__mg$cache2 = new Func<string, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeUnknownTypeMemberOfNamespaceError);
					}
					result = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, arg_AB_1, arg_AB_2, MonoCSharpCompilerOutputParser.<>f__mg$cache2);
				}
			}
			return result;
		}
	}
}
