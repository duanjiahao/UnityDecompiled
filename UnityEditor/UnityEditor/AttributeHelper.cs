using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
						else if (methodInfo.DeclaringType != null && methodInfo.DeclaringType.IsGenericTypeDefinition)
						{
							UnityEngine.Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but is defined on a generic type definition, so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
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
									goto IL_1DD;
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
						IL_1DD:;
					}
				}
			}
			return list.ToArray();
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
	}
}
