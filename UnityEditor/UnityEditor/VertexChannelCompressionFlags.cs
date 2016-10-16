using System;

namespace UnityEditor
{
	[Flags]
	public enum VertexChannelCompressionFlags
	{
		kPosition = 1,
		kNormal = 2,
		kColor = 4,
		kUV0 = 8,
		kUV1 = 16,
		kUV2 = 32,
		kUV3 = 64,
		kTangent = 128
	}
}
