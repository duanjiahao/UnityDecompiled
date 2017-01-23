using System;

namespace UnityEditor.Scripting.Compilers
{
	internal class GendarmeRuleData
	{
		public int LastIndex = 0;

		public int Line = 0;

		public string File = "";

		public string Problem;

		public string Details;

		public string Severity;

		public string Source;

		public string Location;

		public string Target;

		public bool IsAssemblyError;
	}
}
