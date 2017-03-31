using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class NativeMethodAttribute : Attribute
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

		public bool IsFreeFunction
		{
			get;
			set;
		}
	}
}
