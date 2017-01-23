using System;

namespace Unity.BindingsGenerator.Core.Attributes
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
