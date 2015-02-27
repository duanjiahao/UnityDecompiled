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
			Cil2AsOutputParser.<Parse>c__Iterator5 <Parse>c__Iterator = new Cil2AsOutputParser.<Parse>c__Iterator5();
			<Parse>c__Iterator.errorOutput = errorOutput;
			<Parse>c__Iterator.<$>errorOutput = errorOutput;
			Cil2AsOutputParser.<Parse>c__Iterator5 expr_15 = <Parse>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
		private static CompilerMessage CompilerErrorFor(StringBuilder currentErrorBuffer)
		{
			return new CompilerMessage
			{
				message = currentErrorBuffer.ToString()
			};
		}
	}
}
