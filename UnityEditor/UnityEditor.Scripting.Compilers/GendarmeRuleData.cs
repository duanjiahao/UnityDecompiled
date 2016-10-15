using System;

namespace UnityEditor.Scripting.Compilers
{
	internal class GendarmeRuleData
	{
		public int LastIndex;

		public int Line;

		public string File = string.Empty;

		public string Problem;

		public string Details;

		public string Severity;

		public string Source;

		public string Location;

		public string Target;

		public bool IsAssemblyError;
	}
}
