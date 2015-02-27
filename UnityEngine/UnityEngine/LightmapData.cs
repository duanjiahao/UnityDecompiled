using System;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LightmapData
	{
		internal Texture2D m_Lightmap;
		internal Texture2D m_IndirectLightmap;
		public Texture2D lightmapFar
		{
			get
			{
				return this.m_Lightmap;
			}
			set
			{
				this.m_Lightmap = value;
			}
		}
		[Obsolete("Use lightmapFar instead")]
		public Texture2D lightmap
		{
			get
			{
				return this.m_Lightmap;
			}
			set
			{
				this.m_Lightmap = value;
			}
		}
		public Texture2D lightmapNear
		{
			get
			{
				return this.m_IndirectLightmap;
			}
			set
			{
				this.m_IndirectLightmap = value;
			}
		}
	}
}
