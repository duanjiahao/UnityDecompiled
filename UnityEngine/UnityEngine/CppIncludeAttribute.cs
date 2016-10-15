using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	internal class CppIncludeAttribute : Attribute
	{
		public CppIncludeAttribute(string header)
		{
		}
	}
}
