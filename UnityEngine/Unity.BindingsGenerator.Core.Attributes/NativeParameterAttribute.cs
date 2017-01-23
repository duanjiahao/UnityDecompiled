using System;

namespace Unity.BindingsGenerator.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class NativeParameterAttribute : Attribute
	{
		public bool CanBeNull
		{
			get;
			set;
		}
	}
}
