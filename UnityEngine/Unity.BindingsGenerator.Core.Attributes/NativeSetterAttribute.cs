using System;

namespace Unity.BindingsGenerator.Core.Attributes
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
