using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class NativePropertyAttribute : NativeMethodAttribute
	{
		public NativePropertyAttribute()
		{
		}

		public NativePropertyAttribute(string name) : base(name)
		{
		}

		public NativePropertyAttribute(string name, bool isFreeFunction) : base(name, isFreeFunction)
		{
		}

		public NativePropertyAttribute(string name, bool isFreeFunction, bool isThreadSafe) : base(name, isFreeFunction, isThreadSafe)
		{
		}
	}
}
