using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Rendering
{
	[Obsolete("Use TierSettings instead (UnityUpgradable) -> UnityEditor.Rendering.TierSettings", false)]
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
