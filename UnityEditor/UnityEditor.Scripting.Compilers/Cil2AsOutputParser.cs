using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnityEditor.Scripting.Compilers
{
	internal class Cil2AsOutputParser : UnityScriptCompilerOutputParser
	{
		[DebuggerHidden]
		public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
		{
			Cil2AsOutputParser.<Parse>c__Iterator0 <Parse>c__Iterator = new Cil2AsOutputParser.<Parse>c__Iterator0();
			<Parse>c__Iterator.errorOutput = errorOutput;
			Cil2AsOutputParser.<Parse>c__Iterator0 expr_0E = <Parse>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private static CompilerMessage CompilerErrorFor(StringBuilder currentErrorBuffer)
		{
			return new CompilerMessage
			{
				type = CompilerMessageType.Error,
				message = currentErrorBuffer.ToString()
			};
		}
	}
}
