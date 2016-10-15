using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	public class ColorPickerHDRConfig
	{
		[SerializeField]
		public float minBrightness;

		[SerializeField]
		public float maxBrightness;

		[SerializeField]
		public float minExposureValue;

		[SerializeField]
		public float maxExposureValue;

		private static readonly ColorPickerHDRConfig s_Temp = new ColorPickerHDRConfig(0f, 0f, 0f, 0f);

		public ColorPickerHDRConfig(float minBrightness, float maxBrightness, float minExposureValue, float maxExposureValue)
		{
			this.minBrightness = minBrightness;
			this.maxBrightness = maxBrightness;
			this.minExposureValue = minExposureValue;
			this.maxExposureValue = maxExposureValue;
		}

		internal ColorPickerHDRConfig(ColorPickerHDRConfig other)
		{
			this.minBrightness = other.minBrightness;
			this.maxBrightness = other.maxBrightness;
			this.minExposureValue = other.minExposureValue;
			this.maxExposureValue = other.maxExposureValue;
		}

		internal static ColorPickerHDRConfig Temp(float minBrightness, float maxBrightness, float minExposure, float maxExposure)
		{
			ColorPickerHDRConfig.s_Temp.minBrightness = minBrightness;
			ColorPickerHDRConfig.s_Temp.maxBrightness = maxBrightness;
			ColorPickerHDRConfig.s_Temp.minExposureValue = minExposure;
			ColorPickerHDRConfig.s_Temp.maxExposureValue = maxExposure;
			return ColorPickerHDRConfig.s_Temp;
		}
	}
}
