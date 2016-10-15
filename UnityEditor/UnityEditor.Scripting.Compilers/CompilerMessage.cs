using System;

namespace UnityEditor.Scripting.Compilers
{
	internal struct CompilerMessage
	{
		public string message;

		public string file;

		public int line;

		public int column;

		public CompilerMessageType type;

		public NormalizedCompilerStatus normalizedStatus;
	}
}
