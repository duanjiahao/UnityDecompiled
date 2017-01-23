using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class DopeLine
	{
		private int m_HierarchyNodeID;

		private AnimationWindowCurve[] m_Curves;

		private List<AnimationWindowKeyframe> m_Keys;

		public static GUIStyle dopekeyStyle = "Dopesheetkeyframe";

		public Rect position;

		public Type objectType;

		public bool tallMode;

		public bool hasChildren;

		public bool isMasterDopeline;

		public Type valueType
		{
			get
			{
				Type result;
				if (this.m_Curves.Length > 0)
				{
					Type valueType = this.m_Curves[0].valueType;
					for (int i = 1; i < this.m_Curves.Length; i++)
					{
						if (this.m_Curves[i].valueType != valueType)
						{
							result = null;
							return result;
						}
					}
					result = valueType;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public bool isPptrDopeline
		{
			get
			{
				bool result;
				if (this.m_Curves.Length > 0)
				{
					for (int i = 0; i < this.m_Curves.Length; i++)
					{
						if (!this.m_Curves[i].isPPtrCurve)
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		public bool isEditable
		{
			get
			{
				bool result;
				if (this.m_Curves.Length > 0)
				{
					bool flag = Array.Exists<AnimationWindowCurve>(this.m_Curves, (AnimationWindowCurve curve) => !curve.animationIsEditable);
					result = !flag;
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		public int hierarchyNodeID
		{
			get
			{
				return this.m_HierarchyNodeID;
			}
		}

		public AnimationWindowCurve[] curves
		{
			get
			{
				return this.m_Curves;
			}
		}

		public List<AnimationWindowKeyframe> keys
		{
			get
			{
				if (this.m_Keys == null)
				{
					this.m_Keys = new List<AnimationWindowKeyframe>();
					AnimationWindowCurve[] curves = this.m_Curves;
					for (int i = 0; i < curves.Length; i++)
					{
						AnimationWindowCurve animationWindowCurve = curves[i];
						foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
						{
							this.m_Keys.Add(current);
						}
					}
					this.m_Keys.Sort((AnimationWindowKeyframe a, AnimationWindowKeyframe b) => a.time.CompareTo(b.time));
				}
				return this.m_Keys;
			}
		}

		public DopeLine(int hierarchyNodeID, AnimationWindowCurve[] curves)
		{
			this.m_HierarchyNodeID = hierarchyNodeID;
			this.m_Curves = curves;
		}

		public void InvalidateKeyframes()
		{
			this.m_Keys = null;
		}
	}
}
