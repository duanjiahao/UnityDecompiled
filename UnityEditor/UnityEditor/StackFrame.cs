using System;

namespace UnityEditor
{
	internal struct StackFrame
	{
		public uint lineNumber;

		public string sourceFile;

		public string methodName;

		public string signature;

		public string moduleName;
	}
}
