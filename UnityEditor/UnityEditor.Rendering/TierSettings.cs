using System;
using UnityEngine;

namespace UnityEditor.Rendering
{
	public struct TierSettings
	{
		public ShaderQuality standardShaderQuality;

		public bool cascadedShadowMaps;

		public bool reflectionProbeBoxProjection;

		public bool reflectionProbeBlending;

		public RenderingPath renderingPath;
	}
}
