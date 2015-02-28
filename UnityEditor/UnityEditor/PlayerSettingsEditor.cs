using System;
using System.IO;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	[CustomEditor(typeof(PlayerSettings))]
	internal class PlayerSettingsEditor : Editor
	{
		private class Styles
		{
			public GUIStyle categoryBox = new GUIStyle(EditorStyles.helpBox);
			public GUIContent colorSpaceWarning = EditorGUIUtility.TextContent("PlayerSettings.ActiveColorSpaceWarning");
			public GUIContent cursorHotspot = EditorGUIUtility.TextContent("PlayerSettings.CursorHotspot");
			public GUIContent defaultCursor = EditorGUIUtility.TextContent("PlayerSettings.DefaultCursor");
			public GUIContent defaultIcon = EditorGUIUtility.TextContent("PlayerSettings.DefaultIcon");
			public Styles()
			{
				this.categoryBox.padding.left = 14;
			}
		}
		private enum FakeEnum
		{
			WebplayerSubset
		}
		internal class WebPlayerTemplateManager : WebTemplateManagerBase
		{
			private const string kWebTemplateDefaultIconResource = "BuildSettings.Web.Small";
			public override string customTemplatesFolder
			{
				get
				{
					return Path.Combine(Application.dataPath.Replace('/', Path.DirectorySeparatorChar), "WebPlayerTemplates");
				}
			}
			public override string builtinTemplatesFolder
			{
				get
				{
					return Path.Combine(Path.Combine(EditorApplication.applicationContentsPath.Replace('/', Path.DirectorySeparatorChar), "Resources"), "WebPlayerTemplates");
				}
			}
			public override Texture2D defaultIcon
			{
				get
				{
					return (Texture2D)EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
				}
			}
		}
		private const int kSlotSize = 64;
		private const int kMaxPreviewSize = 96;
		private const int kIconSpacing = 6;
		private static PlayerSettingsEditor.Styles s_Styles;
		private static GUIContent[] kRenderPaths = new GUIContent[]
		{
			new GUIContent("Forward"),
			new GUIContent("Deferred"),
			new GUIContent("Legacy Vertex Lit"),
			new GUIContent("Legacy Deferred (light prepass)")
		};
		private static int[] kRenderPathValues = new int[]
		{
			1,
			3,
			0,
			2
		};
		private SavedInt m_SelectedSection = new SavedInt("PlayerSettings.ShownSection", -1);
		private BuildPlayerWindow.BuildPlatform[] validPlatforms;
		private SerializedProperty m_ApplicationBundleIdentifier;
		private SerializedProperty m_ApplicationBundleVersion;
		private SerializedProperty m_IPhoneApplicationDisplayName;
		private SerializedProperty m_LocationUsageDescription;
		private SerializedProperty m_ApiCompatibilityLevel;
		private SerializedProperty m_IPhoneStrippingLevel;
		private SerializedProperty m_IPhoneScriptCallOptimization;
		private SerializedProperty m_AotOptions;
		private SerializedProperty m_DefaultScreenOrientation;
		private SerializedProperty m_AllowedAutoRotateToPortrait;
		private SerializedProperty m_AllowedAutoRotateToPortraitUpsideDown;
		private SerializedProperty m_AllowedAutoRotateToLandscapeRight;
		private SerializedProperty m_AllowedAutoRotateToLandscapeLeft;
		private SerializedProperty m_UseOSAutoRotation;
		private SerializedProperty m_Use32BitDisplayBuffer;
		private SerializedProperty m_DisableDepthAndStencilBuffers;
		private SerializedProperty m_iosShowActivityIndicatorOnLoading;
		private SerializedProperty m_androidShowActivityIndicatorOnLoading;
		private SerializedProperty m_IPhoneSdkVersion;
		private SerializedProperty m_IPhoneTargetOSVersion;
		private SerializedProperty m_AndroidProfiler;
		private SerializedProperty m_UIPrerenderedIcon;
		private SerializedProperty m_UIRequiresPersistentWiFi;
		private SerializedProperty m_UIStatusBarHidden;
		private SerializedProperty m_UIStatusBarStyle;
		private SerializedProperty m_IOSAppInBackgroundBehavior;
		private SerializedProperty m_SubmitAnalytics;
		private SerializedProperty m_TargetDevice;
		private SerializedProperty m_TargetGlesGraphics;
		private SerializedProperty m_TargetIOSGraphics;
		private SerializedProperty m_AccelerometerFrequency;
		private SerializedProperty m_TargetResolution;
		private SerializedProperty m_OverrideIPodMusic;
		private SerializedProperty m_PrepareIOSForRecording;
		private SerializedProperty m_EnableInternalProfiler;
		private SerializedProperty m_ActionOnDotNetUnhandledException;
		private SerializedProperty m_LogObjCUncaughtExceptions;
		private SerializedProperty m_EnableCrashReportAPI;
		private SerializedProperty m_XboxTitleId;
		private SerializedProperty m_XboxImageXexPath;
		private SerializedProperty m_XboxSpaPath;
		private SerializedProperty m_XboxGenerateSpa;
		private SerializedProperty m_XboxDeployKinectResources;
		private SerializedProperty m_XboxPIXTextureCapture;
		private SerializedProperty m_XboxEnableAvatar;
		private SerializedProperty m_XboxEnableKinect;
		private SerializedProperty m_XboxEnableKinectAutoTracking;
		private SerializedProperty m_XboxEnableHeadOrientation;
		private SerializedProperty m_XboxDeployHeadOrientation;
		private SerializedProperty m_XboxDeployKinectHeadPosition;
		private SerializedProperty m_XboxSplashScreen;
		private SerializedProperty m_XboxEnableSpeech;
		private SerializedProperty m_XboxSpeechDB;
		private SerializedProperty m_XboxEnableFitness;
		private SerializedProperty m_XboxAdditionalTitleMemorySize;
		private SerializedProperty m_XboxEnableGuest;
		private SerializedProperty m_VideoMemoryForVertexBuffers;
		private SerializedProperty m_ps3SplashScreen;
		private SerializedProperty m_CompanyName;
		private SerializedProperty m_ProductName;
		private SerializedProperty m_DefaultCursor;
		private SerializedProperty m_CursorHotspot;
		private SerializedProperty m_DefaultScreenWidth;
		private SerializedProperty m_DefaultScreenHeight;
		private SerializedProperty m_DefaultScreenWidthWeb;
		private SerializedProperty m_DefaultScreenHeightWeb;
		private SerializedProperty m_RenderingPath;
		private SerializedProperty m_MobileRenderingPath;
		private SerializedProperty m_ActiveColorSpace;
		private SerializedProperty m_MTRendering;
		private SerializedProperty m_MobileMTRendering;
		private SerializedProperty m_StripUnusedMeshComponents;
		private SerializedProperty m_DisplayResolutionDialog;
		private SerializedProperty m_DefaultIsFullScreen;
		private SerializedProperty m_DefaultIsNativeResolution;
		private SerializedProperty m_UsePlayerLog;
		private SerializedProperty m_PreloadShaders;
		private SerializedProperty m_PreloadedAssets;
		private SerializedProperty m_BakeCollisionMeshes;
		private SerializedProperty m_ResizableWindow;
		private SerializedProperty m_UseMacAppStoreValidation;
		private SerializedProperty m_MacFullscreenMode;
		private SerializedProperty m_D3D9FullscreenMode;
		private SerializedProperty m_D3D11FullscreenMode;
		private SerializedProperty m_VisibleInBackground;
		private SerializedProperty m_ForceSingleInstance;
		private SerializedProperty m_RunInBackground;
		private SerializedProperty m_CaptureSingleScreen;
		private SerializedProperty m_ResolutionDialogBanner;
		private SerializedProperty m_SupportedAspectRatios;
		private SerializedProperty m_SkinOnGPU;
		private SerializedProperty m_FirstStreamedLevelWithResources;
		private SerializedProperty m_WebPlayerTemplate;
		private PlayerSettingsEditor.WebPlayerTemplateManager m_WebPlayerTemplateManager = new PlayerSettingsEditor.WebPlayerTemplateManager();
		private int selectedPlatform;
		private int scriptingDefinesControlID;
		private AnimBool[] m_SectionAnimators = new AnimBool[6];
		private readonly AnimBool m_ShowDeferredWarning = new AnimBool();
		private readonly AnimBool m_ShowDefaultIsNativeResolution = new AnimBool();
		private readonly AnimBool m_ShowResolution = new AnimBool();
		private static Texture2D s_WarningIcon;
		private bool IsMobileTarget(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BlackBerry || targetGroup == BuildTargetGroup.Tizen || targetGroup == BuildTargetGroup.SamsungTV;
		}
		private static bool IsWP8Player(BuildTargetGroup target)
		{
			return target == BuildTargetGroup.WP8;
		}
		public SerializedProperty FindPropertyAssert(string name)
		{
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(name);
			if (serializedProperty == null)
			{
				Debug.LogError("Failed to find:" + name);
			}
			return serializedProperty;
		}
		private void OnEnable()
		{
			this.validPlatforms = BuildPlayerWindow.GetValidPlatforms().ToArray();
			this.m_IPhoneSdkVersion = this.FindPropertyAssert("iPhoneSdkVersion");
			this.m_IPhoneTargetOSVersion = this.FindPropertyAssert("iPhoneTargetOSVersion");
			this.m_IPhoneStrippingLevel = this.FindPropertyAssert("iPhoneStrippingLevel");
			this.m_IPhoneScriptCallOptimization = this.FindPropertyAssert("iPhoneScriptCallOptimization");
			this.m_AndroidProfiler = this.FindPropertyAssert("AndroidProfiler");
			this.m_CompanyName = this.FindPropertyAssert("companyName");
			this.m_ProductName = this.FindPropertyAssert("productName");
			this.m_DefaultCursor = this.FindPropertyAssert("defaultCursor");
			this.m_CursorHotspot = this.FindPropertyAssert("cursorHotspot");
			this.m_UIPrerenderedIcon = this.FindPropertyAssert("uIPrerenderedIcon");
			this.m_ResolutionDialogBanner = this.FindPropertyAssert("resolutionDialogBanner");
			this.m_UIStatusBarHidden = this.FindPropertyAssert("uIStatusBarHidden");
			this.m_UIStatusBarStyle = this.FindPropertyAssert("uIStatusBarStyle");
			this.m_RenderingPath = this.FindPropertyAssert("m_RenderingPath");
			this.m_MobileRenderingPath = this.FindPropertyAssert("m_MobileRenderingPath");
			this.m_ActiveColorSpace = this.FindPropertyAssert("m_ActiveColorSpace");
			this.m_MTRendering = this.FindPropertyAssert("m_MTRendering");
			this.m_MobileMTRendering = this.FindPropertyAssert("m_MobileMTRendering");
			this.m_StripUnusedMeshComponents = this.FindPropertyAssert("StripUnusedMeshComponents");
			this.m_FirstStreamedLevelWithResources = this.FindPropertyAssert("firstStreamedLevelWithResources");
			this.m_ApplicationBundleIdentifier = base.serializedObject.FindProperty("bundleIdentifier");
			if (this.m_ApplicationBundleIdentifier == null)
			{
				this.m_ApplicationBundleIdentifier = this.FindPropertyAssert("iPhoneBundleIdentifier");
			}
			this.m_ApplicationBundleVersion = base.serializedObject.FindProperty("bundleVersion");
			if (this.m_ApplicationBundleVersion == null)
			{
				this.m_ApplicationBundleVersion = this.FindPropertyAssert("iPhoneBundleVersion");
			}
			this.m_TargetResolution = this.FindPropertyAssert("targetResolution");
			this.m_AccelerometerFrequency = this.FindPropertyAssert("accelerometerFrequency");
			this.m_OverrideIPodMusic = this.FindPropertyAssert("Override IPod Music");
			this.m_PrepareIOSForRecording = this.FindPropertyAssert("Prepare IOS For Recording");
			this.m_UIRequiresPersistentWiFi = this.FindPropertyAssert("uIRequiresPersistentWiFi");
			this.m_IOSAppInBackgroundBehavior = this.FindPropertyAssert("iosAppInBackgroundBehavior");
			this.m_SubmitAnalytics = this.FindPropertyAssert("submitAnalytics");
			this.m_ApiCompatibilityLevel = this.FindPropertyAssert("apiCompatibilityLevel");
			this.m_AotOptions = this.FindPropertyAssert("aotOptions");
			this.m_LocationUsageDescription = this.FindPropertyAssert("locationUsageDescription");
			this.m_EnableInternalProfiler = this.FindPropertyAssert("enableInternalProfiler");
			this.m_ActionOnDotNetUnhandledException = this.FindPropertyAssert("actionOnDotNetUnhandledException");
			this.m_LogObjCUncaughtExceptions = this.FindPropertyAssert("logObjCUncaughtExceptions");
			this.m_EnableCrashReportAPI = this.FindPropertyAssert("enableCrashReportAPI");
			this.m_DefaultScreenWidth = this.FindPropertyAssert("defaultScreenWidth");
			this.m_DefaultScreenHeight = this.FindPropertyAssert("defaultScreenHeight");
			this.m_DefaultScreenWidthWeb = this.FindPropertyAssert("defaultScreenWidthWeb");
			this.m_DefaultScreenHeightWeb = this.FindPropertyAssert("defaultScreenHeightWeb");
			this.m_RunInBackground = this.FindPropertyAssert("runInBackground");
			this.m_DefaultScreenOrientation = this.FindPropertyAssert("defaultScreenOrientation");
			this.m_AllowedAutoRotateToPortrait = this.FindPropertyAssert("allowedAutorotateToPortrait");
			this.m_AllowedAutoRotateToPortraitUpsideDown = this.FindPropertyAssert("allowedAutorotateToPortraitUpsideDown");
			this.m_AllowedAutoRotateToLandscapeRight = this.FindPropertyAssert("allowedAutorotateToLandscapeRight");
			this.m_AllowedAutoRotateToLandscapeLeft = this.FindPropertyAssert("allowedAutorotateToLandscapeLeft");
			this.m_UseOSAutoRotation = this.FindPropertyAssert("useOSAutorotation");
			this.m_Use32BitDisplayBuffer = this.FindPropertyAssert("use32BitDisplayBuffer");
			this.m_DisableDepthAndStencilBuffers = this.FindPropertyAssert("disableDepthAndStencilBuffers");
			this.m_iosShowActivityIndicatorOnLoading = this.FindPropertyAssert("iosShowActivityIndicatorOnLoading");
			this.m_androidShowActivityIndicatorOnLoading = this.FindPropertyAssert("androidShowActivityIndicatorOnLoading");
			this.m_DefaultIsFullScreen = this.FindPropertyAssert("defaultIsFullScreen");
			this.m_DefaultIsNativeResolution = this.FindPropertyAssert("defaultIsNativeResolution");
			this.m_CaptureSingleScreen = this.FindPropertyAssert("captureSingleScreen");
			this.m_DisplayResolutionDialog = this.FindPropertyAssert("displayResolutionDialog");
			this.m_SupportedAspectRatios = this.FindPropertyAssert("m_SupportedAspectRatios");
			this.m_WebPlayerTemplate = this.FindPropertyAssert("webPlayerTemplate");
			this.m_TargetGlesGraphics = this.FindPropertyAssert("targetGlesGraphics");
			this.m_TargetIOSGraphics = this.FindPropertyAssert("targetIOSGraphics");
			this.m_TargetDevice = this.FindPropertyAssert("targetDevice");
			this.m_UsePlayerLog = this.FindPropertyAssert("usePlayerLog");
			this.m_PreloadShaders = this.FindPropertyAssert("preloadShaders");
			this.m_PreloadedAssets = this.FindPropertyAssert("preloadedAssets");
			this.m_BakeCollisionMeshes = this.FindPropertyAssert("bakeCollisionMeshes");
			this.m_ResizableWindow = this.FindPropertyAssert("resizableWindow");
			this.m_UseMacAppStoreValidation = this.FindPropertyAssert("useMacAppStoreValidation");
			this.m_D3D9FullscreenMode = this.FindPropertyAssert("d3d9FullscreenMode");
			this.m_D3D11FullscreenMode = this.FindPropertyAssert("d3d11FullscreenMode");
			this.m_VisibleInBackground = this.FindPropertyAssert("visibleInBackground");
			this.m_MacFullscreenMode = this.FindPropertyAssert("macFullscreenMode");
			this.m_SkinOnGPU = this.FindPropertyAssert("gpuSkinning");
			this.m_ForceSingleInstance = this.FindPropertyAssert("forceSingleInstance");
			this.m_XboxTitleId = this.FindPropertyAssert("XboxTitleId");
			this.m_XboxImageXexPath = this.FindPropertyAssert("XboxImageXexPath");
			this.m_XboxSpaPath = this.FindPropertyAssert("XboxSpaPath");
			this.m_XboxGenerateSpa = this.FindPropertyAssert("XboxGenerateSpa");
			this.m_XboxDeployKinectResources = this.FindPropertyAssert("XboxDeployKinectResources");
			this.m_XboxPIXTextureCapture = this.FindPropertyAssert("xboxPIXTextureCapture");
			this.m_XboxEnableAvatar = this.FindPropertyAssert("xboxEnableAvatar");
			this.m_XboxEnableKinect = this.FindPropertyAssert("xboxEnableKinect");
			this.m_XboxEnableKinectAutoTracking = this.FindPropertyAssert("xboxEnableKinectAutoTracking");
			this.m_XboxSplashScreen = this.FindPropertyAssert("XboxSplashScreen");
			this.m_XboxEnableSpeech = this.FindPropertyAssert("xboxEnableSpeech");
			this.m_XboxSpeechDB = this.FindPropertyAssert("xboxSpeechDB");
			this.m_XboxEnableFitness = this.FindPropertyAssert("xboxEnableFitness");
			this.m_XboxAdditionalTitleMemorySize = this.FindPropertyAssert("xboxAdditionalTitleMemorySize");
			this.m_XboxEnableHeadOrientation = this.FindPropertyAssert("xboxEnableHeadOrientation");
			this.m_XboxDeployHeadOrientation = this.FindPropertyAssert("xboxDeployKinectHeadOrientation");
			this.m_XboxDeployKinectHeadPosition = this.FindPropertyAssert("xboxDeployKinectHeadPosition");
			this.m_XboxEnableGuest = this.FindPropertyAssert("xboxEnableGuest");
			this.m_VideoMemoryForVertexBuffers = this.FindPropertyAssert("videoMemoryForVertexBuffers");
			this.m_ps3SplashScreen = this.FindPropertyAssert("ps3SplashScreen");
			BuildPlayerWindow.BuildPlatform[] array = this.validPlatforms;
			for (int i = 0; i < array.Length; i++)
			{
				BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
				string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(buildPlatform.targetGroup);
				ISettingEditorExtension editorSettingsExtension = ModuleManager.GetEditorSettingsExtension(targetStringFromBuildTargetGroup);
				if (editorSettingsExtension != null)
				{
					editorSettingsExtension.OnEnable(this);
				}
			}
			for (int j = 0; j < this.m_SectionAnimators.Length; j++)
			{
				this.m_SectionAnimators[j] = new AnimBool(this.m_SelectedSection.value == j, new UnityAction(base.Repaint));
			}
			this.m_ShowDeferredWarning.value = (!InternalEditorUtility.HasProFeaturesEnabled() && (PlayerSettings.renderingPath == RenderingPath.DeferredLighting || PlayerSettings.renderingPath == RenderingPath.DeferredLighting));
			this.m_ShowDefaultIsNativeResolution.value = this.m_DefaultIsFullScreen.boolValue;
			this.m_ShowResolution.value = (!this.m_DefaultIsFullScreen.boolValue || !this.m_DefaultIsNativeResolution.boolValue);
			this.m_ShowDeferredWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowDefaultIsNativeResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
		}
		private void OnDisable()
		{
			this.m_WebPlayerTemplateManager.ClearTemplates();
		}
		public override bool UseDefaultMargins()
		{
			return false;
		}
		public override void OnInspectorGUI()
		{
			if (PlayerSettingsEditor.s_Styles == null)
			{
				PlayerSettingsEditor.s_Styles = new PlayerSettingsEditor.Styles();
			}
			base.serializedObject.Update();
			this.m_ShowDeferredWarning.target = (!InternalEditorUtility.HasProFeaturesEnabled() && PlayerSettings.renderingPath == RenderingPath.DeferredLighting);
			EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
			this.CommonSettings();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			int num = this.selectedPlatform;
			this.selectedPlatform = EditorGUILayout.BeginPlatformGrouping(this.validPlatforms, null);
			if (EditorGUI.EndChangeCheck())
			{
				if (EditorGUI.s_DelayedTextEditor.IsEditingControl(this.scriptingDefinesControlID))
				{
					EditorGUI.EndEditingActiveTextField();
					GUIUtility.keyboardControl = 0;
					PlayerSettings.SetScriptingDefineSymbolsForGroup(this.validPlatforms[num].targetGroup, EditorGUI.s_DelayedTextEditor.content.text);
				}
				GUI.FocusControl(string.Empty);
			}
			GUILayout.Label("Settings for " + this.validPlatforms[this.selectedPlatform].title.text, new GUILayoutOption[0]);
			EditorGUIUtility.labelWidth = Mathf.Max(150f, EditorGUIUtility.labelWidth - 8f);
			BuildPlayerWindow.BuildPlatform buildPlatform = this.validPlatforms[this.selectedPlatform];
			BuildTargetGroup targetGroup = buildPlatform.targetGroup;
			string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(targetGroup);
			ISettingEditorExtension editorSettingsExtension = ModuleManager.GetEditorSettingsExtension(targetStringFromBuildTargetGroup);
			this.ResolutionSectionGUI(targetGroup, editorSettingsExtension);
			this.IconSectionGUI(targetGroup, editorSettingsExtension);
			this.SplashSectionGUI(buildPlatform, targetGroup, editorSettingsExtension);
			this.DebugAndCrashReportingGUI(buildPlatform, targetGroup, editorSettingsExtension);
			this.OtherSectionGUI(buildPlatform, targetGroup, editorSettingsExtension);
			this.PublishSectionGUI(targetGroup, editorSettingsExtension);
			EditorGUILayout.EndPlatformGrouping();
			base.serializedObject.ApplyModifiedProperties();
		}
		private void CommonSettings()
		{
			EditorGUILayout.PropertyField(this.m_CompanyName, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ProductName, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUI.changed = false;
			string empty = string.Empty;
			Texture2D[] array = PlayerSettings.GetIconsForPlatform(empty);
			int[] iconSizesForPlatform = PlayerSettings.GetIconSizesForPlatform(empty);
			if (array.Length != iconSizesForPlatform.Length)
			{
				array = new Texture2D[iconSizesForPlatform.Length];
				PlayerSettings.SetIconsForPlatform(empty, array);
			}
			array[0] = (Texture2D)EditorGUILayout.ObjectField(PlayerSettingsEditor.s_Styles.defaultIcon, array[0], typeof(Texture2D), false, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				PlayerSettings.SetIconsForPlatform(empty, array);
			}
			GUILayout.Space(3f);
			this.m_DefaultCursor.objectReferenceValue = EditorGUILayout.ObjectField(PlayerSettingsEditor.s_Styles.defaultCursor, this.m_DefaultCursor.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
			Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			rect = EditorGUI.PrefixLabel(rect, 0, PlayerSettingsEditor.s_Styles.cursorHotspot);
			EditorGUI.PropertyField(rect, this.m_CursorHotspot, GUIContent.none);
		}
		private bool BeginSettingsBox(int nr, GUIContent header)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUILayout.BeginVertical(PlayerSettingsEditor.s_Styles.categoryBox, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(20f, 18f);
			rect.x += 3f;
			rect.width += 6f;
			bool flag = GUI.Toggle(rect, this.m_SelectedSection.value == nr, header, EditorStyles.inspectorTitlebarText);
			if (GUI.changed)
			{
				this.m_SelectedSection.value = ((!flag) ? -1 : nr);
			}
			this.m_SectionAnimators[nr].target = flag;
			GUI.enabled = enabled;
			return EditorGUILayout.BeginFadeGroup(this.m_SectionAnimators[nr].faded);
		}
		private void EndSettingsBox()
		{
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.EndVertical();
		}
		private void ShowNoSettings()
		{
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.NotApplicableForPlatform"), EditorStyles.miniLabel, new GUILayoutOption[0]);
		}
		private void ShowSharedNote()
		{
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.SharedSettingsFootnote"), EditorStyles.miniLabel, new GUILayoutOption[0]);
		}
		private void IconSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUI.changed = false;
			if (this.BeginSettingsBox(1, EditorGUIUtility.TextContent("PlayerSettings.IconHeader")))
			{
				bool flag = this.selectedPlatform < 0;
				BuildPlayerWindow.BuildPlatform buildPlatform = null;
				targetGroup = BuildTargetGroup.Standalone;
				string platform = string.Empty;
				if (!flag)
				{
					buildPlatform = this.validPlatforms[this.selectedPlatform];
					targetGroup = buildPlatform.targetGroup;
					platform = buildPlatform.name;
				}
				bool enabled = GUI.enabled;
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.WebPlayer || PlayerSettingsEditor.IsWP8Player(targetGroup) || targetGroup == BuildTargetGroup.SamsungTV)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				else
				{
					if (targetGroup != BuildTargetGroup.Metro)
					{
						Texture2D[] array = PlayerSettings.GetIconsForPlatform(platform);
						int[] iconSizesForPlatform = PlayerSettings.GetIconSizesForPlatform(platform);
						bool flag2 = true;
						if (flag)
						{
							if (array.Length != iconSizesForPlatform.Length)
							{
								array = new Texture2D[iconSizesForPlatform.Length];
								PlayerSettings.SetIconsForPlatform(platform, array);
							}
						}
						else
						{
							GUI.changed = false;
							flag2 = (array.Length == iconSizesForPlatform.Length);
							flag2 = GUILayout.Toggle(flag2, "Override for " + buildPlatform.name, new GUILayoutOption[0]);
							GUI.enabled = (enabled && flag2);
							if (GUI.changed || (!flag2 && array.Length > 0))
							{
								if (flag2)
								{
									array = new Texture2D[iconSizesForPlatform.Length];
								}
								else
								{
									array = new Texture2D[0];
								}
								PlayerSettings.SetIconsForPlatform(platform, array);
							}
						}
						GUI.changed = false;
						for (int i = 0; i < iconSizesForPlatform.Length; i++)
						{
							int num = Mathf.Min(96, iconSizesForPlatform[i]);
							Rect rect = GUILayoutUtility.GetRect(64f, (float)(Mathf.Max(64, num) + 6));
							float num2 = Mathf.Min(rect.width, EditorGUIUtility.labelWidth + 4f + 64f + 6f + 96f);
							string text = iconSizesForPlatform[i] + "x" + iconSizesForPlatform[i];
							GUI.Label(new Rect(rect.x, rect.y, num2 - 96f - 64f - 12f, 20f), text);
							if (flag2)
							{
								array[i] = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x + num2 - 96f - 64f - 6f, rect.y, 64f, 64f), array[i], typeof(Texture2D), false);
							}
							Rect position = new Rect(rect.x + num2 - 96f, rect.y, (float)num, (float)num);
							Texture2D iconForPlatformAtSize = PlayerSettings.GetIconForPlatformAtSize(platform, iconSizesForPlatform[i]);
							if (iconForPlatformAtSize != null)
							{
								GUI.DrawTexture(position, iconForPlatformAtSize);
							}
							else
							{
								GUI.Box(position, string.Empty);
							}
						}
						if (GUI.changed)
						{
							PlayerSettings.SetIconsForPlatform(platform, array);
						}
						GUI.enabled = enabled;
						if (targetGroup == BuildTargetGroup.iOS)
						{
							EditorGUILayout.PropertyField(this.m_UIPrerenderedIcon, EditorGUIUtility.TextContent("PlayerSettings.UIPrerenderedIcon"), new GUILayoutOption[0]);
							EditorGUILayout.Space();
						}
					}
				}
				if (settingsExtension != null)
				{
					settingsExtension.IconSectionGUI();
				}
			}
			this.EndSettingsBox();
		}
		private void SplashSectionGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUI.changed = false;
			if (this.BeginSettingsBox(2, EditorGUIUtility.TextContent("PlayerSettings.SplashHeader")))
			{
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					this.m_ResolutionDialogBanner.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.ResolutionDialogBanner"), (Texture2D)this.m_ResolutionDialogBanner.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.WebPlayer || PlayerSettingsEditor.IsWP8Player(targetGroup))
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.m_XboxSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.XboxSplashScreen"), (Texture2D)this.m_XboxSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.PS3)
				{
					this.m_ps3SplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.ps3SplashScreen"), (Texture2D)this.m_ps3SplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				bool flag = InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget) || targetGroup == BuildTargetGroup.iOS;
				EditorGUI.BeginDisabledGroup(!flag);
				if (settingsExtension != null)
				{
					settingsExtension.SplashSectionGUI();
				}
				EditorGUI.EndDisabledGroup();
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android)
				{
					this.ShowSharedNote();
				}
			}
			this.EndSettingsBox();
		}
		public void ResolutionSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUI.changed = false;
			if (this.BeginSettingsBox(0, EditorGUIUtility.TextContent("PlayerSettings.ResolutionHeader")))
			{
				if (settingsExtension != null)
				{
					float h = 16f;
					float midWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
					float maxWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
					settingsExtension.ResolutionSectionGUI(h, midWidth, maxWidth);
				}
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.SamsungTV)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ResolutionSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, new GUILayoutOption[0]);
					this.m_ShowDefaultIsNativeResolution.target = this.m_DefaultIsFullScreen.boolValue;
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowDefaultIsNativeResolution.faded))
					{
						EditorGUILayout.PropertyField(this.m_DefaultIsNativeResolution, new GUILayoutOption[0]);
					}
					if (this.m_ShowDefaultIsNativeResolution.faded != 0f && this.m_ShowDefaultIsNativeResolution.faded != 1f)
					{
						EditorGUILayout.EndFadeGroup();
					}
					this.m_ShowResolution.target = (!this.m_DefaultIsFullScreen.boolValue || !this.m_DefaultIsNativeResolution.boolValue);
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolution.faded))
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_DefaultScreenWidth, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenWidth"), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck() && this.m_DefaultScreenWidth.intValue < 1)
						{
							this.m_DefaultScreenWidth.intValue = 1;
						}
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_DefaultScreenHeight, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenHeight"), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck() && this.m_DefaultScreenHeight.intValue < 1)
						{
							this.m_DefaultScreenHeight.intValue = 1;
						}
					}
					if (this.m_ShowResolution.faded != 0f && this.m_ShowResolution.faded != 1f)
					{
						EditorGUILayout.EndFadeGroup();
					}
				}
				if (targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.WebGL)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ResolutionSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_DefaultScreenWidthWeb, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenWidthWeb"), new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck() && this.m_DefaultScreenWidthWeb.intValue < 1)
					{
						this.m_DefaultScreenWidthWeb.intValue = 1;
					}
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_DefaultScreenHeightWeb, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenHeightWeb"), new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck() && this.m_DefaultScreenHeightWeb.intValue < 1)
					{
						this.m_DefaultScreenHeightWeb.intValue = 1;
					}
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.BlackBerry || targetGroup == BuildTargetGroup.WebGL)
				{
					EditorGUILayout.PropertyField(this.m_RunInBackground, EditorGUIUtility.TextContent("PlayerSettings.RunInBackground"), new GUILayoutOption[0]);
				}
				if ((settingsExtension != null && settingsExtension.SupportsOrientation()) || PlayerSettingsEditor.IsWP8Player(targetGroup))
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ScreenOrientationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultScreenOrientation, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenOrientation"), new GUILayoutOption[0]);
					if (this.m_DefaultScreenOrientation.enumValueIndex == 4)
					{
						if (targetGroup == BuildTargetGroup.iOS)
						{
							EditorGUILayout.PropertyField(this.m_UseOSAutoRotation, EditorGUIUtility.TextContent("PlayerSettings.UseOSAutorotation"), new GUILayoutOption[0]);
						}
						EditorGUI.indentLevel++;
						GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.AutoRotationAllowedOrientation"), EditorStyles.boldLabel, new GUILayoutOption[0]);
						if (!this.m_AllowedAutoRotateToPortrait.boolValue && !this.m_AllowedAutoRotateToPortraitUpsideDown.boolValue && !this.m_AllowedAutoRotateToLandscapeRight.boolValue && !this.m_AllowedAutoRotateToLandscapeLeft.boolValue)
						{
							this.m_AllowedAutoRotateToPortrait.boolValue = true;
							Debug.LogError("All orientations are disabled. Allowing portrait");
						}
						EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortrait, EditorGUIUtility.TextContent("PlayerSettings.PortraitOrientation"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortraitUpsideDown, EditorGUIUtility.TextContent("PlayerSettings.PortraitUpsideDownOrientation"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeRight, EditorGUIUtility.TextContent("PlayerSettings.LandscapeRightOrientation"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeLeft, EditorGUIUtility.TextContent("PlayerSettings.LandscapeLeftOrientation"), new GUILayoutOption[0]);
						EditorGUI.indentLevel--;
					}
				}
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android)
				{
					if (targetGroup != BuildTargetGroup.WP8)
					{
						GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.StatusBarSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_UIStatusBarHidden, EditorGUIUtility.TextContent("PlayerSettings.UIStatusBarHidden"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_UIStatusBarStyle, EditorGUIUtility.TextContent("PlayerSettings.UIStatusBarStyle"), new GUILayoutOption[0]);
						EditorGUILayout.Space();
					}
				}
				EditorGUILayout.Space();
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.StandalonePlayerSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_CaptureSingleScreen, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DisplayResolutionDialog, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UsePlayerLog, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ResizableWindow, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UseMacAppStoreValidation, EditorGUIUtility.TempContent("Mac App Store Validation"), new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_MacFullscreenMode, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_D3D9FullscreenMode, EditorGUIUtility.TempContent("D3D9 Fullscreen Mode"), new GUILayoutOption[0]);
					bool useDirect3D = PlayerSettings.useDirect3D11;
					bool flag = true;
					EditorGUI.BeginDisabledGroup(useDirect3D);
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(!useDirect3D);
					EditorGUILayout.PropertyField(this.m_D3D11FullscreenMode, EditorGUIUtility.TempContent("D3D11 Fullscreen Mode"), new GUILayoutOption[0]);
					if (PlayerSettings.d3d11FullscreenMode == D3D11FullscreenMode.ExclusiveMode)
					{
						EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("PlayerSettings.DX11ExclusiveFullscreenWarning").text, MessageType.Info);
					}
					EditorGUI.EndDisabledGroup();
					if (!useDirect3D && flag)
					{
						this.m_VisibleInBackground.boolValue = false;
					}
					EditorGUI.BeginDisabledGroup(!useDirect3D && flag);
					EditorGUILayout.PropertyField(this.m_VisibleInBackground, EditorGUIUtility.TempContent("Visible In Background"), new GUILayoutOption[0]);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.PropertyField(this.m_ForceSingleInstance, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_SupportedAspectRatios, true, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.WebPlayer)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.WebPlayerTemplateSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					this.m_WebPlayerTemplateManager.SelectionUI(this.m_WebPlayerTemplate);
					EditorGUILayout.Space();
				}
				if (this.IsMobileTarget(targetGroup))
				{
					if (targetGroup != BuildTargetGroup.Tizen && targetGroup != BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_Use32BitDisplayBuffer, EditorGUIUtility.TextContent("PlayerSettings.Use32BitDisplayBuffer"), new GUILayoutOption[0]);
					}
					EditorGUILayout.PropertyField(this.m_DisableDepthAndStencilBuffers, EditorGUIUtility.TextContent("PlayerSettings.DisableDepthAndStencilBuffers"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iOS)
				{
					EditorGUILayout.PropertyField(this.m_iosShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("PlayerSettings.iosShowActivityIndicatorOnLoading"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Android)
				{
					EditorGUILayout.PropertyField(this.m_androidShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("PlayerSettings.androidShowActivityIndicatorOnLoading"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android)
				{
					EditorGUILayout.Space();
				}
				this.ShowSharedNote();
			}
			this.EndSettingsBox();
		}
		private void ShowDisabledFakeEnumPopup(PlayerSettingsEditor.FakeEnum enumValue)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(this.m_ApiCompatibilityLevel.displayName);
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.EnumPopup(enumValue, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			GUILayout.EndHorizontal();
		}
		private void ShowAdvancedOptionWarning(string option, BuildTarget target)
		{
			if (!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(target))
			{
				GUIContent gUIContent = new GUIContent(option + " requires " + BuildPipeline.GetBuildTargetAdvancedLicenseName(target));
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning);
			}
		}
		private void DX11SettingGUI(BuildTargetGroup targetGroup)
		{
			if (targetGroup != BuildTargetGroup.Standalone && targetGroup != BuildTargetGroup.WebPlayer)
			{
				return;
			}
			bool flag = Application.platform == RuntimePlatform.WindowsEditor;
			bool useDirect3D = PlayerSettings.useDirect3D11;
			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			bool flag2 = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.UseDX11"), useDirect3D, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			if (flag2 != useDirect3D)
			{
				if (flag)
				{
					if (EditorUtility.DisplayDialog("Changing graphics device", "Changing DX11 option requires reloading all graphics objects, it might take a while", "Apply", "Cancel") && EditorApplication.SaveCurrentSceneIfUserWantsTo())
					{
						PlayerSettings.useDirect3D11 = flag2;
						ShaderUtil.RecreateGfxDevice();
						GUIUtility.ExitGUI();
					}
				}
				else
				{
					PlayerSettings.useDirect3D11 = flag2;
				}
			}
			if (flag2)
			{
				if (!flag)
				{
					EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("PlayerSettings.DX11Warning").text, MessageType.Warning);
				}
				else
				{
					if (!SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11"))
					{
						EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("PlayerSettings.DX11NotSupported").text, MessageType.Warning);
					}
				}
			}
		}
		public void DebugAndCrashReportingGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (targetGroup != BuildTargetGroup.iOS)
			{
				return;
			}
			GUI.changed = false;
			if (this.BeginSettingsBox(3, EditorGUIUtility.TextContent("PlayerSettings.DebugAndCrashReportingHeader")))
			{
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.DebugSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_EnableInternalProfiler, EditorGUIUtility.TextContent("PlayerSettings.EnableInternalProfiler"), new GUILayoutOption[0]);
				EditorGUILayout.Space();
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.CrashReportingSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ActionOnDotNetUnhandledException, EditorGUIUtility.TextContent("PlayerSettings.ActionOnDotNetUnhandledException"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_LogObjCUncaughtExceptions, EditorGUIUtility.TextContent("PlayerSettings.LogObjCUncaughtExceptions"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_EnableCrashReportAPI, EditorGUIUtility.TextContent("PlayerSettings.EnableCrashReportAPI"), new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
			this.EndSettingsBox();
		}
		public static void BuildDisabledEnumPopup(GUIContent selected, string uiString)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.Popup(EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]), EditorGUIUtility.TextContent(uiString), 0, new GUIContent[]
			{
				selected
			});
			EditorGUI.EndDisabledGroup();
		}
		public static int BuildEnumPopup<T>(int value, string uiString, string textPrefixOptional, T[] options)
		{
			T t = (T)((object)value);
			int selectedIndex = 0;
			for (int i = 1; i < options.Length; i++)
			{
				if (t.Equals(options[i]))
				{
					selectedIndex = i;
					break;
				}
			}
			GUIContent[] array = new GUIContent[options.Length];
			for (int j = 0; j < options.Length; j++)
			{
				array[j] = ((textPrefixOptional != null) ? EditorGUIUtility.TextContent(string.Format("{0}.{1}", textPrefixOptional, (int)((object)options[j]))) : new GUIContent(options[j].ToString()));
			}
			int num = EditorGUILayout.Popup(EditorGUIUtility.TextContent(uiString), selectedIndex, array, new GUILayoutOption[0]);
			return (int)((object)options[num]);
		}
		public void OtherSectionGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUI.changed = false;
			if (this.BeginSettingsBox(4, EditorGUIUtility.TextContent("PlayerSettings.OtherHeader")))
			{
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.RenderingSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || this.IsMobileTarget(targetGroup) || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.PS4)
				{
					bool flag = this.IsMobileTarget(targetGroup);
					EditorGUILayout.IntPopup((!flag) ? this.m_RenderingPath : this.m_MobileRenderingPath, PlayerSettingsEditor.kRenderPaths, PlayerSettingsEditor.kRenderPathValues, EditorGUIUtility.TextContent("PlayerSettings.RenderingPath"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer)
				{
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowDeferredWarning.faded))
					{
						GUIContent gUIContent = EditorGUIUtility.TextContent("CameraEditor.DeferredProOnly");
						EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, false);
					}
					EditorGUILayout.EndFadeGroup();
				}
				if ((targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.Metro || targetGroup == BuildTargetGroup.XBOX360) && InternalEditorUtility.HasProFeaturesEnabled())
				{
					EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
					EditorGUILayout.PropertyField(this.m_ActiveColorSpace, EditorGUIUtility.TextContent("PlayerSettings.ActiveColorSpace"), new GUILayoutOption[0]);
					EditorGUI.EndDisabledGroup();
					if (QualitySettings.activeColorSpace != QualitySettings.desiredColorSpace)
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditor.s_Styles.colorSpaceWarning.text, MessageType.Warning);
					}
				}
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.SamsungTV)
				{
					if (this.IsMobileTarget(targetGroup))
					{
						this.m_MobileMTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.MultithreadedRendering"), this.m_MobileMTRendering.boolValue, new GUILayoutOption[0]);
					}
					else
					{
						this.m_MTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.MultithreadedRendering"), this.m_MTRendering.boolValue, new GUILayoutOption[0]);
					}
				}
				else
				{
					if (targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
					{
						if (Unsupported.IsDeveloperBuild())
						{
							this.m_MTRendering.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.MultithreadedRendering"), this.m_MTRendering.boolValue, new GUILayoutOption[0]);
						}
						else
						{
							this.m_MTRendering.boolValue = true;
						}
					}
				}
				this.DX11SettingGUI(targetGroup);
				bool flag2 = targetGroup != BuildTargetGroup.PS3;
				bool flag3 = targetGroup != BuildTargetGroup.PS3 && targetGroup != BuildTargetGroup.XBOX360;
				int num;
				int num2;
				PlayerSettings.GetBatchingForPlatform(platform.DefaultTarget, out num, out num2);
				bool flag4 = false;
				if (!flag2 && num == 1)
				{
					num = 0;
					flag4 = true;
				}
				if (!flag3 && num2 == 1)
				{
					num2 = 0;
					flag4 = true;
				}
				if (flag4)
				{
					PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
				}
				EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget) || !flag2);
				if (GUI.enabled)
				{
					num = ((!EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.StaticBatching"), num != 0, new GUILayoutOption[0])) ? 0 : 1);
				}
				else
				{
					EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.StaticBatching"), false, new GUILayoutOption[0]);
				}
				EditorGUI.EndDisabledGroup();
				EditorGUI.BeginDisabledGroup(!flag3);
				num2 = ((!EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.DynamicBatching"), num2 != 0, new GUILayoutOption[0])) ? 0 : 1);
				EditorGUI.EndDisabledGroup();
				this.ShowAdvancedOptionWarning("Static Batching", platform.DefaultTarget);
				if (GUI.changed)
				{
					Undo.RecordObject(this.target, "Changed Batching Settings");
					PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
				}
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.Metro)
				{
					bool flag5 = InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget);
					EditorGUI.BeginDisabledGroup(!flag5);
					if (GUI.enabled)
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_SkinOnGPU, EditorGUIUtility.TextContent((targetGroup == BuildTargetGroup.PS4) ? "PlayerSettings.ComputeSkinning" : "PlayerSettings.GPUSkinning"), new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck())
						{
							ShaderUtil.RecreateSkinnedMeshResources();
						}
					}
					else
					{
						EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.GPUSkinning"), false, new GUILayoutOption[0]);
					}
					EditorGUI.EndDisabledGroup();
					if (!flag5)
					{
						this.ShowAdvancedOptionWarning("GPU skinning", platform.DefaultTarget);
					}
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.m_XboxPIXTextureCapture.boolValue = EditorGUILayout.Toggle("Enable PIX texture capture", this.m_XboxPIXTextureCapture.boolValue, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.Metro)
				{
					EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget));
					if (GUI.enabled)
					{
						PlayerSettings.stereoscopic3D = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.Stereo3D"), PlayerSettings.stereoscopic3D, new GUILayoutOption[0]);
					}
					else
					{
						EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.Stereo3D"), false, new GUILayoutOption[0]);
					}
					EditorGUI.EndDisabledGroup();
				}
				EditorGUILayout.Space();
				if (targetGroup == BuildTargetGroup.WebPlayer)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.StreamingSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_FirstStreamedLevelWithResources, EditorGUIUtility.TextContent("PlayerSettings.FirstStreamedLevelWithResources"), new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (settingsExtension != null && settingsExtension.HasIdentificationGUI())
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.IdentificationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ApplicationBundleIdentifier, EditorGUIUtility.TextContent("PlayerSettings.bundleIdentifier"), new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("PlayerSettings.bundleVersion"), new GUILayoutOption[0]);
					if (settingsExtension != null)
					{
						settingsExtension.IdentificationSectionGUI();
					}
					EditorGUILayout.Space();
				}
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ConfigurationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(targetGroup);
				IScriptingImplementations scriptingImplementations = ModuleManager.GetScriptingImplementations(targetStringFromBuildTargetGroup);
				if (scriptingImplementations == null)
				{
					PlayerSettingsEditor.BuildDisabledEnumPopup(EditorGUIUtility.TextContent("PlayerSettings.ScriptingBackend.Default"), "PlayerSettings.ScriptingBackend");
				}
				else
				{
					ScriptingImplementation[] enabled = scriptingImplementations.Enabled;
					int propertyInt = PlayerSettings.GetPropertyInt("ScriptingBackend", targetGroup);
					int num3 = PlayerSettingsEditor.BuildEnumPopup<ScriptingImplementation>(propertyInt, "PlayerSettings.ScriptingBackend", "PlayerSettings.ScriptingBackend", enabled);
					if (num3 != propertyInt)
					{
						PlayerSettings.SetPropertyInt("ScriptingBackend", num3, targetGroup);
					}
					if (enabled.Any((ScriptingImplementation backend) => backend == ScriptingImplementation.IL2CPP))
					{
						bool flag6 = false;
						PlayerSettings.GetPropertyOptionalBool("UseIl2CppPrecompiledHeader", ref flag6, targetGroup);
						bool flag7 = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.UseIl2CppPrecompiledHeader"), flag6, new GUILayoutOption[0]);
						if (flag7 != flag6)
						{
							PlayerSettings.SetPropertyBool("UseIl2CppPrecompiledHeader", flag7, targetGroup);
						}
					}
				}
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.Metro || targetGroup == BuildTargetGroup.BlackBerry || PlayerSettingsEditor.IsWP8Player(targetGroup))
				{
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_TargetDevice, new GUILayoutOption[0]);
						if ((this.m_TargetDevice.intValue == 1 || this.m_TargetDevice.intValue == 2) && this.m_IPhoneTargetOSVersion.intValue <= 6)
						{
							this.m_IPhoneTargetOSVersion.intValue = 7;
						}
						EditorGUILayout.PropertyField(this.m_TargetResolution, EditorGUIUtility.TextContent("PlayerSettings.TargetResolution"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BlackBerry)
					{
						EditorGUILayout.PropertyField(this.m_TargetGlesGraphics, EditorGUIUtility.TextContent("PlayerSettings.TargetGlesGraphics"), new GUILayoutOption[0]);
					}
					else
					{
						if (targetGroup == BuildTargetGroup.iOS)
						{
							EditorGUILayout.PropertyField(this.m_TargetIOSGraphics, EditorGUIUtility.TextContent("PlayerSettings.TargetIOSGraphics"), new GUILayoutOption[0]);
						}
					}
					if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Metro || PlayerSettingsEditor.IsWP8Player(targetGroup))
					{
						EditorGUILayout.PropertyField(this.m_AccelerometerFrequency, EditorGUIUtility.TextContent("PlayerSettings.AccelerometerFrequency"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_LocationUsageDescription, EditorGUIUtility.TextContent("PlayerSettings.IOSLocationUsageDescription"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_OverrideIPodMusic, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_PrepareIOSForRecording, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_UIRequiresPersistentWiFi, EditorGUIUtility.TextContent("PlayerSettings.UIRequiresPersistentWiFi"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_IOSAppInBackgroundBehavior, EditorGUIUtility.TextContent("PlayerSettings.iosAppInBackgroundBehavior"), new GUILayoutOption[0]);
					}
				}
				EditorGUI.BeginDisabledGroup(!Application.HasProLicense());
				bool flag8 = !this.m_SubmitAnalytics.boolValue;
				bool flag9 = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.submitAnalytics"), flag8, new GUILayoutOption[0]);
				if (flag8 != flag9)
				{
					this.m_SubmitAnalytics.boolValue = !flag9;
				}
				if (!Application.HasProLicense())
				{
					this.m_SubmitAnalytics.boolValue = true;
				}
				EditorGUI.EndDisabledGroup();
				if (settingsExtension != null)
				{
					settingsExtension.ConfigurationSectionGUI();
				}
				EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.scriptingDefineSymbols"), new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				string defines = EditorGUILayout.DelayedTextField(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup), null, EditorStyles.textField, new GUILayoutOption[0]);
				this.scriptingDefinesControlID = EditorGUIUtility.s_LastControlID;
				if (EditorGUI.EndChangeCheck())
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
				}
				EditorGUILayout.Space();
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.OptimizationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.WebPlayer)
				{
					this.ShowDisabledFakeEnumPopup(PlayerSettingsEditor.FakeEnum.WebplayerSubset);
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_ApiCompatibilityLevel, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						PlayerSettings.SetApiCompatibilityInternal(this.m_ApiCompatibilityLevel.intValue);
					}
				}
				EditorGUILayout.PropertyField(this.m_BakeCollisionMeshes, EditorGUIUtility.TextContent("PlayerSettings.BakeCollisionMeshes"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_PreloadShaders, EditorGUIUtility.TextContent("PlayerSettings.PreloadShaders"), new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_PreloadedAssets, EditorGUIUtility.TextContent("PlayerSettings.PreloadedAssets"), true, new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.XboxOne || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.PSP2)
				{
					EditorGUILayout.PropertyField(this.m_AotOptions, EditorGUIUtility.TextContent("PlayerSettings.aotOptions"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BlackBerry || targetGroup == BuildTargetGroup.Tizen || targetGroup == BuildTargetGroup.WebGL || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.XboxOne)
				{
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_IPhoneSdkVersion, EditorGUIUtility.TextContent("PlayerSettings.IPhoneSdkVersion"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_IPhoneTargetOSVersion, EditorGUIUtility.TextContent("PlayerSettings.IPhoneTargetOSVersion"), new GUILayoutOption[0]);
					}
					if (InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget))
					{
						EditorGUILayout.PropertyField(this.m_IPhoneStrippingLevel, EditorGUIUtility.TextContent("PlayerSettings.IPhoneStrippingLevel"), new GUILayoutOption[0]);
					}
					else
					{
						EditorGUI.BeginDisabledGroup(true);
						int[] optionValues = new int[1];
						GUIContent[] displayedOptions = new GUIContent[]
						{
							new GUIContent("Disabled")
						};
						EditorGUILayout.IntPopup(EditorGUIUtility.TextContent("PlayerSettings.IPhoneStrippingLevel"), 0, displayedOptions, optionValues, new GUILayoutOption[0]);
						EditorGUI.EndDisabledGroup();
					}
					if (targetGroup == BuildTargetGroup.iOS)
					{
						EditorGUILayout.PropertyField(this.m_IPhoneScriptCallOptimization, EditorGUIUtility.TextContent("PlayerSettings.IPhoneScriptCallOptimization"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.Android)
					{
						EditorGUILayout.PropertyField(this.m_AndroidProfiler, EditorGUIUtility.TextContent("PlayerSettings.AndroidProfiler"), new GUILayoutOption[0]);
					}
					EditorGUILayout.Space();
				}
				if (targetGroup != BuildTargetGroup.PSM)
				{
					EditorGUILayout.PropertyField(this.m_StripUnusedMeshComponents, EditorGUIUtility.TextContent("PlayerSettings.StripUnusedMeshComponents"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_VideoMemoryForVertexBuffers, EditorGUIUtility.TextContent("PlayerSettings.VideoMemoryForVertexBuffers"), new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						if (this.m_VideoMemoryForVertexBuffers.intValue < 0)
						{
							this.m_VideoMemoryForVertexBuffers.intValue = 0;
						}
						else
						{
							if (this.m_VideoMemoryForVertexBuffers.intValue > 192)
							{
								this.m_VideoMemoryForVertexBuffers.intValue = 192;
							}
						}
					}
				}
				EditorGUILayout.Space();
				this.ShowSharedNote();
			}
			this.EndSettingsBox();
		}
		private void AutoAssignProperty(SerializedProperty property, string packageDir, string fileName)
		{
			if (property.stringValue.Length == 0 || !File.Exists(Path.Combine(packageDir, property.stringValue)))
			{
				string path = Path.Combine(packageDir, fileName);
				if (File.Exists(path))
				{
					property.stringValue = fileName;
				}
			}
		}
		public void BrowseablePathProperty(string propertyLabel, SerializedProperty property, string browsePanelTitle, string extension, string dir)
		{
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUILayout.PrefixLabel(EditorGUIUtility.TextContent(propertyLabel));
			GUIContent content = new GUIContent("...");
			Vector2 vector = GUI.skin.GetStyle("Button").CalcSize(content);
			if (GUILayout.Button(content, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(vector.x)
			}))
			{
				GUI.FocusControl(string.Empty);
				string text = EditorGUIUtility.TextContent(browsePanelTitle).text;
				string text2 = (!string.IsNullOrEmpty(dir)) ? (dir.Replace('\\', '/') + "/") : (Directory.GetCurrentDirectory().Replace('\\', '/') + "/");
				string text3 = string.Empty;
				if (string.IsNullOrEmpty(extension))
				{
					text3 = EditorUtility.OpenFolderPanel(text, text2, string.Empty);
				}
				else
				{
					text3 = EditorUtility.OpenFilePanel(text, text2, extension);
				}
				if (text3.StartsWith(text2))
				{
					text3 = text3.Substring(text2.Length);
				}
				if (!string.IsNullOrEmpty(text3))
				{
					property.stringValue = text3;
					base.serializedObject.ApplyModifiedProperties();
					GUIUtility.ExitGUI();
				}
			}
			GUIContent gUIContent;
			if (string.IsNullOrEmpty(property.stringValue))
			{
				gUIContent = EditorGUIUtility.TextContent("Not selected.");
				EditorGUI.BeginDisabledGroup(true);
			}
			else
			{
				gUIContent = EditorGUIUtility.TempContent(property.stringValue);
				EditorGUI.BeginDisabledGroup(false);
			}
			EditorGUI.BeginChangeCheck();
			GUILayoutOption[] options = new GUILayoutOption[]
			{
				GUILayout.Width(32f),
				GUILayout.ExpandWidth(true)
			};
			string value = EditorGUILayout.TextArea(gUIContent.text, options);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck() && string.IsNullOrEmpty(value))
			{
				property.stringValue = string.Empty;
				base.serializedObject.ApplyModifiedProperties();
				GUI.FocusControl(string.Empty);
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext)
		{
			PlayerSettingsEditor.BuildFileBoxButton(prop, uiString, directory, ext, null);
		}
		internal static void BuildFileBoxButton(SerializedProperty prop, string uiString, string directory, string ext, Action onSelect)
		{
			float num = 16f;
			float minWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
			float maxWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
			Rect rect = GUILayoutUtility.GetRect(minWidth, maxWidth, num, num, EditorStyles.layerMaskField, null);
			float labelWidth = EditorGUIUtility.labelWidth;
			Rect position = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth - EditorGUI.indent, rect.height);
			Rect position2 = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
			string text = (prop.stringValue.Length != 0) ? prop.stringValue : "Not selected.";
			EditorGUI.TextArea(position2, text, EditorStyles.label);
			if (GUI.Button(position, EditorGUIUtility.TextContent(uiString)))
			{
				string text2 = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent(uiString).text, directory, ext);
				string projectRelativePath = FileUtil.GetProjectRelativePath(text2);
				prop.stringValue = ((!(projectRelativePath != string.Empty)) ? text2 : projectRelativePath);
				if (onSelect != null)
				{
					onSelect();
				}
				prop.serializedObject.ApplyModifiedProperties();
				GUIUtility.ExitGUI();
			}
		}
		public void PublishSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (targetGroup != BuildTargetGroup.Metro && targetGroup != BuildTargetGroup.XBOX360 && targetGroup != BuildTargetGroup.PS3 && targetGroup != BuildTargetGroup.PSP2 && targetGroup != BuildTargetGroup.PSM && (settingsExtension == null || !settingsExtension.HasPublishSection()))
			{
				return;
			}
			GUI.changed = false;
			if (this.BeginSettingsBox(5, EditorGUIUtility.TextContent("PlayerSettings.PublishingHeader")))
			{
				string directory = FileUtil.DeleteLastPathNameComponent(Application.dataPath);
				float h = 16f;
				float midWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
				float maxWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
				if (settingsExtension != null)
				{
					settingsExtension.PublishSectionGUI(h, midWidth, maxWidth);
				}
				if (targetGroup == BuildTargetGroup.PSM)
				{
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.m_XboxAdditionalTitleMemorySize = base.serializedObject.FindProperty("xboxAdditionalTitleMemorySize");
					this.m_XboxAdditionalTitleMemorySize.intValue = (int)EditorGUILayout.Slider(EditorGUIUtility.TextContent("PlayerSettings.XboxAdditionalTitleMemorySize"), (float)this.m_XboxAdditionalTitleMemorySize.intValue, 0f, 416f, new GUILayoutOption[0]);
					if (this.m_XboxAdditionalTitleMemorySize.intValue > 0)
					{
						PlayerSettingsEditor.ShowWarning(EditorGUIUtility.TextContent("PlayerSettings.XboxAdditionalTitleMemoryWarning"));
					}
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.SubmissionSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_XboxTitleId, EditorGUIUtility.TextContent("PlayerSettings.XboxTitleId"), new GUILayoutOption[0]);
					EditorGUILayout.Space();
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxImageConversion"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					PlayerSettingsEditor.BuildFileBoxButton(this.m_XboxImageXexPath, "PlayerSettings.XboxImageXEXFile", directory, "cfg", null);
					EditorGUILayout.Space();
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxLive"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					Action onSelect = delegate
					{
						if (this.m_XboxTitleId.stringValue.Length == 0)
						{
							Debug.LogWarning("Title id must be present when using a SPA file.");
						}
					};
					PlayerSettingsEditor.BuildFileBoxButton(this.m_XboxSpaPath, "PlayerSettings.XboxSpaFile", directory, "spa", onSelect);
					if (this.m_XboxSpaPath.stringValue.Length > 0)
					{
						bool boolValue = this.m_XboxGenerateSpa.boolValue;
						this.m_XboxGenerateSpa.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.XboxGenerateSPAConfig"), boolValue, new GUILayoutOption[0]);
						if (!boolValue && this.m_XboxGenerateSpa.boolValue)
						{
							InternalEditorUtility.Xbox360GenerateSPAConfig(this.m_XboxSpaPath.stringValue);
						}
					}
					this.m_XboxEnableGuest.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.XboxEnableGuest"), this.m_XboxEnableGuest.boolValue, new GUILayoutOption[0]);
					EditorGUILayout.Space();
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxServices"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					this.m_XboxEnableAvatar.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.XboxAvatarEnable"), this.m_XboxEnableAvatar.boolValue, new GUILayoutOption[0]);
					this.KinectGUI();
				}
			}
			this.EndSettingsBox();
		}
		private void KinectGUI()
		{
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxKinect"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_XboxEnableKinect.boolValue = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("PlayerSettings.XboxEnableKinect"), this.m_XboxEnableKinect.boolValue, new GUILayoutOption[0]);
			if (this.m_XboxEnableKinect.boolValue)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				this.m_XboxEnableHeadOrientation.boolValue = GUILayout.Toggle(this.m_XboxEnableHeadOrientation.boolValue, new GUIContent("Head Orientation", "Head orientation support"), new GUILayoutOption[0]);
				this.m_XboxEnableKinectAutoTracking.boolValue = GUILayout.Toggle(this.m_XboxEnableKinectAutoTracking.boolValue, new GUIContent("Auto Tracking", "Automatic player tracking"), new GUILayoutOption[0]);
				this.m_XboxEnableFitness.boolValue = GUILayout.Toggle(this.m_XboxEnableFitness.boolValue, new GUIContent("Fitness", "Fitness support"), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				this.m_XboxEnableSpeech.boolValue = GUILayout.Toggle(this.m_XboxEnableSpeech.boolValue, new GUIContent("Speech", "Speech Recognition Support"), new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				this.m_XboxDeployKinectResources.boolValue = true;
				if (this.m_XboxEnableHeadOrientation.boolValue)
				{
					this.m_XboxDeployHeadOrientation.boolValue = true;
				}
			}
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxKinectDeployResources"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUI.enabled = !this.m_XboxEnableKinect.boolValue;
			this.m_XboxDeployKinectResources.boolValue = GUILayout.Toggle(this.m_XboxDeployKinectResources.boolValue, new GUIContent("Base", "Identity and Skeleton Database files"), new GUILayoutOption[0]);
			GUI.enabled = (!this.m_XboxEnableHeadOrientation.boolValue || !this.m_XboxEnableKinect.boolValue);
			this.m_XboxDeployHeadOrientation.boolValue = GUILayout.Toggle(this.m_XboxDeployHeadOrientation.boolValue, new GUIContent("Head Orientation", "Head orientation database"), new GUILayoutOption[0]);
			GUI.enabled = true;
			this.m_XboxDeployKinectHeadPosition.boolValue = GUILayout.Toggle(this.m_XboxDeployKinectHeadPosition.boolValue, new GUIContent("Head Position", "Head position database"), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxKinectSpeech"), new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 1) != 0, new GUIContent("en-US", "Speech database: English - US, Canada"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 1) != 0)) ? 0 : 1);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 2) != 0, new GUIContent("fr-CA", "Speech database: French - Canada"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 2) != 0)) ? 0 : 2);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 4) != 0, new GUIContent("en-GB", "Speech database: English - United Kingdom, Ireland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 4) != 0)) ? 0 : 4);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 8) != 0, new GUIContent("es-MX", "Speech database: Spanish - Mexico"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 8) != 0)) ? 0 : 8);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 16) != 0, new GUIContent("ja-JP", "Speech database: Japanese - Japan"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 16) != 0)) ? 0 : 16);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 32) != 0, new GUIContent("fr-FR", "Speech database: French - France, Switzerland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 32) != 0)) ? 0 : 32);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 64) != 0, new GUIContent("es-ES", "Speech database: Spanish - Spain"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 64) != 0)) ? 0 : 64);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 128) != 0, new GUIContent("de-DE", "Speech database: German - Germany, Austria, Switzerland"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 128) != 0)) ? 0 : 128);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 256) != 0, new GUIContent("it-IT", "Speech database: Italian - Italy"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 256) != 0)) ? 0 : 256);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 512) != 0, new GUIContent("en-AU", "Speech database: English - Australia, New Zealand"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 512) != 0)) ? 0 : 512);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			this.m_XboxSpeechDB.intValue ^= ((GUILayout.Toggle((this.m_XboxSpeechDB.intValue & 1024) != 0, new GUIContent("pt-BR", "Speech database: Portuguese - Brazil"), new GUILayoutOption[0]) == ((this.m_XboxSpeechDB.intValue & 1024) != 0)) ? 0 : 1024);
			GUILayout.EndHorizontal();
		}
		private static void ShowWarning(GUIContent warningMessage)
		{
			if (PlayerSettingsEditor.s_WarningIcon == null)
			{
				PlayerSettingsEditor.s_WarningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
			}
			warningMessage.image = PlayerSettingsEditor.s_WarningIcon;
			GUILayout.Space(5f);
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUILayout.Label(warningMessage, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}
	}
}
