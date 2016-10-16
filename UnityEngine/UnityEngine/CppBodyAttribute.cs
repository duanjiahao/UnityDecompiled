using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
	internal class CppBodyAttribute : Attribute
	{
		public CppBodyAttribute(string body)
		{
		}
	}
}
