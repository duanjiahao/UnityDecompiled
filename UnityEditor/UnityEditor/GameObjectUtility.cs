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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StaticEditorFlags GetStaticEditorFlags(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticEditorFlags(GameObject go, StaticEditorFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AreStaticEditorFlagsSet(GameObject go, StaticEditorFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshArea(GameObject go);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNavMeshArea(GameObject go, int areaIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshAreaFromName(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetNavMeshAreaNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetUniqueNameForSibling(Transform parent, string name);

		internal static bool ContainsStatic(GameObject[] objects)
		{
			bool result;
			if (objects == null || objects.Length == 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < objects.Length; i++)
				{
					if (objects[i] != null && objects[i].isStatic)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		internal static bool HasChildren(IEnumerable<GameObject> gameObjects)
		{
			return gameObjects.Any((GameObject go) => go.transform.childCount > 0);
		}

		internal static GameObjectUtility.ShouldIncludeChildren DisplayUpdateChildrenDialogIfNeeded(IEnumerable<GameObject> gameObjects, string title, string message)
		{
			GameObjectUtility.ShouldIncludeChildren result;
			if (!GameObjectUtility.HasChildren(gameObjects))
			{
				result = GameObjectUtility.ShouldIncludeChildren.HasNoChildren;
			}
			else
			{
				result = (GameObjectUtility.ShouldIncludeChildren)EditorUtility.DisplayDialogComplex(title, message, "Yes, change children", "No, this object only", "Cancel");
			}
			return result;
		}

		public static void SetParentAndAlign(GameObject child, GameObject parent)
		{
			if (!(parent == null))
			{
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
				GameObjectUtility.SetLayerRecursively(child, parent.layer);
			}
		}

		private static void SetLayerRecursively(GameObject go, int layer)
		{
			go.layer = layer;
			Transform transform = go.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObjectUtility.SetLayerRecursively(transform.GetChild(i).gameObject, layer);
			}
		}

		[Obsolete("GetNavMeshArea instead.")]
		public static int GetNavMeshLayer(GameObject go)
		{
			return GameObjectUtility.GetNavMeshArea(go);
		}

		[Obsolete("SetNavMeshArea instead.")]
		public static void SetNavMeshLayer(GameObject go, int areaIndex)
		{
			GameObjectUtility.SetNavMeshArea(go, areaIndex);
		}

		[Obsolete("GetNavMeshAreaFromName instead.")]
		public static int GetNavMeshLayerFromName(string name)
		{
			return GameObjectUtility.GetNavMeshAreaFromName(name);
		}

		[Obsolete("GetNavMeshAreaNames instead.")]
		public static string[] GetNavMeshLayerNames()
		{
			return GameObjectUtility.GetNavMeshAreaNames();
		}

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
