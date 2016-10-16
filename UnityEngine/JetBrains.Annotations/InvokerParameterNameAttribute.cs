using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InvokerParameterNameAttribute : Attribute
	{
	}
}
