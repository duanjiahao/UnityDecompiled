using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
			public GUIStyle thumbnail = "IN ThumbnailShadow";
			public GUIStyle thumbnailLabel = "IN ThumbnailSelection";
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
			WebplayerSubset,
			FlashPlayerSubset
		}
		internal class WebPlayerTemplate
		{
			private static PlayerSettingsEditor.WebPlayerTemplate[] s_Templates;
			private static GUIContent[] s_TemplateGUIThumbnails;
			private string m_Path;
			private string m_Name;
			private Texture2D m_Thumbnail;
			private string[] m_CustomKeys;
			public string[] CustomKeys
			{
				get
				{
					return this.m_CustomKeys;
				}
			}
			public static PlayerSettingsEditor.WebPlayerTemplate[] Templates
			{
				get
				{
					if (PlayerSettingsEditor.WebPlayerTemplate.s_Templates == null || PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails == null)
					{
						PlayerSettingsEditor.WebPlayerTemplate.BuildTemplateList();
					}
					return PlayerSettingsEditor.WebPlayerTemplate.s_Templates;
				}
			}
			public static GUIContent[] TemplateGUIThumbnails
			{
				get
				{
					if (PlayerSettingsEditor.WebPlayerTemplate.s_Templates == null || PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails == null)
					{
						PlayerSettingsEditor.WebPlayerTemplate.BuildTemplateList();
					}
					return PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails;
				}
			}
			public static int GetTemplateIndex(string path)
			{
				for (int i = 0; i < PlayerSettingsEditor.WebPlayerTemplate.Templates.Length; i++)
				{
					if (path.Equals(PlayerSettingsEditor.WebPlayerTemplate.Templates[i].ToString()))
					{
						return i;
					}
				}
				return 0;
			}
			public static void ClearTemplates()
			{
				PlayerSettingsEditor.WebPlayerTemplate.s_Templates = null;
				PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails = null;
			}
			private static void BuildTemplateList()
			{
				List<PlayerSettingsEditor.WebPlayerTemplate> list = new List<PlayerSettingsEditor.WebPlayerTemplate>();
				string path = Path.Combine(Application.dataPath.Replace('/', Path.DirectorySeparatorChar), "WebPlayerTemplates");
				if (Directory.Exists(path))
				{
					list.AddRange(PlayerSettingsEditor.WebPlayerTemplate.ListTemplates(path));
				}
				string path2 = Path.Combine(Path.Combine(EditorApplication.applicationContentsPath.Replace('/', Path.DirectorySeparatorChar), "Resources"), "WebPlayerTemplates");
				if (Directory.Exists(path2))
				{
					list.AddRange(PlayerSettingsEditor.WebPlayerTemplate.ListTemplates(path2));
				}
				else
				{
					Debug.LogError("Did not find built-in templates.");
				}
				PlayerSettingsEditor.WebPlayerTemplate.s_Templates = list.ToArray();
				PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails = new GUIContent[PlayerSettingsEditor.WebPlayerTemplate.s_Templates.Length];
				for (int i = 0; i < PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails.Length; i++)
				{
					PlayerSettingsEditor.WebPlayerTemplate.s_TemplateGUIThumbnails[i] = PlayerSettingsEditor.WebPlayerTemplate.s_Templates[i].ToGUIContent();
				}
			}
			private static PlayerSettingsEditor.WebPlayerTemplate Load(string path)
			{
				if (!Directory.Exists(path) || Directory.GetFiles(path, "index.*").Length < 1)
				{
					return null;
				}
				string[] array = path.Split(new char[]
				{
					Path.DirectorySeparatorChar
				});
				PlayerSettingsEditor.WebPlayerTemplate webPlayerTemplate = new PlayerSettingsEditor.WebPlayerTemplate();
				webPlayerTemplate.m_Name = array[array.Length - 1];
				if (array.Length > 3 && array[array.Length - 3].Equals("Assets"))
				{
					webPlayerTemplate.m_Path = "PROJECT:" + webPlayerTemplate.m_Name;
				}
				else
				{
					webPlayerTemplate.m_Path = "APPLICATION:" + webPlayerTemplate.m_Name;
				}
				string[] files = Directory.GetFiles(path, "thumbnail.*");
				if (files.Length > 0)
				{
					webPlayerTemplate.m_Thumbnail = new Texture2D(2, 2);
					webPlayerTemplate.m_Thumbnail.LoadImage(File.ReadAllBytes(files[0]));
				}
				List<string> list = new List<string>();
				Regex regex = new Regex("\\%UNITY_CUSTOM_([A-Z_]+)\\%");
				MatchCollection matchCollection = regex.Matches(File.ReadAllText(Directory.GetFiles(path, "index.*")[0]));
				foreach (Match match in matchCollection)
				{
					string text = match.Value.Substring("%UNITY_CUSTOM_".Length);
					text = text.Substring(0, text.Length - 1);
					if (!list.Contains(text))
					{
						list.Add(text);
					}
				}
				webPlayerTemplate.m_CustomKeys = list.ToArray();
				return webPlayerTemplate;
			}
			private static List<PlayerSettingsEditor.WebPlayerTemplate> ListTemplates(string path)
			{
				List<PlayerSettingsEditor.WebPlayerTemplate> list = new List<PlayerSettingsEditor.WebPlayerTemplate>();
				string[] directories = Directory.GetDirectories(path);
				string[] array = directories;
				for (int i = 0; i < array.Length; i++)
				{
					string path2 = array[i];
					PlayerSettingsEditor.WebPlayerTemplate webPlayerTemplate = PlayerSettingsEditor.WebPlayerTemplate.Load(path2);
					if (webPlayerTemplate != null)
					{
						list.Add(webPlayerTemplate);
					}
				}
				return list;
			}
			public override bool Equals(object other)
			{
				return other is PlayerSettingsEditor.WebPlayerTemplate && other.ToString().Equals(this.ToString());
			}
			public override int GetHashCode()
			{
				return base.GetHashCode() ^ this.m_Path.GetHashCode();
			}
			public override string ToString()
			{
				return this.m_Path;
			}
			public GUIContent ToGUIContent()
			{
				return new GUIContent(this.m_Name, (!(this.m_Thumbnail == null)) ? this.m_Thumbnail : ((Texture2D)EditorGUIUtility.IconContent("BuildSettings.Web.Small").image));
			}
		}
		private const int kSlotSize = 64;
		private const int kMaxPreviewSize = 96;
		private const int kIconSpacing = 6;
		private const string kWebPlayerTemplateDefaultIconResource = "BuildSettings.Web.Small";
		private const float kWebPlayerTemplateGridPadding = 15f;
		private const float kThumbnailSize = 80f;
		private const float kThumbnailLabelHeight = 20f;
		private const float kThumbnailPadding = 5f;
		private static PlayerSettingsEditor.Styles s_Styles;
		private SavedInt m_SelectedSection = new SavedInt("PlayerSettings.ShownSection", -1);
		private BuildPlayerWindow.BuildPlatform[] validPlatforms;
		private SerializedProperty m_IPhoneBundleIdentifier;
		private SerializedProperty m_IPhoneBundleVersion;
		private SerializedProperty m_IPhoneShortBundleVersion;
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
		private SerializedProperty m_Use24BitDepthBuffer;
		private SerializedProperty m_iosShowActivityIndicatorOnLoading;
		private SerializedProperty m_androidShowActivityIndicatorOnLoading;
		private SerializedProperty m_IPhoneSdkVersion;
		private SerializedProperty m_IPhoneTargetOSVersion;
		private SerializedProperty m_AndroidProfiler;
		private SerializedProperty m_UIPrerenderedIcon;
		private SerializedProperty m_UIRequiresPersistentWiFi;
		private SerializedProperty m_UIStatusBarHidden;
		private SerializedProperty m_UIStatusBarStyle;
		private SerializedProperty m_UIExitOnSuspend;
		private SerializedProperty m_EnableHWStatistics;
		private SerializedProperty m_TargetDevice;
		private SerializedProperty m_TargetGlesGraphics;
		private SerializedProperty m_TargetIOSGraphics;
		private SerializedProperty m_TargetResolution;
		private SerializedProperty m_AccelerometerFrequency;
		private SerializedProperty m_OverrideIPodMusic;
		private SerializedProperty m_PrepareIOSForRecording;
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
		private SerializedProperty m_PS3TitleConfigPath;
		private SerializedProperty m_PS3DLCConfigPath;
		private SerializedProperty m_PS3ThumbnailPath;
		private SerializedProperty m_PS3BackgroundPath;
		private SerializedProperty m_PS3SoundPath;
		private SerializedProperty m_PS3TrophyCommId;
		private SerializedProperty m_PS3NpCommunicationPassphrase;
		private SerializedProperty m_PS3TrophyPackagePath;
		private SerializedProperty m_PS3BootCheckMaxSaveGameSizeKB;
		private SerializedProperty m_PS3TrophyCommSig;
		private SerializedProperty m_PS3TrialMode;
		private SerializedProperty m_PS3SaveGameSlots;
		private SerializedProperty m_PSP2SplashScreen;
		private SerializedProperty m_PSP2LiveAreaGate;
		private SerializedProperty m_PSP2LiveAreaBackground;
		private SerializedProperty m_PSP2NPCommsID;
		private SerializedProperty m_PSP2NPCommsPassphrase;
		private SerializedProperty m_PSP2NPCommsSig;
		private SerializedProperty m_PSP2TrophyPackPath;
		private SerializedProperty m_PSP2ParamSfxPath;
		private SerializedProperty m_PSP2PackagePassword;
		private SerializedProperty m_FlashStrippingLevel;
		private SerializedProperty m_VideoMemoryForVertexBuffers;
		private SerializedProperty m_MetroPackageName;
		private SerializedProperty m_MetroPackageLogo;
		private SerializedProperty m_MetroPackageLogo140;
		private SerializedProperty m_MetroPackageLogo180;
		private SerializedProperty m_MetroPackageLogo240;
		private SerializedProperty m_MetroPackageVersion;
		private SerializedProperty m_MetroApplicationDescription;
		private SerializedProperty m_MetroTileShortName;
		private SerializedProperty m_MetroTileBackgroundColor;
		private SerializedProperty m_MetroStoreTileLogo80;
		private SerializedProperty m_MetroStoreTileLogo;
		private SerializedProperty m_MetroStoreTileLogo140;
		private SerializedProperty m_MetroStoreTileLogo180;
		private SerializedProperty m_MetroStoreTileWideLogo80;
		private SerializedProperty m_MetroStoreTileWideLogo;
		private SerializedProperty m_MetroStoreTileWideLogo140;
		private SerializedProperty m_MetroStoreTileWideLogo180;
		private SerializedProperty m_MetroStoreTileSmallLogo80;
		private SerializedProperty m_MetroStoreTileSmallLogo;
		private SerializedProperty m_MetroStoreTileSmallLogo140;
		private SerializedProperty m_MetroStoreTileSmallLogo180;
		private SerializedProperty m_MetroStoreSmallTile80;
		private SerializedProperty m_MetroStoreSmallTile;
		private SerializedProperty m_MetroStoreSmallTile140;
		private SerializedProperty m_MetroStoreSmallTile180;
		private SerializedProperty m_MetroStoreLargeTile80;
		private SerializedProperty m_MetroStoreLargeTile;
		private SerializedProperty m_MetroStoreLargeTile140;
		private SerializedProperty m_MetroStoreLargeTile180;
		private SerializedProperty m_MetroStoreSplashScreenImage;
		private SerializedProperty m_MetroStoreSplashScreenImage140;
		private SerializedProperty m_MetroStoreSplashScreenImage180;
		private SerializedProperty m_MetroPhoneAppIcon;
		private SerializedProperty m_MetroPhoneAppIcon140;
		private SerializedProperty m_MetroPhoneAppIcon240;
		private SerializedProperty m_MetroPhoneSmallTile;
		private SerializedProperty m_MetroPhoneSmallTile140;
		private SerializedProperty m_MetroPhoneSmallTile240;
		private SerializedProperty m_MetroPhoneMediumTile;
		private SerializedProperty m_MetroPhoneMediumTile140;
		private SerializedProperty m_MetroPhoneMediumTile240;
		private SerializedProperty m_MetroPhoneWideTile;
		private SerializedProperty m_MetroPhoneWideTile140;
		private SerializedProperty m_MetroPhoneWideTile240;
		private SerializedProperty m_MetroPhoneSplashScreenImage;
		private SerializedProperty m_MetroPhoneSplashScreenImage140;
		private SerializedProperty m_MetroPhoneSplashScreenImage240;
		private SerializedProperty m_MetroUnprocessedPlugins;
		private SerializedProperty m_MetroEnableIndependentInputSource;
		private SerializedProperty m_MetroEnableLowLatencyPresentationAPI;
		private SerializedProperty m_WP8UnprocessedPlugins;
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
		private SerializedProperty m_ResizableWindow;
		private SerializedProperty m_StripPhysics;
		private SerializedProperty m_UseMacAppStoreValidation;
		private SerializedProperty m_MacFullscreenMode;
		private SerializedProperty m_D3D9FullscreenMode;
		private SerializedProperty m_D3D11ForceExclusiveMode;
		private SerializedProperty m_VisibleInBackground;
		private SerializedProperty m_ForceSingleInstance;
		private SerializedProperty m_RunInBackground;
		private SerializedProperty m_CaptureSingleScreen;
		private SerializedProperty m_ResolutionDialogBanner;
		private SerializedProperty m_SupportedAspectRatios;
		private SerializedProperty m_SkinOnGPU;
		private SerializedProperty m_FirstStreamedLevelWithResources;
		private SerializedProperty m_WebPlayerTemplate;
		private int selectedPlatform;
		private int scriptingDefinesControlID;
		private AnimBool[] m_SectionAnimators = new AnimBool[5];
		private readonly AnimBool m_ShowDeferredWarning = new AnimBool();
		private readonly AnimBool m_ShowDefaultIsNativeResolution = new AnimBool();
		private readonly AnimBool m_ShowResolution = new AnimBool();
		private static Texture2D s_WarningIcon;
		private Vector2 capScrollViewPosition = Vector2.zero;
		private static readonly Regex metroPackageNameRegex = new Regex("^[A-Za-z0-9\\.\\-]{2,49}[A-Za-z0-9\\-]$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private static readonly Regex metroPackageVersionRegex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private bool IsMobileTarget(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BB10 || targetGroup == BuildTargetGroup.Tizen;
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
			this.m_PS3TitleConfigPath = this.FindPropertyAssert("ps3TitleConfigPath");
			this.m_PS3DLCConfigPath = this.FindPropertyAssert("ps3DLCConfigPath");
			this.m_PS3ThumbnailPath = this.FindPropertyAssert("ps3ThumbnailPath");
			this.m_PS3BackgroundPath = this.FindPropertyAssert("ps3BackgroundPath");
			this.m_PS3SoundPath = this.FindPropertyAssert("ps3SoundPath");
			this.m_PS3TrophyCommId = this.FindPropertyAssert("ps3TrophyCommId");
			this.m_PS3NpCommunicationPassphrase = this.FindPropertyAssert("ps3NpCommunicationPassphrase");
			this.m_PS3TrophyPackagePath = this.FindPropertyAssert("ps3TrophyPackagePath");
			this.m_PS3BootCheckMaxSaveGameSizeKB = this.FindPropertyAssert("ps3BootCheckMaxSaveGameSizeKB");
			this.m_PS3TrophyCommSig = this.FindPropertyAssert("ps3TrophyCommSig");
			this.m_PS3TrialMode = this.FindPropertyAssert("ps3TrialMode");
			this.m_PS3SaveGameSlots = this.FindPropertyAssert("ps3SaveGameSlots");
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
			this.m_IPhoneBundleIdentifier = this.FindPropertyAssert("iPhoneBundleIdentifier");
			this.m_IPhoneBundleVersion = this.FindPropertyAssert("iPhoneBundleVersion");
			this.m_IPhoneShortBundleVersion = this.FindPropertyAssert("iPhoneShortBundleVersion");
			this.m_TargetResolution = this.FindPropertyAssert("targetResolution");
			this.m_AccelerometerFrequency = this.FindPropertyAssert("accelerometerFrequency");
			this.m_OverrideIPodMusic = this.FindPropertyAssert("Override IPod Music");
			this.m_PrepareIOSForRecording = this.FindPropertyAssert("Prepare IOS For Recording");
			this.m_UIRequiresPersistentWiFi = this.FindPropertyAssert("uIRequiresPersistentWiFi");
			this.m_UIExitOnSuspend = this.FindPropertyAssert("uIExitOnSuspend");
			this.m_EnableHWStatistics = this.FindPropertyAssert("enableHWStatistics");
			this.m_ApiCompatibilityLevel = this.FindPropertyAssert("apiCompatibilityLevel");
			this.m_AotOptions = this.FindPropertyAssert("aotOptions");
			this.m_LocationUsageDescription = this.FindPropertyAssert("locationUsageDescription");
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
			this.m_Use24BitDepthBuffer = this.FindPropertyAssert("use24BitDepthBuffer");
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
			this.m_ResizableWindow = this.FindPropertyAssert("resizableWindow");
			this.m_StripPhysics = this.FindPropertyAssert("stripPhysics");
			this.m_UseMacAppStoreValidation = this.FindPropertyAssert("useMacAppStoreValidation");
			this.m_D3D9FullscreenMode = this.FindPropertyAssert("d3d9FullscreenMode");
			this.m_D3D11ForceExclusiveMode = this.FindPropertyAssert("d3d11ForceExclusiveMode");
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
			this.m_FlashStrippingLevel = this.FindPropertyAssert("flashStrippingLevel");
			this.m_PSP2SplashScreen = this.FindPropertyAssert("psp2Splashimage");
			this.m_PSP2LiveAreaGate = this.FindPropertyAssert("psp2LiveAreaGate");
			this.m_PSP2LiveAreaBackground = this.FindPropertyAssert("psp2LiveAreaBackround");
			this.m_PSP2NPCommsID = this.FindPropertyAssert("psp2NPCommsID");
			this.m_PSP2NPCommsPassphrase = this.FindPropertyAssert("psp2NPCommsPassphrase");
			this.m_PSP2NPCommsSig = this.FindPropertyAssert("psp2NPCommsSig");
			this.m_PSP2TrophyPackPath = this.FindPropertyAssert("psp2NPTrophyPackPath");
			this.m_PSP2ParamSfxPath = this.FindPropertyAssert("psp2ParamSfxPath");
			this.m_PSP2PackagePassword = this.FindPropertyAssert("psp2PackagePassword");
			this.m_VideoMemoryForVertexBuffers = this.FindPropertyAssert("videoMemoryForVertexBuffers");
			this.m_MetroPackageName = this.FindPropertyAssert("metroPackageName");
			this.m_MetroPackageName.stringValue = this.ValidateMetroPackageName(this.m_MetroPackageName.stringValue);
			this.m_MetroPackageLogo = this.FindPropertyAssert("metroPackageLogo");
			this.m_MetroPackageLogo140 = this.FindPropertyAssert("metroPackageLogo140");
			this.m_MetroPackageLogo180 = this.FindPropertyAssert("metroPackageLogo180");
			this.m_MetroPackageLogo240 = this.FindPropertyAssert("metroPackageLogo240");
			this.m_MetroPackageVersion = this.FindPropertyAssert("metroPackageVersion");
			this.m_MetroPackageVersion.stringValue = PlayerSettingsEditor.ValidateMetroPackageVersion(this.m_MetroPackageVersion.stringValue);
			this.m_MetroApplicationDescription = this.FindPropertyAssert("metroApplicationDescription");
			this.m_MetroApplicationDescription.stringValue = this.ValidateMetroApplicationDescription(this.m_MetroApplicationDescription.stringValue);
			this.m_MetroStoreTileLogo80 = this.FindPropertyAssert("metroStoreTileLogo80");
			this.m_MetroStoreTileLogo = this.FindPropertyAssert("metroStoreTileLogo");
			this.m_MetroStoreTileLogo140 = this.FindPropertyAssert("metroStoreTileLogo140");
			this.m_MetroStoreTileLogo180 = this.FindPropertyAssert("metroStoreTileLogo180");
			this.m_MetroStoreTileWideLogo80 = this.FindPropertyAssert("metroStoreTileWideLogo80");
			this.m_MetroStoreTileWideLogo = this.FindPropertyAssert("metroStoreTileWideLogo");
			this.m_MetroStoreTileWideLogo140 = this.FindPropertyAssert("metroStoreTileWideLogo140");
			this.m_MetroStoreTileWideLogo180 = this.FindPropertyAssert("metroStoreTileWideLogo180");
			this.m_MetroStoreTileSmallLogo80 = this.FindPropertyAssert("metroStoreTileSmallLogo80");
			this.m_MetroStoreTileSmallLogo = this.FindPropertyAssert("metroStoreTileSmallLogo");
			this.m_MetroStoreTileSmallLogo140 = this.FindPropertyAssert("metroStoreTileSmallLogo140");
			this.m_MetroStoreTileSmallLogo180 = this.FindPropertyAssert("metroStoreTileSmallLogo180");
			this.m_MetroStoreSmallTile80 = this.FindPropertyAssert("metroStoreSmallTile80");
			this.m_MetroStoreSmallTile = this.FindPropertyAssert("metroStoreSmallTile");
			this.m_MetroStoreSmallTile140 = this.FindPropertyAssert("metroStoreSmallTile140");
			this.m_MetroStoreSmallTile180 = this.FindPropertyAssert("metroStoreSmallTile180");
			this.m_MetroStoreLargeTile80 = this.FindPropertyAssert("metroStoreLargeTile80");
			this.m_MetroStoreLargeTile = this.FindPropertyAssert("metroStoreLargeTile");
			this.m_MetroStoreLargeTile140 = this.FindPropertyAssert("metroStoreLargeTile140");
			this.m_MetroStoreLargeTile180 = this.FindPropertyAssert("metroStoreLargeTile180");
			this.m_MetroStoreSplashScreenImage = this.FindPropertyAssert("metroStoreSplashScreenImage");
			this.m_MetroStoreSplashScreenImage140 = this.FindPropertyAssert("metroStoreSplashScreenImage140");
			this.m_MetroStoreSplashScreenImage180 = this.FindPropertyAssert("metroStoreSplashScreenImage180");
			this.m_MetroPhoneAppIcon = this.FindPropertyAssert("metroPhoneAppIcon");
			this.m_MetroPhoneAppIcon140 = this.FindPropertyAssert("metroPhoneAppIcon140");
			this.m_MetroPhoneAppIcon240 = this.FindPropertyAssert("metroPhoneAppIcon240");
			this.m_MetroPhoneSmallTile = this.FindPropertyAssert("metroPhoneSmallTile");
			this.m_MetroPhoneSmallTile140 = this.FindPropertyAssert("metroPhoneSmallTile140");
			this.m_MetroPhoneSmallTile240 = this.FindPropertyAssert("metroPhoneSmallTile240");
			this.m_MetroPhoneMediumTile = this.FindPropertyAssert("metroPhoneMediumTile");
			this.m_MetroPhoneMediumTile140 = this.FindPropertyAssert("metroPhoneMediumTile140");
			this.m_MetroPhoneMediumTile240 = this.FindPropertyAssert("metroPhoneMediumTile240");
			this.m_MetroPhoneWideTile = this.FindPropertyAssert("metroPhoneWideTile");
			this.m_MetroPhoneWideTile140 = this.FindPropertyAssert("metroPhoneWideTile140");
			this.m_MetroPhoneWideTile240 = this.FindPropertyAssert("metroPhoneWideTile240");
			this.m_MetroPhoneSplashScreenImage = this.FindPropertyAssert("metroPhoneSplashScreenImage");
			this.m_MetroPhoneSplashScreenImage140 = this.FindPropertyAssert("metroPhoneSplashScreenImage140");
			this.m_MetroPhoneSplashScreenImage240 = this.FindPropertyAssert("metroPhoneSplashScreenImage240");
			this.m_MetroTileShortName = this.FindPropertyAssert("metroTileShortName");
			this.m_MetroTileShortName.stringValue = this.ValidateMetroTileShortName(this.m_MetroTileShortName.stringValue);
			this.m_MetroTileBackgroundColor = this.FindPropertyAssert("metroTileBackgroundColor");
			this.m_MetroUnprocessedPlugins = this.FindPropertyAssert("metroUnprocessedPlugins");
			this.m_MetroEnableIndependentInputSource = this.FindPropertyAssert("metroEnableIndependentInputSource");
			this.m_MetroEnableLowLatencyPresentationAPI = this.FindPropertyAssert("metroEnableLowLatencyPresentationAPI");
			this.m_WP8UnprocessedPlugins = this.FindPropertyAssert("wp8UnprocessedPlugins");
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
			this.m_ShowDeferredWarning.value = (!InternalEditorUtility.HasPro() && PlayerSettings.renderingPath == RenderingPath.DeferredLighting);
			this.m_ShowDefaultIsNativeResolution.value = this.m_DefaultIsFullScreen.boolValue;
			this.m_ShowResolution.value = (!this.m_DefaultIsFullScreen.boolValue || !this.m_DefaultIsNativeResolution.boolValue);
			this.m_ShowDeferredWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowDefaultIsNativeResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
		}
		private void OnDisable()
		{
			PlayerSettingsEditor.WebPlayerTemplate.ClearTemplates();
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
			this.m_ShowDeferredWarning.target = (!InternalEditorUtility.HasPro() && PlayerSettings.renderingPath == RenderingPath.DeferredLighting);
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
			this.IconSectionGUI(targetGroup);
			this.SplashSectionGUI(buildPlatform, targetGroup, editorSettingsExtension);
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
		private void IconSectionGUI(BuildTargetGroup targetGroup)
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
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.FlashPlayer || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.WP8)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				else
				{
					if (targetGroup == BuildTargetGroup.Metro)
					{
						float num = 16f;
						float num2 = 80f + EditorGUIUtility.fieldWidth + 5f;
						this.MetroLogoSection(num2, num2, num, num);
					}
					else
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
							int num3 = Mathf.Min(96, iconSizesForPlatform[i]);
							Rect rect = GUILayoutUtility.GetRect(64f, (float)(Mathf.Max(64, num3) + 6));
							float num4 = Mathf.Min(rect.width, EditorGUIUtility.labelWidth + 4f + 64f + 6f + 96f);
							string text = iconSizesForPlatform[i] + "x" + iconSizesForPlatform[i];
							GUI.Label(new Rect(rect.x, rect.y, num4 - 96f - 64f - 12f, 20f), text);
							if (flag2)
							{
								array[i] = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x + num4 - 96f - 64f - 6f, rect.y, 64f, 64f), array[i], typeof(Texture2D), false);
							}
							Rect position = new Rect(rect.x + num4 - 96f, rect.y, (float)num3, (float)num3);
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
						if (targetGroup == BuildTargetGroup.iPhone)
						{
							EditorGUILayout.PropertyField(this.m_UIPrerenderedIcon, EditorGUIUtility.TextContent("PlayerSettings.UIPrerenderedIcon"), new GUILayoutOption[0]);
							EditorGUILayout.Space();
						}
					}
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
				if (targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.FlashPlayer || targetGroup == BuildTargetGroup.NaCl || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.WP8)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.m_XboxSplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.XboxSplashScreen"), (Texture2D)this.m_XboxSplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
				{
					this.m_PSP2SplashScreen.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.psp2SplashScreen"), (Texture2D)this.m_PSP2SplashScreen.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				EditorGUI.BeginDisabledGroup(!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget));
				if (targetGroup == BuildTargetGroup.Metro)
				{
					float num = 16f;
					float num2 = 80f + EditorGUIUtility.fieldWidth + 5f;
					this.MetroSplashScreenSection(num2, num2, num, num);
				}
				if (settingsExtension != null)
				{
					settingsExtension.SplashSectionGUI();
				}
				EditorGUI.EndDisabledGroup();
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android)
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
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ResolutionSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, new GUILayoutOption[0]);
					this.m_ShowDefaultIsNativeResolution.target = this.m_DefaultIsFullScreen.boolValue;
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowDefaultIsNativeResolution.faded))
					{
						EditorGUILayout.PropertyField(this.m_DefaultIsNativeResolution, new GUILayoutOption[0]);
					}
					EditorGUILayout.EndFadeGroup();
					this.m_ShowResolution.target = (!this.m_DefaultIsFullScreen.boolValue || !this.m_DefaultIsNativeResolution.boolValue);
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowResolution.faded))
					{
						EditorGUILayout.PropertyField(this.m_DefaultScreenWidth, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenWidth"), new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_DefaultScreenHeight, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenHeight"), new GUILayoutOption[0]);
					}
					EditorGUILayout.EndFadeGroup();
				}
				if (targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.FlashPlayer || targetGroup == BuildTargetGroup.NaCl)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ResolutionSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultScreenWidthWeb, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenWidthWeb"), new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultScreenHeightWeb, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenHeightWeb"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.XBOX360)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.Metro)
				{
					this.ShowNoSettings();
					EditorGUILayout.Space();
				}
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.BB10)
				{
					EditorGUILayout.PropertyField(this.m_RunInBackground, EditorGUIUtility.TextContent("PlayerSettings.RunInBackground"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || (settingsExtension != null && settingsExtension.SupportsOrientation()) || targetGroup == BuildTargetGroup.WP8)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ScreenOrientationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultScreenOrientation, EditorGUIUtility.TextContent("PlayerSettings.DefaultScreenOrientation"), new GUILayoutOption[0]);
					if (this.m_DefaultScreenOrientation.enumValueIndex == 4)
					{
						if (targetGroup == BuildTargetGroup.iPhone)
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
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android)
				{
					if (targetGroup != BuildTargetGroup.WP8)
					{
						GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.StatusBarSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_UIStatusBarHidden, EditorGUIUtility.TextContent("PlayerSettings.UIStatusBarHidden"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.iPhone)
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
					bool useDirect3D = PlayerSettings.useDirect3D11;
					bool flag = this.m_D3D9FullscreenMode.intValue == 0;
					if (useDirect3D)
					{
						this.m_D3D9FullscreenMode.intValue = 1;
					}
					EditorGUI.BeginDisabledGroup(useDirect3D);
					EditorGUILayout.PropertyField(this.m_D3D9FullscreenMode, EditorGUIUtility.TempContent("D3D9 Fullscreen Mode"), new GUILayoutOption[0]);
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(!useDirect3D);
					EditorGUILayout.PropertyField(this.m_D3D11ForceExclusiveMode, new GUIContent("D3D11 Force Exclusive Mode", "Only recommended to fix Oculus vsync.  There are known issues with alt-tab."), new GUILayoutOption[0]);
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
					if (PlayerSettingsEditor.WebPlayerTemplate.TemplateGUIThumbnails.Length < 1)
					{
						GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.NoTemplatesFound"), new GUILayoutOption[0]);
					}
					else
					{
						int num = Mathf.Min((int)Mathf.Max(((float)Screen.width - 30f) / 80f, 1f), PlayerSettingsEditor.WebPlayerTemplate.TemplateGUIThumbnails.Length);
						int num2 = Mathf.Max((int)Mathf.Ceil((float)PlayerSettingsEditor.WebPlayerTemplate.TemplateGUIThumbnails.Length / (float)num), 1);
						bool changed = GUI.changed;
						this.m_WebPlayerTemplate.stringValue = PlayerSettingsEditor.WebPlayerTemplate.Templates[PlayerSettingsEditor.ThumbnailList(GUILayoutUtility.GetRect((float)num * 80f, (float)num2 * 100f, new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(false)
						}), PlayerSettingsEditor.WebPlayerTemplate.GetTemplateIndex(this.m_WebPlayerTemplate.stringValue), PlayerSettingsEditor.WebPlayerTemplate.TemplateGUIThumbnails, num)].ToString();
						bool flag2 = !changed && GUI.changed;
						bool changed2 = GUI.changed;
						GUI.changed = false;
						string[] templateCustomKeys = PlayerSettings.templateCustomKeys;
						for (int i = 0; i < templateCustomKeys.Length; i++)
						{
							string text = templateCustomKeys[i];
							string text2 = PlayerSettings.GetTemplateCustomValue(text);
							text2 = EditorGUILayout.TextField(PlayerSettingsEditor.PrettyTemplateKeyName(text), text2, new GUILayoutOption[0]);
							PlayerSettings.SetTemplateCustomValue(text, text2);
						}
						if (GUI.changed)
						{
							base.serializedObject.Update();
						}
						GUI.changed |= changed2;
						if (flag2)
						{
							GUIUtility.hotControl = 0;
							GUIUtility.keyboardControl = 0;
							base.serializedObject.ApplyModifiedProperties();
							PlayerSettings.templateCustomKeys = PlayerSettingsEditor.WebPlayerTemplate.Templates[PlayerSettingsEditor.WebPlayerTemplate.GetTemplateIndex(this.m_WebPlayerTemplate.stringValue)].CustomKeys;
							base.serializedObject.Update();
						}
					}
					EditorGUILayout.Space();
				}
				if (this.IsMobileTarget(targetGroup))
				{
					if (targetGroup != BuildTargetGroup.Tizen)
					{
						EditorGUILayout.PropertyField(this.m_Use32BitDisplayBuffer, EditorGUIUtility.TextContent("PlayerSettings.Use32BitDisplayBuffer"), new GUILayoutOption[0]);
					}
					EditorGUILayout.PropertyField(this.m_Use24BitDepthBuffer, EditorGUIUtility.TextContent("PlayerSettings.Use24BitDepthBuffer"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone)
				{
					EditorGUILayout.PropertyField(this.m_iosShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("PlayerSettings.iosShowActivityIndicatorOnLoading"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Android)
				{
					EditorGUILayout.PropertyField(this.m_androidShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("PlayerSettings.androidShowActivityIndicatorOnLoading"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android)
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
			if (this.BeginSettingsBox(3, EditorGUIUtility.TextContent("PlayerSettings.OtherHeader")))
			{
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.RenderingSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || this.IsMobileTarget(targetGroup))
				{
					bool flag = this.IsMobileTarget(targetGroup);
					EditorGUILayout.PropertyField((!flag) ? this.m_RenderingPath : this.m_MobileRenderingPath, EditorGUIUtility.TextContent("PlayerSettings.RenderingPath"), new GUILayoutOption[0]);
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
				if ((targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.XBOX360) && InternalEditorUtility.HasPro())
				{
					EditorGUILayout.PropertyField(this.m_ActiveColorSpace, EditorGUIUtility.TextContent("PlayerSettings.ActiveColorSpace"), new GUILayoutOption[0]);
					if (QualitySettings.activeColorSpace != QualitySettings.desiredColorSpace)
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditor.s_Styles.colorSpaceWarning.text, MessageType.Warning);
					}
				}
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.Android)
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
				if (targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.WebPlayer || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Metro)
				{
					bool flag5 = InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(platform.DefaultTarget);
					EditorGUI.BeginDisabledGroup(!flag5);
					if (GUI.enabled)
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_SkinOnGPU, EditorGUIUtility.TextContent("PlayerSettings.GPUSkinning"), new GUILayoutOption[0]);
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
				if (targetGroup == BuildTargetGroup.iPhone || (settingsExtension != null && settingsExtension.HasIdentificationGUI()))
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.IdentificationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IPhoneBundleIdentifier, EditorGUIUtility.TextContent("PlayerSettings.IPhoneBundleIdentifier"), new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IPhoneBundleVersion, EditorGUIUtility.TextContent("PlayerSettings.IPhoneBundleVersion"), new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IPhoneShortBundleVersion, EditorGUIUtility.TextContent("PlayerSettings.IPhoneShortBundleVersion"), new GUILayoutOption[0]);
					if (settingsExtension != null)
					{
						settingsExtension.IdentificationSectionGUI();
					}
					EditorGUILayout.Space();
				}
				if (targetGroup != BuildTargetGroup.FlashPlayer)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ConfigurationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					if (targetGroup == BuildTargetGroup.iPhone)
					{
						ScriptingImplementation[] options = new ScriptingImplementation[]
						{
							ScriptingImplementation.Mono2x,
							ScriptingImplementation.IL2CPP
						};
						iPhoneArchitecture[] options2 = new iPhoneArchitecture[]
						{
							iPhoneArchitecture.ARMv7,
							iPhoneArchitecture.ARM64,
							iPhoneArchitecture.Universal
						};
						int num3 = 0;
						PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref num3, targetGroup);
						int num4 = PlayerSettingsEditor.BuildEnumPopup<ScriptingImplementation>(num3, "PlayerSettings.ScriptingBackend", "PlayerSettings.ScriptingBackend", options);
						if (num4 != num3)
						{
							PlayerSettings.SetPropertyInt("ScriptingBackend", num4, targetGroup);
						}
						int propertyInt = PlayerSettings.GetPropertyInt("Architecture", targetGroup);
						int num5;
						if (num3 == 1)
						{
							num5 = PlayerSettingsEditor.BuildEnumPopup<iPhoneArchitecture>(propertyInt, "PlayerSettings.iOS.Architecture", null, options2);
						}
						else
						{
							num5 = 0;
							PlayerSettingsEditor.BuildDisabledEnumPopup(new GUIContent("ARMv7"), "PlayerSettings.iOS.Architecture");
						}
						if (num5 != propertyInt)
						{
							PlayerSettings.SetPropertyInt("Architecture", num5, targetGroup);
						}
					}
					if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.Metro || targetGroup == BuildTargetGroup.BB10 || targetGroup == BuildTargetGroup.WP8)
					{
						if (targetGroup == BuildTargetGroup.iPhone)
						{
							EditorGUILayout.PropertyField(this.m_TargetDevice, new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_TargetResolution, EditorGUIUtility.TextContent("PlayerSettings.TargetResolution"), new GUILayoutOption[0]);
						}
						if (targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BB10)
						{
							EditorGUILayout.PropertyField(this.m_TargetGlesGraphics, EditorGUIUtility.TextContent("PlayerSettings.TargetGlesGraphics"), new GUILayoutOption[0]);
						}
						else
						{
							if (targetGroup == BuildTargetGroup.iPhone)
							{
								EditorGUILayout.PropertyField(this.m_TargetIOSGraphics, EditorGUIUtility.TextContent("PlayerSettings.TargetIOSGraphics"), new GUILayoutOption[0]);
							}
						}
						if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Metro || targetGroup == BuildTargetGroup.WP8)
						{
							EditorGUILayout.PropertyField(this.m_AccelerometerFrequency, EditorGUIUtility.TextContent("PlayerSettings.AccelerometerFrequency"), new GUILayoutOption[0]);
						}
						if (targetGroup == BuildTargetGroup.iPhone)
						{
							EditorGUILayout.PropertyField(this.m_LocationUsageDescription, EditorGUIUtility.TextContent("PlayerSettings.IOSLocationUsageDescription"), new GUILayoutOption[0]);
						}
						if (targetGroup == BuildTargetGroup.iPhone)
						{
							EditorGUILayout.PropertyField(this.m_OverrideIPodMusic, new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_PrepareIOSForRecording, new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_UIRequiresPersistentWiFi, EditorGUIUtility.TextContent("PlayerSettings.UIRequiresPersistentWiFi"), new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_UIExitOnSuspend, EditorGUIUtility.TextContent("PlayerSettings.UIExitOnSuspend"), new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_EnableHWStatistics, EditorGUIUtility.TextContent("PlayerSettings.enableHWStatistics"), new GUILayoutOption[0]);
						}
					}
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
				}
				GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.OptimizationSubHeader"), EditorStyles.boldLabel, new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.WebPlayer)
				{
					this.ShowDisabledFakeEnumPopup(PlayerSettingsEditor.FakeEnum.WebplayerSubset);
				}
				else
				{
					if (targetGroup == BuildTargetGroup.FlashPlayer)
					{
						this.ShowDisabledFakeEnumPopup(PlayerSettingsEditor.FakeEnum.FlashPlayerSubset);
						EditorGUILayout.PropertyField(this.m_FlashStrippingLevel, EditorGUIUtility.TextContent("PlayerSettings.flashStrippingLevel"), new GUILayoutOption[0]);
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
				}
				if (targetGroup == BuildTargetGroup.NaCl || targetGroup == BuildTargetGroup.FlashPlayer)
				{
					EditorGUILayout.PropertyField(this.m_StripPhysics, EditorGUIUtility.TextContent("PlayerSettings.StripPhysics"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.XboxOne || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2)
				{
					EditorGUILayout.PropertyField(this.m_AotOptions, EditorGUIUtility.TextContent("PlayerSettings.aotOptions"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.BB10 || targetGroup == BuildTargetGroup.Tizen || targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.XBOX360 || targetGroup == BuildTargetGroup.XboxOne)
				{
					if (targetGroup == BuildTargetGroup.iPhone)
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
					if (targetGroup == BuildTargetGroup.iPhone)
					{
						EditorGUILayout.PropertyField(this.m_IPhoneScriptCallOptimization, EditorGUIUtility.TextContent("PlayerSettings.IPhoneScriptCallOptimization"), new GUILayoutOption[0]);
					}
					if (targetGroup == BuildTargetGroup.Android)
					{
						EditorGUILayout.PropertyField(this.m_AndroidProfiler, EditorGUIUtility.TextContent("PlayerSettings.AndroidProfiler"), new GUILayoutOption[0]);
					}
					EditorGUILayout.Space();
				}
				EditorGUILayout.PropertyField(this.m_StripUnusedMeshComponents, EditorGUIUtility.TextContent("PlayerSettings.StripUnusedMeshComponents"), new GUILayoutOption[0]);
				if (targetGroup == BuildTargetGroup.PS3 || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
				{
					EditorGUILayout.PropertyField(this.m_VideoMemoryForVertexBuffers, EditorGUIUtility.TextContent("PlayerSettings.VideoMemoryForVertexBuffers"), new GUILayoutOption[0]);
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
		private void ShowBrowseableProperty(SerializedProperty property, string textContent, string extension, string dir, float kLabelFloatMinW, float kLabelFloatMaxW, float h)
		{
			bool flag = textContent.Length != 0;
			if (flag)
			{
				GUILayout.Label(EditorGUIUtility.TextContent(textContent), EditorStyles.boldLabel, new GUILayoutOption[0]);
			}
			Rect rect = GUILayoutUtility.GetRect(kLabelFloatMinW, kLabelFloatMaxW, h, h, EditorStyles.layerMaskField, null);
			float labelWidth = EditorGUIUtility.labelWidth;
			Rect position = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth - EditorGUI.indent, rect.height);
			Rect position2 = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
			string text = (property.stringValue.Length != 0) ? property.stringValue : "Not selected.";
			EditorGUI.TextArea(position2, text, EditorStyles.label);
			if (GUI.Button(position, EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric")))
			{
				property.stringValue = FileUtil.GetLastPathNameComponent(EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric").text, dir, extension));
				base.serializedObject.ApplyModifiedProperties();
				GUIUtility.ExitGUI();
			}
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
			if (targetGroup != BuildTargetGroup.Metro && targetGroup != BuildTargetGroup.WP8 && targetGroup != BuildTargetGroup.XBOX360 && targetGroup != BuildTargetGroup.PS3 && targetGroup != BuildTargetGroup.PSP2 && targetGroup != BuildTargetGroup.PSM && (settingsExtension == null || !settingsExtension.HasPublishSection()))
			{
				return;
			}
			GUI.changed = false;
			if (this.BeginSettingsBox(4, EditorGUIUtility.TextContent("PlayerSettings.PublishingHeader")))
			{
				string text = "Assets";
				string directory = FileUtil.DeleteLastPathNameComponent(Application.dataPath);
				float num = 16f;
				float num2 = 80f + EditorGUIUtility.fieldWidth + 5f;
				float num3 = 80f + EditorGUIUtility.fieldWidth + 5f;
				if (settingsExtension != null)
				{
					settingsExtension.PublishSectionGUI(num, num2, num3);
				}
				if (targetGroup == BuildTargetGroup.Metro)
				{
					this.PublishSectionGUIMetro(num2, num3, num, num);
				}
				if (targetGroup == BuildTargetGroup.WP8)
				{
					this.PublishSectionGUIWP8(num2, num3, num, num);
				}
				if (targetGroup == BuildTargetGroup.PS3)
				{
					string text2 = Path.Combine(Application.dataPath, "PS3 Submission Package");
					if (Directory.Exists(text2))
					{
						this.AutoAssignProperty(this.m_PS3TitleConfigPath, text2, "TITLECONFIG.XML");
						this.AutoAssignProperty(this.m_PS3DLCConfigPath, text2, "DLCconfig.txt");
						this.AutoAssignProperty(this.m_PS3ThumbnailPath, text2, "ICON0.PNG");
						this.AutoAssignProperty(this.m_PS3BackgroundPath, text2, "BACKGROUND0.PNG");
						this.AutoAssignProperty(this.m_PS3TrophyPackagePath, text2, "TROPHY.TRP");
						this.AutoAssignProperty(this.m_PS3SoundPath, text2, "SDN0.AT3");
					}
					else
					{
						text2 = text;
					}
					this.ShowBrowseableProperty(this.m_PS3TitleConfigPath, "PlayerSettings.ps3TitleConfigPath", "xml", text2, num2, num3, num);
					this.ShowBrowseableProperty(this.m_PS3DLCConfigPath, "PlayerSettings.ps3DLCConfigPath", "txt", text2, num2, num3, num);
					this.ShowBrowseableProperty(this.m_PS3ThumbnailPath, "PlayerSettings.ps3ThumbnailPath", "png", text2, num2, num3, num);
					this.ShowBrowseableProperty(this.m_PS3BackgroundPath, "PlayerSettings.ps3BackgroundPath", "png", text2, num2, num3, num);
					this.ShowBrowseableProperty(this.m_PS3SoundPath, "PlayerSettings.ps3SoundPath", "at3", text2, num2, num3, num);
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ps3TrophyPackagePath"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					this.ShowBrowseableProperty(this.m_PS3TrophyPackagePath, string.Empty, "trp", text2, num2, num3, num);
					EditorGUILayout.PropertyField(this.m_PS3TrophyCommId, EditorGUIUtility.TextContent("PlayerSettings.ps3TrophyCommId"), new GUILayoutOption[0]);
					this.m_PS3NpCommunicationPassphrase.stringValue = EditorGUILayout.TextField(EditorGUIUtility.TextContent("PlayerSettings.ps3NpCommunicationPassphrase"), this.m_PS3NpCommunicationPassphrase.stringValue, new GUILayoutOption[]
					{
						GUILayout.Height(280f)
					});
					this.m_PS3TrophyCommSig.stringValue = EditorGUILayout.TextField(EditorGUIUtility.TextContent("PlayerSettings.ps3TrophyCommSig"), this.m_PS3TrophyCommSig.stringValue, new GUILayoutOption[]
					{
						GUILayout.Height(280f)
					});
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.ps3TitleSettings"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_PS3TrialMode, EditorGUIUtility.TextContent("PlayerSettings.ps3TrialMode"), new GUILayoutOption[0]);
					this.m_PS3BootCheckMaxSaveGameSizeKB.intValue = EditorGUILayout.IntField(EditorGUIUtility.TextContent("PlayerSettings.ps3BootCheckMaxSaveGameSizeKB"), this.m_PS3BootCheckMaxSaveGameSizeKB.intValue, new GUILayoutOption[0]);
					this.m_PS3SaveGameSlots.intValue = EditorGUILayout.IntField(EditorGUIUtility.TextContent("PlayerSettings.ps3SaveGameSlots"), this.m_PS3SaveGameSlots.intValue, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.PSP2)
				{
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.psp2LiveArea"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					this.m_PSP2LiveAreaBackground.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.psp2LiveAreaBackround"), (Texture2D)this.m_PSP2LiveAreaBackground.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					this.m_PSP2LiveAreaGate.objectReferenceValue = EditorGUILayout.ObjectField(EditorGUIUtility.TextContent("PlayerSettings.psp2LiveAreaGate"), (Texture2D)this.m_PSP2LiveAreaGate.objectReferenceValue, typeof(Texture2D), false, new GUILayoutOption[0]);
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.psp2NP"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.psp2NPTrophyPath"), new GUILayoutOption[0]);
					Rect rect = GUILayoutUtility.GetRect(num2, num3, num, num, EditorStyles.layerMaskField, null);
					GUIContent gUIContent;
					if (this.m_PSP2TrophyPackPath.stringValue.Length == 0)
					{
						gUIContent = EditorGUIUtility.TextContent("Not selected.");
						EditorGUI.BeginDisabledGroup(true);
					}
					else
					{
						gUIContent = EditorGUIUtility.TempContent(this.m_PSP2TrophyPackPath.stringValue);
						EditorGUI.BeginDisabledGroup(false);
					}
					float labelWidth = EditorGUIUtility.labelWidth;
					Rect position = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth - EditorGUI.indent, rect.height);
					Rect position2 = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
					EditorGUI.TextArea(position2, gUIContent.text, EditorStyles.label);
					EditorGUI.EndDisabledGroup();
					if (GUI.Button(position, EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric")))
					{
						string text3 = Directory.GetCurrentDirectory().Replace('\\', '/');
						this.m_PSP2TrophyPackPath.stringValue = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric").text, text3, "trp");
						text3 += "/";
						if (this.m_PSP2TrophyPackPath.stringValue.StartsWith(text3))
						{
							this.m_PSP2TrophyPackPath.stringValue = this.m_PSP2TrophyPackPath.stringValue.Substring(text3.Length);
						}
						base.serializedObject.ApplyModifiedProperties();
						GUIUtility.ExitGUI();
					}
					EditorGUILayout.PropertyField(this.m_PSP2NPCommsID, EditorGUIUtility.TextContent("PlayerSettings.psp2NPCommsID"), new GUILayoutOption[0]);
					this.m_PSP2NPCommsPassphrase.stringValue = EditorGUILayout.TextField(EditorGUIUtility.TextContent("PlayerSettings.psp2NPCommsPassphrase"), this.m_PSP2NPCommsPassphrase.stringValue, new GUILayoutOption[]
					{
						GUILayout.Height(280f)
					});
					this.m_PSP2NPCommsSig.stringValue = EditorGUILayout.TextField(EditorGUIUtility.TextContent("PlayerSettings.psp2NPCommsSig"), this.m_PSP2NPCommsSig.stringValue, new GUILayoutOption[]
					{
						GUILayout.Height(280f)
					});
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.psp2PackageParams"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.psp2ParamSfxPath"), new GUILayoutOption[0]);
					rect = GUILayoutUtility.GetRect(num2, num3, num, num, EditorStyles.layerMaskField, null);
					GUIContent gUIContent2;
					if (this.m_PSP2ParamSfxPath.stringValue.Length == 0)
					{
						gUIContent2 = EditorGUIUtility.TextContent("Not selected.");
						EditorGUI.BeginDisabledGroup(true);
					}
					else
					{
						gUIContent2 = EditorGUIUtility.TempContent(this.m_PSP2ParamSfxPath.stringValue);
						EditorGUI.BeginDisabledGroup(false);
					}
					float labelWidth2 = EditorGUIUtility.labelWidth;
					Rect position3 = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth2 - EditorGUI.indent, rect.height);
					Rect position4 = new Rect(rect.x + labelWidth2, rect.y, rect.width - labelWidth2, rect.height);
					EditorGUI.TextArea(position4, gUIContent2.text, EditorStyles.label);
					EditorGUI.EndDisabledGroup();
					if (GUI.Button(position3, EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric")))
					{
						string text4 = Directory.GetCurrentDirectory().Replace('\\', '/');
						this.m_PSP2ParamSfxPath.stringValue = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("PlayerSettings.BrowseGeneric").text, text4, "sfx");
						text4 += "/";
						if (this.m_PSP2ParamSfxPath.stringValue.StartsWith(text4))
						{
							this.m_PSP2ParamSfxPath.stringValue = this.m_PSP2ParamSfxPath.stringValue.Substring(text4.Length);
						}
						base.serializedObject.ApplyModifiedProperties();
						GUIUtility.ExitGUI();
					}
					EditorGUILayout.PropertyField(this.m_PSP2PackagePassword, EditorGUIUtility.TextContent("PlayerSettings.psp2PackagePassword"), new GUILayoutOption[0]);
					if (this.m_PSP2PackagePassword.stringValue.Length == 0)
					{
						System.Random random = new System.Random();
						StringBuilder stringBuilder = new StringBuilder();
						string text5 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
						for (int i = 0; i < 32; i++)
						{
							int index = random.Next(text5.Length);
							stringBuilder.Append(text5[index]);
						}
						this.m_PSP2PackagePassword.stringValue = stringBuilder.ToString();
						base.serializedObject.ApplyModifiedProperties();
					}
					else
					{
						if (this.m_PSP2PackagePassword.stringValue.Length != 32)
						{
							GUIContent content = EditorGUIUtility.TextContent("PlayerSettings.psp2PasswordBadLength");
							GUILayout.Label(content, EditorStyles.miniLabel, new GUILayoutOption[0]);
						}
					}
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
					Rect rect = GUILayoutUtility.GetRect(num2, num3, num, num, EditorStyles.layerMaskField, null);
					float labelWidth3 = EditorGUIUtility.labelWidth;
					Rect position5 = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth3 - EditorGUI.indent, rect.height);
					Rect position6 = new Rect(rect.x + labelWidth3, rect.y, rect.width - labelWidth3, rect.height);
					string text6 = (this.m_XboxImageXexPath.stringValue.Length != 0) ? this.m_XboxImageXexPath.stringValue : "Not selected.";
					EditorGUI.TextArea(position6, text6, EditorStyles.label);
					if (GUI.Button(position5, EditorGUIUtility.TextContent("PlayerSettings.XboxImageXEXFile")))
					{
						string text7 = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("PlayerSettings.XboxImageXEXFile").text, directory, "cfg");
						this.m_XboxImageXexPath.stringValue = text7;
						text7 = FileUtil.GetProjectRelativePath(text7);
						if (text7 != string.Empty)
						{
							this.m_XboxImageXexPath.stringValue = text7;
						}
						base.serializedObject.ApplyModifiedProperties();
						GUIUtility.ExitGUI();
					}
					EditorGUILayout.Space();
					GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.XboxLive"), EditorStyles.boldLabel, new GUILayoutOption[0]);
					rect = GUILayoutUtility.GetRect(num2, num3, num, num, EditorStyles.layerMaskField, null);
					float labelWidth4 = EditorGUIUtility.labelWidth;
					Rect position7 = new Rect(rect.x + EditorGUI.indent, rect.y, labelWidth4 - EditorGUI.indent, rect.height);
					Rect position8 = new Rect(rect.x + labelWidth4, rect.y, rect.width - labelWidth4, rect.height);
					string text8 = (this.m_XboxSpaPath.stringValue.Length != 0) ? this.m_XboxSpaPath.stringValue : "Not selected.";
					EditorGUI.TextArea(position8, text8, EditorStyles.label);
					if (GUI.Button(position7, EditorGUIUtility.TextContent("PlayerSettings.XboxSpaFile")))
					{
						string text9 = EditorUtility.OpenFilePanel(EditorGUIUtility.TextContent("PlayerSettings.XboxSpaFile").text, directory, "spa");
						this.m_XboxSpaPath.stringValue = text9;
						text9 = FileUtil.GetProjectRelativePath(text9);
						if (text9 != string.Empty)
						{
							this.m_XboxSpaPath.stringValue = text9;
						}
						if (this.m_XboxTitleId.stringValue.Length == 0)
						{
							Debug.LogWarning("Title id must be present when using a SPA file.");
						}
						base.serializedObject.ApplyModifiedProperties();
						GUIUtility.ExitGUI();
					}
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
		public void PublishSectionGUIMetro(float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight)
		{
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroPackaging"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroPackageName, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageName"), new GUILayoutOption[0]);
			this.m_MetroPackageName.stringValue = this.ValidateMetroPackageName(this.m_MetroPackageName.stringValue);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroPackageDisplayName"), new GUIContent(this.m_ProductName.stringValue), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroPackageVersion, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageVersion"), new GUILayoutOption[0]);
			this.m_MetroPackageVersion.stringValue = PlayerSettingsEditor.ValidateMetroPackageVersion(this.m_MetroPackageVersion.stringValue);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroPackagePublisherDisplayName"), new GUIContent(this.m_CompanyName.stringValue), new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroCertificate"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroCertificatePublisher"), new GUIContent(PlayerSettings.Metro.certificateSubject), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroCertificateIssuer"), new GUIContent(PlayerSettings.Metro.certificateIssuer), new GUILayoutOption[0]);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroCertificateNotAfter"), new GUIContent((!PlayerSettings.Metro.certificateNotAfter.HasValue) ? null : PlayerSettings.Metro.certificateNotAfter.Value.ToShortDateString()), new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, EditorStyles.layerMaskField);
			Rect position = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
			string text = PlayerSettings.Metro.certificatePath;
			GUIContent content;
			if (string.IsNullOrEmpty(text))
			{
				content = EditorGUIUtility.TextContent("PlayerSettings.MetroCertificateSelect");
			}
			else
			{
				content = new GUIContent(FileUtil.GetLastPathNameComponent(text), text);
			}
			if (GUI.Button(position, content))
			{
				text = EditorUtility.OpenFilePanel(null, Application.dataPath, "pfx").Replace('\\', '/');
				string projectRelativePath = FileUtil.GetProjectRelativePath(text);
				if (string.IsNullOrEmpty(projectRelativePath) && !string.IsNullOrEmpty(text))
				{
					Debug.LogError("Certificate path '" + Path.GetFullPath(text) + "' has to be relative to " + Path.GetFullPath(Application.dataPath + "\\.."));
				}
				else
				{
					try
					{
						if (!PlayerSettings.Metro.SetCertificate(text, null))
						{
							MetroCertificatePasswordWindow.Show(text);
						}
					}
					catch (UnityException ex)
					{
						Debug.LogError(ex.Message);
					}
				}
			}
			Rect rect2 = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, EditorStyles.layerMaskField);
			Rect position2 = new Rect(rect2.x + EditorGUIUtility.labelWidth, rect2.y, rect2.width - EditorGUIUtility.labelWidth, rect2.height);
			if (GUI.Button(position2, EditorGUIUtility.TextContent("PlayerSettings.MetroCertificateCreate")))
			{
				MetroCreateTestCertificateWindow.Show(this.m_CompanyName.stringValue);
			}
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroApplication"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.LabelField(EditorGUIUtility.TextContent("PlayerSettings.MetroApplicationDisplayName"), new GUIContent(this.m_ProductName.stringValue), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroApplicationDescription, EditorGUIUtility.TextContent("PlayerSettings.MetroApplicationDescription"), new GUILayoutOption[0]);
			this.m_MetroApplicationDescription.stringValue = this.ValidateMetroApplicationDescription(this.m_MetroApplicationDescription.stringValue);
			EditorGUILayout.Space();
			GUILayout.Label("Compilation", EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettings.Metro.compilationOverrides = (PlayerSettings.MetroCompilationOverrides)EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("PlayerSettings.MetroCompilationOverrides"), PlayerSettings.Metro.compilationOverrides, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label("Misc", EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroUnprocessedPlugins, EditorGUIUtility.TextContent("PlayerSettings.MetroUnprocessedPlugins"), true, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_MetroEnableIndependentInputSource, EditorGUIUtility.TextContent("PlayerSettings.MetroEnableIndependentInputSource"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroEnableLowLatencyPresentationAPI, EditorGUIUtility.TextContent("PlayerSettings.MetroEnableLowLatencyPresentationAPI"), new GUILayoutOption[0]);
			GUILayout.Label("Capabilities", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.capScrollViewPosition = GUILayout.BeginScrollView(this.capScrollViewPosition, EditorStyles.helpBox, new GUILayoutOption[]
			{
				GUILayout.MinHeight(200f)
			});
			IEnumerator enumerator = Enum.GetValues(typeof(PlayerSettings.MetroCapability)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PlayerSettings.MetroCapability metroCapability = (PlayerSettings.MetroCapability)((int)enumerator.Current);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					bool enabled = GUILayout.Toggle(PlayerSettings.Metro.GetCapability(metroCapability), metroCapability.ToString(), new GUILayoutOption[]
					{
						GUILayout.MinWidth(150f)
					});
					PlayerSettings.Metro.SetCapability(metroCapability, enabled);
					GUILayout.EndHorizontal();
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			GUILayout.EndScrollView();
		}
		private static void ImageField(SerializedProperty property, GUIContent label, float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight, int imageWidth, int imageHeight)
		{
			Rect rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, EditorStyles.layerMaskField);
			Rect position = new Rect(rect.x - EditorGUI.indent, rect.y, EditorGUIUtility.labelWidth - EditorGUI.indent, rect.height);
			Rect position2 = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
			EditorGUI.LabelField(position, label);
			GUIContent content;
			if (string.IsNullOrEmpty(property.stringValue))
			{
				content = EditorGUIUtility.TextContent("PlayerSettings.MetroImageSelect");
			}
			else
			{
				content = new GUIContent(FileUtil.GetLastPathNameComponent(property.stringValue), property.stringValue);
			}
			if (GUI.Button(position2, content))
			{
				string text = EditorUtility.OpenFilePanel(null, Application.dataPath, "png").Replace('\\', '/');
				if (string.IsNullOrEmpty(text))
				{
					property.stringValue = text;
				}
				else
				{
					if (!PlayerSettingsEditor.ValidateImage(text, imageWidth, imageHeight))
					{
						property.stringValue = string.Empty;
						return;
					}
					property.stringValue = text;
				}
				text = FileUtil.GetProjectRelativePath(text);
				if (!string.IsNullOrEmpty(text))
				{
					property.stringValue = text;
				}
			}
		}
		private static bool ValidateImage(string imageFile, int width, int height)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(File.ReadAllBytes(imageFile));
			int width2 = texture2D.width;
			int height2 = texture2D.height;
			UnityEngine.Object.DestroyImmediate(texture2D);
			if (width2 != width || height2 != height)
			{
				Debug.LogError(string.Format("Invalid image size ({0}x{1}), should be {2}x{3}", new object[]
				{
					width2,
					height2,
					width,
					height
				}));
				return false;
			}
			return true;
		}
		private string ValidateMetroPackageName(string value)
		{
			if (PlayerSettingsEditor.IsValidMetroPackageName(value))
			{
				return value;
			}
			return this.GetDefaultMetroPackageName();
		}
		private static bool IsValidMetroPackageName(string value)
		{
			if (!PlayerSettingsEditor.metroPackageNameRegex.IsMatch(value))
			{
				return false;
			}
			string text = value.ToUpper();
			if (text != null)
			{
				if (PlayerSettingsEditor.<>f__switch$map15 == null)
				{
					PlayerSettingsEditor.<>f__switch$map15 = new Dictionary<string, int>(22)
					{

						{
							"CON",
							0
						},

						{
							"PRN",
							0
						},

						{
							"AUX",
							0
						},

						{
							"NUL",
							0
						},

						{
							"COM1",
							0
						},

						{
							"COM2",
							0
						},

						{
							"COM3",
							0
						},

						{
							"COM4",
							0
						},

						{
							"COM5",
							0
						},

						{
							"COM6",
							0
						},

						{
							"COM7",
							0
						},

						{
							"COM8",
							0
						},

						{
							"COM9",
							0
						},

						{
							"LPT1",
							0
						},

						{
							"LPT2",
							0
						},

						{
							"LPT3",
							0
						},

						{
							"LPT4",
							0
						},

						{
							"LPT5",
							0
						},

						{
							"LPT6",
							0
						},

						{
							"LPT7",
							0
						},

						{
							"LPT8",
							0
						},

						{
							"LPT9",
							0
						}
					};
				}
				int num;
				if (PlayerSettingsEditor.<>f__switch$map15.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						return false;
					}
				}
			}
			return true;
		}
		private string GetDefaultMetroPackageName()
		{
			string text = this.m_ProductName.stringValue;
			if (text != null)
			{
				StringBuilder stringBuilder = new StringBuilder(text.Length);
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (char.IsLetterOrDigit(c) || c == '-')
					{
						stringBuilder.Append(c);
					}
					else
					{
						if (c == '.' && i != text.Length - 1)
						{
							stringBuilder.Append(c);
						}
					}
				}
				text = stringBuilder.ToString();
			}
			if (!PlayerSettingsEditor.IsValidMetroPackageName(text))
			{
				text = "DefaultPackageName";
			}
			return text;
		}
		internal static string ValidateMetroPackageVersion(string value)
		{
			if (PlayerSettingsEditor.metroPackageVersionRegex.IsMatch(value))
			{
				return value;
			}
			return "1.0.0.0";
		}
		private string ValidateMetroApplicationDescription(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return this.m_ProductName.stringValue;
			}
			return value;
		}
		private string ValidateMetroTileShortName(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				value = this.m_ProductName.stringValue;
			}
			if (value != null && value.Length > 13)
			{
				return value.Substring(0, 13).TrimEnd(new char[]
				{
					' '
				});
			}
			return value;
		}
		private void MetroLogoSection(float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight)
		{
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroPackageLogo"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPackageLogo, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageLogoScale100"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 50, 50);
			PlayerSettingsEditor.ImageField(this.m_MetroPackageLogo140, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageLogoScale140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 70, 70);
			PlayerSettingsEditor.ImageField(this.m_MetroPackageLogo180, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageLogoScale180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 90, 90);
			PlayerSettingsEditor.ImageField(this.m_MetroPackageLogo240, EditorGUIUtility.TextContent("PlayerSettings.MetroPackageLogoScale240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 120, 120);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroTileShortName, EditorGUIUtility.TextContent("PlayerSettings.MetroTileShortName"), new GUILayoutOption[0]);
			this.m_MetroTileShortName.stringValue = this.ValidateMetroTileShortName(this.m_MetroTileShortName.stringValue);
			PlayerSettings.MetroApplicationShowName tileShowName = PlayerSettings.Metro.tileShowName;
			if (tileShowName != PlayerSettings.MetroApplicationShowName.NotSet)
			{
				switch (tileShowName)
				{
				case PlayerSettings.MetroApplicationShowName.AllLogos:
					PlayerSettings.Metro.mediumTileShowName = true;
					PlayerSettings.Metro.largeTileShowName = true;
					PlayerSettings.Metro.wideTileShowName = true;
					break;
				case PlayerSettings.MetroApplicationShowName.StandardLogoOnly:
					PlayerSettings.Metro.mediumTileShowName = true;
					PlayerSettings.Metro.largeTileShowName = true;
					break;
				case PlayerSettings.MetroApplicationShowName.WideLogoOnly:
					PlayerSettings.Metro.wideTileShowName = true;
					break;
				}
				PlayerSettings.Metro.tileShowName = PlayerSettings.MetroApplicationShowName.NotSet;
			}
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroTileShowName"), new GUILayoutOption[0]);
			PlayerSettings.Metro.mediumTileShowName = GUILayout.Toggle(PlayerSettings.Metro.mediumTileShowName, EditorGUIUtility.TextContent("PlayerSettings.MetroMediumTile"), new GUILayoutOption[0]);
			PlayerSettings.Metro.largeTileShowName = GUILayout.Toggle(PlayerSettings.Metro.largeTileShowName, EditorGUIUtility.TextContent("PlayerSettings.MetroLargeTile"), new GUILayoutOption[0]);
			PlayerSettings.Metro.wideTileShowName = GUILayout.Toggle(PlayerSettings.Metro.wideTileShowName, EditorGUIUtility.TextContent("PlayerSettings.MetroWideTile"), new GUILayoutOption[0]);
			PlayerSettings.Metro.tileForegroundText = (PlayerSettings.MetroApplicationForegroundText)EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("PlayerSettings.MetroTileForegroundText"), PlayerSettings.Metro.tileForegroundText, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MetroTileBackgroundColor, EditorGUIUtility.TextContent("PlayerSettings.MetroTileBackgroundColor"), new GUILayoutOption[0]);
			PlayerSettings.Metro.defaultTileSize = (PlayerSettings.MetroDefaultTileSize)EditorGUILayout.EnumPopup(EditorGUIUtility.TextContent("PlayerSettings.MetroDefaultTileSize"), PlayerSettings.Metro.defaultTileSize, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.WindowsTiles"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroSmallLogo"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileSmallLogo80, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallLogoScale80"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 24, 24);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileSmallLogo, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallLogoScale100"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 30, 30);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileSmallLogo140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallLogoScale140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 42, 42);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileSmallLogo180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallLogoScale180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 54, 54);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroMediumTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileLogo80, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileLogo80"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 120, 120);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileLogo, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileLogo"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 150, 150);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileLogo140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileLogo140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 210, 210);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileLogo180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileLogo180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 270, 270);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroWideTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileWideLogo80, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileWideLogo80"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 248, 120);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileWideLogo, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileWideLogo"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 310, 150);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileWideLogo140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileWideLogo140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 434, 210);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreTileWideLogo180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreTileWideLogo180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 558, 270);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroSmallTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSmallTile80, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallTile80"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 56, 56);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSmallTile, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallTile100"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 70, 70);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSmallTile140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallTile140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 98, 98);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSmallTile180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSmallTile180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 126, 126);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroLargeTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreLargeTile80, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreLargeTile80"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 248, 248);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreLargeTile, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreLargeTile100"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 310, 310);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreLargeTile140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreLargeTile140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 434, 434);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreLargeTile180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreLargeTile180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 558, 558);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.WindowsPhoneTiles"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroAppIcon"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneAppIcon, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneAppIcon"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 44, 44);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneAppIcon140, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneAppIcon140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 62, 62);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneAppIcon240, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneAppIcon240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 106, 106);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroSmallTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSmallTile, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSmallTile"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 71, 71);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSmallTile140, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSmallTile140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 99, 99);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSmallTile240, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSmallTile240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 170, 170);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroMediumTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneMediumTile, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneMediumTile"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 150, 150);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneMediumTile140, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneMediumTile140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 210, 210);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneMediumTile240, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneMediumTile240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 360, 360);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroWideTile"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneWideTile, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneWideTile"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 310, 150);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneWideTile140, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneWideTile140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 434, 210);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneWideTile240, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneWideTile240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 744, 360);
			EditorGUILayout.Space();
		}
		private void MetroSplashScreenSection(float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight)
		{
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroWindows"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSplashScreenImage, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSplashScreenImage"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 620, 300);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSplashScreenImage140, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSplashScreenImage140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 868, 420);
			PlayerSettingsEditor.ImageField(this.m_MetroStoreSplashScreenImage180, EditorGUIUtility.TextContent("PlayerSettings.MetroStoreSplashScreenImage180"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 1116, 540);
			EditorGUILayout.Space();
			GUILayout.Label(EditorGUIUtility.TextContent("PlayerSettings.MetroWindowsPhone"), EditorStyles.boldLabel, new GUILayoutOption[0]);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSplashScreenImage, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSplashScreenImage"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 480, 800);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSplashScreenImage140, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSplashScreenImage140"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 672, 1120);
			PlayerSettingsEditor.ImageField(this.m_MetroPhoneSplashScreenImage240, EditorGUIUtility.TextContent("PlayerSettings.MetroPhoneSplashScreenImage240"), kLabelMaxWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, 1152, 1920);
			EditorGUILayout.Space();
			bool hasValue = PlayerSettings.Metro.splashScreenBackgroundColor.HasValue;
			bool flag = EditorGUILayout.BeginToggleGroup(EditorGUIUtility.TextContent("PlayerSettings.MetroSplashScreenOverwriteBackgroundColor"), hasValue);
			Rect rect = GUILayoutUtility.GetRect(kLabelMinWidth, kLabelMaxWidth, kLabelMinHeight, kLabelMaxHeight, EditorStyles.layerMaskField);
			Rect position = new Rect(rect.x - EditorGUI.indent, rect.y, EditorGUIUtility.labelWidth - EditorGUI.indent, rect.height);
			Rect position2 = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
			EditorGUI.LabelField(position, EditorGUIUtility.TextContent("PlayerSettings.MetroSplashScreenBackgroundColor"));
			if (flag != hasValue)
			{
				if (flag)
				{
					PlayerSettings.Metro.splashScreenBackgroundColor = new Color?(PlayerSettings.Metro.tileBackgroundColor);
				}
				else
				{
					PlayerSettings.Metro.splashScreenBackgroundColor = null;
				}
			}
			if (flag)
			{
				PlayerSettings.Metro.splashScreenBackgroundColor = new Color?(EditorGUI.ColorField(position2, PlayerSettings.Metro.splashScreenBackgroundColor.Value));
			}
			else
			{
				EditorGUI.ColorField(position2, PlayerSettings.Metro.tileBackgroundColor);
			}
			EditorGUILayout.EndToggleGroup();
		}
		public void PublishSectionGUIWP8(float kLabelMinWidth, float kLabelMaxWidth, float kLabelMinHeight, float kLabelMaxHeight)
		{
			EditorGUILayout.PropertyField(this.m_WP8UnprocessedPlugins, EditorGUIUtility.TextContent("PlayerSettings.WP8UnprocessedPlugins"), true, new GUILayoutOption[0]);
		}
		private static string PrettyTemplateKeyName(string name)
		{
			string[] array = name.Split(new char[]
			{
				'_'
			});
			array[0] = PlayerSettingsEditor.UppercaseFirst(array[0].ToLower());
			for (int i = 1; i < array.Length; i++)
			{
				array[i] = array[i].ToLower();
			}
			return string.Join(" ", array);
		}
		private static string UppercaseFirst(string target)
		{
			if (string.IsNullOrEmpty(target))
			{
				return string.Empty;
			}
			return char.ToUpper(target[0]) + target.Substring(1);
		}
		private static int ThumbnailList(Rect rect, int selection, GUIContent[] thumbnails, int maxRowItems)
		{
			int num = 0;
			int i = 0;
			while (i < thumbnails.Length)
			{
				int num2 = 0;
				while (num2 < maxRowItems && i < thumbnails.Length)
				{
					if (PlayerSettingsEditor.ThumbnailListItem(new Rect(rect.x + (float)num2 * 80f, rect.y + (float)num * 100f, 80f, 100f), i == selection, thumbnails[i]))
					{
						selection = i;
					}
					num2++;
					i++;
				}
				num++;
			}
			return selection;
		}
		private static bool ThumbnailListItem(Rect rect, bool selected, GUIContent content)
		{
			EventType type = Event.current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					Rect position = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 20f - 10f);
					PlayerSettingsEditor.s_Styles.thumbnail.Draw(position, content.image, false, false, selected, selected);
					PlayerSettingsEditor.s_Styles.thumbnailLabel.Draw(new Rect(rect.x, rect.y + rect.height - 20f, rect.width, 20f), content.text, false, false, selected, selected);
				}
			}
			else
			{
				if (rect.Contains(Event.current.mousePosition))
				{
					if (!selected)
					{
						GUI.changed = true;
					}
					selected = true;
					Event.current.Use();
				}
			}
			return selected;
		}
	}
}
