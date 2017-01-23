using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public enum GraphicsDeviceType
	{
		[Obsolete("OpenGL2 is no longer supported in Unity 5.5+")]
		OpenGL2,
		Direct3D9,
		Direct3D11,
		[Obsolete("PS3 is no longer supported in Unity 5.5+")]
		PlayStation3,
		Null,
		[Obsolete("Xbox360 is no longer supported in Unity 5.5+")]
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
		N3DS,
		Vulkan = 21
	}
}
