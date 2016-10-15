using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class DopeLine
	{
		public static GUIStyle dopekeyStyle = "Dopesheetkeyframe";

		public Rect position;

		public AnimationWindowCurve[] m_Curves;

		public List<AnimationWindowKeyframe> keys;

		public int m_HierarchyNodeID;

		public Type objectType;

		public bool tallMode;

		public bool hasChildren;

		public bool isMasterDopeline;

		public Type valueType
		{
			get
			{
				if (this.m_Curves.Length > 0)
				{
					Type valueType = this.m_Curves[0].m_ValueType;
					for (int i = 1; i < this.m_Curves.Length; i++)
					{
						if (this.m_Curves[i].m_ValueType != valueType)
						{
							return null;
						}
					}
					return valueType;
				}
				return null;
			}
		}

		public bool isPptrDopeline
		{
			get
			{
				if (this.m_Curves.Length > 0)
				{
					for (int i = 0; i < this.m_Curves.Length; i++)
					{
						if (!this.m_Curves[i].isPPtrCurve)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		public bool isEditable
		{
			get
			{
				if (this.m_Curves.Length > 0)
				{
					bool flag = Array.Exists<AnimationWindowCurve>(this.m_Curves, (AnimationWindowCurve curve) => !curve.animationIsEditable);
					return !flag;
				}
				return false;
			}
		}

		public DopeLine(int hierarchyNodeId, AnimationWindowCurve[] curves)
		{
			this.m_HierarchyNodeID = hierarchyNodeId;
			this.m_Curves = curves;
			this.LoadKeyframes();
		}

		public void LoadKeyframes()
		{
			this.keys = new List<AnimationWindowKeyframe>();
			AnimationWindowCurve[] curves = this.m_Curves;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
				{
					this.keys.Add(current);
				}
			}
			this.keys.Sort((AnimationWindowKeyframe a, AnimationWindowKeyframe b) => a.time.CompareTo(b.time));
		}
	}
}
