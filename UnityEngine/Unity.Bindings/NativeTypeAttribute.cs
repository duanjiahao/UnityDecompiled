using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class NativeTypeAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public string Header
		{
			get;
			set;
		}
	}
}
