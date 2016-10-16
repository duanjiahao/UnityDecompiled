using System;
using System.Text.RegularExpressions;

namespace UnityEditor.Scripting.Compilers
{
	internal class MicrosoftCSharpCompilerOutputParser : CompilerOutputParserBase
	{
		private static Regex sCompilerOutput = new Regex("\\s*(?<filename>.*)\\((?<line>\\d+),(?<column>\\d+)\\):\\s*(?<type>warning|error)\\s*(?<id>[^:]*):\\s*(?<message>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

		protected override Regex GetOutputRegex()
		{
			return MicrosoftCSharpCompilerOutputParser.sCompilerOutput;
		}

		protected override string GetErrorIdentifier()
		{
			return "error";
		}
	}
}
