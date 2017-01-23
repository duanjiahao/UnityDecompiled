using System;

namespace UnityEditor
{
	[Obsolete("TargetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
	public enum TargetIOSGraphics
	{
		OpenGLES_2_0 = 2,
		OpenGLES_3_0,
		Metal,
		Automatic = -1
	}
}
