using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class ComponentUtility
	{
		public delegate bool IsDesiredComponent(Component c);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentUp(Component component);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool MoveComponentDown(Component component);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyComponent(Component component);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentValues(Component component);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool PasteComponentAsNew(GameObject go);

		private static bool CompareComponentOrderAndTypes(List<Component> srcComponents, List<Component> dstComponents)
		{
			if (srcComponents.Count != dstComponents.Count)
			{
				return false;
			}
			for (int num = 0; num != srcComponents.Count; num++)
			{
				if (srcComponents[num].GetType() != dstComponents[num].GetType())
				{
					return false;
				}
			}
			return true;
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
