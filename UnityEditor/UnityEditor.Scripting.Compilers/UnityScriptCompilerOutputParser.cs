using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace UnityEditor.Scripting.Compilers
{
	internal class UnityScriptCompilerOutputParser : CompilerOutputParserBase
	{
		private static Regex sCompilerOutput = new Regex("\\s*(?<filename>.*)\\((?<line>\\d+),(?<column>\\d+)\\):\\s*[BU]C(?<type>W|E)(?<id>[^:]*):\\s*(?<message>.*)", RegexOptions.ExplicitCapture);

		private static Regex sUnknownTypeOrNamespace = new Regex("[^']*'(?<type_name>[^']+)'.*", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		[CompilerGenerated]
		private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<Match, Regex, NormalizedCompilerStatus> <>f__mg$cache1;

		protected override string GetErrorIdentifier()
		{
			return "E";
		}

		protected override Regex GetOutputRegex()
		{
			return UnityScriptCompilerOutputParser.sCompilerOutput;
		}

		protected override NormalizedCompilerStatus NormalizedStatusFor(Match match)
		{
			string arg_29_1 = "0018";
			Regex arg_29_2 = UnityScriptCompilerOutputParser.sUnknownTypeOrNamespace;
			if (UnityScriptCompilerOutputParser.<>f__mg$cache0 == null)
			{
				UnityScriptCompilerOutputParser.<>f__mg$cache0 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
			}
			NormalizedCompilerStatus normalizedCompilerStatus = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, arg_29_1, arg_29_2, UnityScriptCompilerOutputParser.<>f__mg$cache0);
			NormalizedCompilerStatus result;
			if (normalizedCompilerStatus.code != NormalizedCompilerStatusCode.NotNormalized)
			{
				result = normalizedCompilerStatus;
			}
			else
			{
				string arg_6A_1 = "0005";
				Regex arg_6A_2 = UnityScriptCompilerOutputParser.sUnknownTypeOrNamespace;
				if (UnityScriptCompilerOutputParser.<>f__mg$cache1 == null)
				{
					UnityScriptCompilerOutputParser.<>f__mg$cache1 = new Func<Match, Regex, NormalizedCompilerStatus>(CompilerOutputParserBase.NormalizeSimpleUnknownTypeOfNamespaceError);
				}
				result = CompilerOutputParserBase.TryNormalizeCompilerStatus(match, arg_6A_1, arg_6A_2, UnityScriptCompilerOutputParser.<>f__mg$cache1);
			}
			return result;
		}
	}
}
