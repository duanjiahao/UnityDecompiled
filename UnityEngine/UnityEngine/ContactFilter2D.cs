using System;

namespace UnityEngine
{
	[Serializable]
	public struct ContactFilter2D
	{
		public bool useTriggers;

		public bool useLayerMask;

		public bool useDepth;

		public bool useNormalAngle;

		public LayerMask layerMask;

		public float minDepth;

		public float maxDepth;

		public float minNormalAngle;

		public float maxNormalAngle;

		public ContactFilter2D NoFilter()
		{
			this.useTriggers = false;
			this.useLayerMask = false;
			this.layerMask = -1;
			this.useDepth = false;
			this.minDepth = float.NegativeInfinity;
			this.maxDepth = float.PositiveInfinity;
			this.useNormalAngle = false;
			this.minNormalAngle = float.NegativeInfinity;
			this.maxNormalAngle = float.PositiveInfinity;
			return this;
		}

		public void SetLayerMask(LayerMask layerMask)
		{
			this.layerMask = layerMask;
			this.useLayerMask = true;
		}

		public void ClearLayerMask()
		{
			this.useLayerMask = false;
		}

		public void SetDepth(float minDepth, float maxDepth)
		{
			this.minDepth = minDepth;
			this.maxDepth = maxDepth;
			this.useDepth = true;
		}

		public void ClearDepth()
		{
			this.useDepth = false;
		}

		public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
		{
			this.minNormalAngle = minNormalAngle;
			this.maxNormalAngle = maxNormalAngle;
			this.useNormalAngle = true;
		}

		public void ClearNormalAngle()
		{
			this.useNormalAngle = false;
		}

		internal static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
		{
			ContactFilter2D result = default(ContactFilter2D);
			result.useTriggers = Physics2D.queriesHitTriggers;
			result.SetLayerMask(layerMask);
			result.SetDepth(minDepth, maxDepth);
			return result;
		}
	}
}
