using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class FreeFunctionAttribute : NativeMethodAttribute
	{
		public FreeFunctionAttribute()
		{
			base.IsFreeFunction = true;
		}

		public FreeFunctionAttribute(string name) : base(name, true)
		{
		}

		public FreeFunctionAttribute(string name, bool isThreadSafe) : base(name, true, isThreadSafe)
		{
		}
	}
}
