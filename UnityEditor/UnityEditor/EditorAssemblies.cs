using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal static class EditorAssemblies
	{
		internal static List<RuntimeInitializeClassInfo> m_RuntimeInitializeClassInfoList;

		internal static int m_TotalNumRuntimeInitializeMethods;

		internal static Assembly[] loadedAssemblies
		{
			get;
			private set;
		}

		internal static IEnumerable<Type> loadedTypes
		{
			get
			{
				return EditorAssemblies.loadedAssemblies.SelectMany((Assembly assembly) => AssemblyHelper.GetTypesFromAssembly(assembly));
			}
		}

		internal static IEnumerable<Type> SubclassesOf(Type parent)
		{
			return from klass in EditorAssemblies.loadedTypes
			where klass.IsSubclassOf(parent)
			select klass;
		}

		[RequiredByNativeCode]
		private static void SetLoadedEditorAssemblies(Assembly[] assemblies)
		{
			EditorAssemblies.loadedAssemblies = assemblies;
		}

		internal static void FindClassesThatImplementAnyInterface(List<Type> results, params Type[] interfaces)
		{
			results.AddRange(from x in EditorAssemblies.loadedTypes
			where interfaces.Any((Type i) => i.IsAssignableFrom(x) && i != x)
			select x);
		}

		[RequiredByNativeCode]
		private static RuntimeInitializeClassInfo[] GetRuntimeInitializeClassInfos()
		{
			RuntimeInitializeClassInfo[] result;
			if (EditorAssemblies.m_RuntimeInitializeClassInfoList == null)
			{
				result = null;
			}
			else
			{
				result = EditorAssemblies.m_RuntimeInitializeClassInfoList.ToArray();
			}
			return result;
		}

		[RequiredByNativeCode]
		private static int GetTotalNumRuntimeInitializeMethods()
		{
			return EditorAssemblies.m_TotalNumRuntimeInitializeMethods;
		}

		private static void StoreRuntimeInitializeClassInfo(Type type, List<string> methodNames, List<RuntimeInitializeLoadType> loadTypes)
		{
			RuntimeInitializeClassInfo runtimeInitializeClassInfo = new RuntimeInitializeClassInfo();
			runtimeInitializeClassInfo.assemblyName = type.Assembly.GetName().Name.ToString();
			runtimeInitializeClassInfo.className = type.ToString();
			runtimeInitializeClassInfo.methodNames = methodNames.ToArray();
			runtimeInitializeClassInfo.loadTypes = loadTypes.ToArray();
			EditorAssemblies.m_RuntimeInitializeClassInfoList.Add(runtimeInitializeClassInfo);
			EditorAssemblies.m_TotalNumRuntimeInitializeMethods += methodNames.Count;
		}

		private static void ProcessStaticMethodAttributes(Type type)
		{
			List<string> list = null;
			List<RuntimeInitializeLoadType> list2 = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.GetLength(0); i++)
			{
				MethodInfo methodInfo = methods[i];
				if (Attribute.IsDefined(methodInfo, typeof(RuntimeInitializeOnLoadMethodAttribute)))
				{
					RuntimeInitializeLoadType item = RuntimeInitializeLoadType.AfterSceneLoad;
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
					if (customAttributes != null && customAttributes.Length > 0)
					{
						item = ((RuntimeInitializeOnLoadMethodAttribute)customAttributes[0]).loadType;
					}
					if (list == null)
					{
						list = new List<string>();
						list2 = new List<RuntimeInitializeLoadType>();
					}
					list.Add(methodInfo.Name);
					list2.Add(item);
				}
				if (Attribute.IsDefined(methodInfo, typeof(InitializeOnLoadMethodAttribute)))
				{
					try
					{
						methodInfo.Invoke(null, null);
					}
					catch (TargetInvocationException ex)
					{
						Debug.LogError(ex.InnerException);
					}
				}
			}
			if (list != null)
			{
				EditorAssemblies.StoreRuntimeInitializeClassInfo(type, list, list2);
			}
		}

		private static void ProcessEditorInitializeOnLoad(Type type)
		{
			try
			{
				RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}
			catch (TypeInitializationException ex)
			{
				Debug.LogError(ex.InnerException);
			}
		}

		[RequiredByNativeCode]
		private static int[] ProcessInitializeOnLoadAttributes()
		{
			List<int> list = null;
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			EditorAssemblies.m_TotalNumRuntimeInitializeMethods = 0;
			EditorAssemblies.m_RuntimeInitializeClassInfoList = new List<RuntimeInitializeClassInfo>();
			for (int i = 0; i < loadedAssemblies.Length; i++)
			{
				int totalNumRuntimeInitializeMethods = EditorAssemblies.m_TotalNumRuntimeInitializeMethods;
				int count = EditorAssemblies.m_RuntimeInitializeClassInfoList.Count;
				try
				{
					Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(loadedAssemblies[i]);
					Type[] array = typesFromAssembly;
					for (int j = 0; j < array.Length; j++)
					{
						Type type = array[j];
						if (type.IsDefined(typeof(InitializeOnLoadAttribute), false))
						{
							EditorAssemblies.ProcessEditorInitializeOnLoad(type);
						}
						EditorAssemblies.ProcessStaticMethodAttributes(type);
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					if (list == null)
					{
						list = new List<int>();
					}
					if (totalNumRuntimeInitializeMethods != EditorAssemblies.m_TotalNumRuntimeInitializeMethods)
					{
						EditorAssemblies.m_TotalNumRuntimeInitializeMethods = totalNumRuntimeInitializeMethods;
					}
					if (count != EditorAssemblies.m_RuntimeInitializeClassInfoList.Count)
					{
						EditorAssemblies.m_RuntimeInitializeClassInfoList.RemoveRange(count, EditorAssemblies.m_RuntimeInitializeClassInfoList.Count - count);
					}
					list.Add(i);
				}
			}
			int[] result;
			if (list == null)
			{
				result = null;
			}
			else
			{
				result = list.ToArray();
			}
			return result;
		}
	}
}
