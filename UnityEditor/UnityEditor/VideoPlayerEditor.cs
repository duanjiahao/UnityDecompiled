using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(VideoPlayer))]
	internal class VideoPlayerEditor : Editor
	{
		private class Styles
		{
			public GUIContent dataSourceContent = EditorGUIUtility.TextContent("Source|Type of source the movie will be read from.");

			public GUIContent videoClipContent = EditorGUIUtility.TextContent("Video Clip|VideoClips can be imported using the asset pipeline.");

			public GUIContent urlContent = EditorGUIUtility.TextContent("URL|URLs can be http:// or file://. File URLs can be relative [file://] or absolute [file:///].  For file URLs, the prefix is optional.");

			public GUIContent browseContent = EditorGUIUtility.TextContent("Browse...|Click to set a file:// URL.  http:// URLs have to be written or copy-pasted manually.");

			public GUIContent playOnAwakeContent = EditorGUIUtility.TextContent("Play On Awake|Start playback as soon as the game is started.");

			public GUIContent waitForFirstFrameContent = EditorGUIUtility.TextContent("Wait For First Frame|Wait for first frame to be ready before starting playback. When on, player time will only start increasing when the first image is ready.  When off, the first few frames may be skipped while clip preparation is ongoing.");

			public GUIContent loopContent = EditorGUIUtility.TextContent("Loop|Start playback at the beginning when end is reached.");

			public GUIContent playbackSpeedContent = EditorGUIUtility.TextContent("Playback Speed|Increase or decrease the playback speed. 1.0 is the normal speed.");

			public GUIContent renderModeContent = EditorGUIUtility.TextContent("Render Mode|Type of object on which the played images will be drawn.");

			public GUIContent cameraContent = EditorGUIUtility.TextContent("Camera|Camera where the images will be drawn, behind (Back Plane) or in front of (Front Plane) of the scene.");

			public GUIContent textureContent = EditorGUIUtility.TextContent("Target Texture|RenderTexture where the images will be drawn.  RenderTextures can be created under the Assets folder and the used on other objects.");

			public GUIContent alphaContent = EditorGUIUtility.TextContent("Alpha|A value less than 1.0 will reveal the content behind the video.");

			public GUIContent audioOutputModeContent = EditorGUIUtility.TextContent("Audio Output Mode|Where the audio in the movie will be output.");

			public GUIContent audioSourceContent = EditorGUIUtility.TextContent("Audio Source|AudioSource component tha will receive this track's audio samples.");

			public GUIContent aspectRatioLabel = EditorGUIUtility.TextContent("Aspect Ratio");

			public GUIContent muteLabel = EditorGUIUtility.TextContent("Mute");

			public GUIContent volumeLabel = EditorGUIUtility.TextContent("Volume");

			public GUIContent controlledAudioTrackCountContent = EditorGUIUtility.TextContent("Controlled Tracks|How many audio tracks will the player control.  The actual number of tracks is only known during playback when the source is a URL.");

			public GUIContent materialRendererContent = EditorGUIUtility.TextContent("Renderer|Renderer that will receive the images. Defaults to the first renderer on the game object.");

			public GUIContent materialPropertyContent = EditorGUIUtility.TextContent("Material Property|Texture property of the current Material that will receive the images.");

			public string selectUniformVideoSourceHelp = "Select a uniform video source type before a video clip or URL can be selected.";

			public string rendererMaterialsHaveNoTexPropsHelp = "Renderer materials have no texture properties.";

			public string someRendererMaterialsHaveNoTexPropsHelp = "Some selected renderers have materials with no texture properties.";

			public string invalidTexPropSelectionHelp = "Invalid texture property selection.";

			public string oneInvalidTexPropSelectionHelp = "1 selected object has an invalid texture property selection.";

			public string someInvalidTexPropSelectionsHelp = "{0} selected objects have invalid texture property selections.";

			public string texPropInAllMaterialsHelp = "Texture property appears in all renderer materials.";

			public string texPropInSomeMaterialsHelp = "Texture property appears in {0} out of {1} renderer materials.";

			public string selectUniformVideoRenderModeHelp = "Select a uniform video render mode type before a target camera, render texture or material parameter can be selected.";

			public string selectUniformAudioOutputModeHelp = "Select a uniform audio target before audio settings can be edited.";

			public string selectUniformAudioTracksHelp = "Only sources with the same number of audio tracks can be edited during multi-selection.";

			public string selectMovieFile = "Select movie file.";

			public string audioControlsNotEditableHelp = "Audio controls not editable when using muliple selection.";

			public string enableDecodingTooltip = "Enable decoding for this track.  Only effective when not playing.  When playing from a URL, track details are shown only while playing back.";
		}

		internal class AudioTrackInfo
		{
			public string language;

			public ushort channelCount;

			public GUIContent content;

			public AudioTrackInfo()
			{
				this.language = "";
				this.channelCount = 0;
			}
		}

		private delegate List<string> EntryGenerator(UnityEngine.Object obj, bool multiSelect, out int selection, out bool invalidSelection);

		private static VideoPlayerEditor.Styles s_Styles;

		private SerializedProperty m_DataSource;

		private SerializedProperty m_VideoClip;

		private SerializedProperty m_Url;

		private SerializedProperty m_PlayOnAwake;

		private SerializedProperty m_WaitForFirstFrame;

		private SerializedProperty m_Looping;

		private SerializedProperty m_PlaybackSpeed;

		private SerializedProperty m_RenderMode;

		private SerializedProperty m_TargetTexture;

		private SerializedProperty m_TargetCamera;

		private SerializedProperty m_TargetMaterialRenderer;

		private SerializedProperty m_TargetMaterialProperty;

		private SerializedProperty m_AspectRatio;

		private SerializedProperty m_TargetCameraAlpha;

		private SerializedProperty m_AudioOutputMode;

		private SerializedProperty m_ControlledAudioTrackCount;

		private SerializedProperty m_EnabledAudioTracks;

		private SerializedProperty m_TargetAudioSources;

		private SerializedProperty m_DirectAudioVolumes;

		private SerializedProperty m_DirectAudioMutes;

		private readonly AnimBool m_ShowRenderTexture = new AnimBool();

		private readonly AnimBool m_ShowTargetCamera = new AnimBool();

		private readonly AnimBool m_ShowRenderer = new AnimBool();

		private readonly AnimBool m_ShowMaterialProperty = new AnimBool();

		private readonly AnimBool m_DataSourceIsClip = new AnimBool();

		private readonly AnimBool m_ShowAspectRatio = new AnimBool();

		private readonly AnimBool m_ShowAudioControls = new AnimBool();

		private ushort m_AudioTrackCountCached = 0;

		private GUIContent m_ControlledAudioTrackCountContent;

		private List<VideoPlayerEditor.AudioTrackInfo> m_AudioTrackInfos;

		private int m_MaterialPropertyPopupContentHash;

		private GUIContent[] m_MaterialPropertyPopupContent;

		private int m_MaterialPropertyPopupSelection;

		private int m_MaterialPropertyPopupInvalidSelections;

		private string m_MultiMaterialInfo = null;

		[CompilerGenerated]
		private static VideoPlayerEditor.EntryGenerator <>f__mg$cache0;

		[CompilerGenerated]
		private static VideoPlayerEditor.EntryGenerator <>f__mg$cache1;

		private void OnEnable()
		{
			this.m_ShowRenderTexture.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowTargetCamera.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowRenderer.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowMaterialProperty.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_DataSourceIsClip.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAspectRatio.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAudioControls.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_DataSource = base.serializedObject.FindProperty("m_DataSource");
			this.m_VideoClip = base.serializedObject.FindProperty("m_VideoClip");
			this.m_Url = base.serializedObject.FindProperty("m_Url");
			this.m_PlayOnAwake = base.serializedObject.FindProperty("m_PlayOnAwake");
			this.m_WaitForFirstFrame = base.serializedObject.FindProperty("m_WaitForFirstFrame");
			this.m_Looping = base.serializedObject.FindProperty("m_Looping");
			this.m_PlaybackSpeed = base.serializedObject.FindProperty("m_PlaybackSpeed");
			this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
			this.m_TargetTexture = base.serializedObject.FindProperty("m_TargetTexture");
			this.m_TargetCamera = base.serializedObject.FindProperty("m_TargetCamera");
			this.m_TargetMaterialRenderer = base.serializedObject.FindProperty("m_TargetMaterialRenderer");
			this.m_TargetMaterialProperty = base.serializedObject.FindProperty("m_TargetMaterialProperty");
			this.m_AspectRatio = base.serializedObject.FindProperty("m_AspectRatio");
			this.m_TargetCameraAlpha = base.serializedObject.FindProperty("m_TargetCameraAlpha");
			this.m_AudioOutputMode = base.serializedObject.FindProperty("m_AudioOutputMode");
			this.m_ControlledAudioTrackCount = base.serializedObject.FindProperty("m_ControlledAudioTrackCount");
			this.m_EnabledAudioTracks = base.serializedObject.FindProperty("m_EnabledAudioTracks");
			this.m_TargetAudioSources = base.serializedObject.FindProperty("m_TargetAudioSources");
			this.m_DirectAudioVolumes = base.serializedObject.FindProperty("m_DirectAudioVolumes");
			this.m_DirectAudioMutes = base.serializedObject.FindProperty("m_DirectAudioMutes");
			this.m_ShowRenderTexture.value = (this.m_RenderMode.intValue == 2);
			this.m_ShowTargetCamera.value = (this.m_RenderMode.intValue == 0 || this.m_RenderMode.intValue == 1);
			this.m_ShowRenderer.value = (this.m_RenderMode.intValue == 3);
			UnityEngine.Object[] arg_30B_0 = base.targets;
			if (VideoPlayerEditor.<>f__mg$cache0 == null)
			{
				VideoPlayerEditor.<>f__mg$cache0 = new VideoPlayerEditor.EntryGenerator(VideoPlayerEditor.GetMaterialPropertyNames);
			}
			this.m_MaterialPropertyPopupContent = VideoPlayerEditor.BuildPopupEntries(arg_30B_0, VideoPlayerEditor.<>f__mg$cache0, out this.m_MaterialPropertyPopupSelection, out this.m_MaterialPropertyPopupInvalidSelections);
			this.m_MaterialPropertyPopupContentHash = VideoPlayerEditor.GetMaterialPropertyPopupHash(base.targets);
			this.m_ShowMaterialProperty.value = (base.targets.Count<UnityEngine.Object>() > 1 || (this.m_MaterialPropertyPopupSelection >= 0 && this.m_MaterialPropertyPopupContent.Length > 0));
			this.m_DataSourceIsClip.value = (this.m_DataSource.intValue == 0);
			this.m_ShowAspectRatio.value = (this.m_RenderMode.intValue != 3 && this.m_RenderMode.intValue != 4);
			this.m_ShowAudioControls.value = (this.m_AudioOutputMode.intValue != 0);
			VideoPlayer videoPlayer = base.target as VideoPlayer;
			videoPlayer.prepareCompleted += new VideoPlayer.EventHandler(this.PrepareCompleted);
			this.m_AudioTrackInfos = new List<VideoPlayerEditor.AudioTrackInfo>();
		}

		private void OnDisable()
		{
			this.m_ShowRenderTexture.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowTargetCamera.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowRenderer.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowMaterialProperty.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_DataSourceIsClip.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowAspectRatio.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowAudioControls.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			if (VideoPlayerEditor.s_Styles == null)
			{
				VideoPlayerEditor.s_Styles = new VideoPlayerEditor.Styles();
			}
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_DataSource, VideoPlayerEditor.s_Styles.dataSourceContent, new GUILayoutOption[0]);
			this.HandleDataSourceField();
			EditorGUILayout.PropertyField(this.m_PlayOnAwake, VideoPlayerEditor.s_Styles.playOnAwakeContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_WaitForFirstFrame, VideoPlayerEditor.s_Styles.waitForFirstFrameContent, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Looping, VideoPlayerEditor.s_Styles.loopContent, new GUILayoutOption[0]);
			EditorGUILayout.Slider(this.m_PlaybackSpeed, 0f, 10f, VideoPlayerEditor.s_Styles.playbackSpeedContent, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_RenderMode, VideoPlayerEditor.s_Styles.renderModeContent, new GUILayoutOption[0]);
			if (this.m_RenderMode.hasMultipleDifferentValues)
			{
				EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.selectUniformVideoRenderModeHelp, MessageType.Warning, false);
			}
			else
			{
				VideoRenderMode intValue = (VideoRenderMode)this.m_RenderMode.intValue;
				this.HandleTargetField(intValue);
			}
			this.HandleAudio();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void HandleDataSourceField()
		{
			this.m_DataSourceIsClip.target = (this.m_DataSource.intValue == 0);
			if (this.m_DataSource.hasMultipleDifferentValues)
			{
				EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.selectUniformVideoSourceHelp, MessageType.Warning, false);
			}
			else if (EditorGUILayout.BeginFadeGroup(this.m_DataSourceIsClip.faded))
			{
				EditorGUILayout.PropertyField(this.m_VideoClip, VideoPlayerEditor.s_Styles.videoClipContent, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.PropertyField(this.m_Url, VideoPlayerEditor.s_Styles.urlContent, new GUILayoutOption[0]);
				Rect controlRect = EditorGUILayout.GetControlRect(true, 16f, new GUILayoutOption[0]);
				controlRect.xMin += EditorGUIUtility.labelWidth;
				controlRect.xMax = controlRect.xMin + GUI.skin.label.CalcSize(VideoPlayerEditor.s_Styles.browseContent).x + 10f;
				if (EditorGUI.DropdownButton(controlRect, VideoPlayerEditor.s_Styles.browseContent, FocusType.Passive, GUISkin.current.button))
				{
					string[] filters = new string[]
					{
						"Movie files",
						"dv,mp4,mpg,mpeg,m4v,ogv,vp8,webm",
						"All files",
						"*"
					};
					string text = EditorUtility.OpenFilePanelWithFilters(VideoPlayerEditor.s_Styles.selectMovieFile, "", filters);
					if (!string.IsNullOrEmpty(text))
					{
						this.m_Url.stringValue = "file://" + text;
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private static int GetMaterialPropertyPopupHash(UnityEngine.Object[] objects)
		{
			int num = 0;
			for (int i = 0; i < objects.Length; i++)
			{
				VideoPlayer videoPlayer = (VideoPlayer)objects[i];
				if (videoPlayer)
				{
					Renderer targetRenderer = VideoPlayerEditor.GetTargetRenderer(videoPlayer);
					if (targetRenderer)
					{
						num ^= videoPlayer.targetMaterialProperty.GetHashCode();
						Material[] sharedMaterials = targetRenderer.sharedMaterials;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							Material material = sharedMaterials[j];
							if (material)
							{
								num ^= material.name.GetHashCode();
								int k = 0;
								int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
								while (k < propertyCount)
								{
									if (ShaderUtil.GetPropertyType(material.shader, k) == ShaderUtil.ShaderPropertyType.TexEnv)
									{
										num ^= ShaderUtil.GetPropertyName(material.shader, k).GetHashCode();
									}
									k++;
								}
							}
						}
					}
				}
			}
			return num;
		}

		private static List<string> GetMaterialPropertyNames(UnityEngine.Object obj, bool multiSelect, out int selection, out bool invalidSelection)
		{
			selection = -1;
			invalidSelection = true;
			List<string> list = new List<string>();
			VideoPlayer videoPlayer = obj as VideoPlayer;
			List<string> result;
			if (!videoPlayer)
			{
				result = list;
			}
			else
			{
				Renderer targetRenderer = VideoPlayerEditor.GetTargetRenderer(videoPlayer);
				if (!targetRenderer)
				{
					result = list;
				}
				else
				{
					Material[] sharedMaterials = targetRenderer.sharedMaterials;
					for (int i = 0; i < sharedMaterials.Length; i++)
					{
						Material material = sharedMaterials[i];
						if (material)
						{
							int j = 0;
							int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
							while (j < propertyCount)
							{
								if (ShaderUtil.GetPropertyType(material.shader, j) == ShaderUtil.ShaderPropertyType.TexEnv)
								{
									string propertyName = ShaderUtil.GetPropertyName(material.shader, j);
									if (!list.Contains(propertyName))
									{
										list.Add(propertyName);
									}
								}
								j++;
							}
							selection = list.IndexOf(videoPlayer.targetMaterialProperty);
							invalidSelection = (selection < 0 && list.Count<string>() > 0);
							if (invalidSelection && !multiSelect)
							{
								selection = list.Count<string>();
								list.Add(videoPlayer.targetMaterialProperty);
							}
						}
					}
					result = list;
				}
			}
			return result;
		}

		private static GUIContent[] BuildPopupEntries(UnityEngine.Object[] objects, VideoPlayerEditor.EntryGenerator func, out int selection, out int invalidSelections)
		{
			selection = -1;
			invalidSelections = 0;
			List<string> list = null;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object obj = objects[i];
				int num;
				bool flag;
				List<string> list2 = func(obj, objects.Count<UnityEngine.Object>() > 1, out num, out flag);
				if (list2 != null)
				{
					if (flag)
					{
						invalidSelections++;
					}
					List<string> list3 = (list != null) ? new List<string>(list.Intersect(list2)) : list2;
					selection = ((list != null) ? ((selection >= 0 && num >= 0 && !(list[selection] != list2[num])) ? list3.IndexOf(list[selection]) : -1) : num);
					list = list3;
				}
			}
			if (list == null)
			{
				list = new List<string>();
			}
			return (from x in list
			select new GUIContent(x)).ToArray<GUIContent>();
		}

		private static void HandlePopup(GUIContent content, SerializedProperty property, GUIContent[] entries, int selection)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(true, 16f, new GUILayoutOption[0]);
			GUIContent label = EditorGUI.BeginProperty(controlRect, content, property);
			EditorGUI.BeginChangeCheck();
			EditorGUI.BeginDisabledGroup(entries.Count<GUIContent>() == 0);
			selection = EditorGUI.Popup(controlRect, label, selection, entries);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				property.stringValue = entries[selection].text;
			}
			EditorGUI.EndProperty();
		}

		private void HandleTargetField(VideoRenderMode currentRenderMode)
		{
			this.m_ShowRenderTexture.target = (currentRenderMode == VideoRenderMode.RenderTexture);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowRenderTexture.faded))
			{
				EditorGUILayout.PropertyField(this.m_TargetTexture, VideoPlayerEditor.s_Styles.textureContent, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			this.m_ShowTargetCamera.target = (currentRenderMode == VideoRenderMode.CameraFarPlane || currentRenderMode == VideoRenderMode.CameraNearPlane);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowTargetCamera.faded))
			{
				EditorGUILayout.PropertyField(this.m_TargetCamera, VideoPlayerEditor.s_Styles.cameraContent, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_TargetCameraAlpha, 0f, 1f, VideoPlayerEditor.s_Styles.alphaContent, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			this.m_ShowRenderer.target = (currentRenderMode == VideoRenderMode.MaterialOverride);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowRenderer.faded))
			{
				bool flag = base.targets.Count<UnityEngine.Object>() > 1;
				if (flag)
				{
					EditorGUILayout.PropertyField(this.m_TargetMaterialRenderer, VideoPlayerEditor.s_Styles.materialRendererContent, new GUILayoutOption[0]);
				}
				else
				{
					Rect controlRect = EditorGUILayout.GetControlRect(true, 16f, new GUILayoutOption[0]);
					GUIContent label = EditorGUI.BeginProperty(controlRect, VideoPlayerEditor.s_Styles.materialRendererContent, this.m_TargetMaterialRenderer);
					EditorGUI.BeginChangeCheck();
					UnityEngine.Object objectReferenceValue = EditorGUI.ObjectField(controlRect, label, VideoPlayerEditor.GetTargetRenderer((VideoPlayer)base.target), typeof(Renderer), true);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_TargetMaterialRenderer.objectReferenceValue = objectReferenceValue;
					}
					EditorGUI.EndProperty();
				}
				int materialPropertyPopupHash = VideoPlayerEditor.GetMaterialPropertyPopupHash(base.targets);
				if (this.m_MaterialPropertyPopupContentHash != materialPropertyPopupHash)
				{
					UnityEngine.Object[] arg_1CE_0 = base.targets;
					if (VideoPlayerEditor.<>f__mg$cache1 == null)
					{
						VideoPlayerEditor.<>f__mg$cache1 = new VideoPlayerEditor.EntryGenerator(VideoPlayerEditor.GetMaterialPropertyNames);
					}
					this.m_MaterialPropertyPopupContent = VideoPlayerEditor.BuildPopupEntries(arg_1CE_0, VideoPlayerEditor.<>f__mg$cache1, out this.m_MaterialPropertyPopupSelection, out this.m_MaterialPropertyPopupInvalidSelections);
				}
				VideoPlayerEditor.HandlePopup(VideoPlayerEditor.s_Styles.materialPropertyContent, this.m_TargetMaterialProperty, this.m_MaterialPropertyPopupContent, this.m_MaterialPropertyPopupSelection);
				if (this.m_MaterialPropertyPopupInvalidSelections > 0 || this.m_MaterialPropertyPopupContent.Length == 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Space(EditorGUIUtility.labelWidth);
					if (this.m_MaterialPropertyPopupContent.Length == 0)
					{
						if (!flag)
						{
							EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.rendererMaterialsHaveNoTexPropsHelp, MessageType.Warning);
						}
						else
						{
							EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.someRendererMaterialsHaveNoTexPropsHelp, MessageType.Warning);
						}
					}
					else if (!flag)
					{
						EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.invalidTexPropSelectionHelp, MessageType.Warning);
					}
					else if (this.m_MaterialPropertyPopupInvalidSelections == 1)
					{
						EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.oneInvalidTexPropSelectionHelp, MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox(string.Format(VideoPlayerEditor.s_Styles.someInvalidTexPropSelectionsHelp, this.m_MaterialPropertyPopupInvalidSelections), MessageType.Warning);
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					this.DisplayMultiMaterialInformation(this.m_MaterialPropertyPopupContentHash != materialPropertyPopupHash);
				}
				this.m_MaterialPropertyPopupContentHash = materialPropertyPopupHash;
			}
			EditorGUILayout.EndFadeGroup();
			this.m_ShowAspectRatio.target = (currentRenderMode != VideoRenderMode.MaterialOverride && currentRenderMode != VideoRenderMode.APIOnly);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAspectRatio.faded))
			{
				EditorGUILayout.PropertyField(this.m_AspectRatio, VideoPlayerEditor.s_Styles.aspectRatioLabel, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void DisplayMultiMaterialInformation(bool refreshInfo)
		{
			if (refreshInfo || this.m_MultiMaterialInfo == null)
			{
				this.m_MultiMaterialInfo = this.GenerateMultiMaterialinformation();
			}
			if (!string.IsNullOrEmpty(this.m_MultiMaterialInfo))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(EditorGUIUtility.labelWidth);
				EditorGUILayout.HelpBox(this.m_MultiMaterialInfo, MessageType.Info);
				GUILayout.EndHorizontal();
			}
		}

		private string GenerateMultiMaterialinformation()
		{
			string result;
			if (base.targets.Count<UnityEngine.Object>() > 1)
			{
				result = "";
			}
			else
			{
				VideoPlayer videoPlayer = base.target as VideoPlayer;
				if (!videoPlayer)
				{
					result = "";
				}
				else
				{
					Renderer targetRenderer = VideoPlayerEditor.GetTargetRenderer(videoPlayer);
					if (!targetRenderer)
					{
						result = "";
					}
					else
					{
						Material[] sharedMaterials = targetRenderer.sharedMaterials;
						if (sharedMaterials == null || sharedMaterials.Count<Material>() <= 1)
						{
							result = "";
						}
						else
						{
							List<string> list = new List<string>();
							Material[] array = sharedMaterials;
							for (int i = 0; i < array.Length; i++)
							{
								Material material = array[i];
								if (material)
								{
									int j = 0;
									int propertyCount = ShaderUtil.GetPropertyCount(material.shader);
									while (j < propertyCount)
									{
										if (ShaderUtil.GetPropertyType(material.shader, j) == ShaderUtil.ShaderPropertyType.TexEnv && ShaderUtil.GetPropertyName(material.shader, j) == this.m_TargetMaterialProperty.stringValue)
										{
											list.Add(material.name);
											break;
										}
										j++;
									}
								}
							}
							if (list.Count<string>() == sharedMaterials.Count<Material>())
							{
								result = VideoPlayerEditor.s_Styles.texPropInAllMaterialsHelp;
							}
							else
							{
								result = string.Format(VideoPlayerEditor.s_Styles.texPropInSomeMaterialsHelp, list.Count<string>(), sharedMaterials.Count<Material>()) + ": " + string.Join(", ", list.ToArray());
							}
						}
					}
				}
			}
			return result;
		}

		private void HandleAudio()
		{
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_AudioOutputMode, VideoPlayerEditor.s_Styles.audioOutputModeContent, new GUILayoutOption[0]);
			this.m_ShowAudioControls.target = (this.m_AudioOutputMode.intValue != 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAudioControls.faded))
			{
				if (base.serializedObject.isEditingMultipleObjects)
				{
					EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.audioControlsNotEditableHelp, MessageType.Warning, false);
				}
				else if (this.m_AudioOutputMode.hasMultipleDifferentValues)
				{
					EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.selectUniformAudioOutputModeHelp, MessageType.Warning, false);
				}
				else
				{
					ushort val = (ushort)this.m_ControlledAudioTrackCount.intValue;
					this.HandleControlledAudioTrackCount();
					if (this.m_ControlledAudioTrackCount.hasMultipleDifferentValues)
					{
						EditorGUILayout.HelpBox(VideoPlayerEditor.s_Styles.selectUniformAudioTracksHelp, MessageType.Warning, false);
					}
					else
					{
						VideoAudioOutputMode intValue = (VideoAudioOutputMode)this.m_AudioOutputMode.intValue;
						ushort num = Math.Min((ushort)this.m_ControlledAudioTrackCount.intValue, val);
						num = (ushort)Math.Min((int)num, this.m_EnabledAudioTracks.arraySize);
						for (ushort num2 = 0; num2 < num; num2 += 1)
						{
							EditorGUILayout.PropertyField(this.m_EnabledAudioTracks.GetArrayElementAtIndex((int)num2), this.GetAudioTrackEnabledContent(num2), new GUILayoutOption[0]);
							EditorGUI.indentLevel++;
							if (intValue == VideoAudioOutputMode.AudioSource)
							{
								EditorGUILayout.PropertyField(this.m_TargetAudioSources.GetArrayElementAtIndex((int)num2), VideoPlayerEditor.s_Styles.audioSourceContent, new GUILayoutOption[0]);
							}
							else if (intValue == VideoAudioOutputMode.Direct)
							{
								EditorGUILayout.PropertyField(this.m_DirectAudioMutes.GetArrayElementAtIndex((int)num2), VideoPlayerEditor.s_Styles.muteLabel, new GUILayoutOption[0]);
								EditorGUILayout.Slider(this.m_DirectAudioVolumes.GetArrayElementAtIndex((int)num2), 0f, 1f, VideoPlayerEditor.s_Styles.volumeLabel, new GUILayoutOption[0]);
							}
							EditorGUI.indentLevel--;
						}
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private GUIContent GetAudioTrackEnabledContent(ushort trackIdx)
		{
			while (this.m_AudioTrackInfos.Count <= (int)trackIdx)
			{
				this.m_AudioTrackInfos.Add(new VideoPlayerEditor.AudioTrackInfo());
			}
			VideoPlayerEditor.AudioTrackInfo audioTrackInfo = this.m_AudioTrackInfos[(int)trackIdx];
			VideoPlayer videoPlayer = null;
			if (!base.serializedObject.isEditingMultipleObjects)
			{
				videoPlayer = (VideoPlayer)base.target;
			}
			string text = (!videoPlayer) ? "" : videoPlayer.GetAudioLanguageCode(trackIdx);
			ushort num = (!videoPlayer) ? 0 : videoPlayer.GetAudioChannelCount(trackIdx);
			if (text != audioTrackInfo.language || num != audioTrackInfo.channelCount || audioTrackInfo.content == null)
			{
				string text2 = "";
				if (text.Length > 0)
				{
					text2 += text;
				}
				if (num > 0)
				{
					if (text2.Length > 0)
					{
						text2 += ", ";
					}
					text2 = text2 + num.ToString() + " ch";
				}
				if (text2.Length > 0)
				{
					text2 = " [" + text2 + "]";
				}
				audioTrackInfo.content = EditorGUIUtility.TextContent("Track " + trackIdx + text2);
				audioTrackInfo.content.tooltip = VideoPlayerEditor.s_Styles.enableDecodingTooltip;
			}
			return audioTrackInfo.content;
		}

		private void HandleControlledAudioTrackCount()
		{
			if (!this.m_DataSourceIsClip.value && !this.m_DataSource.hasMultipleDifferentValues)
			{
				VideoPlayer videoPlayer = (VideoPlayer)base.target;
				ushort num = (!base.serializedObject.isEditingMultipleObjects) ? videoPlayer.audioTrackCount : 0;
				GUIContent controlledAudioTrackCountContent;
				if (num == 0)
				{
					controlledAudioTrackCountContent = VideoPlayerEditor.s_Styles.controlledAudioTrackCountContent;
				}
				else
				{
					if (num != this.m_AudioTrackCountCached)
					{
						this.m_AudioTrackCountCached = num;
						this.m_ControlledAudioTrackCountContent = EditorGUIUtility.TextContent(string.Concat(new object[]
						{
							VideoPlayerEditor.s_Styles.controlledAudioTrackCountContent.text,
							" [",
							num,
							" found]"
						}));
						this.m_ControlledAudioTrackCountContent.tooltip = VideoPlayerEditor.s_Styles.controlledAudioTrackCountContent.tooltip;
					}
					controlledAudioTrackCountContent = this.m_ControlledAudioTrackCountContent;
				}
				EditorGUILayout.PropertyField(this.m_ControlledAudioTrackCount, controlledAudioTrackCountContent, new GUILayoutOption[0]);
			}
		}

		private void PrepareCompleted(VideoPlayer vp)
		{
			base.Repaint();
		}

		private static Renderer GetTargetRenderer(VideoPlayer vp)
		{
			Renderer targetMaterialRenderer = vp.targetMaterialRenderer;
			Renderer result;
			if (targetMaterialRenderer)
			{
				result = targetMaterialRenderer;
			}
			else
			{
				result = vp.gameObject.GetComponent<Renderer>();
			}
			return result;
		}
	}
}
