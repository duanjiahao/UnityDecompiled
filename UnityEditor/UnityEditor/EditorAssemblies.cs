using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

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

		private static void SetLoadedEditorAssemblies(Assembly[] assemblies)
		{
			EditorAssemblies.loadedAssemblies = assemblies;
			EditorAssemblies.ProcessInitializeOnLoadAttributes();
		}

		private static RuntimeInitializeClassInfo[] GetRuntimeInitializeClassInfos()
		{
			if (EditorAssemblies.m_RuntimeInitializeClassInfoList == null)
			{
				return null;
			}
			return EditorAssemblies.m_RuntimeInitializeClassInfoList.ToArray();
		}

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
					methodInfo.Invoke(null, null);
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

		private static void ProcessInitializeOnLoadAttributes()
		{
			EditorAssemblies.m_TotalNumRuntimeInitializeMethods = 0;
			EditorAssemblies.m_RuntimeInitializeClassInfoList = new List<RuntimeInitializeClassInfo>();
			foreach (Type current in EditorAssemblies.loadedTypes)
			{
				if (current.IsDefined(typeof(InitializeOnLoadAttribute), false))
				{
					EditorAssemblies.ProcessEditorInitializeOnLoad(current);
				}
				EditorAssemblies.ProcessStaticMethodAttributes(current);
			}
		}
	}
}
