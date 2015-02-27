using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class FoldoutComponentState
	{
		private UnityEngine.Object m_Object;
		private bool m_Expanded;
		private bool m_Animated;
		private CurveState[] m_CurveStates;
		public UnityEngine.Object obj
		{
			get
			{
				return this.m_Object;
			}
			set
			{
				this.m_Object = value;
			}
		}
		public bool expanded
		{
			get
			{
				return this.m_Expanded;
			}
		}
		public bool animated
		{
			get
			{
				return this.m_Animated;
			}
		}
		public CurveState[] curveStates
		{
			get
			{
				return this.m_CurveStates;
			}
		}
		public FoldoutComponentState(UnityEngine.Object obj, SerializedStringTable expandedFoldouts, Transform tr, int level)
		{
			this.m_Object = obj;
			if (level == 0 && this.m_Object.GetType() == typeof(Transform))
			{
				this.m_Expanded = true;
			}
			else
			{
				if (expandedFoldouts.Contains(this.GetString(tr)))
				{
					this.m_Expanded = true;
				}
			}
		}
		public string GetString(Transform tr)
		{
			return AnimationSelection.GetPath(tr) + "/:" + ObjectNames.GetInspectorTitle(this.obj);
		}
		public void Expand(AnimationHierarchyData data, Transform tr, int level)
		{
			this.m_Expanded = true;
			if (level > 0 || this.m_Object.GetType() != typeof(Transform))
			{
				data.expandedFoldouts.Set(this.GetString(tr));
			}
			data.animationWindow.SetDirtyCurves();
		}
		public void Collapse(AnimationHierarchyData data, Transform tr, int level)
		{
			this.m_Expanded = false;
			this.Hide(data, tr, level);
			if (level > 0 || this.m_Object.GetType() != typeof(Transform))
			{
				data.expandedFoldouts.Remove(this.GetString(tr));
			}
			data.animationWindow.SetDirtyCurves();
		}
		public void Hide(AnimationHierarchyData data, Transform tr, int level)
		{
			this.m_CurveStates = null;
		}
		public void RefreshAnimatedState(AnimationHierarchyData data, Transform tr)
		{
			this.m_Animated = false;
			string path = AnimationUtility.CalculateTransformPath(tr, data.animated.transform);
			foreach (int num in data.animatedPaths.Keys)
			{
				if (num == CurveUtility.GetPathAndTypeID(path, this.obj.GetType()))
				{
					this.m_Animated = true;
					break;
				}
			}
		}
		public void AddChildCurvesToList(List<CurveState> curves, AnimationHierarchyData data)
		{
			if (!this.expanded)
			{
				return;
			}
			CurveState[] curveStates = this.curveStates;
			for (int i = 0; i < curveStates.Length; i++)
			{
				CurveState curveState = curveStates[i];
				if (curveState.animated || data.animationWindow.showAllProperties)
				{
					curves.Add(curveState);
				}
			}
		}
	}
}
