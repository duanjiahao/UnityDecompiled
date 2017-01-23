using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class AudioImporter : AssetImporter
	{
		public extern AudioImporterSampleSettings defaultSampleSettings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool forceToMono
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool loadInBackground
		{
			get
			{
				return this.Internal_GetLoadInBackground();
			}
			set
			{
				this.Internal_SetLoadInBackground(value);
			}
		}

		public bool preloadAudioData
		{
			get
			{
				return this.Internal_GetPreloadAudioData();
			}
			set
			{
				this.Internal_SetPreloadAudioData(value);
			}
		}

		[Obsolete("Setting and getting the compression format is not used anymore (use compressionFormat in defaultSampleSettings instead). Source audio file is assumed to be PCM Wav.")]
		private AudioImporterFormat format
		{
			get
			{
				return (this.defaultSampleSettings.compressionFormat != AudioCompressionFormat.PCM) ? AudioImporterFormat.Compressed : AudioImporterFormat.Native;
			}
			set
			{
				AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
				defaultSampleSettings.compressionFormat = ((value != AudioImporterFormat.Native) ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM);
				this.defaultSampleSettings = defaultSampleSettings;
			}
		}

		[Obsolete("Setting and getting import channels is not used anymore (use forceToMono instead)", true)]
		public AudioImporterChannels channels
		{
			get
			{
				return AudioImporterChannels.Automatic;
			}
			set
			{
			}
		}

		[Obsolete("AudioImporter.compressionBitrate is no longer supported", true)]
		public extern int compressionBitrate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("AudioImporter.loopable is no longer supported. All audio assets encoded by Unity are by default loopable.")]
		public extern bool loopable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("AudioImporter.hardware is no longer supported. All mixing of audio is done by software and only some platforms use hardware acceleration to perform decoding.")]
		public extern bool hardware
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Setting/Getting decompressOnLoad is deprecated. Use AudioImporterSampleSettings.loadType instead.")]
		private bool decompressOnLoad
		{
			get
			{
				return this.defaultSampleSettings.loadType == AudioClipLoadType.DecompressOnLoad;
			}
			set
			{
				AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
				defaultSampleSettings.loadType = ((!value) ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad);
				this.defaultSampleSettings = defaultSampleSettings;
			}
		}

		[Obsolete("AudioImporter.quality is no longer supported. Use AudioImporterSampleSettings.")]
		private float quality
		{
			get
			{
				return this.defaultSampleSettings.quality;
			}
			set
			{
				AudioImporterSampleSettings defaultSampleSettings = this.defaultSampleSettings;
				defaultSampleSettings.quality = value;
				this.defaultSampleSettings = defaultSampleSettings;
			}
		}

		[Obsolete("AudioImporter.threeD is no longer supported")]
		public extern bool threeD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("AudioImporter.durationMS is deprecated.", true)]
		internal extern int durationMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.frequency is deprecated.", true)]
		internal extern int frequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.origChannelCount is deprecated.", true)]
		internal extern int origChannelCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.origIsCompressible is deprecated.", true)]
		internal extern bool origIsCompressible
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.origIsMonoForcable is deprecated.", true)]
		internal extern bool origIsMonoForcable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.defaultBitrate is deprecated.", true)]
		internal extern int defaultBitrate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("AudioImporter.origType is deprecated.", true)]
		internal AudioType origType
		{
			get
			{
				return AudioType.UNKNOWN;
			}
		}

		[Obsolete("AudioImporter.origFileSize is deprecated.", true)]
		internal extern int origFileSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int origSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int compSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public bool ContainsSampleSettingsOverride(string platform)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			bool result;
			if (buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				Debug.LogError("Unknown platform passed to AudioImporter.ContainsSampleSettingsOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
				result = false;
			}
			else
			{
				result = this.Internal_ContainsSampleSettingsOverride(buildTargetGroupByName);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool Internal_ContainsSampleSettingsOverride(BuildTargetGroup platformGroup);

		public AudioImporterSampleSettings GetOverrideSampleSettings(string platform)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			AudioImporterSampleSettings result;
			if (buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				Debug.LogError("Unknown platform passed to AudioImporter.GetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
				result = this.defaultSampleSettings;
			}
			else
			{
				result = this.Internal_GetOverrideSampleSettings(buildTargetGroupByName);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AudioImporterSampleSettings Internal_GetOverrideSampleSettings(BuildTargetGroup platformGroup);

		public bool SetOverrideSampleSettings(string platform, AudioImporterSampleSettings settings)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			bool result;
			if (buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				Debug.LogError("Unknown platform passed to AudioImporter.SetOverrideSampleSettings (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
				result = false;
			}
			else
			{
				result = this.Internal_SetOverrideSampleSettings(buildTargetGroupByName, settings);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool Internal_SetOverrideSampleSettings(BuildTargetGroup platformGroup, AudioImporterSampleSettings settings);

		public bool ClearSampleSettingOverride(string platform)
		{
			BuildTargetGroup buildTargetGroupByName = BuildPipeline.GetBuildTargetGroupByName(platform);
			bool result;
			if (buildTargetGroupByName == BuildTargetGroup.Unknown)
			{
				Debug.LogError("Unknown platform passed to AudioImporter.ClearSampleSettingOverride (" + platform + "), please use one of 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'PSM', 'XboxOne' or 'WSA'");
				result = false;
			}
			else
			{
				result = this.Internal_ClearSampleSettingOverride(buildTargetGroupByName);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool Internal_ClearSampleSettingOverride(BuildTargetGroup platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetLoadInBackground(bool flag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_GetLoadInBackground();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetPreloadAudioData(bool flag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_GetPreloadAudioData();

		[Obsolete("AudioImporter.updateOrigData is deprecated.", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void updateOrigData();

		[Obsolete("AudioImporter.minBitrate is deprecated.", true)]
		internal int minBitrate(AudioType type)
		{
			return 0;
		}

		[Obsolete("AudioImporter.maxBitrate is deprecated.", true)]
		internal int maxBitrate(AudioType type)
		{
			return 0;
		}
	}
}
