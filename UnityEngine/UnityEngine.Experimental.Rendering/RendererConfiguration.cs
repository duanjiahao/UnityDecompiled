using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum RendererConfiguration
	{
		None = 0,
		PerObjectLightProbe = 1,
		PerObjectReflectionProbes = 2,
		PerObjectLightProbeProxyVolume = 4,
		PerObjectLightmaps = 8,
		ProvideLightIndices = 16
	}
}
