using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	public sealed class GameObjectUtility
	{
		internal enum ShouldIncludeChildren
		{
			HasNoChildren = -1,
			IncludeChildren,
			DontIncludeChildren,
			Cancel
		}
		internal static bool ContainsStatic(GameObject[] objects)
		{
			if (objects == null || objects.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] != null && objects[i].isStatic)
				{
					return true;
				}
			}
			return false;
		}
		internal static bool HasChildren(IEnumerable<GameObject> gameObjects)
		{
			return gameObjects.Any((GameObject go) => go.transform.childCount > 0);
		}
		internal static GameObjectUtility.ShouldIncludeChildren DisplayUpdateChildrenDialogIfNeeded(IEnumerable<GameObject> gameObjects, string title, string message)
		{
			if (!GameObjectUtility.HasChildren(gameObjects))
			{
				return GameObjectUtility.ShouldIncludeChildren.HasNoChildren;
			}
			return (GameObjectUtility.ShouldIncludeChildren)EditorUtility.DisplayDialogComplex(title, message, "Yes, change children", "No, this object only", "Cancel");
		}
		public static void SetParentAndAlign(GameObject child, GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			child.transform.SetParent(parent.transform, false);
			RectTransform rectTransform = child.transform as RectTransform;
			if (rectTransform)
			{
				rectTransform.anchoredPosition = Vector2.zero;
				Vector3 localPosition = rectTransform.localPosition;
				localPosition.z = 0f;
				rectTransform.localPosition = localPosition;
			}
			else
			{
				child.transform.localPosition = Vector3.zero;
			}
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
			child.layer = parent.layer;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StaticEditorFlags GetStaticEditorFlags(GameObject go);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AreStaticEditorFlagsSet(GameObject go, StaticEditorFlags flags);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticEditorFlags(GameObject go, StaticEditorFlags flags);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshLayer(GameObject go);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshLayerFromName(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNavMeshLayer(GameObject go, int layer);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetNavMeshLayerNames();
		[Obsolete("use AnimatorUtility.OptimizeTransformHierarchy instead.")]
		private static void OptimizeTransformHierarchy(GameObject go)
		{
			AnimatorUtility.OptimizeTransformHierarchy(go, null);
		}
		[Obsolete("use AnimatorUtility.DeoptimizeTransformHierarchy instead.")]
		private static void DeoptimizeTransformHierarchy(GameObject go)
		{
			AnimatorUtility.DeoptimizeTransformHierarchy(go);
		}
	}
}
