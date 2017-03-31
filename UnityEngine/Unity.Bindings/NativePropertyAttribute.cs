using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class NativePropertyAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public bool IsThreadSafe
		{
			get;
			set;
		}
	}
}
