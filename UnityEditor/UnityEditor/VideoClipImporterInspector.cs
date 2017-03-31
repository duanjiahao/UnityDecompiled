using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(VideoClipImporter))]
	internal class VideoClipImporterInspector : AssetImporterInspector
	{
		internal struct MultiTargetSettingState
		{
			public bool mixedTranscoding;

			public bool mixedCodec;

			public bool mixedResizeMode;

			public bool mixedAspectRatio;

			public bool mixedCustomWidth;

			public bool mixedCustomHeight;

			public bool mixedBitrateMode;

			public bool mixedSpatialQuality;

			public bool firstTranscoding;

			public VideoCodec firstCodec;

			public VideoResizeMode firstResizeMode;

			public VideoEncodeAspectRatio firstAspectRatio;

			public int firstCustomWidth;

			public int firstCustomHeight;

			public VideoBitrateMode firstBitrateMode;

			public VideoSpatialQuality firstSpatialQuality;

			public void Init()
			{
				this.mixedTranscoding = false;
				this.mixedCodec = false;
				this.mixedResizeMode = false;
				this.mixedAspectRatio = false;
				this.mixedCustomWidth = false;
				this.mixedCustomHeight = false;
				this.mixedBitrateMode = false;
				this.mixedSpatialQuality = false;
				this.firstTranscoding = false;
				this.firstCodec = VideoCodec.Auto;
				this.firstResizeMode = VideoResizeMode.OriginalSize;
				this.firstAspectRatio = VideoEncodeAspectRatio.NoScaling;
				this.firstCustomWidth = -1;
				this.firstCustomHeight = -1;
				this.firstBitrateMode = VideoBitrateMode.High;
				this.firstSpatialQuality = VideoSpatialQuality.HighSpatialQuality;
			}
		}

		internal class InspectorTargetSettings
		{
			public bool overridePlatform;

			public VideoImporterTargetSettings settings;
		}

		private class Styles
		{
			public GUIContent[] playIcons = new GUIContent[]
			{
				EditorGUIUtility.IconContent("preAudioPlayOff"),
				EditorGUIUtility.IconContent("preAudioPlayOn")
			};

			public GUIContent keepAlphaContent = EditorGUIUtility.TextContent("Keep Alpha|If the source clip has alpha, this will encode it in the resulting clip so that transparency is usable during render.");

			public GUIContent deinterlaceContent = EditorGUIUtility.TextContent("Deinterlace|Remove interlacing on this video.");

			public GUIContent flipHorizontalContent = EditorGUIUtility.TextContent("Flip Horizontally|Flip the video horizontally during transcoding.");

			public GUIContent flipVerticalContent = EditorGUIUtility.TextContent("Flip Vertically|Flip the video vertically during transcoding.");

			public GUIContent importAudioContent = EditorGUIUtility.TextContent("Import Audio|Defines if the audio tracks will be imported during transcoding.");

			public GUIContent transcodeContent = EditorGUIUtility.TextContent("Transcode|Transcoding a clip gives more flexibility through the options below, but takes more time.");

			public GUIContent dimensionsContent = EditorGUIUtility.TextContent("Dimensions|Pixel size of the resulting video.");

			public GUIContent widthContent = EditorGUIUtility.TextContent("Width|Width in pixels of the resulting video.");

			public GUIContent heightContent = EditorGUIUtility.TextContent("Height|Height in pixels of the resulting video.");

			public GUIContent aspectRatioContent = EditorGUIUtility.TextContent("Aspect Ratio|How the original video is mapped into the target dimensions.");

			public GUIContent codecContent = EditorGUIUtility.TextContent("Codec|Codec for the resulting clip. Automatic will make the best choice for the target platform.");

			public GUIContent bitrateContent = EditorGUIUtility.TextContent("Bitrate Mode|Higher bit rates give a better quality, but impose higher load on network connections or storage.");

			public GUIContent spatialQualityContent = EditorGUIUtility.TextContent("Spatial Quality|Adds a downsize during import to reduce bitrate using resolution.");

			public GUIContent importerVersionContent = EditorGUIUtility.TextContent("Importer Version|Selects the type of asset produced (legacy MovieTexture or new VideoClip).");

			public GUIContent[] importerVersionOptions = new GUIContent[]
			{
				EditorGUIUtility.TextContent("MovieTexture (Legacy)|Produce MovieTexture asset (old version)"),
				EditorGUIUtility.TextContent("VideoClip|Produce VideoClip asset (for use with VideoPlayer)")
			};

			public GUIContent transcodeWarning = EditorGUIUtility.TextContent("Not all platforms transcoded. Clip is not guaranteed to be compatible on platforms without transcoding.");
		}

		private SerializedProperty m_UseLegacyImporter;

		private SerializedProperty m_Quality;

		private SerializedProperty m_IsColorLinear;

		private SerializedProperty m_EncodeAlpha;

		private SerializedProperty m_Deinterlace;

		private SerializedProperty m_FlipVertical;

		private SerializedProperty m_FlipHorizontal;

		private SerializedProperty m_ImportAudio;

		private VideoClipImporterInspector.InspectorTargetSettings[,] m_TargetSettings;

		private bool m_IsPlaying = false;

		private Vector2 m_Position = Vector2.zero;

		private AnimBool m_ShowResizeModeOptions = new AnimBool();

		private bool m_ModifiedTargetSettings;

		private GUIContent m_PreviewTitle;

		private static VideoClipImporterInspector.Styles s_Styles;

		private static string[] s_LegacyFileTypes = new string[]
		{
			".ogg",
			".ogv",
			".mov",
			".asf",
			".mpg",
			".mpeg",
			".mp4"
		};

		private const int kNarrowLabelWidth = 42;

		private const int kToggleButtonWidth = 16;

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		protected override bool useAssetDrawPreview
		{
			get
			{
				return false;
			}
		}

		private void ResetSettingsFromBackend()
		{
			this.m_TargetSettings = null;
			if (base.targets.Length > 0)
			{
				List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
				this.m_TargetSettings = new VideoClipImporterInspector.InspectorTargetSettings[base.targets.Length, validPlatforms.Count + 1];
				for (int i = 0; i < base.targets.Length; i++)
				{
					VideoClipImporter videoClipImporter = (VideoClipImporter)base.targets[i];
					this.m_TargetSettings[i, 0] = new VideoClipImporterInspector.InspectorTargetSettings();
					this.m_TargetSettings[i, 0].overridePlatform = true;
					this.m_TargetSettings[i, 0].settings = videoClipImporter.defaultTargetSettings;
					for (int j = 1; j < validPlatforms.Count + 1; j++)
					{
						BuildTargetGroup targetGroup = validPlatforms[j - 1].targetGroup;
						this.m_TargetSettings[i, j] = new VideoClipImporterInspector.InspectorTargetSettings();
						this.m_TargetSettings[i, j].settings = videoClipImporter.Internal_GetTargetSettings(targetGroup);
						this.m_TargetSettings[i, j].overridePlatform = (this.m_TargetSettings[i, j].settings != null);
					}
				}
			}
			this.m_ModifiedTargetSettings = false;
		}

		private void WriteSettingsToBackend()
		{
			if (this.m_TargetSettings != null)
			{
				List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
				for (int i = 0; i < base.targets.Length; i++)
				{
					VideoClipImporter videoClipImporter = (VideoClipImporter)base.targets[i];
					videoClipImporter.defaultTargetSettings = this.m_TargetSettings[i, 0].settings;
					for (int j = 1; j < validPlatforms.Count + 1; j++)
					{
						BuildTargetGroup targetGroup = validPlatforms[j - 1].targetGroup;
						if (this.m_TargetSettings[i, j].settings != null && this.m_TargetSettings[i, j].overridePlatform)
						{
							videoClipImporter.Internal_SetTargetSettings(targetGroup, this.m_TargetSettings[i, j].settings);
						}
						else
						{
							videoClipImporter.Internal_ClearTargetSettings(targetGroup);
						}
					}
				}
			}
			this.m_ModifiedTargetSettings = false;
		}

		public void OnEnable()
		{
			if (VideoClipImporterInspector.s_Styles == null)
			{
				VideoClipImporterInspector.s_Styles = new VideoClipImporterInspector.Styles();
			}
			this.m_UseLegacyImporter = base.serializedObject.FindProperty("m_UseLegacyImporter");
			this.m_Quality = base.serializedObject.FindProperty("m_Quality");
			this.m_IsColorLinear = base.serializedObject.FindProperty("m_IsColorLinear");
			this.m_EncodeAlpha = base.serializedObject.FindProperty("m_EncodeAlpha");
			this.m_Deinterlace = base.serializedObject.FindProperty("m_Deinterlace");
			this.m_FlipVertical = base.serializedObject.FindProperty("m_FlipVertical");
			this.m_FlipHorizontal = base.serializedObject.FindProperty("m_FlipHorizontal");
			this.m_ImportAudio = base.serializedObject.FindProperty("m_ImportAudio");
			this.ResetSettingsFromBackend();
			VideoClipImporterInspector.MultiTargetSettingState multiTargetSettingState = this.CalculateMultiTargetSettingState(0);
			this.m_ShowResizeModeOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowResizeModeOptions.value = (multiTargetSettingState.mixedResizeMode || multiTargetSettingState.firstResizeMode != VideoResizeMode.OriginalSize);
		}

		public override void OnDisable()
		{
			VideoClipImporter videoClipImporter = base.target as VideoClipImporter;
			if (videoClipImporter)
			{
				videoClipImporter.StopPreview();
			}
			base.OnDisable();
		}

		private List<GUIContent> GetResizeModeList()
		{
			List<GUIContent> list = new List<GUIContent>();
			VideoClipImporter videoClipImporter = (VideoClipImporter)base.target;
			IEnumerator enumerator = Enum.GetValues(typeof(VideoResizeMode)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VideoResizeMode mode = (VideoResizeMode)enumerator.Current;
					list.Add(EditorGUIUtility.TextContent(videoClipImporter.GetResizeModeName(mode)));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}

		private bool AnySettingsNotTranscoded()
		{
			bool result;
			if (this.m_TargetSettings != null)
			{
				for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
				{
					for (int j = 0; j < this.m_TargetSettings.GetLength(1); j++)
					{
						if (this.m_TargetSettings[i, j].settings != null && !this.m_TargetSettings[i, j].settings.enableTranscoding)
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}

		private VideoClipImporterInspector.MultiTargetSettingState CalculateMultiTargetSettingState(int platformIndex)
		{
			VideoClipImporterInspector.MultiTargetSettingState multiTargetSettingState = default(VideoClipImporterInspector.MultiTargetSettingState);
			multiTargetSettingState.Init();
			VideoClipImporterInspector.MultiTargetSettingState result;
			if (this.m_TargetSettings == null || this.m_TargetSettings.Length == 0)
			{
				result = multiTargetSettingState;
			}
			else
			{
				int num = -1;
				for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
				{
					if (this.m_TargetSettings[i, platformIndex].overridePlatform)
					{
						if (num == -1)
						{
							num = i;
							multiTargetSettingState.firstTranscoding = this.m_TargetSettings[i, platformIndex].settings.enableTranscoding;
							multiTargetSettingState.firstCodec = this.m_TargetSettings[i, platformIndex].settings.codec;
							multiTargetSettingState.firstResizeMode = this.m_TargetSettings[i, platformIndex].settings.resizeMode;
							multiTargetSettingState.firstAspectRatio = this.m_TargetSettings[i, platformIndex].settings.aspectRatio;
							multiTargetSettingState.firstCustomWidth = this.m_TargetSettings[i, platformIndex].settings.customWidth;
							multiTargetSettingState.firstCustomHeight = this.m_TargetSettings[i, platformIndex].settings.customHeight;
							multiTargetSettingState.firstBitrateMode = this.m_TargetSettings[i, platformIndex].settings.bitrateMode;
							multiTargetSettingState.firstSpatialQuality = this.m_TargetSettings[i, platformIndex].settings.spatialQuality;
						}
						else
						{
							multiTargetSettingState.mixedTranscoding = (multiTargetSettingState.firstTranscoding != this.m_TargetSettings[i, platformIndex].settings.enableTranscoding);
							multiTargetSettingState.mixedCodec = (multiTargetSettingState.firstCodec != this.m_TargetSettings[i, platformIndex].settings.codec);
							multiTargetSettingState.mixedResizeMode = (multiTargetSettingState.firstResizeMode != this.m_TargetSettings[i, platformIndex].settings.resizeMode);
							multiTargetSettingState.mixedAspectRatio = (multiTargetSettingState.firstAspectRatio != this.m_TargetSettings[i, platformIndex].settings.aspectRatio);
							multiTargetSettingState.mixedCustomWidth = (multiTargetSettingState.firstCustomWidth != this.m_TargetSettings[i, platformIndex].settings.customWidth);
							multiTargetSettingState.mixedCustomHeight = (multiTargetSettingState.firstCustomHeight != this.m_TargetSettings[i, platformIndex].settings.customHeight);
							multiTargetSettingState.mixedBitrateMode = (multiTargetSettingState.firstBitrateMode != this.m_TargetSettings[i, platformIndex].settings.bitrateMode);
							multiTargetSettingState.mixedSpatialQuality = (multiTargetSettingState.firstSpatialQuality != this.m_TargetSettings[i, platformIndex].settings.spatialQuality);
						}
					}
				}
				if (num == -1)
				{
					multiTargetSettingState.firstTranscoding = this.m_TargetSettings[0, 0].settings.enableTranscoding;
					multiTargetSettingState.firstCodec = this.m_TargetSettings[0, 0].settings.codec;
					multiTargetSettingState.firstResizeMode = this.m_TargetSettings[0, 0].settings.resizeMode;
					multiTargetSettingState.firstAspectRatio = this.m_TargetSettings[0, 0].settings.aspectRatio;
					multiTargetSettingState.firstCustomWidth = this.m_TargetSettings[0, 0].settings.customWidth;
					multiTargetSettingState.firstCustomHeight = this.m_TargetSettings[0, 0].settings.customHeight;
					multiTargetSettingState.firstBitrateMode = this.m_TargetSettings[0, 0].settings.bitrateMode;
					multiTargetSettingState.firstSpatialQuality = this.m_TargetSettings[0, 0].settings.spatialQuality;
				}
				result = multiTargetSettingState;
			}
			return result;
		}

		private void OnCrossTargetInspectorGUI()
		{
			bool flag = true;
			bool flag2 = true;
			for (int i = 0; i < base.targets.Length; i++)
			{
				VideoClipImporter videoClipImporter = (VideoClipImporter)base.targets[i];
				flag &= videoClipImporter.sourceHasAlpha;
				flag2 &= (videoClipImporter.sourceAudioTrackCount > 0);
			}
			if (flag)
			{
				EditorGUILayout.PropertyField(this.m_EncodeAlpha, VideoClipImporterInspector.s_Styles.keepAlphaContent, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_Deinterlace, VideoClipImporterInspector.s_Styles.deinterlaceContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_FlipHorizontal, VideoClipImporterInspector.s_Styles.flipHorizontalContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_FlipVertical, VideoClipImporterInspector.s_Styles.flipVerticalContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			using (new EditorGUI.DisabledScope(!flag2))
			{
				EditorGUILayout.PropertyField(this.m_ImportAudio, VideoClipImporterInspector.s_Styles.importAudioContent, new GUILayoutOption[0]);
			}
		}

		private void FrameSettingsGUI(int platformIndex, VideoClipImporterInspector.MultiTargetSettingState multiState)
		{
			EditorGUI.showMixedValue = multiState.mixedResizeMode;
			EditorGUI.BeginChangeCheck();
			VideoResizeMode videoResizeMode = (VideoResizeMode)EditorGUILayout.Popup(VideoClipImporterInspector.s_Styles.dimensionsContent, (int)multiState.firstResizeMode, this.GetResizeModeList().ToArray(), new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
				{
					if (this.m_TargetSettings[i, platformIndex].settings != null)
					{
						this.m_TargetSettings[i, platformIndex].settings.resizeMode = videoResizeMode;
						this.m_ModifiedTargetSettings = true;
					}
				}
			}
			this.m_ShowResizeModeOptions.target = (videoResizeMode != VideoResizeMode.OriginalSize);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowResizeModeOptions.faded))
			{
				EditorGUI.indentLevel++;
				if (videoResizeMode == VideoResizeMode.CustomSize)
				{
					EditorGUI.showMixedValue = multiState.mixedCustomWidth;
					EditorGUI.BeginChangeCheck();
					int customWidth = EditorGUILayout.IntField(VideoClipImporterInspector.s_Styles.widthContent, multiState.firstCustomWidth, new GUILayoutOption[0]);
					EditorGUI.showMixedValue = false;
					if (EditorGUI.EndChangeCheck())
					{
						for (int j = 0; j < this.m_TargetSettings.GetLength(0); j++)
						{
							if (this.m_TargetSettings[j, platformIndex].settings != null)
							{
								this.m_TargetSettings[j, platformIndex].settings.customWidth = customWidth;
								this.m_ModifiedTargetSettings = true;
							}
						}
					}
					EditorGUI.showMixedValue = multiState.mixedCustomHeight;
					EditorGUI.BeginChangeCheck();
					int customHeight = EditorGUILayout.IntField(VideoClipImporterInspector.s_Styles.heightContent, multiState.firstCustomHeight, new GUILayoutOption[0]);
					EditorGUI.showMixedValue = false;
					if (EditorGUI.EndChangeCheck())
					{
						for (int k = 0; k < this.m_TargetSettings.GetLength(0); k++)
						{
							if (this.m_TargetSettings[k, platformIndex].settings != null)
							{
								this.m_TargetSettings[k, platformIndex].settings.customHeight = customHeight;
								this.m_ModifiedTargetSettings = true;
							}
						}
					}
				}
				EditorGUI.showMixedValue = multiState.mixedAspectRatio;
				EditorGUI.BeginChangeCheck();
				VideoEncodeAspectRatio aspectRatio = (VideoEncodeAspectRatio)EditorGUILayout.EnumPopup(VideoClipImporterInspector.s_Styles.aspectRatioContent, multiState.firstAspectRatio, new GUILayoutOption[0]);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int l = 0; l < this.m_TargetSettings.GetLength(0); l++)
					{
						if (this.m_TargetSettings[l, platformIndex].settings != null)
						{
							this.m_TargetSettings[l, platformIndex].settings.aspectRatio = aspectRatio;
							this.m_ModifiedTargetSettings = true;
						}
					}
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void EncodingSettingsGUI(int platformIndex, VideoClipImporterInspector.MultiTargetSettingState multiState)
		{
			EditorGUI.showMixedValue = multiState.mixedCodec;
			EditorGUI.BeginChangeCheck();
			VideoCodec codec = (VideoCodec)EditorGUILayout.EnumPopup(VideoClipImporterInspector.s_Styles.codecContent, multiState.firstCodec, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
				{
					if (this.m_TargetSettings[i, platformIndex].settings != null)
					{
						this.m_TargetSettings[i, platformIndex].settings.codec = codec;
						this.m_ModifiedTargetSettings = true;
					}
				}
			}
			EditorGUI.showMixedValue = multiState.mixedBitrateMode;
			EditorGUI.BeginChangeCheck();
			VideoBitrateMode bitrateMode = (VideoBitrateMode)EditorGUILayout.EnumPopup(VideoClipImporterInspector.s_Styles.bitrateContent, multiState.firstBitrateMode, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				for (int j = 0; j < this.m_TargetSettings.GetLength(0); j++)
				{
					if (this.m_TargetSettings[j, platformIndex].settings != null)
					{
						this.m_TargetSettings[j, platformIndex].settings.bitrateMode = bitrateMode;
						this.m_ModifiedTargetSettings = true;
					}
				}
			}
			EditorGUI.showMixedValue = multiState.mixedSpatialQuality;
			EditorGUI.BeginChangeCheck();
			VideoSpatialQuality spatialQuality = (VideoSpatialQuality)EditorGUILayout.EnumPopup(VideoClipImporterInspector.s_Styles.spatialQualityContent, multiState.firstSpatialQuality, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				for (int k = 0; k < this.m_TargetSettings.GetLength(0); k++)
				{
					if (this.m_TargetSettings[k, platformIndex].settings != null)
					{
						this.m_TargetSettings[k, platformIndex].settings.spatialQuality = spatialQuality;
						this.m_ModifiedTargetSettings = true;
					}
				}
			}
		}

		private bool HasMixedOverrideStatus(int platformIndex, out bool overrideState)
		{
			overrideState = false;
			bool result;
			if (this.m_TargetSettings == null || this.m_TargetSettings.Length == 0)
			{
				result = false;
			}
			else
			{
				overrideState = this.m_TargetSettings[0, platformIndex].overridePlatform;
				for (int i = 1; i < this.m_TargetSettings.GetLength(0); i++)
				{
					if (this.m_TargetSettings[i, platformIndex].overridePlatform != overrideState)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private VideoImporterTargetSettings CloneTargetSettings(VideoImporterTargetSettings settings)
		{
			return new VideoImporterTargetSettings
			{
				enableTranscoding = settings.enableTranscoding,
				codec = settings.codec,
				resizeMode = settings.resizeMode,
				aspectRatio = settings.aspectRatio,
				customWidth = settings.customWidth,
				customHeight = settings.customHeight,
				bitrateMode = settings.bitrateMode,
				spatialQuality = settings.spatialQuality
			};
		}

		private void OnTargetSettingsInspectorGUI(int platformIndex, VideoClipImporterInspector.MultiTargetSettingState multiState)
		{
			EditorGUI.showMixedValue = multiState.mixedTranscoding;
			EditorGUI.BeginChangeCheck();
			bool flag = EditorGUILayout.Toggle(VideoClipImporterInspector.s_Styles.transcodeContent, multiState.firstTranscoding, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
				{
					if (this.m_TargetSettings[i, platformIndex].settings != null)
					{
						this.m_TargetSettings[i, platformIndex].settings.enableTranscoding = flag;
						this.m_ModifiedTargetSettings = true;
					}
				}
			}
			EditorGUI.indentLevel++;
			using (new EditorGUI.DisabledScope(!flag && !multiState.mixedTranscoding))
			{
				this.FrameSettingsGUI(platformIndex, multiState);
				this.EncodingSettingsGUI(platformIndex, multiState);
			}
			EditorGUI.indentLevel--;
		}

		private void OnTargetInspectorGUI(int platformIndex, string platformName)
		{
			bool flag = true;
			if (platformIndex != 0)
			{
				bool flag2;
				EditorGUI.showMixedValue = this.HasMixedOverrideStatus(platformIndex, out flag2);
				EditorGUI.BeginChangeCheck();
				flag2 = EditorGUILayout.Toggle("Override for " + platformName, flag2, new GUILayoutOption[0]);
				flag = (flag2 || EditorGUI.showMixedValue);
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					for (int i = 0; i < this.m_TargetSettings.GetLength(0); i++)
					{
						this.m_TargetSettings[i, platformIndex].overridePlatform = flag2;
						this.m_ModifiedTargetSettings = true;
						if (this.m_TargetSettings[i, platformIndex].settings == null)
						{
							this.m_TargetSettings[i, platformIndex].settings = this.CloneTargetSettings(this.m_TargetSettings[i, 0].settings);
						}
					}
				}
			}
			EditorGUILayout.Space();
			VideoClipImporterInspector.MultiTargetSettingState multiState = this.CalculateMultiTargetSettingState(platformIndex);
			using (new EditorGUI.DisabledScope(!flag))
			{
				this.OnTargetSettingsInspectorGUI(platformIndex, multiState);
			}
		}

		private void OnTargetsInspectorGUI()
		{
			BuildPlayerWindow.BuildPlatform[] array = BuildPlayerWindow.GetValidPlatforms().ToArray();
			int num = EditorGUILayout.BeginPlatformGrouping(array, GUIContent.Temp("Default"));
			string platformName = (num != -1) ? array[num].name : "Default";
			this.OnTargetInspectorGUI(num + 1, platformName);
			EditorGUILayout.EndPlatformGrouping();
		}

		internal override void OnHeaderControlsGUI()
		{
			base.serializedObject.UpdateIfRequiredOrScript();
			bool flag = true;
			int num = 0;
			while (flag && num < base.targets.Length)
			{
				VideoClipImporter videoClipImporter = (VideoClipImporter)base.targets[num];
				flag &= this.IsFileSupportedByLegacy(videoClipImporter.assetPath);
				num++;
			}
			if (!flag)
			{
				base.OnHeaderControlsGUI();
			}
			else
			{
				EditorGUI.showMixedValue = this.m_UseLegacyImporter.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 100f;
				int num2 = EditorGUILayout.Popup(VideoClipImporterInspector.s_Styles.importerVersionContent, (!this.m_UseLegacyImporter.boolValue) ? 1 : 0, VideoClipImporterInspector.s_Styles.importerVersionOptions, EditorStyles.popup, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(230f)
				});
				EditorGUIUtility.labelWidth = labelWidth;
				EditorGUI.showMixedValue = false;
				if (EditorGUI.EndChangeCheck())
				{
					this.m_UseLegacyImporter.boolValue = (num2 == 0);
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Open", EditorStyles.miniButton, new GUILayoutOption[0]))
				{
					AssetDatabase.OpenAsset(this.assetEditor.targets);
					GUIUtility.ExitGUI();
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.UpdateIfRequiredOrScript();
			if (this.m_UseLegacyImporter.boolValue)
			{
				EditorGUILayout.PropertyField(this.m_IsColorLinear, MovieImporterInspector.linearTextureContent, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Quality, 0f, 1f, new GUILayoutOption[0]);
			}
			else
			{
				this.OnCrossTargetInspectorGUI();
				EditorGUILayout.Space();
				this.OnTargetsInspectorGUI();
				if (this.AnySettingsNotTranscoded())
				{
					EditorGUILayout.HelpBox(VideoClipImporterInspector.s_Styles.transcodeWarning.text, MessageType.Info);
				}
			}
			base.ApplyRevertGUI();
		}

		internal override bool HasModified()
		{
			return base.HasModified() || this.m_ModifiedTargetSettings;
		}

		internal override void Apply()
		{
			base.Apply();
			this.WriteSettingsToBackend();
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}

		public override bool HasPreviewGUI()
		{
			return base.target != null;
		}

		public override GUIContent GetPreviewTitle()
		{
			GUIContent previewTitle;
			if (this.m_PreviewTitle != null)
			{
				previewTitle = this.m_PreviewTitle;
			}
			else
			{
				this.m_PreviewTitle = new GUIContent();
				if (base.targets.Length == 1)
				{
					AssetImporter assetImporter = (AssetImporter)base.target;
					this.m_PreviewTitle.text = Path.GetFileName(assetImporter.assetPath);
				}
				else
				{
					this.m_PreviewTitle.text = base.targets.Length + " Video Clips";
				}
				previewTitle = this.m_PreviewTitle;
			}
			return previewTitle;
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.OnEnable();
		}

		public override void OnPreviewSettings()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying || this.HasModified());
			this.m_IsPlaying = (PreviewGUI.CycleButton((!this.m_IsPlaying) ? 0 : 1, VideoClipImporterInspector.s_Styles.playIcons) != 0);
			EditorGUI.EndDisabledGroup();
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
				VideoClipImporter videoClipImporter = (VideoClipImporter)base.target;
				if (this.m_IsPlaying && !videoClipImporter.isPlayingPreview)
				{
					videoClipImporter.PlayPreview();
				}
				else if (!this.m_IsPlaying && videoClipImporter.isPlayingPreview)
				{
					videoClipImporter.StopPreview();
				}
				Texture previewTexture = videoClipImporter.GetPreviewTexture();
				if (previewTexture && previewTexture.width != 0 && previewTexture.height != 0)
				{
					float num = 1f;
					float num2 = 1f;
					if (videoClipImporter.defaultTargetSettings.enableTranscoding)
					{
						VideoResizeMode resizeMode = videoClipImporter.defaultTargetSettings.resizeMode;
						num = (float)(videoClipImporter.GetResizeWidth(resizeMode) / previewTexture.width);
						num2 = (float)(videoClipImporter.GetResizeHeight(resizeMode) / previewTexture.height);
					}
					float num3 = Mathf.Min(new float[]
					{
						num * r.width / (float)previewTexture.width,
						num2 * r.height / (float)previewTexture.height,
						num,
						num2
					});
					Rect rect = new Rect(r.x, r.y, (float)previewTexture.width * num3, (float)previewTexture.height * num3);
					PreviewGUI.BeginScrollView(r, this.m_Position, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
					EditorGUI.DrawTextureTransparent(rect, previewTexture, ScaleMode.StretchToFill);
					this.m_Position = PreviewGUI.EndScrollView();
					if (this.m_IsPlaying)
					{
						GUIView.current.Repaint();
					}
				}
			}
		}

		private bool IsFileSupportedByLegacy(string assetPath)
		{
			return Array.IndexOf<string>(VideoClipImporterInspector.s_LegacyFileTypes, Path.GetExtension(assetPath).ToLower()) != -1;
		}
	}
}
