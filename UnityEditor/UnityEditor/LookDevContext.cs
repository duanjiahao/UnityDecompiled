using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class LookDevContext
	{
		[Serializable]
		public class LookDevPropertyValue
		{
			public float floatValue = 0f;

			public int intValue = 0;
		}

		[SerializeField]
		private LookDevContext.LookDevPropertyValue[] m_Properties = new LookDevContext.LookDevPropertyValue[5];

		public float exposureValue
		{
			get
			{
				return this.m_Properties[0].floatValue;
			}
		}

		public float envRotation
		{
			get
			{
				return this.m_Properties[3].floatValue;
			}
			set
			{
				this.m_Properties[3].floatValue = value;
			}
		}

		public int currentHDRIIndex
		{
			get
			{
				return this.m_Properties[1].intValue;
			}
			set
			{
				this.m_Properties[1].intValue = value;
			}
		}

		public int shadingMode
		{
			get
			{
				return this.m_Properties[2].intValue;
			}
		}

		public int lodIndex
		{
			get
			{
				return this.m_Properties[4].intValue;
			}
		}

		public LookDevContext()
		{
			for (int i = 0; i < 5; i++)
			{
				this.m_Properties[i] = new LookDevContext.LookDevPropertyValue();
			}
			this.m_Properties[0].floatValue = 0f;
			this.m_Properties[1].intValue = 0;
			this.m_Properties[2].intValue = -1;
			this.m_Properties[4].intValue = -1;
			this.m_Properties[3].floatValue = 0f;
		}

		public LookDevContext.LookDevPropertyValue GetProperty(LookDevProperty property)
		{
			return this.m_Properties[(int)property];
		}

		public void UpdateProperty(LookDevProperty property, float value)
		{
			this.m_Properties[(int)property].floatValue = value;
		}

		public void UpdateProperty(LookDevProperty property, int value)
		{
			this.m_Properties[(int)property].intValue = value;
		}
	}
}
