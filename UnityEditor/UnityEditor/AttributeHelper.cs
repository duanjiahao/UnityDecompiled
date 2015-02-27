using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
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
			public string execute;
			public string validate;
			public int priority;
			public int index;
			public Type type;
		}
		internal class CompareMenuIndex : IComparer
		{
			int IComparer.Compare(object xo, object yo)
			{
				AttributeHelper.MonoMenuItem monoMenuItem = (AttributeHelper.MonoMenuItem)xo;
				AttributeHelper.MonoMenuItem monoMenuItem2 = (AttributeHelper.MonoMenuItem)yo;
				if (monoMenuItem.priority != monoMenuItem2.priority)
				{
					return monoMenuItem.priority.CompareTo(monoMenuItem2.priority);
				}
				return monoMenuItem.index.CompareTo(monoMenuItem2.index);
			}
		}
		private static AttributeHelper.MonoGizmoMethod[] ExtractGizmos(Assembly assembly)
		{
			ArrayList arrayList = new ArrayList();
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
							Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take parameters (ComponentType, GizmoType) so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
						}
						else
						{
							AttributeHelper.MonoGizmoMethod monoGizmoMethod = default(AttributeHelper.MonoGizmoMethod);
							if (drawGizmo.drawnType == null)
							{
								monoGizmoMethod.drawnType = parameters[0].ParameterType;
							}
							else
							{
								if (!parameters[0].ParameterType.IsAssignableFrom(drawGizmo.drawnType))
								{
									Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but the component type it applies to could not be determined.", methodInfo.DeclaringType.FullName, methodInfo.Name));
									goto IL_194;
								}
								monoGizmoMethod.drawnType = drawGizmo.drawnType;
							}
							if (parameters[1].ParameterType != typeof(GizmoType) && parameters[1].ParameterType != typeof(int))
							{
								Debug.LogWarning(string.Format("Method {0}.{1} is marked with the DrawGizmo attribute but does not take a second parameter of type GizmoType so will be ignored.", methodInfo.DeclaringType.FullName, methodInfo.Name));
							}
							else
							{
								monoGizmoMethod.drawGizmo = methodInfo;
								monoGizmoMethod.options = (int)drawGizmo.drawOptions;
								arrayList.Add(monoGizmoMethod);
							}
						}
						IL_194:;
					}
				}
			}
			AttributeHelper.MonoGizmoMethod[] array3 = new AttributeHelper.MonoGizmoMethod[arrayList.Count];
			int num = 0;
			foreach (AttributeHelper.MonoGizmoMethod monoGizmoMethod2 in arrayList)
			{
				array3[num++] = monoGizmoMethod2;
			}
			return array3;
		}
		private static AttributeHelper.MonoMenuItem[] ExtractMenuCommands(Assembly assembly)
		{
			bool @bool = EditorPrefs.GetBool("InternalMode", false);
			Hashtable hashtable = new Hashtable();
			Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
			Type[] array = typesFromAssembly;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				for (int j = 0; j < methods.GetLength(0); j++)
				{
					MethodInfo methodInfo = methods[j];
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MenuItem), false);
					object[] array2 = customAttributes;
					int k = 0;
					while (k < array2.Length)
					{
						MenuItem menuItem = (MenuItem)array2[k];
						AttributeHelper.MonoMenuItem monoMenuItem = default(AttributeHelper.MonoMenuItem);
						if (hashtable[menuItem.menuItem] != null)
						{
							monoMenuItem = (AttributeHelper.MonoMenuItem)hashtable[menuItem.menuItem];
						}
						if (!menuItem.menuItem.StartsWith("internal:", StringComparison.Ordinal))
						{
							monoMenuItem.menuItem = menuItem.menuItem;
							goto IL_E7;
						}
						if (@bool)
						{
							monoMenuItem.menuItem = menuItem.menuItem.Substring(9);
							goto IL_E7;
						}
						IL_147:
						k++;
						continue;
						IL_E7:
						monoMenuItem.type = type;
						if (menuItem.validate)
						{
							monoMenuItem.validate = methodInfo.Name;
						}
						else
						{
							monoMenuItem.execute = methodInfo.Name;
							monoMenuItem.index = j;
							monoMenuItem.priority = menuItem.priority;
						}
						hashtable[menuItem.menuItem] = monoMenuItem;
						goto IL_147;
					}
				}
			}
			AttributeHelper.MonoMenuItem[] array3 = new AttributeHelper.MonoMenuItem[hashtable.Count];
			int num = 0;
			foreach (AttributeHelper.MonoMenuItem monoMenuItem2 in hashtable.Values)
			{
				array3[num++] = monoMenuItem2;
			}
			Array.Sort(array3, new AttributeHelper.CompareMenuIndex());
			return array3;
		}
		private static AttributeHelper.MonoMenuItem[] ExtractContextMenu(Type klass)
		{
			Hashtable hashtable = new Hashtable();
			MethodInfo[] methods = klass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.GetLength(0); i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ContextMenu), false);
				object[] array = customAttributes;
				for (int j = 0; j < array.Length; j++)
				{
					ContextMenu contextMenu = (ContextMenu)array[j];
					AttributeHelper.MonoMenuItem monoMenuItem = default(AttributeHelper.MonoMenuItem);
					if (hashtable[contextMenu.menuItem] != null)
					{
						monoMenuItem = (AttributeHelper.MonoMenuItem)hashtable[contextMenu.menuItem];
					}
					monoMenuItem.menuItem = contextMenu.menuItem;
					monoMenuItem.type = klass;
					monoMenuItem.execute = methodInfo.Name;
					hashtable[contextMenu.menuItem] = monoMenuItem;
				}
			}
			AttributeHelper.MonoMenuItem[] array2 = new AttributeHelper.MonoMenuItem[hashtable.Count];
			int num = 0;
			foreach (AttributeHelper.MonoMenuItem monoMenuItem2 in hashtable.Values)
			{
				array2[num++] = monoMenuItem2;
			}
			return array2;
		}
		private static string GetComponentMenuName(Type klass)
		{
			object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				return addComponentMenu.componentMenu;
			}
			return null;
		}
		private static int GetComponentMenuOrdering(Type klass)
		{
			object[] customAttributes = klass.GetCustomAttributes(typeof(AddComponentMenu), false);
			if (customAttributes.Length > 0)
			{
				AddComponentMenu addComponentMenu = (AddComponentMenu)customAttributes[0];
				return addComponentMenu.componentOrder;
			}
			return 0;
		}
		internal static ArrayList FindEditorClassesWithAttribute(Type attrib)
		{
			ArrayList arrayList = new ArrayList();
			foreach (Type current in EditorAssemblies.loadedTypes)
			{
				if (current.GetCustomAttributes(attrib, false).Length != 0)
				{
					arrayList.Add(current);
				}
			}
			return arrayList;
		}
		internal static void InvokeStaticMethod(Type type, string methodName, object[] arguments)
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, arguments);
			}
		}
		internal static void InvokeMethod(Type type, string methodName, object[] arguments)
		{
			MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, arguments);
			}
		}
		internal static object InvokeMemberIfAvailable(object target, string methodName, object[] args)
		{
			MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				return method.Invoke(target, args);
			}
			return null;
		}
		internal static bool GameObjectContainsAttribute(GameObject go, Type attributeType)
		{
			Component[] components = go.GetComponents(typeof(MonoBehaviour));
			for (int i = 0; i < components.Length; i++)
			{
				Component component = components[i];
				if (!(component == null))
				{
					Type type = component.GetType();
					if (type.GetCustomAttributes(attributeType, true).Length > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
