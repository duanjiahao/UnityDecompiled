using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class NativeGetterAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}
	}
}
