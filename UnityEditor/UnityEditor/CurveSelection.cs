using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class CurveSelection : IComparable
	{
		internal enum SelectionType
		{
			Key,
			InTangent,
			OutTangent,
			Count
		}

		[NonSerialized]
		internal CurveEditor m_Host;

		private int m_CurveID;

		private int m_Key = -1;

		internal bool semiSelected;

		internal CurveSelection.SelectionType type;

		internal CurveWrapper curveWrapper
		{
			get
			{
				return this.m_Host.GetCurveFromID(this.m_CurveID);
			}
		}

		internal AnimationCurve curve
		{
			get
			{
				return (this.curveWrapper == null) ? null : this.curveWrapper.curve;
			}
		}

		public int curveID
		{
			get
			{
				return this.m_CurveID;
			}
			set
			{
				this.m_CurveID = value;
			}
		}

		public int key
		{
			get
			{
				return this.m_Key;
			}
			set
			{
				this.m_Key = value;
			}
		}

		internal Keyframe keyframe
		{
			get
			{
				if (this.validKey())
				{
					return this.curve[this.m_Key];
				}
				return default(Keyframe);
			}
		}

		internal CurveSelection(int curveID, CurveEditor host, int keyIndex)
		{
			this.m_CurveID = curveID;
			this.m_Host = host;
			this.m_Key = keyIndex;
			this.type = CurveSelection.SelectionType.Key;
		}

		internal CurveSelection(int curveID, CurveEditor host, int keyIndex, CurveSelection.SelectionType t)
		{
			this.m_CurveID = curveID;
			this.m_Host = host;
			this.m_Key = keyIndex;
			this.type = t;
		}

		internal bool validKey()
		{
			return this.curve != null && this.m_Key >= 0 && this.m_Key < this.curve.length;
		}

		public int CompareTo(object _other)
		{
			CurveSelection curveSelection = (CurveSelection)_other;
			int num = this.curveID - curveSelection.curveID;
			if (num != 0)
			{
				return num;
			}
			num = this.key - curveSelection.key;
			if (num != 0)
			{
				return num;
			}
			return this.type - curveSelection.type;
		}

		public override bool Equals(object _other)
		{
			CurveSelection curveSelection = (CurveSelection)_other;
			return curveSelection.curveID == this.curveID && curveSelection.key == this.key && curveSelection.type == this.type;
		}

		public override int GetHashCode()
		{
			return (int)(this.curveID * 729 + this.key * 27 + this.type);
		}
	}
}
