using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class VideoClipImporter : AssetImporter
	{
		public extern float quality
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool linearColor
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useLegacyImporter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ulong sourceFileSize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ulong outputFileSize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int frameCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern double frameRate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool keepAlpha
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool sourceHasAlpha
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern VideoDeinterlaceMode deinterlaceMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool flipVertical
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool flipHorizontal
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool importAudio
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public VideoImporterTargetSettings defaultTargetSettings
		{
			get
			{
				return this.GetTargetSettings(VideoClipImporter.defaultTargetName);
			}
			set
			{
				this.SetTargetSettings(VideoClipImporter.defaultTargetName, value);
			}
		}

		public extern bool isPlayingPreview
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string defaultTargetName
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern ushort sourceAudioTrackCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public VideoImporterTargetSettings GetTargetSettings(string platform)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			if (!platform.Equals(VideoClipImporter.defaultTargetName, StringComparison.OrdinalIgnoreCase) && buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
			}
			return this.Internal_GetTargetSettings(buildTargetGroupByName);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern VideoImporterTargetSettings Internal_GetTargetSettings(BuildTargetGroup group);

		public void SetTargetSettings(string platform, VideoImporterTargetSettings settings)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			if (!platform.Equals(VideoClipImporter.defaultTargetName, StringComparison.OrdinalIgnoreCase) && buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
			}
			this.Internal_SetTargetSettings(buildTargetGroupByName, settings);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_SetTargetSettings(BuildTargetGroup group, VideoImporterTargetSettings settings);

		public void ClearTargetSettings(string platform)
		{
			if (platform.Equals(VideoClipImporter.defaultTargetName, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Cannot clear the Default VideoClipTargetSettings.");
			}
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			if (buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				throw new ArgumentException("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XBox360', 'XboxOne', 'WP8', or 'WSA'");
			}
			this.Internal_ClearTargetSettings(buildTargetGroupByName);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_ClearTargetSettings(BuildTargetGroup group);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PlayPreview();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopPreview();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture GetPreviewTexture();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool EqualsDefaultTargetSettings(VideoImporterTargetSettings settings);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResizeModeName(VideoResizeMode mode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetResizeWidth(VideoResizeMode mode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetResizeHeight(VideoResizeMode mode);

		public ushort GetSourceAudioChannelCount(ushort audioTrackIdx)
		{
			return VideoClipImporter.INTERNAL_CALL_GetSourceAudioChannelCount(this, audioTrackIdx);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ushort INTERNAL_CALL_GetSourceAudioChannelCount(VideoClipImporter self, ushort audioTrackIdx);

		public uint GetSourceAudioSampleRate(ushort audioTrackIdx)
		{
			return VideoClipImporter.INTERNAL_CALL_GetSourceAudioSampleRate(this, audioTrackIdx);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint INTERNAL_CALL_GetSourceAudioSampleRate(VideoClipImporter self, ushort audioTrackIdx);
	}
}
