using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class CurveBindingUtility
	{
		private static GameObject s_Root;

		public static Type GetEditorCurveValueType(GameObject rootGameObject, EditorCurveBinding curveBinding)
		{
			if (rootGameObject != null)
			{
				return AnimationUtility.GetEditorCurveValueType(rootGameObject, curveBinding);
			}
			return CurveBindingUtility.GetEditorCurveValueType(curveBinding);
		}

		public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
		{
			if (rootGameObject != null)
			{
				return AnimationWindowUtility.GetCurrentValue(rootGameObject, curveBinding);
			}
			return CurveBindingUtility.GetCurrentValue(curveBinding);
		}

		public static void SampleAnimationClip(GameObject rootGameObject, AnimationClip clip, float time)
		{
			if (rootGameObject != null)
			{
				AnimationMode.SampleAnimationClip(rootGameObject, clip, time);
			}
			else
			{
				CurveBindingUtility.SampleAnimationClip(clip, time);
			}
		}

		public static void Cleanup()
		{
			if (CurveBindingUtility.s_Root != null)
			{
				UnityEngine.Object.DestroyImmediate(CurveBindingUtility.s_Root);
				EditorUtility.UnloadUnusedAssetsImmediate();
			}
		}

		private static Type GetEditorCurveValueType(EditorCurveBinding curveBinding)
		{
			CurveBindingUtility.PrepareHierarchy(curveBinding);
			return AnimationUtility.GetEditorCurveValueType(CurveBindingUtility.s_Root, curveBinding);
		}

		private static object GetCurrentValue(EditorCurveBinding curveBinding)
		{
			CurveBindingUtility.PrepareHierarchy(curveBinding);
			return AnimationWindowUtility.GetCurrentValue(CurveBindingUtility.s_Root, curveBinding);
		}

		private static void SampleAnimationClip(AnimationClip clip, float time)
		{
			CurveBindingUtility.PrepareHierarchy(clip);
			AnimationMode.SampleAnimationClip(CurveBindingUtility.s_Root, clip, time);
		}

		private static void PrepareHierarchy(AnimationClip clip)
		{
			EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
			EditorCurveBinding[] array = curveBindings;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = array[i];
				GameObject gameObject = CurveBindingUtility.CreateOrGetGameObject(editorCurveBinding.path);
				if (gameObject.GetComponent(editorCurveBinding.type) == null)
				{
					Component component = gameObject.AddComponent(editorCurveBinding.type);
					Behaviour behaviour = component as Behaviour;
					if (behaviour != null)
					{
						behaviour.enabled = false;
					}
				}
			}
		}

		private static void PrepareHierarchy(EditorCurveBinding curveBinding)
		{
			GameObject gameObject = CurveBindingUtility.CreateOrGetGameObject(curveBinding.path);
			if (gameObject.GetComponent(curveBinding.type) == null)
			{
				Component component = gameObject.AddComponent(curveBinding.type);
				Behaviour behaviour = component as Behaviour;
				if (behaviour != null)
				{
					behaviour.enabled = false;
				}
			}
		}

		private static GameObject CreateOrGetGameObject(string path)
		{
			if (CurveBindingUtility.s_Root == null)
			{
				CurveBindingUtility.s_Root = CurveBindingUtility.CreateNewGameObject(null, "Root");
			}
			if (path.Length == 0)
			{
				return CurveBindingUtility.s_Root;
			}
			string[] array = path.Split(new char[]
			{
				'/'
			});
			Transform transform = CurveBindingUtility.s_Root.transform;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string name = array2[i];
				Transform transform2 = transform.FindChild(name);
				if (transform2 == null)
				{
					transform = CurveBindingUtility.CreateNewGameObject(transform, name).transform;
				}
				else
				{
					transform = transform2;
				}
			}
			return transform.gameObject;
		}

		private static GameObject CreateNewGameObject(Transform parent, string name)
		{
			GameObject gameObject = new GameObject(name);
			if (parent != null)
			{
				gameObject.transform.parent = parent;
			}
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			return gameObject;
		}
	}
}
