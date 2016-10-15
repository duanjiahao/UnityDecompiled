using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UnityEditor.Macros
{
	public static class MethodEvaluator
	{
		public class AssemblyResolver
		{
			private readonly string _assemblyDirectory;

			public AssemblyResolver(string assemblyDirectory)
			{
				this._assemblyDirectory = assemblyDirectory;
			}

			public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				string str = args.Name.Split(new char[]
				{
					','
				})[0];
				string text = Path.Combine(this._assemblyDirectory, str + ".dll");
				if (File.Exists(text))
				{
					return Assembly.LoadFrom(text);
				}
				return null;
			}
		}

		public static object Eval(string assemblyFile, string typeName, string methodName, Type[] paramTypes, object[] args)
		{
			string directoryName = Path.GetDirectoryName(assemblyFile);
			MethodEvaluator.AssemblyResolver @object = new MethodEvaluator.AssemblyResolver(directoryName);
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(@object.AssemblyResolve);
			object result;
			try
			{
				Assembly assembly = Assembly.LoadFrom(assemblyFile);
				MethodInfo method = assembly.GetType(typeName, true).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
				if (method == null)
				{
					throw new ArgumentException(string.Format("Method {0}.{1}({2}) not found in assembly {3}!", new object[]
					{
						typeName,
						methodName,
						MethodEvaluator.ToCommaSeparatedString<Type>(paramTypes),
						assembly.FullName
					}));
				}
				result = method.Invoke(null, args);
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(@object.AssemblyResolve);
			}
			return result;
		}

		private static string ToCommaSeparatedString<T>(IEnumerable<T> items)
		{
			return string.Join(", ", (from o in items
			select o.ToString()).ToArray<string>());
		}
	}
}
