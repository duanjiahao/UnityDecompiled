using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class FoldoutObjectState
	{
		private GameObject m_Object;
		private bool m_Expanded;
		private bool m_Animated;
		public FoldoutComponentState[] m_Components;
		private int[] m_Children;
		public GameObject obj
		{
			get
			{
				return this.m_Object;
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
		public FoldoutComponentState[] components
		{
			get
			{
				return this.m_Components;
			}
		}
		public int[] children
		{
			get
			{
				return this.m_Children;
			}
		}
		public FoldoutObjectState(GameObject obj, List<FoldoutObjectState> states, SerializedStringTable expandedFoldouts, int level)
		{
			this.m_Object = obj;
			states.Add(this);
			if (level == 0)
			{
				this.m_Expanded = true;
			}
			else
			{
				if (expandedFoldouts.Contains(AnimationSelection.GetPath(obj.transform)))
				{
					this.m_Expanded = true;
				}
			}
			List<int> list = new List<int>();
			foreach (Transform transform in obj.transform)
			{
				list.Add(states.Count);
				new FoldoutObjectState(transform.gameObject, states, expandedFoldouts, level + 1);
			}
			this.m_Children = list.ToArray();
		}
		public void Expand(AnimationHierarchyData data, int level)
		{
			this.m_Expanded = true;
			if (level > 0)
			{
				data.expandedFoldouts.Set(AnimationSelection.GetPath(this.obj.transform));
			}
			data.animationWindow.SetDirtyCurves();
		}
		public void Collapse(AnimationHierarchyData data, int level)
		{
			this.m_Expanded = false;
			this.Hide(data, level);
			if (level > 0)
			{
				data.expandedFoldouts.Remove(AnimationSelection.GetPath(this.obj.transform));
			}
			data.animationWindow.SetDirtyCurves();
		}
		public void Hide(AnimationHierarchyData data, int level)
		{
			if (this.m_Components != null)
			{
				FoldoutComponentState[] components = this.m_Components;
				for (int i = 0; i < components.Length; i++)
				{
					FoldoutComponentState foldoutComponentState = components[i];
					if (foldoutComponentState.expanded)
					{
						foldoutComponentState.Hide(data, this.obj.transform, level);
					}
				}
			}
			int[] children = this.children;
			for (int j = 0; j < children.Length; j++)
			{
				int num = children[j];
				if (data.states[num].expanded)
				{
					data.states[num].Hide(data, level + 1);
				}
			}
		}
		public void RefreshAnimatedState(AnimationHierarchyData data)
		{
			this.m_Animated = false;
			string text = AnimationUtility.CalculateTransformPath(this.obj.transform, data.animated.transform);
			foreach (int num in data.animatedPaths.Keys)
			{
				if (num == text.GetHashCode())
				{
					this.m_Animated = true;
				}
			}
		}
		public void AddChildCurvesToList(List<CurveState> curves, AnimationHierarchyData data)
		{
			if (!this.expanded)
			{
				return;
			}
			FoldoutComponentState[] components = this.components;
			for (int i = 0; i < components.Length; i++)
			{
				FoldoutComponentState foldoutComponentState = components[i];
				foldoutComponentState.AddChildCurvesToList(curves, data);
			}
			int[] children = this.children;
			for (int j = 0; j < children.Length; j++)
			{
				int num = children[j];
				data.states[num].AddChildCurvesToList(curves, data);
			}
		}
	}
}
