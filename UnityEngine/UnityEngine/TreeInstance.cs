using System;
namespace UnityEngine
{
	public struct TreeInstance
	{
		private Vector3 m_Position;
		private float m_WidthScale;
		private float m_HeightScale;
		private Color32 m_Color;
		private Color32 m_LightmapColor;
		private int m_Index;
		private float m_TemporaryDistance;
		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}
		public float widthScale
		{
			get
			{
				return this.m_WidthScale;
			}
			set
			{
				this.m_WidthScale = value;
			}
		}
		public float heightScale
		{
			get
			{
				return this.m_HeightScale;
			}
			set
			{
				this.m_HeightScale = value;
			}
		}
		public Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				this.m_Color = value;
			}
		}
		public Color lightmapColor
		{
			get
			{
				return this.m_LightmapColor;
			}
			set
			{
				this.m_LightmapColor = value;
			}
		}
		public int prototypeIndex
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				this.m_Index = value;
			}
		}
		internal float temporaryDistance
		{
			get
			{
				return this.m_TemporaryDistance;
			}
			set
			{
				this.m_TemporaryDistance = value;
			}
		}
	}
}
