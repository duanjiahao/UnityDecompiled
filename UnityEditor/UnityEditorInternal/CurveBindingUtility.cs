using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class CurveBindingUtility
	{
		private static GameObject s_Root;

		public static object GetCurrentValue(AnimationWindowState state, AnimationWindowCurve curve)
		{
			object result;
			if (AnimationMode.InAnimationMode() && curve.rootGameObject != null)
			{
				result = AnimationWindowUtility.GetCurrentValue(curve.rootGameObject, curve.binding);
			}
			else
			{
				result = curve.Evaluate(state.currentTime - curve.timeOffset);
			}
			return result;
		}

		public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
		{
			object result;
			if (rootGameObject != null)
			{
				result = AnimationWindowUtility.GetCurrentValue(rootGameObject, curveBinding);
			}
			else if (curveBinding.isPPtrCurve)
			{
				result = null;
			}
			else
			{
				result = 0f;
			}
			return result;
		}
	}
}
