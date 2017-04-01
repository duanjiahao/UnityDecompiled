using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class NativeParameterAttribute : Attribute
	{
		public bool Unmarshalled
		{
			get;
			set;
		}
	}
}
