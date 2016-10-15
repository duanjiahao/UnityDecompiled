using System;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class CannotApplyEqualityOperatorAttribute : Attribute
	{
	}
}
