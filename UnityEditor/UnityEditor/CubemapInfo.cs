using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[Serializable]
	internal class CubemapInfo
	{
		private const float kDefaultShadowIntensity = 0.3f;

		public Cubemap cubemap;

		public CubemapInfo cubemapShadowInfo;

		public float angleOffset = 0f;

		public SphericalHarmonicsL2 ambientProbe;

		public ShadowInfo shadowInfo = new ShadowInfo();

		public int serialIndexMain;

		public int serialIndexShadow;

		[NonSerialized]
		public bool alreadyComputed;

		public void SetCubemapShadowInfo(CubemapInfo newCubemapShadowInfo)
		{
			this.cubemapShadowInfo = newCubemapShadowInfo;
			this.shadowInfo.shadowIntensity = ((newCubemapShadowInfo != this) ? 1f : 0.3f);
			this.shadowInfo.shadowColor = Color.white;
		}

		public void ResetEnvInfos()
		{
			this.angleOffset = 0f;
		}
	}
}
