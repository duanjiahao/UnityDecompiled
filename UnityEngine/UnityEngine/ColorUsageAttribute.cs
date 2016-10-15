using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class ColorUsageAttribute : PropertyAttribute
	{
		public readonly bool showAlpha = true;

		public readonly bool hdr;

		public readonly float minBrightness;

		public readonly float maxBrightness = 8f;

		public readonly float minExposureValue = 0.125f;

		public readonly float maxExposureValue = 3f;

		public ColorUsageAttribute(bool showAlpha)
		{
			this.showAlpha = showAlpha;
		}

		public ColorUsageAttribute(bool showAlpha, bool hdr, float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue)
		{
			this.showAlpha = showAlpha;
			this.hdr = hdr;
			this.minBrightness = minBrightness;
			this.maxBrightness = maxBrightness;
			this.minExposureValue = minExposureValue;
			this.maxExposureValue = maxExposureValue;
		}
	}
}
