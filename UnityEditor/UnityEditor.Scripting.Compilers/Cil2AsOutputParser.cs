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
			Cil2AsOutputParser.<Parse>c__IteratorA <Parse>c__IteratorA = new Cil2AsOutputParser.<Parse>c__IteratorA();
			<Parse>c__IteratorA.errorOutput = errorOutput;
			<Parse>c__IteratorA.<$>errorOutput = errorOutput;
			Cil2AsOutputParser.<Parse>c__IteratorA expr_15 = <Parse>c__IteratorA;
			expr_15.$PC = -2;
			return expr_15;
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
