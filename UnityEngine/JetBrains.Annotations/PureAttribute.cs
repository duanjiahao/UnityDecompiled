using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class PureAttribute : Attribute
	{
	}
}
