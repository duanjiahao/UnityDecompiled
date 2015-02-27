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
			return 
				from klass in EditorAssemblies.loadedTypes
				where klass.IsSubclassOf(parent)
				select klass;
		}
		private static void SetLoadedEditorAssemblies(Assembly[] assemblies)
		{
			EditorAssemblies.loadedAssemblies = assemblies;
			EditorAssemblies.RunClassConstructors();
		}
		private static void RunClassConstructors()
		{
			foreach (Type current in EditorAssemblies.loadedTypes)
			{
				if (current.IsDefined(typeof(InitializeOnLoadAttribute), false))
				{
					try
					{
						RuntimeHelpers.RunClassConstructor(current.TypeHandle);
					}
					catch (TypeInitializationException ex)
					{
						Debug.LogError(ex.InnerException);
					}
				}
			}
		}
	}
}
