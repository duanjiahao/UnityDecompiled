using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioImporter))]
	internal class AudioImporterInspector : AssetImporterInspector
	{
		private static class Styles
		{
			public static readonly string[] kSampleRateStrings = new string[]
			{
				"8,000 Hz",
				"11,025 Hz",
				"22,050 Hz",
				"44,100 Hz",
				"48,000 Hz",
				"96,000 Hz",
				"192,000 Hz"
			};

			public static readonly int[] kSampleRateValues = new int[]
			{
				8000,
				11025,
				22050,
				44100,
				48000,
				96000,
				192000
			};
		}

		private struct MultiValueStatus
		{
			public bool multiLoadType;

			public bool multiSampleRateSetting;

			public bool multiSampleRateOverride;

			public bool multiCompressionFormat;

			public bool multiQuality;

			public bool multiConversionMode;
		}

		private struct SampleSettingProperties
		{
			public AudioImporterSampleSettings settings;

			public bool forcedOverrideState;

			public bool overrideIsForced;

			public bool loadTypeChanged;

			public bool sampleRateSettingChanged;

			public bool sampleRateOverrideChanged;

			public bool compressionFormatChanged;

			public bool qualityChanged;

			public bool conversionModeChanged;

			public bool HasModified()
			{
				return this.overrideIsForced || this.loadTypeChanged || this.sampleRateSettingChanged || this.sampleRateOverrideChanged || this.compressionFormatChanged || this.qualityChanged || this.conversionModeChanged;
			}

			public void ClearChangedFlags()
			{
				this.forcedOverrideState = false;
				this.overrideIsForced = false;
				this.loadTypeChanged = false;
				this.sampleRateSettingChanged = false;
				this.sampleRateOverrideChanged = false;
				this.compressionFormatChanged = false;
				this.qualityChanged = false;
				this.conversionModeChanged = false;
			}
		}

		private enum OverrideStatus
		{
			NoOverrides,
			MixedOverrides,
			AllOverrides
		}

		public SerializedProperty m_ForceToMono;

		public SerializedProperty m_Normalize;

		public SerializedProperty m_PreloadAudioData;

		public SerializedProperty m_LoadInBackground;

		public SerializedProperty m_OrigSize;

		public SerializedProperty m_CompSize;

		private AudioImporterInspector.SampleSettingProperties m_DefaultSampleSettings;

		private Dictionary<BuildTargetGroup, AudioImporterInspector.SampleSettingProperties> m_SampleSettingOverrides;

		[DebuggerHidden]
		private IEnumerable<AudioImporter> GetAllAudioImporterTargets()
		{
			AudioImporterInspector.<GetAllAudioImporterTargets>c__Iterator7 <GetAllAudioImporterTargets>c__Iterator = new AudioImporterInspector.<GetAllAudioImporterTargets>c__Iterator7();
			<GetAllAudioImporterTargets>c__Iterator.<>f__this = this;
			AudioImporterInspector.<GetAllAudioImporterTargets>c__Iterator7 expr_0E = <GetAllAudioImporterTargets>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}

		private bool SyncSettingsToBackend()
		{
			BuildPlayerWindow.BuildPlatform[] array = BuildPlayerWindow.GetValidPlatforms().ToArray();
			foreach (AudioImporter current in this.GetAllAudioImporterTargets())
			{
				AudioImporterSampleSettings defaultSampleSettings = current.defaultSampleSettings;
				if (this.m_DefaultSampleSettings.loadTypeChanged)
				{
					defaultSampleSettings.loadType = this.m_DefaultSampleSettings.settings.loadType;
				}
				if (this.m_DefaultSampleSettings.sampleRateSettingChanged)
				{
					defaultSampleSettings.sampleRateSetting = this.m_DefaultSampleSettings.settings.sampleRateSetting;
				}
				if (this.m_DefaultSampleSettings.sampleRateOverrideChanged)
				{
					defaultSampleSettings.sampleRateOverride = this.m_DefaultSampleSettings.settings.sampleRateOverride;
				}
				if (this.m_DefaultSampleSettings.compressionFormatChanged)
				{
					defaultSampleSettings.compressionFormat = this.m_DefaultSampleSettings.settings.compressionFormat;
				}
				if (this.m_DefaultSampleSettings.qualityChanged)
				{
					defaultSampleSettings.quality = this.m_DefaultSampleSettings.settings.quality;
				}
				if (this.m_DefaultSampleSettings.conversionModeChanged)
				{
					defaultSampleSettings.conversionMode = this.m_DefaultSampleSettings.settings.conversionMode;
				}
				current.defaultSampleSettings = defaultSampleSettings;
				BuildPlayerWindow.BuildPlatform[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					BuildPlayerWindow.BuildPlatform buildPlatform = array2[i];
					BuildTargetGroup targetGroup = buildPlatform.targetGroup;
					if (this.m_SampleSettingOverrides.ContainsKey(targetGroup))
					{
						AudioImporterInspector.SampleSettingProperties value = this.m_SampleSettingOverrides[targetGroup];
						if (value.overrideIsForced && !value.forcedOverrideState)
						{
							current.Internal_ClearSampleSettingOverride(targetGroup);
						}
						else if (current.Internal_ContainsSampleSettingsOverride(targetGroup) || (value.overrideIsForced && value.forcedOverrideState))
						{
							AudioImporterSampleSettings settings = current.Internal_GetOverrideSampleSettings(targetGroup);
							if (value.loadTypeChanged)
							{
								settings.loadType = value.settings.loadType;
							}
							if (value.sampleRateSettingChanged)
							{
								settings.sampleRateSetting = value.settings.sampleRateSetting;
							}
							if (value.sampleRateOverrideChanged)
							{
								settings.sampleRateOverride = value.settings.sampleRateOverride;
							}
							if (value.compressionFormatChanged)
							{
								settings.compressionFormat = value.settings.compressionFormat;
							}
							if (value.qualityChanged)
							{
								settings.quality = value.settings.quality;
							}
							if (value.conversionModeChanged)
							{
								settings.conversionMode = value.settings.conversionMode;
							}
							current.Internal_SetOverrideSampleSettings(targetGroup, settings);
						}
						this.m_SampleSettingOverrides[targetGroup] = value;
					}
				}
			}
			this.m_DefaultSampleSettings.ClearChangedFlags();
			BuildPlayerWindow.BuildPlatform[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				BuildPlayerWindow.BuildPlatform buildPlatform2 = array3[j];
				BuildTargetGroup targetGroup2 = buildPlatform2.targetGroup;
				if (this.m_SampleSettingOverrides.ContainsKey(targetGroup2))
				{
					AudioImporterInspector.SampleSettingProperties value2 = this.m_SampleSettingOverrides[targetGroup2];
					value2.ClearChangedFlags();
					this.m_SampleSettingOverrides[targetGroup2] = value2;
				}
			}
			return true;
		}

		private bool ResetSettingsFromBackend()
		{
			if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
			{
				AudioImporter audioImporter = this.GetAllAudioImporterTargets().First<AudioImporter>();
				this.m_DefaultSampleSettings.settings = audioImporter.defaultSampleSettings;
				this.m_DefaultSampleSettings.ClearChangedFlags();
				this.m_SampleSettingOverrides = new Dictionary<BuildTargetGroup, AudioImporterInspector.SampleSettingProperties>();
				List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
				foreach (BuildPlayerWindow.BuildPlatform current in validPlatforms)
				{
					BuildTargetGroup targetGroup = current.targetGroup;
					foreach (AudioImporter current2 in this.GetAllAudioImporterTargets())
					{
						if (current2.Internal_ContainsSampleSettingsOverride(targetGroup))
						{
							AudioImporterInspector.SampleSettingProperties value = default(AudioImporterInspector.SampleSettingProperties);
							value.settings = current2.Internal_GetOverrideSampleSettings(targetGroup);
							this.m_SampleSettingOverrides[targetGroup] = value;
							break;
						}
					}
					if (!this.m_SampleSettingOverrides.ContainsKey(targetGroup))
					{
						AudioImporterInspector.SampleSettingProperties value2 = default(AudioImporterInspector.SampleSettingProperties);
						value2.settings = audioImporter.Internal_GetOverrideSampleSettings(targetGroup);
						this.m_SampleSettingOverrides[targetGroup] = value2;
					}
				}
			}
			return true;
		}

		public bool CurrentPlatformHasAutoTranslatedCompression()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			foreach (AudioImporter current in this.GetAllAudioImporterTargets())
			{
				AudioCompressionFormat compressionFormat = current.defaultSampleSettings.compressionFormat;
				if (!current.Internal_ContainsSampleSettingsOverride(buildTargetGroup))
				{
					AudioCompressionFormat compressionFormat2 = current.Internal_GetOverrideSampleSettings(buildTargetGroup).compressionFormat;
					if (compressionFormat != compressionFormat2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsHardwareSound(AudioCompressionFormat format)
		{
			switch (format)
			{
			case AudioCompressionFormat.VAG:
			case AudioCompressionFormat.HEVAG:
			case AudioCompressionFormat.XMA:
			case AudioCompressionFormat.GCADPCM:
				return true;
			}
			return false;
		}

		public bool CurrentSelectionContainsHardwareSounds()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			foreach (AudioImporter current in this.GetAllAudioImporterTargets())
			{
				if (this.IsHardwareSound(current.Internal_GetOverrideSampleSettings(buildTargetGroup).compressionFormat))
				{
					return true;
				}
			}
			return false;
		}

		public void OnEnable()
		{
			this.m_ForceToMono = base.serializedObject.FindProperty("m_ForceToMono");
			this.m_Normalize = base.serializedObject.FindProperty("m_Normalize");
			this.m_PreloadAudioData = base.serializedObject.FindProperty("m_PreloadAudioData");
			this.m_LoadInBackground = base.serializedObject.FindProperty("m_LoadInBackground");
			this.m_OrigSize = base.serializedObject.FindProperty("m_PreviewData.m_OrigSize");
			this.m_CompSize = base.serializedObject.FindProperty("m_PreviewData.m_CompSize");
			this.ResetSettingsFromBackend();
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.ResetSettingsFromBackend();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.UpdateIfDirtyOrScript();
			bool selectionContainsTrackerFile = false;
			foreach (AudioImporter current in this.GetAllAudioImporterTargets())
			{
				string assetPath = current.assetPath;
				string a = FileUtil.GetPathExtension(assetPath).ToLowerInvariant();
				if (a == "mod" || a == "it" || a == "s3m" || a == "xm")
				{
					selectionContainsTrackerFile = true;
					break;
				}
			}
			this.OnAudioImporterGUI(selectionContainsTrackerFile);
			int num = 0;
			int num2 = 0;
			foreach (AudioImporter current2 in this.GetAllAudioImporterTargets())
			{
				num += current2.origSize;
				num2 += current2.compSize;
			}
			GUILayout.Space(10f);
			EditorGUILayout.HelpBox(string.Concat(new string[]
			{
				"Original Size: \t",
				EditorUtility.FormatBytes(num),
				"\nImported Size: \t",
				EditorUtility.FormatBytes(num2),
				"\nRatio: \t\t",
				(100f * (float)num2 / (float)num).ToString("0.00"),
				"%"
			}), MessageType.Info);
			if (this.CurrentPlatformHasAutoTranslatedCompression())
			{
				GUILayout.Space(10f);
				EditorGUILayout.HelpBox("The selection contains different compression formats to the default settings for the current build platform.", MessageType.Info);
			}
			if (this.CurrentSelectionContainsHardwareSounds())
			{
				GUILayout.Space(10f);
				EditorGUILayout.HelpBox("The selection contains sounds that are decompressed in hardware. Advanced mixing is not available for these sounds.", MessageType.Info);
			}
			base.ApplyRevertGUI();
		}

		private AudioImporterInspector.MultiValueStatus GetMultiValueStatus(BuildTargetGroup platform)
		{
			AudioImporterInspector.MultiValueStatus result;
			result.multiLoadType = false;
			result.multiSampleRateSetting = false;
			result.multiSampleRateOverride = false;
			result.multiCompressionFormat = false;
			result.multiQuality = false;
			result.multiConversionMode = false;
			if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
			{
				AudioImporter audioImporter = this.GetAllAudioImporterTargets().First<AudioImporter>();
				AudioImporterSampleSettings audioImporterSampleSettings;
				if (platform == BuildTargetGroup.Unknown)
				{
					audioImporterSampleSettings = audioImporter.defaultSampleSettings;
				}
				else
				{
					audioImporterSampleSettings = audioImporter.Internal_GetOverrideSampleSettings(platform);
				}
				foreach (AudioImporter current in this.GetAllAudioImporterTargets().Except(new AudioImporter[]
				{
					audioImporter
				}))
				{
					AudioImporterSampleSettings audioImporterSampleSettings2;
					if (platform == BuildTargetGroup.Unknown)
					{
						audioImporterSampleSettings2 = current.defaultSampleSettings;
					}
					else
					{
						audioImporterSampleSettings2 = current.Internal_GetOverrideSampleSettings(platform);
					}
					result.multiLoadType |= (audioImporterSampleSettings.loadType != audioImporterSampleSettings2.loadType);
					result.multiSampleRateSetting |= (audioImporterSampleSettings.sampleRateSetting != audioImporterSampleSettings2.sampleRateSetting);
					result.multiSampleRateOverride |= (audioImporterSampleSettings.sampleRateOverride != audioImporterSampleSettings2.sampleRateOverride);
					result.multiCompressionFormat |= (audioImporterSampleSettings.compressionFormat != audioImporterSampleSettings2.compressionFormat);
					result.multiQuality |= (audioImporterSampleSettings.quality != audioImporterSampleSettings2.quality);
					result.multiConversionMode |= (audioImporterSampleSettings.conversionMode != audioImporterSampleSettings2.conversionMode);
				}
			}
			return result;
		}

		private AudioImporterInspector.OverrideStatus GetOverrideStatus(BuildTargetGroup platform)
		{
			bool flag = false;
			bool flag2 = false;
			if (this.GetAllAudioImporterTargets().Any<AudioImporter>())
			{
				AudioImporter audioImporter = this.GetAllAudioImporterTargets().First<AudioImporter>();
				flag2 = audioImporter.Internal_ContainsSampleSettingsOverride(platform);
				foreach (AudioImporter current in this.GetAllAudioImporterTargets().Except(new AudioImporter[]
				{
					audioImporter
				}))
				{
					bool flag3 = current.Internal_ContainsSampleSettingsOverride(platform);
					if (flag3 != flag2)
					{
						flag |= true;
					}
					flag2 |= flag3;
				}
			}
			if (!flag2)
			{
				return AudioImporterInspector.OverrideStatus.NoOverrides;
			}
			if (flag)
			{
				return AudioImporterInspector.OverrideStatus.MixedOverrides;
			}
			return AudioImporterInspector.OverrideStatus.AllOverrides;
		}

		private AudioCompressionFormat[] GetFormatsForPlatform(BuildTargetGroup platform)
		{
			List<AudioCompressionFormat> list = new List<AudioCompressionFormat>();
			if (platform == BuildTargetGroup.WebGL)
			{
				list.Add(AudioCompressionFormat.AAC);
				return list.ToArray();
			}
			list.Add(AudioCompressionFormat.PCM);
			if (platform != BuildTargetGroup.PS3 && platform != BuildTargetGroup.PSM && platform != BuildTargetGroup.PSP2)
			{
				list.Add(AudioCompressionFormat.Vorbis);
			}
			list.Add(AudioCompressionFormat.ADPCM);
			if (platform != BuildTargetGroup.Standalone && platform != BuildTargetGroup.Metro && platform != BuildTargetGroup.WiiU && platform != BuildTargetGroup.XboxOne && platform != BuildTargetGroup.XBOX360 && platform != BuildTargetGroup.Unknown)
			{
				list.Add(AudioCompressionFormat.MP3);
			}
			if (platform == BuildTargetGroup.PSM)
			{
				list.Add(AudioCompressionFormat.VAG);
			}
			if (platform == BuildTargetGroup.PSP2)
			{
				list.Add(AudioCompressionFormat.HEVAG);
			}
			if (platform == BuildTargetGroup.WiiU)
			{
				list.Add(AudioCompressionFormat.GCADPCM);
			}
			if (platform == BuildTargetGroup.XboxOne)
			{
				list.Add(AudioCompressionFormat.XMA);
			}
			return list.ToArray();
		}

		private bool CompressionFormatHasQuality(AudioCompressionFormat format)
		{
			switch (format)
			{
			case AudioCompressionFormat.Vorbis:
			case AudioCompressionFormat.MP3:
			case AudioCompressionFormat.XMA:
			case AudioCompressionFormat.AAC:
				return true;
			}
			return false;
		}

		private void OnSampleSettingGUI(BuildTargetGroup platform, AudioImporterInspector.MultiValueStatus status, bool selectionContainsTrackerFile, ref AudioImporterInspector.SampleSettingProperties properties, bool disablePreloadAudioDataOption)
		{
			EditorGUI.showMixedValue = (status.multiLoadType && !properties.loadTypeChanged);
			EditorGUI.BeginChangeCheck();
			AudioClipLoadType loadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Load Type", properties.settings.loadType, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				properties.settings.loadType = loadType;
				properties.loadTypeChanged = true;
			}
			using (new EditorGUI.DisabledScope(disablePreloadAudioDataOption))
			{
				if (disablePreloadAudioDataOption)
				{
					EditorGUILayout.Toggle("Preload Audio Data", false, new GUILayoutOption[0]);
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_PreloadAudioData, new GUILayoutOption[0]);
				}
			}
			if (!selectionContainsTrackerFile)
			{
				AudioCompressionFormat[] formatsForPlatform = this.GetFormatsForPlatform(platform);
				EditorGUI.showMixedValue = (status.multiCompressionFormat && !properties.compressionFormatChanged);
				EditorGUI.BeginChangeCheck();
				AudioCompressionFormat compressionFormat = (AudioCompressionFormat)EditorGUILayout.IntPopup("Compression Format", (int)properties.settings.compressionFormat, Array.ConvertAll<AudioCompressionFormat, string>(formatsForPlatform, (AudioCompressionFormat value) => value.ToString()), Array.ConvertAll<AudioCompressionFormat, int>(formatsForPlatform, (AudioCompressionFormat value) => (int)value), new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					properties.settings.compressionFormat = compressionFormat;
					properties.compressionFormatChanged = true;
				}
				if (this.CompressionFormatHasQuality(properties.settings.compressionFormat))
				{
					EditorGUI.showMixedValue = (status.multiQuality && !properties.qualityChanged);
					EditorGUI.BeginChangeCheck();
					int num = EditorGUILayout.IntSlider("Quality", (int)Mathf.Clamp(properties.settings.quality * 100f, 1f, 100f), 1, 100, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						properties.settings.quality = 0.01f * (float)num;
						properties.qualityChanged = true;
					}
				}
				EditorGUI.showMixedValue = (status.multiSampleRateSetting && !properties.sampleRateSettingChanged);
				EditorGUI.BeginChangeCheck();
				AudioSampleRateSetting sampleRateSetting = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Sample Rate Setting", properties.settings.sampleRateSetting, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					properties.settings.sampleRateSetting = sampleRateSetting;
					properties.sampleRateSettingChanged = true;
				}
				if (properties.settings.sampleRateSetting == AudioSampleRateSetting.OverrideSampleRate)
				{
					EditorGUI.showMixedValue = (status.multiSampleRateOverride && !properties.sampleRateOverrideChanged);
					EditorGUI.BeginChangeCheck();
					int sampleRateOverride = EditorGUILayout.IntPopup("Sample Rate", (int)properties.settings.sampleRateOverride, AudioImporterInspector.Styles.kSampleRateStrings, AudioImporterInspector.Styles.kSampleRateValues, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						properties.settings.sampleRateOverride = (uint)sampleRateOverride;
						properties.sampleRateOverrideChanged = true;
					}
				}
				EditorGUI.showMixedValue = false;
			}
		}

		private void OnAudioImporterGUI(bool selectionContainsTrackerFile)
		{
			if (!selectionContainsTrackerFile)
			{
				EditorGUILayout.PropertyField(this.m_ForceToMono, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				using (new EditorGUI.DisabledScope(!this.m_ForceToMono.boolValue))
				{
					EditorGUILayout.PropertyField(this.m_Normalize, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.PropertyField(this.m_LoadInBackground, new GUILayoutOption[0]);
			}
			BuildPlayerWindow.BuildPlatform[] array = BuildPlayerWindow.GetValidPlatforms().ToArray();
			GUILayout.Space(10f);
			int num = EditorGUILayout.BeginPlatformGrouping(array, GUIContent.Temp("Default"));
			if (num == -1)
			{
				bool disablePreloadAudioDataOption = this.m_DefaultSampleSettings.settings.loadType == AudioClipLoadType.Streaming;
				AudioImporterInspector.MultiValueStatus multiValueStatus = this.GetMultiValueStatus(BuildTargetGroup.Unknown);
				this.OnSampleSettingGUI(BuildTargetGroup.Unknown, multiValueStatus, selectionContainsTrackerFile, ref this.m_DefaultSampleSettings, disablePreloadAudioDataOption);
			}
			else
			{
				BuildTargetGroup targetGroup = array[num].targetGroup;
				AudioImporterInspector.SampleSettingProperties value = this.m_SampleSettingOverrides[targetGroup];
				AudioImporterInspector.OverrideStatus overrideStatus = this.GetOverrideStatus(targetGroup);
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = (overrideStatus == AudioImporterInspector.OverrideStatus.MixedOverrides && !value.overrideIsForced);
				bool flag = (value.overrideIsForced && value.forcedOverrideState) || (!value.overrideIsForced && overrideStatus != AudioImporterInspector.OverrideStatus.NoOverrides);
				flag = EditorGUILayout.ToggleLeft("Override for " + array[num].title.text, flag, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					value.forcedOverrideState = flag;
					value.overrideIsForced = true;
				}
				bool disablePreloadAudioDataOption2 = ((value.overrideIsForced && value.forcedOverrideState) || this.GetOverrideStatus(targetGroup) == AudioImporterInspector.OverrideStatus.AllOverrides) && value.settings.loadType == AudioClipLoadType.Streaming;
				AudioImporterInspector.MultiValueStatus multiValueStatus2 = this.GetMultiValueStatus(targetGroup);
				bool disabled = (!value.overrideIsForced || !value.forcedOverrideState) && overrideStatus != AudioImporterInspector.OverrideStatus.AllOverrides;
				using (new EditorGUI.DisabledScope(disabled))
				{
					this.OnSampleSettingGUI(targetGroup, multiValueStatus2, selectionContainsTrackerFile, ref value, disablePreloadAudioDataOption2);
				}
				this.m_SampleSettingOverrides[targetGroup] = value;
			}
			EditorGUILayout.EndPlatformGrouping();
		}

		internal override bool HasModified()
		{
			if (base.HasModified())
			{
				return true;
			}
			if (this.m_DefaultSampleSettings.HasModified())
			{
				return true;
			}
			Dictionary<BuildTargetGroup, AudioImporterInspector.SampleSettingProperties>.ValueCollection values = this.m_SampleSettingOverrides.Values;
			foreach (AudioImporterInspector.SampleSettingProperties current in values)
			{
				if (current.HasModified())
				{
					return true;
				}
			}
			return false;
		}

		internal override void Apply()
		{
			base.Apply();
			this.SyncSettingsToBackend();
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}
	}
}
