using System;

namespace AOT
{
	[AttributeUsage(AttributeTargets.Method)]
	public class MonoPInvokeCallbackAttribute : Attribute
	{
		public MonoPInvokeCallbackAttribute(Type type)
		{
		}
	}
}
