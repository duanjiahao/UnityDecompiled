using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class DoubleCurve
	{
		[SerializeField]
		private AnimationCurve m_MinCurve;

		[SerializeField]
		private AnimationCurve m_MaxCurve;

		[SerializeField]
		private bool m_SignedRange;

		public AnimationCurve minCurve
		{
			get
			{
				return this.m_MinCurve;
			}
			set
			{
				this.m_MinCurve = value;
			}
		}

		public AnimationCurve maxCurve
		{
			get
			{
				return this.m_MaxCurve;
			}
			set
			{
				this.m_MaxCurve = value;
			}
		}

		public bool signedRange
		{
			get
			{
				return this.m_SignedRange;
			}
			set
			{
				this.m_SignedRange = value;
			}
		}

		public DoubleCurve(AnimationCurve minCurve, AnimationCurve maxCurve, bool signedRange)
		{
			if (minCurve != null)
			{
				AnimationCurve animationCurve = new AnimationCurve(minCurve.keys);
				this.m_MinCurve = animationCurve;
			}
			if (maxCurve != null)
			{
				AnimationCurve animationCurve = new AnimationCurve(maxCurve.keys);
				this.m_MaxCurve = animationCurve;
			}
			else
			{
				Debug.LogError("Ensure that maxCurve is not null when creating a double curve. The minCurve can be null for single curves");
			}
			this.m_SignedRange = signedRange;
		}

		public bool IsSingleCurve()
		{
			return this.minCurve == null || this.minCurve.length == 0;
		}
	}
}
