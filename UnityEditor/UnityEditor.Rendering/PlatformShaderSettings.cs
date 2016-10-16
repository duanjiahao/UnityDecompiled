using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEditor.Rendering
{
	[RequiredByNativeCode]
	public struct PlatformShaderSettings
	{
		[MarshalAs(UnmanagedType.I1)]
		public bool cascadedShadowMaps;

		[MarshalAs(UnmanagedType.I1)]
		public bool reflectionProbeBoxProjection;

		[MarshalAs(UnmanagedType.I1)]
		public bool reflectionProbeBlending;

		public ShaderQuality standardShaderQuality;
	}
}
