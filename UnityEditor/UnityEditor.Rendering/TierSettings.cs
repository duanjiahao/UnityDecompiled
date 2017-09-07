using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering
{
	public struct TierSettings
	{
		public ShaderQuality standardShaderQuality;

		public CameraHDRMode hdrMode;

		public bool reflectionProbeBoxProjection;

		public bool reflectionProbeBlending;

		public bool hdr;

		public bool detailNormalMap;

		public bool cascadedShadowMaps;

		public bool prefer32BitShadowMaps;

		public bool enableLPPV;

		public bool semitransparentShadows;

		public RenderingPath renderingPath;

		public RealtimeGICPUUsage realtimeGICPUUsage;
	}
}
