using System;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	[UsedByNativeCode]
	[Serializable]
	public struct BuildCompression
	{
		public static readonly BuildCompression DefaultUncompressed = new BuildCompression
		{
			compression = CompressionType.None,
			level = CompressionLevel.Maximum,
			blockSize = 131072u
		};

		public static readonly BuildCompression DefaultLZ4 = new BuildCompression
		{
			compression = CompressionType.Lz4HC,
			level = CompressionLevel.Maximum,
			blockSize = 131072u
		};

		public static readonly BuildCompression DefaultLZMA = new BuildCompression
		{
			compression = CompressionType.Lzma,
			level = CompressionLevel.Maximum,
			blockSize = 131072u
		};

		public CompressionType compression;

		public CompressionLevel level;

		public uint blockSize;
	}
}
