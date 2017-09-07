using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class UnmarshalledAttribute : Attribute, IBindingsAttribute
	{
	}
}
