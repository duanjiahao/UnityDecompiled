using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	public class ConstructorSafeAttribute : Attribute
	{
	}
}
