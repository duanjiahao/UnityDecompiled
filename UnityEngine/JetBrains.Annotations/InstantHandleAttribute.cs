using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
	public sealed class InstantHandleAttribute : Attribute
	{
	}
}
