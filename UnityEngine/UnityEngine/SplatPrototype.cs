using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class SplatPrototype
	{
		internal Texture2D m_Texture;

		internal Texture2D m_NormalMap;

		internal Vector2 m_TileSize = new Vector2(15f, 15f);

		internal Vector2 m_TileOffset = new Vector2(0f, 0f);

		internal Vector4 m_SpecularMetallic = new Vector4(0f, 0f, 0f, 0f);

		internal float m_Smoothness;

		public Texture2D texture
		{
			get
			{
				return this.m_Texture;
			}
			set
			{
				this.m_Texture = value;
			}
		}

		public Texture2D normalMap
		{
			get
			{
				return this.m_NormalMap;
			}
			set
			{
				this.m_NormalMap = value;
			}
		}

		public Vector2 tileSize
		{
			get
			{
				return this.m_TileSize;
			}
			set
			{
				this.m_TileSize = value;
			}
		}

		public Vector2 tileOffset
		{
			get
			{
				return this.m_TileOffset;
			}
			set
			{
				this.m_TileOffset = value;
			}
		}

		public Color specular
		{
			get
			{
				return new Color(this.m_SpecularMetallic.x, this.m_SpecularMetallic.y, this.m_SpecularMetallic.z);
			}
			set
			{
				this.m_SpecularMetallic.x = value.r;
				this.m_SpecularMetallic.y = value.g;
				this.m_SpecularMetallic.z = value.b;
			}
		}

		public float metallic
		{
			get
			{
				return this.m_SpecularMetallic.w;
			}
			set
			{
				this.m_SpecularMetallic.w = value;
			}
		}

		public float smoothness
		{
			get
			{
				return this.m_Smoothness;
			}
			set
			{
				this.m_Smoothness = value;
			}
		}
	}
}
