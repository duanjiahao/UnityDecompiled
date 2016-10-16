using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public class ThreadSafeAttribute : Attribute
	{
	}
}
