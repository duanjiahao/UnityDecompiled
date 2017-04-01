using System;

namespace UnityEngine
{
	[Flags]
	public enum AdditionalCanvasShaderChannels
	{
		None = 0,
		TexCoord1 = 1,
		TexCoord2 = 2,
		TexCoord3 = 4,
		Normal = 8,
		Tangent = 16
	}
}
