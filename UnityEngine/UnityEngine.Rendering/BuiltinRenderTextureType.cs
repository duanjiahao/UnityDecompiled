using System;

namespace UnityEngine.Rendering
{
	public enum BuiltinRenderTextureType
	{
		None,
		CurrentActive,
		CameraTarget,
		Depth,
		DepthNormals,
		PrepassNormalsSpec = 7,
		PrepassLight,
		PrepassLightSpec,
		GBuffer0,
		GBuffer1,
		GBuffer2,
		GBuffer3,
		Reflections
	}
}
