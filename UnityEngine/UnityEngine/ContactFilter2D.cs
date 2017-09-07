using System;

namespace UnityEngine
{
	[Serializable]
	public struct ContactFilter2D
	{
		public bool useTriggers;

		public bool useLayerMask;

		public bool useDepth;

		public bool useOutsideDepth;

		public bool useNormalAngle;

		public bool useOutsideNormalAngle;

		public LayerMask layerMask;

		public float minDepth;

		public float maxDepth;

		public float minNormalAngle;

		public float maxNormalAngle;

		public const float NormalAngleUpperLimit = 359.9999f;

		public bool isFiltering
		{
			get
			{
				return !this.useTriggers || this.useLayerMask || this.useDepth || this.useNormalAngle;
			}
		}

		public ContactFilter2D NoFilter()
		{
			this.useTriggers = true;
			this.useLayerMask = false;
			this.layerMask = -1;
			this.useDepth = false;
			this.useOutsideDepth = false;
			this.minDepth = float.NegativeInfinity;
			this.maxDepth = float.PositiveInfinity;
			this.useNormalAngle = false;
			this.useOutsideNormalAngle = false;
			this.minNormalAngle = 0f;
			this.maxNormalAngle = 359.9999f;
			return this;
		}

		private void CheckConsistency()
		{
			this.minDepth = ((this.minDepth != float.NegativeInfinity && this.minDepth != float.PositiveInfinity && !float.IsNaN(this.minDepth)) ? this.minDepth : -3.40282347E+38f);
			this.maxDepth = ((this.maxDepth != float.NegativeInfinity && this.maxDepth != float.PositiveInfinity && !float.IsNaN(this.maxDepth)) ? this.maxDepth : 3.40282347E+38f);
			if (this.minDepth > this.maxDepth)
			{
				float num = this.minDepth;
				this.minDepth = this.maxDepth;
				this.maxDepth = num;
			}
			this.minNormalAngle = ((!float.IsNaN(this.minNormalAngle)) ? Mathf.Clamp(this.minNormalAngle, 0f, 359.9999f) : 0f);
			this.maxNormalAngle = ((!float.IsNaN(this.maxNormalAngle)) ? Mathf.Clamp(this.maxNormalAngle, 0f, 359.9999f) : 359.9999f);
			if (this.minNormalAngle > this.maxNormalAngle)
			{
				float num2 = this.minNormalAngle;
				this.minNormalAngle = this.maxNormalAngle;
				this.maxNormalAngle = num2;
			}
		}

		public void ClearLayerMask()
		{
			this.useLayerMask = false;
		}

		public void SetLayerMask(LayerMask layerMask)
		{
			this.layerMask = layerMask;
			this.useLayerMask = true;
		}

		public void ClearDepth()
		{
			this.useDepth = false;
		}

		public void SetDepth(float minDepth, float maxDepth)
		{
			this.minDepth = minDepth;
			this.maxDepth = maxDepth;
			this.useDepth = true;
			this.CheckConsistency();
		}

		public void ClearNormalAngle()
		{
			this.useNormalAngle = false;
		}

		public void SetNormalAngle(float minNormalAngle, float maxNormalAngle)
		{
			this.minNormalAngle = minNormalAngle;
			this.maxNormalAngle = maxNormalAngle;
			this.useNormalAngle = true;
			this.CheckConsistency();
		}

		public bool IsFilteringTrigger(Collider2D collider)
		{
			return !this.useTriggers && collider.isTrigger;
		}

		public bool IsFilteringLayerMask(GameObject obj)
		{
			return this.useLayerMask && (this.layerMask & 1 << obj.layer) == 0;
		}

		public bool IsFilteringDepth(GameObject obj)
		{
			bool result;
			if (!this.useDepth)
			{
				result = false;
			}
			else
			{
				if (this.minDepth > this.maxDepth)
				{
					float num = this.minDepth;
					this.minDepth = this.maxDepth;
					this.maxDepth = num;
				}
				float z = obj.transform.position.z;
				bool flag = z < this.minDepth || z > this.maxDepth;
				if (this.useOutsideDepth)
				{
					result = !flag;
				}
				else
				{
					result = flag;
				}
			}
			return result;
		}

		public bool IsFilteringNormalAngle(Vector2 normal)
		{
			float angle = Mathf.Atan2(normal.y, normal.x) * 57.29578f;
			return this.IsFilteringNormalAngle(angle);
		}

		public bool IsFilteringNormalAngle(float angle)
		{
			angle -= Mathf.Floor(angle / 359.9999f) * 359.9999f;
			float num = Mathf.Clamp(this.minNormalAngle, 0f, 359.9999f);
			float num2 = Mathf.Clamp(this.maxNormalAngle, 0f, 359.9999f);
			if (num > num2)
			{
				float num3 = num;
				num = num2;
				num2 = num3;
			}
			bool flag = angle < num || angle > num2;
			bool result;
			if (this.useOutsideNormalAngle)
			{
				result = !flag;
			}
			else
			{
				result = flag;
			}
			return result;
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
