using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class IgnoreAttribute : Attribute, IBindingsAttribute
	{
	}
}
