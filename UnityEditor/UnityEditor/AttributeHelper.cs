using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal class AttributeHelper
	{
		private struct MonoGizmoMethod
		{
			public MethodInfo drawGizmo;

			public Type drawnType;

			public int options;
		}

		private struct MonoMenuItem
		{
			public string menuItem;

			public int index;

			public int priority;

			public Type executeType;

			public MethodInfo executeMethod;

			public string executeName;

			public Type validateType;

			public MethodInfo validateMethod;

			public string validateName;
		}

		internal class CompareMenuIndex : IComparer
		{
			int IComparer.Compare(object xo, object yo)
			{
				AttributeHelper.MonoMenuItem monoMenuItem = (AttributeHelper.MonoMenuItem)xo;
				AttributeHelper.MonoMenuItem monoMenuItem2 = (AttributeHelper.MonoMenuItem)yo;
				int result;
				if (monoMenuItem.priority != monoMenuItem2.priority)
				{
					result = monoMenuItem.priority.CompareTo(monoMenuItem2.priority);
				}
				else
				{
					result = monoMenuItem.index.CompareTo(monoMenuItem2.index);
				}
				return result;
			}
		}

		private struct MonoCreateAssetItem
		{
			public string menuItem;

			public string fileName;

			public int order;

			public Type type;
		}

		[RequiredByNativeCode]
		private static AttributeHelper.MonoGizmoMethod[] ExtractGizmos(Assembly assembly)
		{
			List<AttributeHelper.MonoGizmoMethod> list = new List<AttributeHelper.MonoGizmoMethod>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.GetLength(0); j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(DrawGizmo), false);
					object[] array2 = customAttributes;
					for (int k = 0; k < array2.Length; k++)
					{
						DrawGizmo drawGizmo = (DrawGizmo)array2[k];
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length != 2)
						{
							UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
						}
						else
						{
							AttributeHelper.MonoGizmoMethod item = default(AttributeHelper.MonoGizmoMethod);
							if (drawGizmo.drawnType == null)
							{
								item.drawnType = parameters[0].ParameterType;
							}
							else
							{
								if (!parameters[0].ParameterType.IsAssignableFrom(drawGizmo.drawnType))
								{
									UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.", methodInfo.DeclaringType.FullName, methodInfo.Name));
									goto IL_198;
								}
								item.drawnType = drawGizmo.drawnType;
							}
							if (parameters[1].ParameterType != typeof(GizmoType) && parameters[1].ParameterType != typeof(int))
							{
								UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
							}
							else
							{
								item.drawGizmo = methodInfo;
								item.options = (int)drawGizmo.drawOptions;
								list.Add(item);
							}
						}
						IL_198:;
					}
				}
			}
			return list.ToArray();
		}

		private static bool ValidateMethodForMenuCommand(MethodInfo mi, bool contextMenu)
		{
			bool result;
			if (contextMenu)
			{
				if (mi.IsStatic)
				{
					UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is static and cannot be used for context menu commands.", new object[]
					{
						mi.DeclaringType.FullName,
						mi.Name
					});
					result = false;
					return result;
				}
			}
			else if (!mi.IsStatic)
			{
				UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is not static and cannot be used for menu commands.", new object[]
				{
					mi.DeclaringType.FullName,
					mi.Name
				});
				result = false;
				return result;
			}
			if (mi.IsGenericMethod)
			{
				UnityEngine.Debug.LogWarningFormat("Method {0}.{1} is generic and cannot be used for menu commands.", new object[]
				{
					mi.DeclaringType.FullName,
					mi.Name
				});
				result = false;
			}
			else if (mi.GetParameters().Length > 1 || (mi.GetParameters().Length == 1 && mi.GetParameters()[0].ParameterType != typeof(MenuCommand)))
			{
				UnityEngine.Debug.LogWarningFormat("Method {0}.{1} has invalid parameters. MenuCommand is the only optional supported parameter.", new object[]
				{
					mi.DeclaringType.FullName,
					mi.Name
				});
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		[RequiredByNativeCode]
		private static AttributeHelper.MonoMenuItem[] ExtractMenuCommands(Assembly assembly, bool modifiedSinceLastReload)
		{
			BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			if (modifiedSinceLastReload)
			{
				bindingFlags |= BindingFlags.Instance;
			}
			bool @bool = EditorPrefs.GetBool("InternalMode", false);
			Dictionary<string, AttributeHelper.MonoMenuItem> dictionary = new Dictionary<string, AttributeHelper.MonoMenuItem>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(bindingFlags);
				for (int j = 0; j < methods.GetLength(0); j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MenuItem), false);
					if (customAttributes.Length > 0 && type.IsGenericTypeDefinition)
					{
						UnityEngine.Debug.LogWarningFormat("Method {0}.{1} cannot be used for menu commands because class {0} is an open generic type.", new object[]
						{
							type.Name,
							methodInfo.Name
						});
					}
					else
					{
						object[] array2 = customAttributes;
						int k = 0;
						while (k < array2.Length)
						{
							MenuItem menuItem = (MenuItem)array2[k];
							AttributeHelper.MonoMenuItem value = (!dictionary.ContainsKey(menuItem.menuItem)) ? default(AttributeHelper.MonoMenuItem) : dictionary[menuItem.menuItem];
							if (!AttributeHelper.ValidateMethodForMenuCommand(methodInfo, false))
							{
								break;
							}
							if (!menuItem.menuItem.StartsWith("internal:", StringComparison.Ordinal))
							{
								value.menuItem = menuItem.menuItem;
								goto IL_154;
							}
							if (@bool)
							{
								value.menuItem = menuItem.menuItem.Substring(9);
								goto IL_154;
							}
							IL_1D0:
							k++;
							continue;
							IL_154:
							if (menuItem.validate)
							{
								value.validateType = type;
								value.validateMethod = methodInfo;
								value.validateName = methodInfo.Name;
							}
							else
							{
								value.index = j;
								value.priority = menuItem.priority;
								value.executeType = type;
								value.executeMethod = methodInfo;
								value.executeName = methodInfo.Name;
							}
							dictionary[menuItem.menuItem] = value;
							goto IL_1D0;
						}
					}
				}
			}
			AttributeHelper.MonoMenuItem[] array3 = dictionary.Values.ToArray<AttributeHelper.MonoMenuItem>();
			Array.Sort(array3, new AttributeHelper.CompareMenuIndex());
			return array3;
		}

		[RequiredByNativeCode]
		private static AttributeHelper.MonoMenuItem[] ExtractContextMenu(Type type)
		{
			Dictionary<string, AttributeHelper.MonoMenuItem> dictionary = new Dictionary<string, AttributeHelper.MonoMenuItem>();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.GetLength(0); i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ContextMenu), false);
				object[] array = customAttributes;
				for (int j = 0; j < array.Length; j++)
				{
					ContextMenu contextMenu = (ContextMenu)array[j];
					AttributeHelper.MonoMenuItem value = (!dictionary.ContainsKey(contextMenu.menuItem)) ? default(AttributeHelper.MonoMenuItem) : dictionary[contextMenu.menuItem];
					if (!AttributeHelper.ValidateMethodForMenuCommand(methodInfo, true))
					{
						break;
					}
					value.menuItem = contextMenu.menuItem;
					if (contextMenu.validate)
					{
						value.validateType = type;
						value.validateMethod = methodInfo;
						value.validateName = methodInfo.Name;
					}
					else
					{
						value.index = i;
						value.priority = contextMenu.priority;
						value.executeType = type;
						value.executeMethod = methodInfo;
						value.executeName = methodInfo.Name;
					}
					dictionary[contextMenu.menuItem] = value;
				}
			}
			AttributeHelper.MonoMenuItem[] array2 = dictionary.Values.ToArray<AttributeHelper.MonoMenuItem>();
			Array.Sort(array2, new AttributeHelper.CompareMenuIndex());
			return array2;
		}

		[RequiredByNativeCode]
		private static string GetComponentMenuName(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
			string result;
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				result = addComponentMenu.componentMenu;
			}
			else
			{
				result = null;
			}
			return result;
		}

		[RequiredByNativeCode]
		private static int GetComponentMenuOrdering(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
			int result;
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				result = addComponentMenu.componentOrder;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		[RequiredByNativeCode]
		private static AttributeHelper.MonoCreateAssetItem[] ExtractCreateAssetMenuItems(Assembly assembly)
		{
			List<AttributeHelper.MonoCreateAssetItem> list = new List<AttributeHelper.MonoCreateAssetItem>();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				CreateAssetMenuAttribute createAssetMenuAttribute = (CreateAssetMenuAttribute)Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute));
				if (createAssetMenuAttribute != null)
				{
					if (!type.IsSubclassOf(typeof(ScriptableObject)))
					{
						UnityEngine.Debug.LogWarningFormat("CreateAssetMenu attribute on {0} will be ignored as {0} is not derived from ScriptableObject.", new object[]
						{
							type.FullName
						});
					}
					else
					{
						string menuItem = (!string.IsNullOrEmpty(createAssetMenuAttribute.menuName)) ? createAssetMenuAttribute.menuName : ObjectNames.NicifyVariableName(type.Name);
						string text = (!string.IsNullOrEmpty(createAssetMenuAttribute.fileName)) ? createAssetMenuAttribute.fileName : ("New " + ObjectNames.NicifyVariableName(type.Name) + ".asset");
						if (!Path.HasExtension(text))
						{
							text += ".asset";
						}
						list.Add(new AttributeHelper.MonoCreateAssetItem
						{
							menuItem = menuItem,
							fileName = text,
							order = createAssetMenuAttribute.order,
							type = type
						});
					}
				}
			}
			return list.ToArray();
		}

		internal static object InvokeMemberIfAvailable(object target, string methodName, object[] args)
		{
			MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object result;
			if (method != null)
			{
				result = method.Invoke(target, args);
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static bool GameObjectContainsAttribute(GameObject go, Type attributeType)
		{
			Component[] components = go.GetComponents(typeof(Component));
			bool result;
			for (int i = 0; i < components.Length; i++)
			{
				Component component = components[i];
				if (!(component == null))
				{
					Type type = component.GetType();
					if (type.GetCustomAttributes(attributeType, true).Length > 0)
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		[DebuggerHidden]
		internal static IEnumerable<T> CallMethodsWithAttribute<T>(Type attributeType, params object[] arguments)
		{
			AttributeHelper.<CallMethodsWithAttribute>c__Iterator0<T> <CallMethodsWithAttribute>c__Iterator = new AttributeHelper.<CallMethodsWithAttribute>c__Iterator0<T>();
			<CallMethodsWithAttribute>c__Iterator.attributeType = attributeType;
			<CallMethodsWithAttribute>c__Iterator.arguments = arguments;
			AttributeHelper.<CallMethodsWithAttribute>c__Iterator0<T> expr_15 = <CallMethodsWithAttribute>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}

		[RequiredByNativeCode]
		internal static string GetHelpURLFromAttribute(Type objectType)
		{
			HelpURLAttribute helpURLAttribute = (HelpURLAttribute)Attribute.GetCustomAttribute(objectType, typeof(HelpURLAttribute));
			return (helpURLAttribute == null) ? null : helpURLAttribute.URL;
		}
	}
}
