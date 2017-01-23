using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class ScriptAttributeUtility
	{
		private struct DrawerKeySet
		{
			public Type drawer;

			public Type type;
		}

		internal static Stack<PropertyDrawer> s_DrawerStack = new Stack<PropertyDrawer>();

		private static Dictionary<Type, ScriptAttributeUtility.DrawerKeySet> s_DrawerTypeForType = null;

		private static Dictionary<string, List<PropertyAttribute>> s_BuiltinAttributes = null;

		private static PropertyHandler s_SharedNullHandler = new PropertyHandler();

		private static PropertyHandler s_NextHandler = new PropertyHandler();

		private static PropertyHandlerCache s_GlobalCache = new PropertyHandlerCache();

		private static PropertyHandlerCache s_CurrentCache = null;

		internal static PropertyHandlerCache propertyHandlerCache
		{
			get
			{
				return ScriptAttributeUtility.s_CurrentCache ?? ScriptAttributeUtility.s_GlobalCache;
			}
			set
			{
				ScriptAttributeUtility.s_CurrentCache = value;
			}
		}

		internal static void ClearGlobalCache()
		{
			ScriptAttributeUtility.s_GlobalCache.Clear();
		}

		private static void PopulateBuiltinAttributes()
		{
			ScriptAttributeUtility.s_BuiltinAttributes = new Dictionary<string, List<PropertyAttribute>>();
			ScriptAttributeUtility.AddBuiltinAttribute("GUIText", "m_Text", new MultilineAttribute());
			ScriptAttributeUtility.AddBuiltinAttribute("TextMesh", "m_Text", new MultilineAttribute());
		}

		private static void AddBuiltinAttribute(string componentTypeName, string propertyPath, PropertyAttribute attr)
		{
			string key = componentTypeName + "_" + propertyPath;
			if (!ScriptAttributeUtility.s_BuiltinAttributes.ContainsKey(key))
			{
				ScriptAttributeUtility.s_BuiltinAttributes.Add(key, new List<PropertyAttribute>());
			}
			ScriptAttributeUtility.s_BuiltinAttributes[key].Add(attr);
		}

		private static List<PropertyAttribute> GetBuiltinAttributes(SerializedProperty property)
		{
			List<PropertyAttribute> result;
			if (property.serializedObject.targetObject == null)
			{
				result = null;
			}
			else
			{
				Type type = property.serializedObject.targetObject.GetType();
				if (type == null)
				{
					result = null;
				}
				else
				{
					string key = type.Name + "_" + property.propertyPath;
					List<PropertyAttribute> list = null;
					ScriptAttributeUtility.s_BuiltinAttributes.TryGetValue(key, out list);
					result = list;
				}
			}
			return result;
		}

		private static void BuildDrawerTypeForTypeDictionary()
		{
			ScriptAttributeUtility.s_DrawerTypeForType = new Dictionary<Type, ScriptAttributeUtility.DrawerKeySet>();
			Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => AssemblyHelper.GetTypesFromAssembly(x)).ToArray<Type>();
			foreach (Type current in EditorAssemblies.SubclassesOf(typeof(GUIDrawer)))
			{
				object[] customAttributes = current.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					CustomPropertyDrawer editor = (CustomPropertyDrawer)array[i];
					ScriptAttributeUtility.s_DrawerTypeForType[editor.m_Type] = new ScriptAttributeUtility.DrawerKeySet
					{
						drawer = current,
						type = editor.m_Type
					};
					if (editor.m_UseForChildren)
					{
						IEnumerable<Type> enumerable = from x in source
						where x.IsSubclassOf(editor.m_Type)
						select x;
						foreach (Type current2 in enumerable)
						{
							if (!ScriptAttributeUtility.s_DrawerTypeForType.ContainsKey(current2) || !editor.m_Type.IsAssignableFrom(ScriptAttributeUtility.s_DrawerTypeForType[current2].type))
							{
								ScriptAttributeUtility.s_DrawerTypeForType[current2] = new ScriptAttributeUtility.DrawerKeySet
								{
									drawer = current,
									type = editor.m_Type
								};
							}
						}
					}
				}
			}
		}

		internal static Type GetDrawerTypeForType(Type type)
		{
			if (ScriptAttributeUtility.s_DrawerTypeForType == null)
			{
				ScriptAttributeUtility.BuildDrawerTypeForTypeDictionary();
			}
			ScriptAttributeUtility.DrawerKeySet drawerKeySet;
			ScriptAttributeUtility.s_DrawerTypeForType.TryGetValue(type, out drawerKeySet);
			Type drawer;
			if (drawerKeySet.drawer != null)
			{
				drawer = drawerKeySet.drawer;
			}
			else
			{
				if (type.IsGenericType)
				{
					ScriptAttributeUtility.s_DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out drawerKeySet);
				}
				drawer = drawerKeySet.drawer;
			}
			return drawer;
		}

		private static List<PropertyAttribute> GetFieldAttributes(FieldInfo field)
		{
			List<PropertyAttribute> result;
			if (field == null)
			{
				result = null;
			}
			else
			{
				object[] customAttributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					result = new List<PropertyAttribute>(from e in customAttributes
					select e as PropertyAttribute into e
					orderby -e.order
					select e);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
		{
			Type scriptTypeFromProperty = ScriptAttributeUtility.GetScriptTypeFromProperty(property);
			FieldInfo result;
			if (scriptTypeFromProperty == null)
			{
				type = null;
				result = null;
			}
			else
			{
				result = ScriptAttributeUtility.GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
			}
			return result;
		}

		private static Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
			Type result;
			if (serializedProperty == null)
			{
				result = null;
			}
			else
			{
				MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
				if (monoScript == null)
				{
					result = null;
				}
				else
				{
					result = monoScript.GetClass();
				}
			}
			return result;
		}

		private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
		{
			FieldInfo fieldInfo = null;
			type = host;
			string[] array = path.Split(new char[]
			{
				'.'
			});
			FieldInfo result;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
				{
					if (type.IsArrayOrList())
					{
						type = type.GetArrayOrListElementType();
					}
					i++;
				}
				else
				{
					FieldInfo fieldInfo2 = null;
					Type type2 = type;
					while (fieldInfo2 == null && type2 != null)
					{
						fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						type2 = type2.BaseType;
					}
					if (fieldInfo2 == null)
					{
						type = null;
						result = null;
						return result;
					}
					fieldInfo = fieldInfo2;
					type = fieldInfo.FieldType;
				}
			}
			result = fieldInfo;
			return result;
		}

		internal static PropertyHandler GetHandler(SerializedProperty property)
		{
			PropertyHandler result;
			if (property == null)
			{
				result = ScriptAttributeUtility.s_SharedNullHandler;
			}
			else if (property.serializedObject.inspectorMode != InspectorMode.Normal)
			{
				result = ScriptAttributeUtility.s_SharedNullHandler;
			}
			else
			{
				PropertyHandler handler = ScriptAttributeUtility.propertyHandlerCache.GetHandler(property);
				if (handler != null)
				{
					result = handler;
				}
				else
				{
					Type type = null;
					List<PropertyAttribute> list = null;
					FieldInfo field = null;
					UnityEngine.Object targetObject = property.serializedObject.targetObject;
					if (targetObject is MonoBehaviour || targetObject is ScriptableObject)
					{
						field = ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type);
						list = ScriptAttributeUtility.GetFieldAttributes(field);
					}
					else
					{
						if (ScriptAttributeUtility.s_BuiltinAttributes == null)
						{
							ScriptAttributeUtility.PopulateBuiltinAttributes();
						}
						if (list == null)
						{
							list = ScriptAttributeUtility.GetBuiltinAttributes(property);
						}
					}
					handler = ScriptAttributeUtility.s_NextHandler;
					if (list != null)
					{
						for (int i = list.Count - 1; i >= 0; i--)
						{
							handler.HandleAttribute(list[i], field, type);
						}
					}
					if (!handler.hasPropertyDrawer && type != null)
					{
						handler.HandleDrawnType(type, type, field, null);
					}
					if (handler.empty)
					{
						ScriptAttributeUtility.propertyHandlerCache.SetHandler(property, ScriptAttributeUtility.s_SharedNullHandler);
						handler = ScriptAttributeUtility.s_SharedNullHandler;
					}
					else
					{
						ScriptAttributeUtility.propertyHandlerCache.SetHandler(property, handler);
						ScriptAttributeUtility.s_NextHandler = new PropertyHandler();
					}
					result = handler;
				}
			}
			return result;
		}
	}
}
