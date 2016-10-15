using System;
using UnityEngine;

namespace UnityEditor
{
	public struct AudioImporterSampleSettings
	{
		public AudioClipLoadType loadType;

		public AudioSampleRateSetting sampleRateSetting;

		public uint sampleRateOverride;

		public AudioCompressionFormat compressionFormat;

		public float quality;

		public int conversionMode;
	}
}
