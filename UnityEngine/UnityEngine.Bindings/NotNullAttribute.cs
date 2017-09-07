using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class NotNullAttribute : Attribute, IBindingsAttribute
	{
	}
}
