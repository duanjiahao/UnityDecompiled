using System;

namespace UnityEngine.Rendering
{
	[Flags]
	public enum ShadowMapPass
	{
		PointlightPositiveX = 1,
		PointlightNegativeX = 2,
		PointlightPositiveY = 4,
		PointlightNegativeY = 8,
		PointlightPositiveZ = 16,
		PointlightNegativeZ = 32,
		DirectionalCascade0 = 64,
		DirectionalCascade1 = 128,
		DirectionalCascade2 = 256,
		DirectionalCascade3 = 512,
		Spotlight = 1024,
		Pointlight = 63,
		Directional = 960,
		All = 2047
	}
}
