using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class NativeSetterAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}
	}
}
