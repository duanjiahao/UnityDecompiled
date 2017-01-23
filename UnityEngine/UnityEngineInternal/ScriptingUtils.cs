using System;
using System.Reflection;

namespace UnityEngineInternal
{
	public class ScriptingUtils
	{
		public static Delegate CreateDelegate(Type type, MethodInfo methodInfo)
		{
			return Delegate.CreateDelegate(type, methodInfo);
		}
	}
}
