using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal class CppPropertyAttribute : Attribute
	{
		public CppPropertyAttribute(string getter, string setter)
		{
		}

		public CppPropertyAttribute(string getter)
		{
		}
	}
}
