using System;
using UnityEngine;

namespace UnityEditor
{
	internal struct ScriptingProceduralMaterialInformation
	{
		private Vector2 m_TextureOffsets;

		private Vector2 m_TextureScales;

		private int m_GenerateAllOutputs;

		private int m_AnimationUpdateRate;

		private bool m_GenerateMipMaps;

		public Vector2 offset
		{
			get
			{
				return this.m_TextureOffsets;
			}
			set
			{
				this.m_TextureOffsets = value;
			}
		}

		public Vector2 scale
		{
			get
			{
				return this.m_TextureScales;
			}
			set
			{
				this.m_TextureScales = value;
			}
		}

		public bool generateAllOutputs
		{
			get
			{
				return this.m_GenerateAllOutputs != 0;
			}
			set
			{
				this.m_GenerateAllOutputs = ((!value) ? 0 : 1);
			}
		}

		public int animationUpdateRate
		{
			get
			{
				return this.m_AnimationUpdateRate;
			}
			set
			{
				this.m_AnimationUpdateRate = value;
			}
		}

		public bool generateMipMaps
		{
			get
			{
				return this.m_GenerateMipMaps;
			}
			set
			{
				this.m_GenerateMipMaps = value;
			}
		}
	}
}
