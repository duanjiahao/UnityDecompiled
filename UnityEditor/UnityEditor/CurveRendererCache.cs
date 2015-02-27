using System;
using System.Collections;
using UnityEngine;
namespace UnityEditor
{
	internal class CurveRendererCache
	{
		private static Hashtable m_CombiRenderers = new Hashtable();
		private static Hashtable m_NormalRenderers = new Hashtable();
		public static void ClearCurveRendererCache()
		{
			CurveRendererCache.m_CombiRenderers = new Hashtable();
			CurveRendererCache.m_NormalRenderers = new Hashtable();
		}
		public static CurveRenderer GetCurveRenderer(AnimationClip clip, EditorCurveBinding curveBinding)
		{
			if (curveBinding.type == typeof(Transform) && curveBinding.propertyName.StartsWith("localEulerAngles."))
			{
				int curveIndexFromName = RotationCurveInterpolation.GetCurveIndexFromName(curveBinding.propertyName);
				string key = CurveUtility.GetCurveGroupID(clip, curveBinding).ToString();
				EulerCurveCombinedRenderer eulerCurveCombinedRenderer = (EulerCurveCombinedRenderer)CurveRendererCache.m_CombiRenderers[key];
				if (eulerCurveCombinedRenderer == null)
				{
					eulerCurveCombinedRenderer = new EulerCurveCombinedRenderer(AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.x")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.y")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.z")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "m_LocalRotation.w")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.x")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.y")), AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(curveBinding.path, typeof(Transform), "localEulerAngles.z")));
					CurveRendererCache.m_CombiRenderers.Add(key, eulerCurveCombinedRenderer);
				}
				return new EulerCurveRenderer(curveIndexFromName, eulerCurveCombinedRenderer);
			}
			string key2 = CurveUtility.GetCurveID(clip, curveBinding).ToString();
			NormalCurveRenderer normalCurveRenderer = (NormalCurveRenderer)CurveRendererCache.m_NormalRenderers[key2];
			if (normalCurveRenderer == null)
			{
				normalCurveRenderer = new NormalCurveRenderer(AnimationUtility.GetEditorCurve(clip, curveBinding));
				CurveRendererCache.m_NormalRenderers.Add(key2, normalCurveRenderer);
			}
			return normalCurveRenderer;
		}
	}
}
