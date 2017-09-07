using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	public sealed class ComponentUtility
	{
		public delegate bool IsDesiredComponent(Component c);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentUp(Component component);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentDown(Component component);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyComponent(Component component);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentValues(Component component);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentAsNew(GameObject go);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CollectConnectedComponents(GameObject targetGameObject, Component[] components, bool copy, out Component[] outCollectedComponents, out string outErrorMessage);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool MoveComponentToGameObject(Component component, GameObject targetGameObject, [DefaultValue("false")] bool validateOnly);

		[ExcludeFromDocs]
		internal static bool MoveComponentToGameObject(Component component, GameObject targetGameObject)
		{
			bool validateOnly = false;
			return ComponentUtility.MoveComponentToGameObject(component, targetGameObject, validateOnly);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool MoveComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget, [DefaultValue("false")] bool validateOnly);

		[ExcludeFromDocs]
		internal static bool MoveComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget)
		{
			bool validateOnly = false;
			return ComponentUtility.MoveComponentRelativeToComponent(component, targetComponent, aboveTarget, validateOnly);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool MoveComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget, [DefaultValue("false")] bool validateOnly);

		[ExcludeFromDocs]
		internal static bool MoveComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget)
		{
			bool validateOnly = false;
			return ComponentUtility.MoveComponentsRelativeToComponents(components, targetComponents, aboveTarget, validateOnly);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CopyComponentToGameObject(Component component, GameObject targetGameObject, bool validateOnly, out Component outNewComponent);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CopyComponentToGameObjects(Component component, GameObject[] targetGameObjects, bool validateOnly, out Component[] outNewComponents);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CopyComponentRelativeToComponent(Component component, Component targetComponent, bool aboveTarget, bool validateOnly, out Component outNewComponent);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CopyComponentRelativeToComponents(Component component, Component[] targetComponents, bool aboveTarget, bool validateOnly, out Component[] outNewComponents);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CopyComponentsRelativeToComponents(Component[] components, Component[] targetComponents, bool aboveTarget, bool validateOnly, out Component[] outNewComponents);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool WarnCanAddScriptComponent(GameObject gameObject, MonoScript script);

		private static bool CompareComponentOrderAndTypes(List<Component> srcComponents, List<Component> dstComponents)
		{
			bool result;
			if (srcComponents.Count != dstComponents.Count)
			{
				result = false;
			}
			else
			{
				for (int num = 0; num != srcComponents.Count; num++)
				{
					if (srcComponents[num].GetType() != dstComponents[num].GetType())
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private static void DestroyComponents(List<Component> components)
		{
			for (int i = components.Count - 1; i >= 0; i--)
			{
				UnityEngine.Object.DestroyImmediate(components[i]);
			}
		}

		public static void DestroyComponentsMatching(GameObject dst, ComponentUtility.IsDesiredComponent componentFilter)
		{
			List<Component> list = new List<Component>();
			dst.GetComponents<Component>(list);
			list.RemoveAll((Component x) => !componentFilter(x));
			ComponentUtility.DestroyComponents(list);
		}

		public static void ReplaceComponentsIfDifferent(GameObject src, GameObject dst, ComponentUtility.IsDesiredComponent componentFilter)
		{
			List<Component> list = new List<Component>();
			src.GetComponents<Component>(list);
			list.RemoveAll((Component x) => !componentFilter(x));
			List<Component> list2 = new List<Component>();
			dst.GetComponents<Component>(list2);
			list2.RemoveAll((Component x) => !componentFilter(x));
			if (!ComponentUtility.CompareComponentOrderAndTypes(list, list2))
			{
				ComponentUtility.DestroyComponents(list2);
				list2.Clear();
				for (int num = 0; num != list.Count; num++)
				{
					Component item = dst.AddComponent(list[num].GetType());
					list2.Add(item);
				}
			}
			for (int num2 = 0; num2 != list.Count; num2++)
			{
				EditorUtility.CopySerializedIfDifferent(list[num2], list2[num2]);
			}
		}
	}
}
