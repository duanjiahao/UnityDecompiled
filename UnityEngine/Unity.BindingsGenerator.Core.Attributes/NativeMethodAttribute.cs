using System;

namespace Unity.BindingsGenerator.Core.Attributes
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
