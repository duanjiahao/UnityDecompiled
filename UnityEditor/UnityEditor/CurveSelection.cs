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

		[SerializeField]
		public int curveID = 0;

		[SerializeField]
		public int key = -1;

		[SerializeField]
		public bool semiSelected = false;

		[SerializeField]
		public CurveSelection.SelectionType type;

		internal CurveSelection(int curveID, int key)
		{
			this.curveID = curveID;
			this.key = key;
			this.type = CurveSelection.SelectionType.Key;
		}

		internal CurveSelection(int curveID, int key, CurveSelection.SelectionType type)
		{
			this.curveID = curveID;
			this.key = key;
			this.type = type;
		}

		public int CompareTo(object _other)
		{
			CurveSelection curveSelection = (CurveSelection)_other;
			int num = this.curveID - curveSelection.curveID;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				num = this.key - curveSelection.key;
				if (num != 0)
				{
					result = num;
				}
				else
				{
					result = this.type - curveSelection.type;
				}
			}
			return result;
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
