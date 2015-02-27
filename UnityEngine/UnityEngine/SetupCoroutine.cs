using System;
using System.Reflection;
namespace UnityEngine
{
	internal class SetupCoroutine
	{
		public static object InvokeMember(object behaviour, string name, object variable)
		{
			object[] args = null;
			if (variable != null)
			{
				args = new object[]
				{
					variable
				};
			}
			return behaviour.GetType().InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, behaviour, args, null, null, null);
		}
		public static object InvokeStatic(Type klass, string name, object variable)
		{
			object[] args = null;
			if (variable != null)
			{
				args = new object[]
				{
					variable
				};
			}
			return klass.InvokeMember(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, args, null, null, null);
		}
	}
}
