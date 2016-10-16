using System;

namespace UnityEditor
{
	[Obsolete("TargetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs")]
	public enum TargetGlesGraphics
	{
		[Obsolete("OpenGL ES 1.x is deprecated, ES 2.0 will be used instead")]
		OpenGLES_1_x,
		OpenGLES_2_0,
		OpenGLES_3_0,
		Automatic = -1
	}
}
