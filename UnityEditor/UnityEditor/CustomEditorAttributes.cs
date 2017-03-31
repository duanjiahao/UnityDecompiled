using System;
using System.Collections.Generic;
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

			public bool m_IsFallback;
		}

		private static readonly List<CustomEditorAttributes.MonoEditorType> kSCustomEditors = new List<CustomEditorAttributes.MonoEditorType>();

		private static readonly List<CustomEditorAttributes.MonoEditorType> kSCustomMultiEditors = new List<CustomEditorAttributes.MonoEditorType>();

		private static readonly Dictionary<Type, Type> kCachedEditorForType = new Dictionary<Type, Type>();

		private static readonly Dictionary<Type, Type> kCachedMultiEditorForType = new Dictionary<Type, Type>();

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
			Type result;
			if (type == null)
			{
				result = null;
			}
			else
			{
				Dictionary<Type, Type> dictionary = (!multiEdit) ? CustomEditorAttributes.kCachedEditorForType : CustomEditorAttributes.kCachedMultiEditorForType;
				Type inspectorType;
				if (dictionary.TryGetValue(type, out inspectorType))
				{
					result = inspectorType;
				}
				else
				{
					List<CustomEditorAttributes.MonoEditorType> list = (!multiEdit) ? CustomEditorAttributes.kSCustomEditors : CustomEditorAttributes.kSCustomMultiEditors;
					for (int j = 0; j < 2; j++)
					{
						for (Type type2 = type; type2 != null; type2 = type2.BaseType)
						{
							for (int k = 0; k < list.Count; k++)
							{
								if (CustomEditorAttributes.IsAppropriateEditor(list[k], type2, type != type2, j == 1))
								{
									inspectorType = list[k].m_InspectorType;
									dictionary.Add(type, inspectorType);
									result = inspectorType;
									return result;
								}
							}
						}
					}
					dictionary.Add(type, null);
					result = null;
				}
			}
			return result;
		}

		private static bool IsAppropriateEditor(CustomEditorAttributes.MonoEditorType editor, Type parentClass, bool isChildClass, bool isFallback)
		{
			return (!isChildClass || editor.m_EditorForChildClasses) && isFallback == editor.m_IsFallback && (parentClass == editor.m_InspectedType || (parentClass.IsGenericType && parentClass.GetGenericTypeDefinition() == editor.m_InspectedType));
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
				int j = 0;
				while (j < array2.Length)
				{
					CustomEditor customEditor = (CustomEditor)array2[j];
					CustomEditorAttributes.MonoEditorType monoEditorType = new CustomEditorAttributes.MonoEditorType();
					if (customEditor.m_InspectedType == null)
					{
						Debug.Log("Can't load custom inspector " + type.Name + " because the inspected type is null.");
					}
					else if (!type.IsSubclassOf(typeof(Editor)))
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
						monoEditorType.m_IsFallback = customEditor.isFallback;
						CustomEditorAttributes.kSCustomEditors.Add(monoEditorType);
						if (type.GetCustomAttributes(typeof(CanEditMultipleObjects), false).Length > 0)
						{
							CustomEditorAttributes.kSCustomMultiEditors.Add(monoEditorType);
						}
					}
					IL_14D:
					j++;
					continue;
					goto IL_14D;
				}
			}
		}
	}
}
