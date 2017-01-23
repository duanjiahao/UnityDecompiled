using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class LookDevPropertyInfo
	{
		[SerializeField]
		private bool m_Linked = false;

		[SerializeField]
		private LookDevPropertyType m_PropertyType;

		public LookDevPropertyType propertyType
		{
			get
			{
				return this.m_PropertyType;
			}
		}

		public bool linked
		{
			get
			{
				return this.m_Linked;
			}
			set
			{
				this.m_Linked = value;
			}
		}

		public LookDevPropertyInfo(LookDevPropertyType type)
		{
			this.m_PropertyType = type;
		}
	}
}
