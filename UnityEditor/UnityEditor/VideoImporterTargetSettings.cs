using System;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	public sealed class VideoImporterTargetSettings
	{
		public bool enableTranscoding;

		public VideoCodec codec;

		public VideoResizeMode resizeMode;

		public VideoEncodeAspectRatio aspectRatio;

		public int customWidth;

		public int customHeight;

		public VideoBitrateMode bitrateMode;

		public VideoSpatialQuality spatialQuality;
	}
}
