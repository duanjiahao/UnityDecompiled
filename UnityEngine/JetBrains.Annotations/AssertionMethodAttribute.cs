using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class AssertionMethodAttribute : Attribute
	{
	}
}
