using System;
using UnityEngine;
namespace UnityEditor
{
	[Serializable]
	public sealed class TextureImportInstructions
	{
		[SerializeField]
		public TextureFormat compressedFormat;
		[SerializeField]
		public TextureFormat uncompressedFormat;
		[SerializeField]
		public TextureFormat recommendedFormat;
		[SerializeField]
		public TextureFormat desiredFormat;
		[SerializeField]
		public TextureUsageMode usageMode;
		[SerializeField]
		public ColorSpace colorSpace;
		[SerializeField]
		public int width;
		[SerializeField]
		public int height;
		[SerializeField]
		public int compressionQuality;
	}
}
