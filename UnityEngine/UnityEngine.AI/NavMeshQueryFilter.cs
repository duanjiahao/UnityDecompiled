using System;

namespace UnityEngine.AI
{
	public struct NavMeshQueryFilter
	{
		private const int AREA_COST_ELEMENT_COUNT = 32;

		private int m_AreaMask;

		private int m_AgentTypeID;

		private float[] m_AreaCost;

		internal float[] costs
		{
			get
			{
				return this.m_AreaCost;
			}
		}

		public int areaMask
		{
			get
			{
				return this.m_AreaMask;
			}
			set
			{
				this.m_AreaMask = value;
			}
		}

		public int agentTypeID
		{
			get
			{
				return this.m_AgentTypeID;
			}
			set
			{
				this.m_AgentTypeID = value;
			}
		}

		public float GetAreaCost(int areaIndex)
		{
			float result;
			if (this.m_AreaCost == null)
			{
				if (areaIndex < 0 || areaIndex >= 32)
				{
					string message = string.Format("The valid range is [0:{0}]", 31);
					throw new IndexOutOfRangeException(message);
				}
				result = 1f;
			}
			else
			{
				result = this.m_AreaCost[areaIndex];
			}
			return result;
		}

		public void SetAreaCost(int areaIndex, float cost)
		{
			if (this.m_AreaCost == null)
			{
				this.m_AreaCost = new float[32];
				for (int i = 0; i < 32; i++)
				{
					this.m_AreaCost[i] = 1f;
				}
			}
			this.m_AreaCost[areaIndex] = cost;
		}
	}
}
