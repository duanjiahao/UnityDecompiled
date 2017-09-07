using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class ThreadSafeAttribute : NativeMethodAttribute
	{
		public ThreadSafeAttribute()
		{
			base.IsThreadSafe = true;
		}
	}
}
