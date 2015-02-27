using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace UnityEditor
{
	internal class CustomEditorAttributes
	{
		private class MonoEditorType
		{
			public Type m_InspectedType;
			public Type m_InspectorType;
			public bool m_EditorForChildClasses;
		}
		private static readonly List<CustomEditorAttributes.MonoEditorType> kSCustomEditors = new List<CustomEditorAttributes.MonoEditorType>();
		private static readonly List<CustomEditorAttributes.MonoEditorType> kSCustomMultiEditors = new List<CustomEditorAttributes.MonoEditorType>();
		private static bool s_Initialized;
		internal static Type FindCustomEditorType(UnityEngine.Object o, bool multiEdit)
		{
			return CustomEditorAttributes.FindCustomEditorTypeByType(o.GetType(), multiEdit);
		}
		internal static Type FindCustomEditorTypeByType(Type type, bool multiEdit)
		{
			if (!CustomEditorAttributes.s_Initialized)
			{
				Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
				for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
				{
					CustomEditorAttributes.Rebuild(loadedAssemblies[i]);
				}
				CustomEditorAttributes.s_Initialized = true;
			}
			List<CustomEditorAttributes.MonoEditorType> source = (!multiEdit) ? CustomEditorAttributes.kSCustomEditors : CustomEditorAttributes.kSCustomMultiEditors;
			Type inspected;
			for (inspected = type; inspected != null; inspected = inspected.BaseType)
			{
				CustomEditorAttributes.MonoEditorType monoEditorType;
				if (type == inspected)
				{
					monoEditorType = source.FirstOrDefault((CustomEditorAttributes.MonoEditorType x) => inspected == x.m_InspectedType);
				}
				else
				{
					monoEditorType = source.FirstOrDefault((CustomEditorAttributes.MonoEditorType x) => inspected == x.m_InspectedType && x.m_EditorForChildClasses);
				}
				if (monoEditorType != null)
				{
					return monoEditorType.m_InspectorType;
				}
			}
			return null;
		}
		internal static void Rebuild(Assembly assembly)
		{
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				object[] customAttributes = type.GetCustomAttributes(typeof(CustomEditor), false);
				object[] array2 = customAttributes;
				for (int j = 0; j < array2.Length; j++)
				{
					CustomEditor customEditor = (CustomEditor)array2[j];
					CustomEditorAttributes.MonoEditorType monoEditorType = new CustomEditorAttributes.MonoEditorType();
					if (customEditor.m_InspectedType == null)
					{
						Debug.Log("Can't load custom inspector " + type.Name + " because the inspected type is null.");
					}
					else
					{
						if (!type.IsSubclassOf(typeof(Editor)))
						{
							if (!(type.FullName == "TweakMode") || !type.IsEnum || !(customEditor.m_InspectedType.FullName == "BloomAndFlares"))
							{
								Debug.LogWarning(type.Name + " uses the CustomEditor attribute but does not inherit from Editor.\nYou must inherit from Editor. See the Editor class script documentation.");
							}
						}
						else
						{
							monoEditorType.m_InspectedType = customEditor.m_InspectedType;
							monoEditorType.m_InspectorType = type;
							monoEditorType.m_EditorForChildClasses = customEditor.m_EditorForChildClasses;
							CustomEditorAttributes.kSCustomEditors.Add(monoEditorType);
							if (type.GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0)
							{
								CustomEditorAttributes.kSCustomMultiEditors.Add(monoEditorType);
							}
						}
					}
				}
			}
		}
	}
}
