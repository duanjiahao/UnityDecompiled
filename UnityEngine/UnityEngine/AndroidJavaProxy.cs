using System;
using System.Reflection;

namespace UnityEngine
{
	public class AndroidJavaProxy
	{
		public readonly AndroidJavaClass javaInterface;

		public AndroidJavaProxy(string javaInterface) : this(new AndroidJavaClass(javaInterface))
		{
		}

		public AndroidJavaProxy(AndroidJavaClass javaInterface)
		{
			this.javaInterface = javaInterface;
		}

		public virtual AndroidJavaObject Invoke(string methodName, object[] args)
		{
			Exception ex = null;
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = ((args[i] != null) ? args[i].GetType() : typeof(AndroidJavaObject));
			}
			try
			{
				MethodInfo method = base.GetType().GetMethod(methodName, bindingAttr, null, array, null);
				if (method != null)
				{
					AndroidJavaObject result = _AndroidJNIHelper.Box(method.Invoke(this, args));
					return result;
				}
			}
			catch (TargetInvocationException ex2)
			{
				ex = ex2.InnerException;
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
			string[] array2 = new string[args.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = array[j].ToString();
			}
			if (ex != null)
			{
				throw new TargetInvocationException(string.Concat(new object[]
				{
					base.GetType(),
					".",
					methodName,
					"(",
					string.Join(",", array2),
					")"
				}), ex);
			}
			throw new Exception(string.Concat(new object[]
			{
				"No such proxy method: ",
				base.GetType(),
				".",
				methodName,
				"(",
				string.Join(",", array2),
				")"
			}));
		}

		public virtual AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
		{
			object[] array = new object[javaArgs.Length];
			for (int i = 0; i < javaArgs.Length; i++)
			{
				array[i] = _AndroidJNIHelper.Unbox(javaArgs[i]);
			}
			return this.Invoke(methodName, array);
		}
	}
}
