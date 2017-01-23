using System;

namespace UnityEngine.Rendering
{
	public enum BuiltinRenderTextureType
	{
		BindableTexture = -1,
		None,
		CurrentActive,
		CameraTarget,
		Depth,
		DepthNormals,
		ResolvedDepth,
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
