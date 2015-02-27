using System;
using UnityEngine;
namespace UnityEditor
{
	internal class CurveState
	{
		private EditorCurveBinding m_CurveBinding;
		private bool m_Animated;
		public AnimationSelection animationSelection;
		public Color color;
		public Vector2 guiPosition;
		public bool even;
		public Type type
		{
			get
			{
				return this.m_CurveBinding.type;
			}
			set
			{
				this.m_CurveBinding.type = value;
			}
		}
		public string propertyName
		{
			get
			{
				return this.m_CurveBinding.propertyName;
			}
			set
			{
				this.m_CurveBinding.propertyName = value;
			}
		}
		public string path
		{
			get
			{
				return this.m_CurveBinding.path;
			}
			set
			{
				this.m_CurveBinding.path = value;
			}
		}
		public EditorCurveBinding curveBinding
		{
			get
			{
				return this.m_CurveBinding;
			}
			set
			{
				this.m_CurveBinding = value;
			}
		}
		public bool animated
		{
			get
			{
				return this.m_Animated;
			}
			set
			{
				this.m_Animated = value;
			}
		}
		public AnimationCurve curve
		{
			get
			{
				CurveRenderer curveRenderer = CurveRendererCache.GetCurveRenderer(this.clip, this.m_CurveBinding);
				return curveRenderer.GetCurve();
			}
		}
		public AnimationClip clip
		{
			get
			{
				return this.animationSelection.clip;
			}
		}
		public CurveState(EditorCurveBinding binding)
		{
			this.m_CurveBinding = binding;
		}
		public void SaveCurve(AnimationCurve animationCurve)
		{
			Undo.RegisterCompleteObjectUndo(this.clip, "Edit Curve");
			QuaternionCurveTangentCalculation.UpdateTangentsFromMode(animationCurve, this.clip, this.m_CurveBinding);
			AnimationUtility.SetEditorCurve(this.clip, this.m_CurveBinding, animationCurve);
		}
		public int GetID()
		{
			return CurveUtility.GetCurveID(this.clip, this.m_CurveBinding);
		}
		public int GetGroupID()
		{
			return CurveUtility.GetCurveGroupID(this.clip, this.m_CurveBinding);
		}
		public float GetSampledOrCurveValue(float time)
		{
			if (this.animated)
			{
				CurveRenderer curveRenderer = CurveRendererCache.GetCurveRenderer(this.clip, this.m_CurveBinding);
				if (curveRenderer == null)
				{
					Debug.LogError("The renderer is null!");
				}
				return curveRenderer.EvaluateCurveSlow(time);
			}
			float result;
			if (!AnimationUtility.GetFloatValue(this.animationSelection.animatedObject, this.m_CurveBinding, out result))
			{
				result = float.PositiveInfinity;
			}
			return result;
		}
	}
}
