using System;

namespace UnityEditor
{
	[Obsolete("TargetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
	public enum TargetGlesGraphics
	{
		OpenGLES_1_x,
		OpenGLES_2_0,
		OpenGLES_3_0,
		Automatic = -1
	}
}
