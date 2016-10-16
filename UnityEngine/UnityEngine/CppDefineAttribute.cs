using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	internal class CppDefineAttribute : Attribute
	{
		public CppDefineAttribute(string symbol, string value)
		{
		}
	}
}
