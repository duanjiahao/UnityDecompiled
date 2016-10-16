using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public enum GraphicsDeviceType
	{
		OpenGL2,
		Direct3D9,
		Direct3D11,
		PlayStation3,
		Null,
		Xbox360 = 6,
		OpenGLES2 = 8,
		OpenGLES3 = 11,
		PlayStationVita,
		PlayStation4,
		XboxOne,
		PlayStationMobile,
		Metal,
		OpenGLCore,
		Direct3D12,
		Nintendo3DS
	}
}
