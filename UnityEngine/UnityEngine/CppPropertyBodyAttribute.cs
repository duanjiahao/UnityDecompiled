using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class CppPropertyBodyAttribute : Attribute
	{
		public CppPropertyBodyAttribute(string getterBody, string setterBody)
		{
		}

		public CppPropertyBodyAttribute(string getterBody)
		{
		}
	}
}
