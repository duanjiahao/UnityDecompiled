using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityEditor.Macros
{
	public static class MethodEvaluator
	{
		private class AssemblyResolver
		{
			private readonly string m_AssemblyDirectory;

			public AssemblyResolver(string assemblyDirectory)
			{
				this.m_AssemblyDirectory = assemblyDirectory;
			}

			public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				string str = args.Name.Split(new char[]
				{
					','
				})[0];
				string text = Path.Combine(this.m_AssemblyDirectory, str + ".dll");
				Assembly result;
				if (File.Exists(text))
				{
					result = Assembly.LoadFrom(text);
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		private static readonly BinaryFormatter s_Formatter = new BinaryFormatter
		{
			AssemblyFormat = FormatterAssemblyStyle.Simple
		};

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

		public static object ExecuteExternalCode(string parcel)
		{
			object result;
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(parcel)))
			{
				string a = (string)MethodEvaluator.s_Formatter.Deserialize(memoryStream);
				if (a != "com.unity3d.automation")
				{
					throw new Exception("Invalid parcel for external code execution.");
				}
				string text = (string)MethodEvaluator.s_Formatter.Deserialize(memoryStream);
				MethodEvaluator.AssemblyResolver @object = new MethodEvaluator.AssemblyResolver(Path.GetDirectoryName(text));
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(@object.AssemblyResolve);
				Assembly assembly = Assembly.LoadFrom(text);
				try
				{
					Type type = (Type)MethodEvaluator.s_Formatter.Deserialize(memoryStream);
					string text2 = (string)MethodEvaluator.s_Formatter.Deserialize(memoryStream);
					Type[] types = (Type[])MethodEvaluator.s_Formatter.Deserialize(memoryStream);
					MethodInfo method = type.GetMethod(text2, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
					if (method == null)
					{
						throw new Exception(string.Format("Could not find method {0}.{1} in assembly {2} located in {3}.", new object[]
						{
							type.FullName,
							text2,
							assembly.GetName().Name,
							text
						}));
					}
					object[] args = (object[])MethodEvaluator.s_Formatter.Deserialize(memoryStream);
					result = MethodEvaluator.ExecuteCode(type, method, args);
				}
				finally
				{
					AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(@object.AssemblyResolve);
				}
			}
			return result;
		}

		private static object ExecuteCode(Type target, MethodInfo method, object[] args)
		{
			return method.Invoke((!method.IsStatic) ? MethodEvaluator.GetActor(target) : null, args);
		}

		private static object GetActor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(new Type[0]);
			return (constructor == null) ? null : constructor.Invoke(new object[0]);
		}

		private static string ToCommaSeparatedString<T>(IEnumerable<T> items)
		{
			return string.Join(", ", (from o in items
			select o.ToString()).ToArray<string>());
		}
	}
}
