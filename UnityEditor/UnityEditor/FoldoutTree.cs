using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	internal class FoldoutTree
	{
		private bool m_Locked;
		private FoldoutObjectState[] m_States;
		public bool locked
		{
			get
			{
				return this.m_Locked;
			}
			set
			{
				this.m_Locked = value;
			}
		}
		public FoldoutObjectState[] states
		{
			get
			{
				return this.m_States;
			}
		}
		public FoldoutObjectState root
		{
			get
			{
				return this.states[0];
			}
		}
		public GameObject rootGameObject
		{
			get
			{
				return this.root.obj;
			}
		}
		public Transform rootTransform
		{
			get
			{
				return this.root.obj.transform;
			}
		}
		public FoldoutTree(Transform tr, SerializedStringTable expandedFoldouts)
		{
			List<FoldoutObjectState> list = new List<FoldoutObjectState>();
			new FoldoutObjectState(tr.gameObject, list, expandedFoldouts, 0);
			this.m_States = list.ToArray();
		}
		public void Refresh(AnimationHierarchyData data)
		{
		}
	}
}
