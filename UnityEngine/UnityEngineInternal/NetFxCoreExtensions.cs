using System;
using System.Reflection;

namespace UnityEngineInternal
{
	internal static class NetFxCoreExtensions
	{
		public static Delegate CreateDelegate(this MethodInfo self, Type delegateType, object target)
		{
			return Delegate.CreateDelegate(delegateType, target, self);
		}

		public static MethodInfo GetMethodInfo(this Delegate self)
		{
			return self.Method;
		}
	}
}
