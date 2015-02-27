using System;
using UnityEngine;
namespace UnityEditor.Sprites
{
	public struct AtlasSettings
	{
		public TextureFormat format;
		public TextureUsageMode usageMode;
		public ColorSpace colorSpace;
		public int compressionQuality;
		public FilterMode filterMode;
		public int maxWidth;
		public int maxHeight;
		public uint paddingPower;
		public int anisoLevel;
		public bool generateMipMaps;
	}
}
