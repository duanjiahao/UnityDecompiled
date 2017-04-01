using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditor.CrashReporting;
using UnityEditor.Modules;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CustomEditor(typeof(PlayerSettings))]
	internal class PlayerSettingsEditor : Editor
	{
		private static class Styles
		{
			public static readonly GUIStyle categoryBox;

			public static readonly GUIContent colorSpaceAndroidWarning;

			public static readonly GUIContent colorSpaceWebGLWarning;

			public static readonly GUIContent colorSpaceIOSWarning;

			public static readonly GUIContent colorSpaceTVOSWarning;

			public static readonly GUIContent recordingInfo;

			public static readonly GUIContent notApplicableInfo;

			public static readonly GUIContent sharedBetweenPlatformsInfo;

			public static readonly GUIContent VRSupportOverridenInfo;

			public static readonly GUIContent IL2CPPAndroidExperimentalInfo;

			public static readonly GUIContent cursorHotspot;

			public static readonly GUIContent defaultCursor;

			public static readonly GUIContent defaultIcon;

			public static readonly GUIContent vertexChannelCompressionMask;

			public static readonly GUIContent iconTitle;

			public static readonly GUIContent resolutionPresentationTitle;

			public static readonly GUIContent resolutionTitle;

			public static readonly GUIContent orientationTitle;

			public static readonly GUIContent allowedOrientationTitle;

			public static readonly GUIContent multitaskingSupportTitle;

			public static readonly GUIContent statusBarTitle;

			public static readonly GUIContent standalonePlayerOptionsTitle;

			public static readonly GUIContent debuggingCrashReportingTitle;

			public static readonly GUIContent debuggingTitle;

			public static readonly GUIContent crashReportingTitle;

			public static readonly GUIContent otherSettingsTitle;

			public static readonly GUIContent renderingTitle;

			public static readonly GUIContent identificationTitle;

			public static readonly GUIContent macAppStoreTitle;

			public static readonly GUIContent configurationTitle;

			public static readonly GUIContent optimizationTitle;

			public static readonly GUIContent loggingTitle;

			public static readonly GUIContent publishingSettingsTitle;

			public static readonly GUIContent bakeCollisionMeshes;

			public static readonly GUIContent keepLoadedShadersAlive;

			public static readonly GUIContent preloadedAssets;

			public static readonly GUIContent stripEngineCode;

			public static readonly GUIContent iPhoneStrippingLevel;

			public static readonly GUIContent iPhoneScriptCallOptimization;

			public static readonly GUIContent enableInternalProfiler;

			public static readonly GUIContent stripUnusedMeshComponents;

			public static readonly GUIContent videoMemoryForVertexBuffers;

			public static readonly GUIContent protectGraphicsMemory;

			public static readonly GUIContent defaultIsFullScreen;

			public static readonly GUIContent useOSAutoRotation;

			public static readonly GUIContent UIPrerenderedIcon;

			public static readonly GUIContent defaultScreenWidth;

			public static readonly GUIContent defaultScreenHeight;

			public static readonly GUIContent runInBackground;

			public static readonly GUIContent defaultScreenOrientation;

			public static readonly GUIContent allowedAutoRotateToPortrait;

			public static readonly GUIContent allowedAutoRotateToPortraitUpsideDown;

			public static readonly GUIContent allowedAutoRotateToLandscapeRight;

			public static readonly GUIContent allowedAutoRotateToLandscapeLeft;

			public static readonly GUIContent UIRequiresFullScreen;

			public static readonly GUIContent UIStatusBarHidden;

			public static readonly GUIContent UIStatusBarStyle;

			public static readonly GUIContent useMacAppStoreValidation;

			public static readonly GUIContent D3D9FullscreenMode;

			public static readonly GUIContent D3D11FullscreenMode;

			public static readonly GUIContent visibleInBackground;

			public static readonly GUIContent allowFullscreenSwitch;

			public static readonly GUIContent use32BitDisplayBuffer;

			public static readonly GUIContent disableDepthAndStencilBuffers;

			public static readonly GUIContent iosShowActivityIndicatorOnLoading;

			public static readonly GUIContent androidShowActivityIndicatorOnLoading;

			public static readonly GUIContent actionOnDotNetUnhandledException;

			public static readonly GUIContent logObjCUncaughtExceptions;

			public static readonly GUIContent enableCrashReportAPI;

			public static readonly GUIContent activeColorSpace;

			public static readonly GUIContent metalForceHardShadows;

			public static readonly GUIContent metalEditorSupport;

			public static readonly GUIContent metalAPIValidation;

			public static readonly GUIContent mTRendering;

			public static readonly GUIContent staticBatching;

			public static readonly GUIContent dynamicBatching;

			public static readonly GUIContent graphicsJobs;

			public static readonly GUIContent graphicsJobsMode;

			public static readonly GUIContent applicationBuildNumber;

			public static readonly GUIContent appleDeveloperTeamID;

			public static readonly GUIContent targetSdkVersion;

			public static readonly GUIContent iPhoneTargetOSVersion;

			public static readonly GUIContent tvOSTargetOSVersion;

			public static readonly GUIContent useOnDemandResources;

			public static readonly GUIContent accelerometerFrequency;

			public static readonly GUIContent cameraUsageDescription;

			public static readonly GUIContent locationUsageDescription;

			public static readonly GUIContent microphoneUsageDescription;

			public static readonly GUIContent muteOtherAudioSources;

			public static readonly GUIContent prepareIOSForRecording;

			public static readonly GUIContent UIRequiresPersistentWiFi;

			public static readonly GUIContent iOSAllowHTTPDownload;

			public static readonly GUIContent iOSURLSchemes;

			public static readonly GUIContent aotOptions;

			public static readonly GUIContent require31;

			public static readonly GUIContent requireAEP;

			public static readonly GUIContent skinOnGPU;

			public static readonly GUIContent skinOnGPUPS4;

			public static readonly GUIContent disableStatistics;

			public static readonly GUIContent scriptingDefineSymbols;

			public static readonly GUIContent scriptingBackend;

			public static readonly GUIContent scriptingMono2x;

			public static readonly GUIContent scriptingWinRTDotNET;

			public static readonly GUIContent scriptingIL2CPP;

			public static readonly GUIContent scriptingDefault;

			public static readonly GUIContent apiCompatibilityLevel;

			public static readonly GUIContent apiCompatibilityLevel_WiiUSubset;

			public static readonly GUIContent apiCompatibilityLevel_NET_2_0;

			public static readonly GUIContent apiCompatibilityLevel_NET_2_0_Subset;

			public static readonly GUIContent apiCompatibilityLevel_NET_4_6;

			public static string undoChangedBundleIdentifierString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Changed macOS bundleIdentifier");
				}
			}

			public static string undoChangedBuildNumberString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Changed macOS build number");
				}
			}

			public static string undoChangedBatchingString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Changed Batching Settings");
				}
			}

			public static string undoChangedIconString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Changed Icon");
				}
			}

			public static string undoChangedGraphicsAPIString
			{
				get
				{
					return LocalizationDatabase.GetLocalizedString("Changed Graphics API Settings");
				}
			}

			static Styles()
			{
				PlayerSettingsEditor.Styles.categoryBox = new GUIStyle(EditorStyles.helpBox);
				PlayerSettingsEditor.Styles.colorSpaceAndroidWarning = EditorGUIUtility.TextContent("On Android, linear colorspace requires OpenGL ES 3.0 or Vulkan, uncheck 'Automatic Graphics API' to remove OpenGL ES 2 API and 'Minimum API Level' must be at least Android 4.3");
				PlayerSettingsEditor.Styles.colorSpaceWebGLWarning = EditorGUIUtility.TextContent("On WebGL, linear colorspace is not supported");
				PlayerSettingsEditor.Styles.colorSpaceIOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API. Additionally, 'minimum iOS version' set to 8.0 at least");
				PlayerSettingsEditor.Styles.colorSpaceTVOSWarning = EditorGUIUtility.TextContent("Linear colorspace requires Metal API only. Uncheck 'Automatic Graphics API' and remove OpenGL ES 2 API.");
				PlayerSettingsEditor.Styles.recordingInfo = EditorGUIUtility.TextContent("Reordering the list will switch editor to the first available platform");
				PlayerSettingsEditor.Styles.notApplicableInfo = EditorGUIUtility.TextContent("Not applicable for this platform.");
				PlayerSettingsEditor.Styles.sharedBetweenPlatformsInfo = EditorGUIUtility.TextContent("* Shared setting between multiple platforms.");
				PlayerSettingsEditor.Styles.VRSupportOverridenInfo = EditorGUIUtility.TextContent("This setting is overridden by Virtual Reality Support.");
				PlayerSettingsEditor.Styles.IL2CPPAndroidExperimentalInfo = EditorGUIUtility.TextContent("IL2CPP on Android is experimental and unsupported");
				PlayerSettingsEditor.Styles.cursorHotspot = EditorGUIUtility.TextContent("Cursor Hotspot");
				PlayerSettingsEditor.Styles.defaultCursor = EditorGUIUtility.TextContent("Default Cursor");
				PlayerSettingsEditor.Styles.defaultIcon = EditorGUIUtility.TextContent("Default Icon");
				PlayerSettingsEditor.Styles.vertexChannelCompressionMask = EditorGUIUtility.TextContent("Vertex Compression*|Select which vertex channels should be compressed. Compression can save memory and bandwidth but precision will be lower.");
				PlayerSettingsEditor.Styles.iconTitle = EditorGUIUtility.TextContent("Icon");
				PlayerSettingsEditor.Styles.resolutionPresentationTitle = EditorGUIUtility.TextContent("Resolution and Presentation");
				PlayerSettingsEditor.Styles.resolutionTitle = EditorGUIUtility.TextContent("Resolution");
				PlayerSettingsEditor.Styles.orientationTitle = EditorGUIUtility.TextContent("Orientation");
				PlayerSettingsEditor.Styles.allowedOrientationTitle = EditorGUIUtility.TextContent("Allowed Orientations for Auto Rotation");
				PlayerSettingsEditor.Styles.multitaskingSupportTitle = EditorGUIUtility.TextContent("Multitasking Support");
				PlayerSettingsEditor.Styles.statusBarTitle = EditorGUIUtility.TextContent("Status Bar");
				PlayerSettingsEditor.Styles.standalonePlayerOptionsTitle = EditorGUIUtility.TextContent("Standalone Player Options");
				PlayerSettingsEditor.Styles.debuggingCrashReportingTitle = EditorGUIUtility.TextContent("Debugging and crash reporting");
				PlayerSettingsEditor.Styles.debuggingTitle = EditorGUIUtility.TextContent("Debugging");
				PlayerSettingsEditor.Styles.crashReportingTitle = EditorGUIUtility.TextContent("Crash Reporting");
				PlayerSettingsEditor.Styles.otherSettingsTitle = EditorGUIUtility.TextContent("Other Settings");
				PlayerSettingsEditor.Styles.renderingTitle = EditorGUIUtility.TextContent("Rendering");
				PlayerSettingsEditor.Styles.identificationTitle = EditorGUIUtility.TextContent("Identification");
				PlayerSettingsEditor.Styles.macAppStoreTitle = EditorGUIUtility.TextContent("Mac App Store Options");
				PlayerSettingsEditor.Styles.configurationTitle = EditorGUIUtility.TextContent("Configuration");
				PlayerSettingsEditor.Styles.optimizationTitle = EditorGUIUtility.TextContent("Optimization");
				PlayerSettingsEditor.Styles.loggingTitle = EditorGUIUtility.TextContent("Logging*");
				PlayerSettingsEditor.Styles.publishingSettingsTitle = EditorGUIUtility.TextContent("Publishing Settings");
				PlayerSettingsEditor.Styles.bakeCollisionMeshes = EditorGUIUtility.TextContent("Prebake Collision Meshes*|Bake collision data into the meshes on build time");
				PlayerSettingsEditor.Styles.keepLoadedShadersAlive = EditorGUIUtility.TextContent("Keep Loaded Shaders Alive*|Prevents shaders from being unloaded");
				PlayerSettingsEditor.Styles.preloadedAssets = EditorGUIUtility.TextContent("Preloaded Assets*|Assets to load at start up in the player and kept alive until the player terminates");
				PlayerSettingsEditor.Styles.stripEngineCode = EditorGUIUtility.TextContent("Strip Engine Code*|Strip Unused Engine Code - Note that byte code stripping of managed assemblies is always enabled for the IL2CPP scripting backend.");
				PlayerSettingsEditor.Styles.iPhoneStrippingLevel = EditorGUIUtility.TextContent("Stripping Level*");
				PlayerSettingsEditor.Styles.iPhoneScriptCallOptimization = EditorGUIUtility.TextContent("Script Call Optimization*");
				PlayerSettingsEditor.Styles.enableInternalProfiler = EditorGUIUtility.TextContent("Enable Internal Profiler*");
				PlayerSettingsEditor.Styles.stripUnusedMeshComponents = EditorGUIUtility.TextContent("Optimize Mesh Data*|Remove unused mesh components");
				PlayerSettingsEditor.Styles.videoMemoryForVertexBuffers = EditorGUIUtility.TextContent("Mesh Video Mem*|How many megabytes of video memory to use for mesh data before we use main memory");
				PlayerSettingsEditor.Styles.protectGraphicsMemory = EditorGUIUtility.TextContent("Protect Graphics Memory|Protect GPU memory from being read (on supported devices). Will prevent user from taking screenshots");
				PlayerSettingsEditor.Styles.defaultIsFullScreen = EditorGUIUtility.TextContent("Default Is Full Screen*");
				PlayerSettingsEditor.Styles.useOSAutoRotation = EditorGUIUtility.TextContent("Use Animated Autorotation|If set OS native animated autorotation method will be used. Otherwise orientation will be changed immediately.");
				PlayerSettingsEditor.Styles.UIPrerenderedIcon = EditorGUIUtility.TextContent("Prerendered Icon");
				PlayerSettingsEditor.Styles.defaultScreenWidth = EditorGUIUtility.TextContent("Default Screen Width");
				PlayerSettingsEditor.Styles.defaultScreenHeight = EditorGUIUtility.TextContent("Default Screen Height");
				PlayerSettingsEditor.Styles.runInBackground = EditorGUIUtility.TextContent("Run In Background*");
				PlayerSettingsEditor.Styles.defaultScreenOrientation = EditorGUIUtility.TextContent("Default Orientation*");
				PlayerSettingsEditor.Styles.allowedAutoRotateToPortrait = EditorGUIUtility.TextContent("Portrait");
				PlayerSettingsEditor.Styles.allowedAutoRotateToPortraitUpsideDown = EditorGUIUtility.TextContent("Portrait Upside Down");
				PlayerSettingsEditor.Styles.allowedAutoRotateToLandscapeRight = EditorGUIUtility.TextContent("Landscape Right");
				PlayerSettingsEditor.Styles.allowedAutoRotateToLandscapeLeft = EditorGUIUtility.TextContent("Landscape Left");
				PlayerSettingsEditor.Styles.UIRequiresFullScreen = EditorGUIUtility.TextContent("Requires Fullscreen");
				PlayerSettingsEditor.Styles.UIStatusBarHidden = EditorGUIUtility.TextContent("Status Bar Hidden");
				PlayerSettingsEditor.Styles.UIStatusBarStyle = EditorGUIUtility.TextContent("Status Bar Style");
				PlayerSettingsEditor.Styles.useMacAppStoreValidation = EditorGUIUtility.TextContent("Mac App Store Validation");
				PlayerSettingsEditor.Styles.D3D9FullscreenMode = EditorGUIUtility.TextContent("D3D9 Fullscreen Mode");
				PlayerSettingsEditor.Styles.D3D11FullscreenMode = EditorGUIUtility.TextContent("D3D11 Fullscreen Mode");
				PlayerSettingsEditor.Styles.visibleInBackground = EditorGUIUtility.TextContent("Visible In Background");
				PlayerSettingsEditor.Styles.allowFullscreenSwitch = EditorGUIUtility.TextContent("Allow Fullscreen Switch");
				PlayerSettingsEditor.Styles.use32BitDisplayBuffer = EditorGUIUtility.TextContent("Use 32-bit Display Buffer*|If set Display Buffer will be created to hold 32-bit color values. Use it only if you see banding, as it has performance implications.");
				PlayerSettingsEditor.Styles.disableDepthAndStencilBuffers = EditorGUIUtility.TextContent("Disable Depth and Stencil*");
				PlayerSettingsEditor.Styles.iosShowActivityIndicatorOnLoading = EditorGUIUtility.TextContent("Show Loading Indicator");
				PlayerSettingsEditor.Styles.androidShowActivityIndicatorOnLoading = EditorGUIUtility.TextContent("Show Loading Indicator");
				PlayerSettingsEditor.Styles.actionOnDotNetUnhandledException = EditorGUIUtility.TextContent("On .Net UnhandledException*");
				PlayerSettingsEditor.Styles.logObjCUncaughtExceptions = EditorGUIUtility.TextContent("Log Obj-C Uncaught Exceptions*");
				PlayerSettingsEditor.Styles.enableCrashReportAPI = EditorGUIUtility.TextContent("Enable CrashReport API*");
				PlayerSettingsEditor.Styles.activeColorSpace = EditorGUIUtility.TextContent("Color Space*");
				PlayerSettingsEditor.Styles.metalForceHardShadows = EditorGUIUtility.TextContent("Force hard shadows on Metal*");
				PlayerSettingsEditor.Styles.metalEditorSupport = EditorGUIUtility.TextContent("Metal Editor Support* (Experimental)");
				PlayerSettingsEditor.Styles.metalAPIValidation = EditorGUIUtility.TextContent("Metal API Validation*");
				PlayerSettingsEditor.Styles.mTRendering = EditorGUIUtility.TextContent("Multithreaded Rendering*");
				PlayerSettingsEditor.Styles.staticBatching = EditorGUIUtility.TextContent("Static Batching");
				PlayerSettingsEditor.Styles.dynamicBatching = EditorGUIUtility.TextContent("Dynamic Batching");
				PlayerSettingsEditor.Styles.graphicsJobs = EditorGUIUtility.TextContent("Graphics Jobs (Experimental)*");
				PlayerSettingsEditor.Styles.graphicsJobsMode = EditorGUIUtility.TextContent("Graphics Jobs Mode*");
				PlayerSettingsEditor.Styles.applicationBuildNumber = EditorGUIUtility.TextContent("Build");
				PlayerSettingsEditor.Styles.appleDeveloperTeamID = EditorGUIUtility.TextContent("iOS Developer Team ID|Developers can retrieve their Team ID by visiting the Apple Developer site under Account > Membership.");
				PlayerSettingsEditor.Styles.targetSdkVersion = EditorGUIUtility.TextContent("Target SDK");
				PlayerSettingsEditor.Styles.iPhoneTargetOSVersion = EditorGUIUtility.TextContent("Target minimum iOS Version");
				PlayerSettingsEditor.Styles.tvOSTargetOSVersion = EditorGUIUtility.TextContent("Target minimum tvOS Version");
				PlayerSettingsEditor.Styles.useOnDemandResources = EditorGUIUtility.TextContent("Use on demand resources*");
				PlayerSettingsEditor.Styles.accelerometerFrequency = EditorGUIUtility.TextContent("Accelerometer Frequency*");
				PlayerSettingsEditor.Styles.cameraUsageDescription = EditorGUIUtility.TextContent("Camera Usage Description*");
				PlayerSettingsEditor.Styles.locationUsageDescription = EditorGUIUtility.TextContent("Location Usage Description*");
				PlayerSettingsEditor.Styles.microphoneUsageDescription = EditorGUIUtility.TextContent("Microphone Usage Description*");
				PlayerSettingsEditor.Styles.muteOtherAudioSources = EditorGUIUtility.TextContent("Mute Other Audio Sources*");
				PlayerSettingsEditor.Styles.prepareIOSForRecording = EditorGUIUtility.TextContent("Prepare iOS for Recording");
				PlayerSettingsEditor.Styles.UIRequiresPersistentWiFi = EditorGUIUtility.TextContent("Requires Persistent WiFi*");
				PlayerSettingsEditor.Styles.iOSAllowHTTPDownload = EditorGUIUtility.TextContent("Allow downloads over HTTP (nonsecure)*");
				PlayerSettingsEditor.Styles.iOSURLSchemes = EditorGUIUtility.TextContent("Supported URL schemes*");
				PlayerSettingsEditor.Styles.aotOptions = EditorGUIUtility.TextContent("AOT Compilation Options*");
				PlayerSettingsEditor.Styles.require31 = EditorGUIUtility.TextContent("Require ES3.1");
				PlayerSettingsEditor.Styles.requireAEP = EditorGUIUtility.TextContent("Require ES3.1+AEP");
				PlayerSettingsEditor.Styles.skinOnGPU = EditorGUIUtility.TextContent("GPU Skinning*|Use DX11/ES3 GPU Skinning");
				PlayerSettingsEditor.Styles.skinOnGPUPS4 = EditorGUIUtility.TextContent("Compute Skinning*|Use Compute pipeline for Skinning");
				PlayerSettingsEditor.Styles.disableStatistics = EditorGUIUtility.TextContent("Disable HW Statistics*|Disables HW Statistics (Pro Only)");
				PlayerSettingsEditor.Styles.scriptingDefineSymbols = EditorGUIUtility.TextContent("Scripting Define Symbols*");
				PlayerSettingsEditor.Styles.scriptingBackend = EditorGUIUtility.TextContent("Scripting Backend");
				PlayerSettingsEditor.Styles.scriptingMono2x = EditorGUIUtility.TextContent("Mono2x");
				PlayerSettingsEditor.Styles.scriptingWinRTDotNET = EditorGUIUtility.TextContent(".NET");
				PlayerSettingsEditor.Styles.scriptingIL2CPP = EditorGUIUtility.TextContent("IL2CPP");
				PlayerSettingsEditor.Styles.scriptingDefault = EditorGUIUtility.TextContent("Default");
				PlayerSettingsEditor.Styles.apiCompatibilityLevel = EditorGUIUtility.TextContent("Api Compatibility Level*");
				PlayerSettingsEditor.Styles.apiCompatibilityLevel_WiiUSubset = EditorGUIUtility.TextContent("WiiU Subset");
				PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_2_0 = EditorGUIUtility.TextContent(".NET 2.0");
				PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_2_0_Subset = EditorGUIUtility.TextContent(".NET 2.0 Subset");
				PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_4_6 = EditorGUIUtility.TextContent(".NET 4.6");
				PlayerSettingsEditor.Styles.categoryBox.padding.left = 14;
			}
		}

		private struct ChangeGraphicsApiAction
		{
			public bool changeList;

			public bool reloadGfx;

			public ChangeGraphicsApiAction(bool doChange, bool doReload)
			{
				this.changeList = doChange;
				this.reloadGfx = doReload;
			}
		}

		private const int kSlotSize = 64;

		private const int kMaxPreviewSize = 96;

		private const int kIconSpacing = 6;

		private PlayerSettingsSplashScreenEditor m_SplashScreenEditor;

		private static GraphicsJobMode[] m_GfxJobModeValues = new GraphicsJobMode[]
		{
			GraphicsJobMode.Native,
			GraphicsJobMode.Legacy
		};

		private static GUIContent[] m_GfxJobModeNames = new GUIContent[]
		{
			new GUIContent("Native"),
			new GUIContent("Legacy")
		};

		private SavedInt m_SelectedSection = new SavedInt("PlayerSettings.ShownSection", -1);

		private BuildPlayerWindow.BuildPlatform[] validPlatforms;

		private SerializedProperty m_StripEngineCode;

		private SerializedProperty m_ApplicationBundleVersion;

		private SerializedProperty m_UseMacAppStoreValidation;

		private SerializedProperty m_IPhoneApplicationDisplayName;

		private SerializedProperty m_CameraUsageDescription;

		private SerializedProperty m_LocationUsageDescription;

		private SerializedProperty m_MicrophoneUsageDescription;

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

		private SerializedProperty m_tizenShowActivityIndicatorOnLoading;

		private SerializedProperty m_IPhoneSdkVersion;

		private SerializedProperty m_IPhoneTargetOSVersion;

		private SerializedProperty m_tvOSSdkVersion;

		private SerializedProperty m_tvOSTargetOSVersion;

		private SerializedProperty m_AndroidProfiler;

		private SerializedProperty m_UIPrerenderedIcon;

		private SerializedProperty m_UIRequiresPersistentWiFi;

		private SerializedProperty m_UIStatusBarHidden;

		private SerializedProperty m_UIRequiresFullScreen;

		private SerializedProperty m_UIStatusBarStyle;

		private SerializedProperty m_IOSAllowHTTPDownload;

		private SerializedProperty m_SubmitAnalytics;

		private SerializedProperty m_IOSURLSchemes;

		private SerializedProperty m_TargetDevice;

		private SerializedProperty m_AccelerometerFrequency;

		private SerializedProperty m_useOnDemandResources;

		private SerializedProperty m_MuteOtherAudioSources;

		private SerializedProperty m_PrepareIOSForRecording;

		private SerializedProperty m_EnableInternalProfiler;

		private SerializedProperty m_ActionOnDotNetUnhandledException;

		private SerializedProperty m_LogObjCUncaughtExceptions;

		private SerializedProperty m_EnableCrashReportAPI;

		private SerializedProperty m_VideoMemoryForVertexBuffers;

		private SerializedProperty m_CompanyName;

		private SerializedProperty m_ProductName;

		private SerializedProperty m_DefaultCursor;

		private SerializedProperty m_CursorHotspot;

		private SerializedProperty m_DefaultScreenWidth;

		private SerializedProperty m_DefaultScreenHeight;

		private SerializedProperty m_StereoRenderingPath;

		private SerializedProperty m_ActiveColorSpace;

		private SerializedProperty m_MTRendering;

		private SerializedProperty m_MobileMTRendering;

		private SerializedProperty m_StripUnusedMeshComponents;

		private SerializedProperty m_VertexChannelCompressionMask;

		private SerializedProperty m_MetalForceHardShadows;

		private SerializedProperty m_MetalEditorSupport;

		private SerializedProperty m_MetalAPIValidation;

		private SerializedProperty m_DisplayResolutionDialog;

		private SerializedProperty m_DefaultIsFullScreen;

		private SerializedProperty m_DefaultIsNativeResolution;

		private SerializedProperty m_UsePlayerLog;

		private SerializedProperty m_KeepLoadedShadersAlive;

		private SerializedProperty m_PreloadedAssets;

		private SerializedProperty m_BakeCollisionMeshes;

		private SerializedProperty m_ResizableWindow;

		private SerializedProperty m_MacFullscreenMode;

		private SerializedProperty m_D3D9FullscreenMode;

		private SerializedProperty m_D3D11FullscreenMode;

		private SerializedProperty m_VisibleInBackground;

		private SerializedProperty m_AllowFullscreenSwitch;

		private SerializedProperty m_ForceSingleInstance;

		private SerializedProperty m_RunInBackground;

		private SerializedProperty m_CaptureSingleScreen;

		private SerializedProperty m_SupportedAspectRatios;

		private SerializedProperty m_SkinOnGPU;

		private SerializedProperty m_GraphicsJobs;

		private SerializedProperty m_RequireES31;

		private SerializedProperty m_RequireES31AEP;

		private static Dictionary<BuildTarget, ReorderableList> s_GraphicsDeviceLists = new Dictionary<BuildTarget, ReorderableList>();

		public PlayerSettingsEditorVR m_VRSettings;

		private int selectedPlatform = 0;

		private int scriptingDefinesControlID = 0;

		private ISettingEditorExtension[] m_SettingsExtensions;

		private AnimBool[] m_SectionAnimators = new AnimBool[6];

		private readonly AnimBool m_ShowDefaultIsNativeResolution = new AnimBool();

		private readonly AnimBool m_ShowResolution = new AnimBool();

		private static Texture2D s_WarningIcon;

		private static ApiCompatibilityLevel[] only_2_0_profiles = new ApiCompatibilityLevel[]
		{
			ApiCompatibilityLevel.NET_2_0,
			ApiCompatibilityLevel.NET_2_0_Subset
		};

		private static ApiCompatibilityLevel[] allProfiles = new ApiCompatibilityLevel[]
		{
			ApiCompatibilityLevel.NET_2_0,
			ApiCompatibilityLevel.NET_2_0_Subset,
			ApiCompatibilityLevel.NET_4_6
		};

		private static Dictionary<ScriptingImplementation, GUIContent> m_NiceScriptingBackendNames;

		private static Dictionary<ApiCompatibilityLevel, GUIContent> m_NiceApiCompatibilityLevelNames;

		private PlayerSettingsSplashScreenEditor splashScreenEditor
		{
			get
			{
				if (this.m_SplashScreenEditor == null)
				{
					this.m_SplashScreenEditor = new PlayerSettingsSplashScreenEditor(this);
				}
				return this.m_SplashScreenEditor;
			}
		}

		public static void SyncPlatformAPIsList(BuildTarget target)
		{
			if (PlayerSettingsEditor.s_GraphicsDeviceLists.ContainsKey(target))
			{
				PlayerSettingsEditor.s_GraphicsDeviceLists[target].list = PlayerSettings.GetGraphicsAPIs(target).ToList<GraphicsDeviceType>();
			}
		}

		private bool IsMobileTarget(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.Tizen || targetGroup == BuildTargetGroup.SamsungTV;
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
			this.validPlatforms = BuildPlayerWindow.GetValidPlatforms(true).ToArray();
			this.m_IPhoneSdkVersion = this.FindPropertyAssert("iPhoneSdkVersion");
			this.m_IPhoneTargetOSVersion = this.FindPropertyAssert("iOSTargetOSVersionString");
			this.m_IPhoneStrippingLevel = this.FindPropertyAssert("iPhoneStrippingLevel");
			this.m_StripEngineCode = this.FindPropertyAssert("stripEngineCode");
			this.m_tvOSSdkVersion = this.FindPropertyAssert("tvOSSdkVersion");
			this.m_tvOSTargetOSVersion = this.FindPropertyAssert("tvOSTargetOSVersionString");
			this.m_IPhoneScriptCallOptimization = this.FindPropertyAssert("iPhoneScriptCallOptimization");
			this.m_AndroidProfiler = this.FindPropertyAssert("AndroidProfiler");
			this.m_CompanyName = this.FindPropertyAssert("companyName");
			this.m_ProductName = this.FindPropertyAssert("productName");
			this.m_DefaultCursor = this.FindPropertyAssert("defaultCursor");
			this.m_CursorHotspot = this.FindPropertyAssert("cursorHotspot");
			this.m_UIPrerenderedIcon = this.FindPropertyAssert("uIPrerenderedIcon");
			this.m_UIRequiresFullScreen = this.FindPropertyAssert("uIRequiresFullScreen");
			this.m_UIStatusBarHidden = this.FindPropertyAssert("uIStatusBarHidden");
			this.m_UIStatusBarStyle = this.FindPropertyAssert("uIStatusBarStyle");
			this.m_StereoRenderingPath = this.FindPropertyAssert("m_StereoRenderingPath");
			this.m_ActiveColorSpace = this.FindPropertyAssert("m_ActiveColorSpace");
			this.m_MTRendering = this.FindPropertyAssert("m_MTRendering");
			this.m_MobileMTRendering = this.FindPropertyAssert("m_MobileMTRendering");
			this.m_StripUnusedMeshComponents = this.FindPropertyAssert("StripUnusedMeshComponents");
			this.m_VertexChannelCompressionMask = this.FindPropertyAssert("VertexChannelCompressionMask");
			this.m_MetalForceHardShadows = this.FindPropertyAssert("iOSMetalForceHardShadows");
			this.m_MetalEditorSupport = this.FindPropertyAssert("metalEditorSupport");
			this.m_MetalAPIValidation = this.FindPropertyAssert("metalAPIValidation");
			this.m_ApplicationBundleVersion = base.serializedObject.FindProperty("bundleVersion");
			if (this.m_ApplicationBundleVersion == null)
			{
				this.m_ApplicationBundleVersion = this.FindPropertyAssert("iPhoneBundleVersion");
			}
			this.m_useOnDemandResources = this.FindPropertyAssert("useOnDemandResources");
			this.m_AccelerometerFrequency = this.FindPropertyAssert("accelerometerFrequency");
			this.m_MuteOtherAudioSources = this.FindPropertyAssert("muteOtherAudioSources");
			this.m_PrepareIOSForRecording = this.FindPropertyAssert("Prepare IOS For Recording");
			this.m_UIRequiresPersistentWiFi = this.FindPropertyAssert("uIRequiresPersistentWiFi");
			this.m_IOSAllowHTTPDownload = this.FindPropertyAssert("iosAllowHTTPDownload");
			this.m_SubmitAnalytics = this.FindPropertyAssert("submitAnalytics");
			this.m_IOSURLSchemes = this.FindPropertyAssert("iOSURLSchemes");
			this.m_AotOptions = this.FindPropertyAssert("aotOptions");
			this.m_CameraUsageDescription = this.FindPropertyAssert("cameraUsageDescription");
			this.m_LocationUsageDescription = this.FindPropertyAssert("locationUsageDescription");
			this.m_MicrophoneUsageDescription = this.FindPropertyAssert("microphoneUsageDescription");
			this.m_EnableInternalProfiler = this.FindPropertyAssert("enableInternalProfiler");
			this.m_ActionOnDotNetUnhandledException = this.FindPropertyAssert("actionOnDotNetUnhandledException");
			this.m_LogObjCUncaughtExceptions = this.FindPropertyAssert("logObjCUncaughtExceptions");
			this.m_EnableCrashReportAPI = this.FindPropertyAssert("enableCrashReportAPI");
			this.m_DefaultScreenWidth = this.FindPropertyAssert("defaultScreenWidth");
			this.m_DefaultScreenHeight = this.FindPropertyAssert("defaultScreenHeight");
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
			this.m_tizenShowActivityIndicatorOnLoading = this.FindPropertyAssert("tizenShowActivityIndicatorOnLoading");
			this.m_DefaultIsFullScreen = this.FindPropertyAssert("defaultIsFullScreen");
			this.m_DefaultIsNativeResolution = this.FindPropertyAssert("defaultIsNativeResolution");
			this.m_CaptureSingleScreen = this.FindPropertyAssert("captureSingleScreen");
			this.m_DisplayResolutionDialog = this.FindPropertyAssert("displayResolutionDialog");
			this.m_SupportedAspectRatios = this.FindPropertyAssert("m_SupportedAspectRatios");
			this.m_TargetDevice = this.FindPropertyAssert("targetDevice");
			this.m_UsePlayerLog = this.FindPropertyAssert("usePlayerLog");
			this.m_KeepLoadedShadersAlive = this.FindPropertyAssert("keepLoadedShadersAlive");
			this.m_PreloadedAssets = this.FindPropertyAssert("preloadedAssets");
			this.m_BakeCollisionMeshes = this.FindPropertyAssert("bakeCollisionMeshes");
			this.m_ResizableWindow = this.FindPropertyAssert("resizableWindow");
			this.m_UseMacAppStoreValidation = this.FindPropertyAssert("useMacAppStoreValidation");
			this.m_D3D9FullscreenMode = this.FindPropertyAssert("d3d9FullscreenMode");
			this.m_D3D11FullscreenMode = this.FindPropertyAssert("d3d11FullscreenMode");
			this.m_VisibleInBackground = this.FindPropertyAssert("visibleInBackground");
			this.m_AllowFullscreenSwitch = this.FindPropertyAssert("allowFullscreenSwitch");
			this.m_MacFullscreenMode = this.FindPropertyAssert("macFullscreenMode");
			this.m_SkinOnGPU = this.FindPropertyAssert("gpuSkinning");
			this.m_GraphicsJobs = this.FindPropertyAssert("graphicsJobs");
			this.m_ForceSingleInstance = this.FindPropertyAssert("forceSingleInstance");
			this.m_RequireES31 = this.FindPropertyAssert("openGLRequireES31");
			this.m_RequireES31AEP = this.FindPropertyAssert("openGLRequireES31AEP");
			this.m_VideoMemoryForVertexBuffers = this.FindPropertyAssert("videoMemoryForVertexBuffers");
			this.m_SettingsExtensions = new ISettingEditorExtension[this.validPlatforms.Length];
			for (int i = 0; i < this.validPlatforms.Length; i++)
			{
				string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(this.validPlatforms[i].targetGroup);
				this.m_SettingsExtensions[i] = ModuleManager.GetEditorSettingsExtension(targetStringFromBuildTargetGroup);
				if (this.m_SettingsExtensions[i] != null)
				{
					this.m_SettingsExtensions[i].OnEnable(this);
				}
			}
			for (int j = 0; j < this.m_SectionAnimators.Length; j++)
			{
				this.m_SectionAnimators[j] = new AnimBool(this.m_SelectedSection.value == j, new UnityAction(base.Repaint));
			}
			this.m_ShowDefaultIsNativeResolution.value = this.m_DefaultIsFullScreen.boolValue;
			this.m_ShowResolution.value = (!this.m_DefaultIsFullScreen.boolValue || !this.m_DefaultIsNativeResolution.boolValue);
			this.m_ShowDefaultIsNativeResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowResolution.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_VRSettings = new PlayerSettingsEditorVR(base.serializedObject);
			this.splashScreenEditor.OnEnable();
			PlayerSettingsEditor.s_GraphicsDeviceLists.Clear();
		}

		public override bool UseDefaultMargins()
		{
			return false;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
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
					PlayerSettings.SetScriptingDefineSymbolsForGroup(this.validPlatforms[num].targetGroup, EditorGUI.s_DelayedTextEditor.text);
				}
				GUI.FocusControl("");
			}
			GUILayout.Label("Settings for " + this.validPlatforms[this.selectedPlatform].title.text, new GUILayoutOption[0]);
			EditorGUIUtility.labelWidth = Mathf.Max(150f, EditorGUIUtility.labelWidth - 8f);
			BuildPlayerWindow.BuildPlatform buildPlatform = this.validPlatforms[this.selectedPlatform];
			BuildTargetGroup targetGroup = buildPlatform.targetGroup;
			this.ResolutionSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			this.IconSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			this.m_SplashScreenEditor.SplashSectionGUI(buildPlatform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			this.DebugAndCrashReportingGUI(buildPlatform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			this.OtherSectionGUI(buildPlatform, targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			this.PublishSectionGUI(targetGroup, this.m_SettingsExtensions[this.selectedPlatform]);
			EditorGUILayout.EndPlatformGrouping();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void CommonSettings()
		{
			EditorGUILayout.PropertyField(this.m_CompanyName, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ProductName, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			GUI.changed = false;
			string platform = "";
			Texture2D[] array = PlayerSettings.GetIconsForPlatform(platform);
			int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(platform);
			if (array.Length != iconWidthsForPlatform.Length)
			{
				array = new Texture2D[iconWidthsForPlatform.Length];
			}
			array[0] = (Texture2D)EditorGUILayout.ObjectField(PlayerSettingsEditor.Styles.defaultIcon, array[0], typeof(Texture2D), false, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				Undo.RecordObject(base.target, PlayerSettingsEditor.Styles.undoChangedIconString);
				PlayerSettings.SetIconsForPlatform(platform, array);
			}
			GUILayout.Space(3f);
			Rect controlRect = EditorGUILayout.GetControlRect(true, 64f, new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, PlayerSettingsEditor.Styles.defaultCursor, this.m_DefaultCursor);
			this.m_DefaultCursor.objectReferenceValue = EditorGUI.ObjectField(controlRect, PlayerSettingsEditor.Styles.defaultCursor, this.m_DefaultCursor.objectReferenceValue, typeof(Texture2D), false);
			EditorGUI.EndProperty();
			Rect rect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			rect = EditorGUI.PrefixLabel(rect, 0, PlayerSettingsEditor.Styles.cursorHotspot);
			EditorGUI.PropertyField(rect, this.m_CursorHotspot, GUIContent.none);
		}

		public bool BeginSettingsBox(int nr, GUIContent header)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUILayout.BeginVertical(PlayerSettingsEditor.Styles.categoryBox, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(20f, 18f);
			rect.x += 3f;
			rect.width += 6f;
			EditorGUI.BeginChangeCheck();
			bool flag = GUI.Toggle(rect, this.m_SelectedSection.value == nr, header, EditorStyles.inspectorTitlebarText);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_SelectedSection.value = ((!flag) ? -1 : nr);
				GUIUtility.keyboardControl = 0;
			}
			this.m_SectionAnimators[nr].target = flag;
			GUI.enabled = enabled;
			return EditorGUILayout.BeginFadeGroup(this.m_SectionAnimators[nr].faded);
		}

		public void EndSettingsBox()
		{
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.EndVertical();
		}

		private void ShowNoSettings()
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.notApplicableInfo, EditorStyles.miniLabel, new GUILayoutOption[0]);
		}

		public void ShowSharedNote()
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.sharedBetweenPlatformsInfo, EditorStyles.miniLabel, new GUILayoutOption[0]);
		}

		private void IconSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (this.BeginSettingsBox(1, PlayerSettingsEditor.Styles.iconTitle))
			{
				bool flag = true;
				if (settingsExtension != null)
				{
					flag = settingsExtension.UsesStandardIcons();
				}
				if (flag)
				{
					bool flag2 = this.selectedPlatform < 0;
					BuildPlayerWindow.BuildPlatform buildPlatform = null;
					targetGroup = BuildTargetGroup.Standalone;
					string platform = "";
					if (!flag2)
					{
						buildPlatform = this.validPlatforms[this.selectedPlatform];
						targetGroup = buildPlatform.targetGroup;
						platform = buildPlatform.name;
					}
					bool enabled = GUI.enabled;
					if (targetGroup == BuildTargetGroup.SamsungTV || targetGroup == BuildTargetGroup.WebGL)
					{
						this.ShowNoSettings();
						EditorGUILayout.Space();
					}
					else if (targetGroup != BuildTargetGroup.WSA)
					{
						Texture2D[] array = PlayerSettings.GetIconsForPlatform(platform);
						int[] iconWidthsForPlatform = PlayerSettings.GetIconWidthsForPlatform(platform);
						int[] iconHeightsForPlatform = PlayerSettings.GetIconHeightsForPlatform(platform);
						bool flag3 = true;
						if (!flag2)
						{
							GUI.changed = false;
							flag3 = (array.Length == iconWidthsForPlatform.Length);
							flag3 = GUILayout.Toggle(flag3, "Override for " + buildPlatform.title.text, new GUILayoutOption[0]);
							GUI.enabled = (enabled && flag3);
							if (GUI.changed || (!flag3 && array.Length > 0))
							{
								if (flag3)
								{
									array = new Texture2D[iconWidthsForPlatform.Length];
								}
								else
								{
									array = new Texture2D[0];
								}
								if (GUI.changed)
								{
									PlayerSettings.SetIconsForPlatform(platform, array);
								}
							}
						}
						GUI.changed = false;
						for (int i = 0; i < iconWidthsForPlatform.Length; i++)
						{
							int num = Mathf.Min(96, iconWidthsForPlatform[i]);
							int num2 = (int)((float)iconHeightsForPlatform[i] * (float)num / (float)iconWidthsForPlatform[i]);
							if (targetGroup == BuildTargetGroup.iPhone)
							{
								if (i + 1 < iconWidthsForPlatform.Length && iconWidthsForPlatform[i + 1] == 80)
								{
									Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 20f);
									GUI.Label(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, 20f), "Spotlight icons", EditorStyles.boldLabel);
								}
								if (iconWidthsForPlatform[i] == 87)
								{
									Rect rect2 = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 20f);
									GUI.Label(new Rect(rect2.x, rect2.y, EditorGUIUtility.labelWidth, 20f), "Settings icons", EditorStyles.boldLabel);
								}
							}
							Rect rect3 = GUILayoutUtility.GetRect(64f, (float)(Mathf.Max(64, num2) + 6));
							float num3 = Mathf.Min(rect3.width, EditorGUIUtility.labelWidth + 4f + 64f + 6f + 96f);
							string text = iconWidthsForPlatform[i] + "x" + iconHeightsForPlatform[i];
							GUI.Label(new Rect(rect3.x, rect3.y, num3 - 96f - 64f - 12f, 20f), text);
							if (flag3)
							{
								int num4 = 64;
								int num5 = (int)((float)iconHeightsForPlatform[i] / (float)iconWidthsForPlatform[i] * 64f);
								array[i] = (Texture2D)EditorGUI.ObjectField(new Rect(rect3.x + num3 - 96f - 64f - 6f, rect3.y, (float)num4, (float)num5), array[i], typeof(Texture2D), false);
							}
							Rect position = new Rect(rect3.x + num3 - 96f, rect3.y, (float)num, (float)num2);
							Texture2D iconForPlatformAtSize = PlayerSettings.GetIconForPlatformAtSize(platform, iconWidthsForPlatform[i], iconHeightsForPlatform[i]);
							if (iconForPlatformAtSize != null)
							{
								GUI.DrawTexture(position, iconForPlatformAtSize);
							}
							else
							{
								GUI.Box(position, "");
							}
						}
						if (GUI.changed)
						{
							Undo.RecordObject(base.target, PlayerSettingsEditor.Styles.undoChangedIconString);
							PlayerSettings.SetIconsForPlatform(platform, array);
						}
						GUI.enabled = enabled;
						if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
						{
							EditorGUILayout.PropertyField(this.m_UIPrerenderedIcon, PlayerSettingsEditor.Styles.UIPrerenderedIcon, new GUILayoutOption[0]);
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

		private static bool TargetSupportsOptionalBuiltinSplashScreen(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			bool result;
			if (settingsExtension != null)
			{
				result = settingsExtension.CanShowUnitySplashScreen();
			}
			else
			{
				result = (targetGroup == BuildTargetGroup.Standalone);
			}
			return result;
		}

		private static bool TargetSupportsProtectedGraphicsMem(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.Android;
		}

		private static bool TargetSupportsHighDynamicRangeDisplays(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.XboxOne;
		}

		public void ResolutionSectionGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (this.BeginSettingsBox(0, PlayerSettingsEditor.Styles.resolutionPresentationTitle))
			{
				if (settingsExtension != null)
				{
					float h = 16f;
					float midWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
					float maxWidth = 80f + EditorGUIUtility.fieldWidth + 5f;
					settingsExtension.ResolutionSectionGUI(h, midWidth, maxWidth);
				}
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					GUILayout.Label(PlayerSettingsEditor.Styles.resolutionTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DefaultIsFullScreen, PlayerSettingsEditor.Styles.defaultIsFullScreen, new GUILayoutOption[0]);
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
						EditorGUILayout.PropertyField(this.m_DefaultScreenWidth, PlayerSettingsEditor.Styles.defaultScreenWidth, new GUILayoutOption[0]);
						if (EditorGUI.EndChangeCheck() && this.m_DefaultScreenWidth.intValue < 1)
						{
							this.m_DefaultScreenWidth.intValue = 1;
						}
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_DefaultScreenHeight, PlayerSettingsEditor.Styles.defaultScreenHeight, new GUILayoutOption[0]);
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
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					EditorGUILayout.PropertyField(this.m_RunInBackground, PlayerSettingsEditor.Styles.runInBackground, new GUILayoutOption[0]);
				}
				if (settingsExtension != null && settingsExtension.SupportsOrientation())
				{
					GUILayout.Label(PlayerSettingsEditor.Styles.orientationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					using (new EditorGUI.DisabledScope(PlayerSettings.virtualRealitySupported))
					{
						EditorGUILayout.PropertyField(this.m_DefaultScreenOrientation, PlayerSettingsEditor.Styles.defaultScreenOrientation, new GUILayoutOption[0]);
						if (PlayerSettings.virtualRealitySupported)
						{
							EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.VRSupportOverridenInfo.text, MessageType.Info);
						}
						if (this.m_DefaultScreenOrientation.enumValueIndex == 4)
						{
							if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Tizen)
							{
								EditorGUILayout.PropertyField(this.m_UseOSAutoRotation, PlayerSettingsEditor.Styles.useOSAutoRotation, new GUILayoutOption[0]);
							}
							EditorGUI.indentLevel++;
							GUILayout.Label(PlayerSettingsEditor.Styles.allowedOrientationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
							if (!this.m_AllowedAutoRotateToPortrait.boolValue && !this.m_AllowedAutoRotateToPortraitUpsideDown.boolValue && !this.m_AllowedAutoRotateToLandscapeRight.boolValue && !this.m_AllowedAutoRotateToLandscapeLeft.boolValue)
							{
								this.m_AllowedAutoRotateToPortrait.boolValue = true;
								Debug.LogError("All orientations are disabled. Allowing portrait");
							}
							EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortrait, PlayerSettingsEditor.Styles.allowedAutoRotateToPortrait, new GUILayoutOption[0]);
							if (targetGroup != BuildTargetGroup.WSA || EditorUserBuildSettings.wsaSDK != WSASDK.PhoneSDK81)
							{
								EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToPortraitUpsideDown, PlayerSettingsEditor.Styles.allowedAutoRotateToPortraitUpsideDown, new GUILayoutOption[0]);
							}
							EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeRight, PlayerSettingsEditor.Styles.allowedAutoRotateToLandscapeRight, new GUILayoutOption[0]);
							EditorGUILayout.PropertyField(this.m_AllowedAutoRotateToLandscapeLeft, PlayerSettingsEditor.Styles.allowedAutoRotateToLandscapeLeft, new GUILayoutOption[0]);
							EditorGUI.indentLevel--;
						}
					}
				}
				if (targetGroup == BuildTargetGroup.iPhone)
				{
					GUILayout.Label(PlayerSettingsEditor.Styles.multitaskingSupportTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UIRequiresFullScreen, PlayerSettingsEditor.Styles.UIRequiresFullScreen, new GUILayoutOption[0]);
					EditorGUILayout.Space();
					GUILayout.Label(PlayerSettingsEditor.Styles.statusBarTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UIStatusBarHidden, PlayerSettingsEditor.Styles.UIStatusBarHidden, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UIStatusBarStyle, PlayerSettingsEditor.Styles.UIStatusBarStyle, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				EditorGUILayout.Space();
				if (targetGroup == BuildTargetGroup.Standalone)
				{
					GUILayout.Label(PlayerSettingsEditor.Styles.standalonePlayerOptionsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_CaptureSingleScreen, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_DisplayResolutionDialog, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_UsePlayerLog, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ResizableWindow, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_MacFullscreenMode, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_D3D9FullscreenMode, PlayerSettingsEditor.Styles.D3D9FullscreenMode, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_D3D11FullscreenMode, PlayerSettingsEditor.Styles.D3D11FullscreenMode, new GUILayoutOption[0]);
					GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
					bool flag = graphicsAPIs.Length >= 1 && graphicsAPIs[0] == GraphicsDeviceType.Direct3D9;
					bool flag2 = this.m_D3D9FullscreenMode.intValue == 0;
					bool flag3 = flag && flag2;
					if (flag3)
					{
						this.m_VisibleInBackground.boolValue = false;
					}
					using (new EditorGUI.DisabledScope(flag3))
					{
						EditorGUILayout.PropertyField(this.m_VisibleInBackground, PlayerSettingsEditor.Styles.visibleInBackground, new GUILayoutOption[0]);
					}
					EditorGUILayout.PropertyField(this.m_AllowFullscreenSwitch, PlayerSettingsEditor.Styles.allowFullscreenSwitch, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ForceSingleInstance, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_SupportedAspectRatios, true, new GUILayoutOption[0]);
					EditorGUILayout.Space();
				}
				if (this.IsMobileTarget(targetGroup))
				{
					if (targetGroup != BuildTargetGroup.Tizen && targetGroup != BuildTargetGroup.iPhone && targetGroup != BuildTargetGroup.tvOS)
					{
						EditorGUILayout.PropertyField(this.m_Use32BitDisplayBuffer, PlayerSettingsEditor.Styles.use32BitDisplayBuffer, new GUILayoutOption[0]);
					}
					EditorGUILayout.PropertyField(this.m_DisableDepthAndStencilBuffers, PlayerSettingsEditor.Styles.disableDepthAndStencilBuffers, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone)
				{
					EditorGUILayout.PropertyField(this.m_iosShowActivityIndicatorOnLoading, PlayerSettingsEditor.Styles.iosShowActivityIndicatorOnLoading, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Android)
				{
					EditorGUILayout.PropertyField(this.m_androidShowActivityIndicatorOnLoading, PlayerSettingsEditor.Styles.androidShowActivityIndicatorOnLoading, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.Tizen)
				{
					EditorGUILayout.PropertyField(this.m_tizenShowActivityIndicatorOnLoading, EditorGUIUtility.TextContent("Show Loading Indicator"), new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.Tizen)
				{
					EditorGUILayout.Space();
				}
				this.ShowSharedNote();
			}
			this.EndSettingsBox();
		}

		private void AddGraphicsDeviceMenuSelected(object userData, string[] options, int selected)
		{
			BuildTarget platform = (BuildTarget)userData;
			GraphicsDeviceType[] array = PlayerSettings.GetGraphicsAPIs(platform);
			if (array != null)
			{
				GraphicsDeviceType item = (GraphicsDeviceType)Enum.Parse(typeof(GraphicsDeviceType), options[selected], true);
				List<GraphicsDeviceType> list = array.ToList<GraphicsDeviceType>();
				list.Add(item);
				array = list.ToArray();
				PlayerSettings.SetGraphicsAPIs(platform, array);
			}
		}

		private void AddGraphicsDeviceElement(BuildTarget target, Rect rect, ReorderableList list)
		{
			GraphicsDeviceType[] supportedGraphicsAPIs = PlayerSettings.GetSupportedGraphicsAPIs(target);
			if (supportedGraphicsAPIs != null && supportedGraphicsAPIs.Length != 0)
			{
				string[] array = new string[supportedGraphicsAPIs.Length];
				bool[] array2 = new bool[supportedGraphicsAPIs.Length];
				for (int i = 0; i < supportedGraphicsAPIs.Length; i++)
				{
					array[i] = supportedGraphicsAPIs[i].ToString();
					array2[i] = !list.list.Contains(supportedGraphicsAPIs[i]);
				}
				EditorUtility.DisplayCustomMenu(rect, array, array2, null, new EditorUtility.SelectMenuItemFunction(this.AddGraphicsDeviceMenuSelected), target);
			}
		}

		private bool CanRemoveGraphicsDeviceElement(ReorderableList list)
		{
			return list.list.Count >= 2;
		}

		private void RemoveGraphicsDeviceElement(BuildTarget target, ReorderableList list)
		{
			GraphicsDeviceType[] array = PlayerSettings.GetGraphicsAPIs(target);
			if (array != null)
			{
				if (array.Length < 2)
				{
					EditorApplication.Beep();
				}
				else
				{
					List<GraphicsDeviceType> list2 = array.ToList<GraphicsDeviceType>();
					list2.RemoveAt(list.index);
					array = list2.ToArray();
					this.ApplyChangedGraphicsAPIList(target, array, list.index == 0);
				}
			}
		}

		private void ReorderGraphicsDeviceElement(BuildTarget target, ReorderableList list)
		{
			GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(target);
			List<GraphicsDeviceType> list2 = (List<GraphicsDeviceType>)list.list;
			GraphicsDeviceType[] array = list2.ToArray();
			bool firstEntryChanged = graphicsAPIs[0] != array[0];
			this.ApplyChangedGraphicsAPIList(target, array, firstEntryChanged);
		}

		private PlayerSettingsEditor.ChangeGraphicsApiAction CheckApplyGraphicsAPIList(BuildTarget target, bool firstEntryChanged)
		{
			bool doChange = true;
			bool doReload = false;
			if (firstEntryChanged && PlayerSettingsEditor.WillEditorUseFirstGraphicsAPI(target))
			{
				doChange = false;
				if (EditorUtility.DisplayDialog("Changing editor graphics device", "Changing active graphics API requires reloading all graphics objects, it might take a while", "Apply", "Cancel"))
				{
					if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					{
						doReload = (doChange = true);
					}
				}
			}
			return new PlayerSettingsEditor.ChangeGraphicsApiAction(doChange, doReload);
		}

		private void ApplyChangeGraphicsApiAction(BuildTarget target, GraphicsDeviceType[] apis, PlayerSettingsEditor.ChangeGraphicsApiAction action)
		{
			if (action.changeList)
			{
				PlayerSettings.SetGraphicsAPIs(target, apis);
			}
			else
			{
				PlayerSettingsEditor.s_GraphicsDeviceLists.Remove(target);
			}
			if (action.reloadGfx)
			{
				ShaderUtil.RecreateGfxDevice();
				GUIUtility.ExitGUI();
			}
		}

		private void ApplyChangedGraphicsAPIList(BuildTarget target, GraphicsDeviceType[] apis, bool firstEntryChanged)
		{
			PlayerSettingsEditor.ChangeGraphicsApiAction action = this.CheckApplyGraphicsAPIList(target, firstEntryChanged);
			this.ApplyChangeGraphicsApiAction(target, apis, action);
		}

		private void DrawGraphicsDeviceElement(BuildTarget target, Rect rect, int index, bool selected, bool focused)
		{
			object obj = PlayerSettingsEditor.s_GraphicsDeviceLists[target].list[index];
			string text = obj.ToString();
			if (text == "Direct3D12")
			{
				text = "Direct3D12 (Experimental)";
			}
			if (text == "Vulkan" && target != BuildTarget.Android)
			{
				text = "Vulkan (Experimental)";
			}
			if (target == BuildTarget.WebGL)
			{
				if (text == "OpenGLES3")
				{
					text = "WebGL 2.0";
				}
				else if (text == "OpenGLES2")
				{
					text = "WebGL 1.0";
				}
			}
			GUI.Label(rect, text, EditorStyles.label);
		}

		private static bool WillEditorUseFirstGraphicsAPI(BuildTarget targetPlatform)
		{
			return (Application.platform == RuntimePlatform.WindowsEditor && targetPlatform == BuildTarget.StandaloneWindows) || (Application.platform == RuntimePlatform.OSXEditor && targetPlatform == BuildTarget.StandaloneOSXUniversal);
		}

		private void OpenGLES31OptionsGUI(BuildTargetGroup targetGroup, BuildTarget targetPlatform)
		{
			if (targetGroup == BuildTargetGroup.Android)
			{
				GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(targetPlatform);
				if (graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES2))
				{
					EditorGUILayout.PropertyField(this.m_RequireES31, PlayerSettingsEditor.Styles.require31, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_RequireES31AEP, PlayerSettingsEditor.Styles.requireAEP, new GUILayoutOption[0]);
				}
			}
		}

		private void GraphicsAPIsGUIOnePlatform(BuildTargetGroup targetGroup, BuildTarget targetPlatform, string platformTitle)
		{
			GraphicsDeviceType[] supportedGraphicsAPIs = PlayerSettings.GetSupportedGraphicsAPIs(targetPlatform);
			if (supportedGraphicsAPIs != null && supportedGraphicsAPIs.Length >= 2)
			{
				EditorGUI.BeginChangeCheck();
				bool flag = PlayerSettings.GetUseDefaultGraphicsAPIs(targetPlatform);
				flag = EditorGUILayout.Toggle("Auto Graphics API" + (platformTitle ?? string.Empty), flag, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(base.target, PlayerSettingsEditor.Styles.undoChangedGraphicsAPIString);
					PlayerSettings.SetUseDefaultGraphicsAPIs(targetPlatform, flag);
				}
				if (!flag)
				{
					if (PlayerSettingsEditor.WillEditorUseFirstGraphicsAPI(targetPlatform))
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.recordingInfo.text, MessageType.Info, true);
					}
					string displayTitle = "Graphics APIs";
					if (platformTitle != null)
					{
						displayTitle += platformTitle;
					}
					if (!PlayerSettingsEditor.s_GraphicsDeviceLists.ContainsKey(targetPlatform))
					{
						GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(targetPlatform);
						List<GraphicsDeviceType> elements = (graphicsAPIs == null) ? new List<GraphicsDeviceType>() : graphicsAPIs.ToList<GraphicsDeviceType>();
						ReorderableList reorderableList = new ReorderableList(elements, typeof(GraphicsDeviceType), true, true, true, true);
						reorderableList.onAddDropdownCallback = delegate(Rect rect, ReorderableList list)
						{
							this.AddGraphicsDeviceElement(targetPlatform, rect, list);
						};
						reorderableList.onCanRemoveCallback = new ReorderableList.CanRemoveCallbackDelegate(this.CanRemoveGraphicsDeviceElement);
						reorderableList.onRemoveCallback = delegate(ReorderableList list)
						{
							this.RemoveGraphicsDeviceElement(targetPlatform, list);
						};
						reorderableList.onReorderCallback = delegate(ReorderableList list)
						{
							this.ReorderGraphicsDeviceElement(targetPlatform, list);
						};
						reorderableList.drawElementCallback = delegate(Rect rect, int index, bool isActive, bool isFocused)
						{
							this.DrawGraphicsDeviceElement(targetPlatform, rect, index, isActive, isFocused);
						};
						reorderableList.drawHeaderCallback = delegate(Rect rect)
						{
							GUI.Label(rect, displayTitle, EditorStyles.label);
						};
						reorderableList.elementHeight = 16f;
						PlayerSettingsEditor.s_GraphicsDeviceLists.Add(targetPlatform, reorderableList);
					}
					PlayerSettingsEditor.s_GraphicsDeviceLists[targetPlatform].DoLayoutList();
					this.OpenGLES31OptionsGUI(targetGroup, targetPlatform);
				}
			}
		}

		private void GraphicsAPIsGUI(BuildTargetGroup targetGroup, BuildTarget target)
		{
			if (targetGroup == BuildTargetGroup.Standalone)
			{
				this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneWindows, " for Windows");
				this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneOSXUniversal, " for Mac");
				this.GraphicsAPIsGUIOnePlatform(targetGroup, BuildTarget.StandaloneLinuxUniversal, " for Linux");
			}
			else
			{
				this.GraphicsAPIsGUIOnePlatform(targetGroup, target, null);
			}
		}

		public void DebugAndCrashReportingGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
			{
				if (this.BeginSettingsBox(3, PlayerSettingsEditor.Styles.debuggingCrashReportingTitle))
				{
					GUILayout.Label(PlayerSettingsEditor.Styles.debuggingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_EnableInternalProfiler, PlayerSettingsEditor.Styles.enableInternalProfiler, new GUILayoutOption[0]);
					EditorGUILayout.Space();
					GUILayout.Label(PlayerSettingsEditor.Styles.crashReportingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ActionOnDotNetUnhandledException, PlayerSettingsEditor.Styles.actionOnDotNetUnhandledException, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LogObjCUncaughtExceptions, PlayerSettingsEditor.Styles.logObjCUncaughtExceptions, new GUILayoutOption[0]);
					GUIContent gUIContent = PlayerSettingsEditor.Styles.enableCrashReportAPI;
					bool disabled = false;
					if (CrashReportingSettings.enabled)
					{
						gUIContent = new GUIContent(gUIContent);
						disabled = true;
						gUIContent.tooltip = "CrashReport API must be enabled for Performance Reporting service.";
						this.m_EnableCrashReportAPI.boolValue = true;
					}
					EditorGUI.BeginDisabledGroup(disabled);
					EditorGUILayout.PropertyField(this.m_EnableCrashReportAPI, gUIContent, new GUILayoutOption[0]);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.Space();
				}
				this.EndSettingsBox();
			}
		}

		public static void BuildDisabledEnumPopup(GUIContent selected, GUIContent uiString)
		{
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUI.Popup(EditorGUILayout.GetControlRect(true, new GUILayoutOption[0]), uiString, 0, new GUIContent[]
				{
					selected
				});
			}
		}

		public static void BuildEnumPopup<T>(SerializedProperty prop, GUIContent uiString, T[] options, GUIContent[] optionNames)
		{
			T t = (T)((object)prop.intValue);
			T t2 = PlayerSettingsEditor.BuildEnumPopup<T>(uiString, t, options, optionNames);
			if (!t2.Equals(t))
			{
				prop.intValue = (int)((object)t2);
				prop.serializedObject.ApplyModifiedProperties();
			}
		}

		public static T BuildEnumPopup<T>(GUIContent uiString, T selected, T[] options, GUIContent[] optionNames)
		{
			int selectedIndex = 0;
			for (int i = 1; i < options.Length; i++)
			{
				if (selected.Equals(options[i]))
				{
					selectedIndex = i;
					break;
				}
			}
			int num = EditorGUILayout.Popup(uiString, selectedIndex, optionNames, new GUILayoutOption[0]);
			return options[num];
		}

		public void OtherSectionGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (this.BeginSettingsBox(4, PlayerSettingsEditor.Styles.otherSettingsTitle))
			{
				this.OtherSectionRenderingGUI(platform, targetGroup, settingsExtension);
				this.OtherSectionIdentificationGUI(targetGroup, settingsExtension);
				this.OtherSectionConfigurationGUI(targetGroup, settingsExtension);
				this.OtherSectionOptimizationGUI(targetGroup);
				this.OtherSectionLoggingGUI();
				this.ShowSharedNote();
			}
			this.EndSettingsBox();
		}

		private void OtherSectionRenderingGUI(BuildPlayerWindow.BuildPlatform platform, BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.renderingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.XboxOne || targetGroup == BuildTargetGroup.WSA || targetGroup == BuildTargetGroup.WiiU || targetGroup == BuildTargetGroup.WebGL)
			{
				using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_ActiveColorSpace, PlayerSettingsEditor.Styles.activeColorSpace, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						base.serializedObject.ApplyModifiedProperties();
						GUIUtility.ExitGUI();
					}
				}
				if (PlayerSettings.colorSpace == ColorSpace.Linear)
				{
					if (targetGroup == BuildTargetGroup.iPhone)
					{
						GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
						bool flag = !graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES2);
						Version v = new Version(8, 0);
						Version version = new Version(6, 0);
						Version v2 = (!string.IsNullOrEmpty(PlayerSettings.iOS.targetOSVersionString)) ? new Version(PlayerSettings.iOS.targetOSVersionString) : version;
						if (!flag || v2 < v)
						{
							EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.colorSpaceIOSWarning.text, MessageType.Warning);
						}
					}
					if (targetGroup == BuildTargetGroup.tvOS)
					{
						GraphicsDeviceType[] graphicsAPIs2 = PlayerSettings.GetGraphicsAPIs(BuildTarget.tvOS);
						if (graphicsAPIs2.Contains(GraphicsDeviceType.OpenGLES3) || graphicsAPIs2.Contains(GraphicsDeviceType.OpenGLES2))
						{
							EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.colorSpaceTVOSWarning.text, MessageType.Warning);
						}
					}
					if (targetGroup == BuildTargetGroup.Android)
					{
						GraphicsDeviceType[] graphicsAPIs3 = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
						bool flag2 = (graphicsAPIs3.Contains(GraphicsDeviceType.Vulkan) || graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES3)) && !graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES2);
						if (!flag2 || PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel18)
						{
							EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.colorSpaceAndroidWarning.text, MessageType.Warning);
						}
					}
					if (targetGroup == BuildTargetGroup.WebGL)
					{
						EditorGUILayout.HelpBox(PlayerSettingsEditor.Styles.colorSpaceWebGLWarning.text, MessageType.Error);
					}
				}
			}
			this.GraphicsAPIsGUI(targetGroup, platform.DefaultTarget);
			if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
			{
				this.m_MetalForceHardShadows.boolValue = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.metalForceHardShadows, this.m_MetalForceHardShadows.boolValue, new GUILayoutOption[0]);
			}
			if (Application.platform == RuntimePlatform.OSXEditor && (targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS))
			{
				bool flag3 = this.m_MetalEditorSupport.boolValue || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal;
				bool flag4 = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.metalEditorSupport, flag3, new GUILayoutOption[0]);
				if (flag4 != flag3)
				{
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						GraphicsDeviceType[] graphicsAPIs4 = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneOSXUniversal);
						bool firstEntryChanged = graphicsAPIs4[0] != SystemInfo.graphicsDeviceType;
						if (!flag4 && SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
						{
							firstEntryChanged = true;
						}
						if (flag4 && graphicsAPIs4[0] == GraphicsDeviceType.Metal)
						{
							firstEntryChanged = true;
						}
						PlayerSettingsEditor.ChangeGraphicsApiAction action = this.CheckApplyGraphicsAPIList(BuildTarget.StandaloneOSXUniversal, firstEntryChanged);
						if (action.changeList)
						{
							this.m_MetalEditorSupport.boolValue = flag4;
							base.serializedObject.ApplyModifiedProperties();
						}
						this.ApplyChangeGraphicsApiAction(BuildTarget.StandaloneOSXUniversal, graphicsAPIs4, action);
					}
					else
					{
						this.m_MetalEditorSupport.boolValue = flag4;
						base.serializedObject.ApplyModifiedProperties();
					}
				}
				if (this.m_MetalEditorSupport.boolValue)
				{
					this.m_MetalAPIValidation.boolValue = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.metalAPIValidation, this.m_MetalAPIValidation.boolValue, new GUILayoutOption[0]);
				}
			}
			if (targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.SamsungTV)
			{
				if (this.IsMobileTarget(targetGroup))
				{
					this.m_MobileMTRendering.boolValue = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.mTRendering, this.m_MobileMTRendering.boolValue, new GUILayoutOption[0]);
				}
				else
				{
					this.m_MTRendering.boolValue = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.mTRendering, this.m_MTRendering.boolValue, new GUILayoutOption[0]);
				}
			}
			else if (targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
			{
				if (Unsupported.IsDeveloperBuild())
				{
					this.m_MTRendering.boolValue = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.mTRendering, this.m_MTRendering.boolValue, new GUILayoutOption[0]);
				}
				else
				{
					this.m_MTRendering.boolValue = true;
				}
			}
			bool flag5 = true;
			bool flag6 = true;
			if (settingsExtension != null)
			{
				flag5 = settingsExtension.SupportsStaticBatching();
				flag6 = settingsExtension.SupportsDynamicBatching();
			}
			int num;
			int num2;
			PlayerSettings.GetBatchingForPlatform(platform.DefaultTarget, out num, out num2);
			bool flag7 = false;
			if (!flag5 && num == 1)
			{
				num = 0;
				flag7 = true;
			}
			if (!flag6 && num2 == 1)
			{
				num2 = 0;
				flag7 = true;
			}
			if (flag7)
			{
				PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
			}
			EditorGUI.BeginChangeCheck();
			using (new EditorGUI.DisabledScope(!flag5))
			{
				if (GUI.enabled)
				{
					num = ((!EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.staticBatching, num != 0, new GUILayoutOption[0])) ? 0 : 1);
				}
				else
				{
					EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.staticBatching, false, new GUILayoutOption[0]);
				}
			}
			using (new EditorGUI.DisabledScope(!flag6))
			{
				num2 = ((!EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.dynamicBatching, num2 != 0, new GUILayoutOption[0])) ? 0 : 1);
			}
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(base.target, PlayerSettingsEditor.Styles.undoChangedBatchingString);
				PlayerSettings.SetBatchingForPlatform(platform.DefaultTarget, num, num2);
			}
			if (targetGroup == BuildTargetGroup.WiiU || targetGroup == BuildTargetGroup.Standalone || targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.PSM || targetGroup == BuildTargetGroup.WSA)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_SkinOnGPU, (targetGroup == BuildTargetGroup.PS4) ? PlayerSettingsEditor.Styles.skinOnGPUPS4 : PlayerSettingsEditor.Styles.skinOnGPU, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					ShaderUtil.RecreateSkinnedMeshResources();
				}
			}
			EditorGUILayout.PropertyField(this.m_GraphicsJobs, PlayerSettingsEditor.Styles.graphicsJobs, new GUILayoutOption[0]);
			if (this.PlatformSupportsGfxJobModes(targetGroup))
			{
				using (new EditorGUI.DisabledScope(!this.m_GraphicsJobs.boolValue))
				{
					GraphicsJobMode graphicsJobMode = PlayerSettings.graphicsJobMode;
					GraphicsJobMode graphicsJobMode2 = PlayerSettingsEditor.BuildEnumPopup<GraphicsJobMode>(PlayerSettingsEditor.Styles.graphicsJobsMode, graphicsJobMode, PlayerSettingsEditor.m_GfxJobModeValues, PlayerSettingsEditor.m_GfxJobModeNames);
					if (graphicsJobMode2 != graphicsJobMode)
					{
						PlayerSettings.graphicsJobMode = graphicsJobMode2;
					}
				}
			}
			if (this.m_VRSettings.TargetGroupSupportsVirtualReality(targetGroup))
			{
				this.m_VRSettings.DevicesGUI(targetGroup);
				using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
				{
					this.m_VRSettings.SinglePassStereoGUI(targetGroup, this.m_StereoRenderingPath);
				}
			}
			if (PlayerSettingsEditor.TargetSupportsProtectedGraphicsMem(targetGroup))
			{
				PlayerSettings.protectGraphicsMemory = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.protectGraphicsMemory, PlayerSettings.protectGraphicsMemory, new GUILayoutOption[0]);
			}
			if (PlayerSettingsEditor.TargetSupportsHighDynamicRangeDisplays(targetGroup))
			{
				PlayerSettings.useHDRDisplay = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Use display in HDR mode|Automatically switch the display to HDR output (on supported displays) at start of application."), PlayerSettings.useHDRDisplay, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
		}

		private void OtherSectionIdentificationGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			if (settingsExtension != null && settingsExtension.HasIdentificationGUI())
			{
				GUILayout.Label(PlayerSettingsEditor.Styles.identificationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
				settingsExtension.IdentificationSectionGUI();
				EditorGUILayout.Space();
			}
			else if (targetGroup == BuildTargetGroup.Standalone)
			{
				GUILayout.Label(PlayerSettingsEditor.Styles.macAppStoreTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
				PlayerSettingsEditor.ShowApplicationIdentifierUI(base.serializedObject, BuildTargetGroup.Standalone, "Bundle Identifier", PlayerSettingsEditor.Styles.undoChangedBundleIdentifierString);
				EditorGUILayout.PropertyField(this.m_ApplicationBundleVersion, EditorGUIUtility.TextContent("Version*"), new GUILayoutOption[0]);
				PlayerSettingsEditor.ShowBuildNumberUI(base.serializedObject, BuildTargetGroup.Standalone, "Build", PlayerSettingsEditor.Styles.undoChangedBuildNumberString);
				EditorGUILayout.PropertyField(this.m_UseMacAppStoreValidation, PlayerSettingsEditor.Styles.useMacAppStoreValidation, new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
		}

		internal static void ShowApplicationIdentifierUI(SerializedObject serializedObject, BuildTargetGroup targetGroup, string label, string undoText)
		{
			EditorGUI.BeginChangeCheck();
			string identifier = EditorGUILayout.DelayedTextField(EditorGUIUtility.TextContent(label), PlayerSettings.GetApplicationIdentifier(targetGroup), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedObject.targetObject, undoText);
				PlayerSettings.SetApplicationIdentifier(targetGroup, identifier);
			}
		}

		internal static void ShowBuildNumberUI(SerializedObject serializedObject, BuildTargetGroup targetGroup, string label, string undoText)
		{
			EditorGUI.BeginChangeCheck();
			string buildNumber = EditorGUILayout.DelayedTextField(EditorGUIUtility.TextContent(label), PlayerSettings.GetBuildNumber(targetGroup), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(serializedObject.targetObject, undoText);
				PlayerSettings.SetBuildNumber(targetGroup, buildNumber);
			}
		}

		private Version ParseIosVersion(string text)
		{
			if (text.IndexOf('.') < 0)
			{
				text += ".0";
			}
			Version result;
			try
			{
				result = new Version(text);
			}
			catch
			{
				result = new Version();
			}
			return result;
		}

		private void OtherSectionConfigurationGUI(BuildTargetGroup targetGroup, ISettingEditorExtension settingsExtension)
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.configurationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
			IScriptingImplementations scriptingImplementations = ModuleManager.GetScriptingImplementations(targetGroup);
			if (scriptingImplementations == null)
			{
				PlayerSettingsEditor.BuildDisabledEnumPopup(PlayerSettingsEditor.Styles.scriptingDefault, PlayerSettingsEditor.Styles.scriptingBackend);
			}
			else
			{
				ScriptingImplementation[] array = scriptingImplementations.Enabled();
				ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
				ScriptingImplementation scriptingImplementation;
				if (targetGroup == BuildTargetGroup.tvOS)
				{
					scriptingImplementation = ScriptingImplementation.IL2CPP;
					PlayerSettingsEditor.BuildDisabledEnumPopup(PlayerSettingsEditor.Styles.scriptingIL2CPP, PlayerSettingsEditor.Styles.scriptingBackend);
				}
				else
				{
					scriptingImplementation = PlayerSettingsEditor.BuildEnumPopup<ScriptingImplementation>(PlayerSettingsEditor.Styles.scriptingBackend, scriptingBackend, array, PlayerSettingsEditor.GetNiceScriptingBackendNames(array));
				}
				if (scriptingImplementation != scriptingBackend)
				{
					PlayerSettings.SetScriptingBackend(targetGroup, scriptingImplementation);
				}
			}
			if (targetGroup == BuildTargetGroup.WiiU)
			{
				PlayerSettingsEditor.BuildDisabledEnumPopup(PlayerSettingsEditor.Styles.apiCompatibilityLevel_WiiUSubset, PlayerSettingsEditor.Styles.apiCompatibilityLevel);
			}
			else
			{
				ApiCompatibilityLevel apiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(targetGroup);
				ApiCompatibilityLevel[] availableApiCompatibilityLevels = this.GetAvailableApiCompatibilityLevels(targetGroup);
				ApiCompatibilityLevel apiCompatibilityLevel2 = PlayerSettingsEditor.BuildEnumPopup<ApiCompatibilityLevel>(PlayerSettingsEditor.Styles.apiCompatibilityLevel, apiCompatibilityLevel, availableApiCompatibilityLevels, PlayerSettingsEditor.GetNiceApiCompatibilityLevelNames(availableApiCompatibilityLevels));
				if (apiCompatibilityLevel != apiCompatibilityLevel2)
				{
					PlayerSettings.SetApiCompatibilityLevel(targetGroup, apiCompatibilityLevel2);
				}
			}
			bool flag = targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.WSA;
			if (flag)
			{
				if (targetGroup == BuildTargetGroup.iPhone)
				{
					EditorGUILayout.PropertyField(this.m_TargetDevice, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IPhoneSdkVersion, PlayerSettingsEditor.Styles.targetSdkVersion, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IPhoneTargetOSVersion, PlayerSettingsEditor.Styles.iPhoneTargetOSVersion, new GUILayoutOption[0]);
					Version version = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
					this.m_IPhoneTargetOSVersion.stringValue = ((version.Major != 0) ? version.ToString() : "7.0");
				}
				if (targetGroup == BuildTargetGroup.tvOS)
				{
					EditorGUILayout.PropertyField(this.m_tvOSSdkVersion, PlayerSettingsEditor.Styles.targetSdkVersion, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_tvOSTargetOSVersion, PlayerSettingsEditor.Styles.tvOSTargetOSVersion, new GUILayoutOption[0]);
					Version version2 = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
					this.m_IPhoneTargetOSVersion.stringValue = ((version2.Major != 0) ? version2.ToString() : "7.0");
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
				{
					EditorGUILayout.PropertyField(this.m_useOnDemandResources, PlayerSettingsEditor.Styles.useOnDemandResources, new GUILayoutOption[0]);
					Version version3 = this.ParseIosVersion(this.m_IPhoneTargetOSVersion.stringValue);
					this.m_IPhoneTargetOSVersion.stringValue = ((!this.m_useOnDemandResources.boolValue || version3.Major >= 9) ? version3.ToString() : "9.0");
				}
				bool flag2 = targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.WSA;
				if (flag2)
				{
					EditorGUILayout.PropertyField(this.m_AccelerometerFrequency, PlayerSettingsEditor.Styles.accelerometerFrequency, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
				{
					EditorGUILayout.PropertyField(this.m_CameraUsageDescription, PlayerSettingsEditor.Styles.cameraUsageDescription, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_LocationUsageDescription, PlayerSettingsEditor.Styles.locationUsageDescription, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_MicrophoneUsageDescription, PlayerSettingsEditor.Styles.microphoneUsageDescription, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android)
				{
					EditorGUILayout.PropertyField(this.m_MuteOtherAudioSources, PlayerSettingsEditor.Styles.muteOtherAudioSources, new GUILayoutOption[0]);
				}
				if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
				{
					if (targetGroup == BuildTargetGroup.iPhone)
					{
						EditorGUILayout.PropertyField(this.m_PrepareIOSForRecording, PlayerSettingsEditor.Styles.prepareIOSForRecording, new GUILayoutOption[0]);
					}
					EditorGUILayout.PropertyField(this.m_UIRequiresPersistentWiFi, PlayerSettingsEditor.Styles.UIRequiresPersistentWiFi, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IOSAllowHTTPDownload, PlayerSettingsEditor.Styles.iOSAllowHTTPDownload, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_IOSURLSchemes, PlayerSettingsEditor.Styles.iOSURLSchemes, true, new GUILayoutOption[0]);
				}
			}
			using (new EditorGUI.DisabledScope(!Application.HasProLicense()))
			{
				bool flag3 = !this.m_SubmitAnalytics.boolValue;
				bool flag4 = EditorGUILayout.Toggle(PlayerSettingsEditor.Styles.disableStatistics, flag3, new GUILayoutOption[0]);
				if (flag3 != flag4)
				{
					this.m_SubmitAnalytics.boolValue = !flag4;
				}
				if (!Application.HasProLicense())
				{
					this.m_SubmitAnalytics.boolValue = true;
				}
			}
			if (settingsExtension != null)
			{
				settingsExtension.ConfigurationSectionGUI();
			}
			EditorGUILayout.LabelField(PlayerSettingsEditor.Styles.scriptingDefineSymbols, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			string defines = EditorGUILayout.DelayedTextField(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup), EditorStyles.textField, new GUILayoutOption[0]);
			this.scriptingDefinesControlID = EditorGUIUtility.s_LastControlID;
			if (EditorGUI.EndChangeCheck())
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
			}
			EditorGUILayout.Space();
		}

		private void OtherSectionOptimizationGUI(BuildTargetGroup targetGroup)
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.optimizationTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BakeCollisionMeshes, PlayerSettingsEditor.Styles.bakeCollisionMeshes, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_KeepLoadedShadersAlive, PlayerSettingsEditor.Styles.keepLoadedShadersAlive, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_PreloadedAssets, PlayerSettingsEditor.Styles.preloadedAssets, true, new GUILayoutOption[0]);
			bool flag = targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.XboxOne || targetGroup == BuildTargetGroup.WiiU || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.PSP2;
			if (flag)
			{
				EditorGUILayout.PropertyField(this.m_AotOptions, PlayerSettingsEditor.Styles.aotOptions, new GUILayoutOption[0]);
			}
			bool flag2 = targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS || targetGroup == BuildTargetGroup.Android || targetGroup == BuildTargetGroup.Tizen || targetGroup == BuildTargetGroup.WebGL || targetGroup == BuildTargetGroup.WiiU || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PS4 || targetGroup == BuildTargetGroup.XboxOne || targetGroup == BuildTargetGroup.WSA;
			if (flag2)
			{
				ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(targetGroup);
				if (targetGroup == BuildTargetGroup.WebGL || scriptingBackend == ScriptingImplementation.IL2CPP)
				{
					EditorGUILayout.PropertyField(this.m_StripEngineCode, PlayerSettingsEditor.Styles.stripEngineCode, new GUILayoutOption[0]);
				}
				else if (scriptingBackend != ScriptingImplementation.WinRTDotNET)
				{
					EditorGUILayout.PropertyField(this.m_IPhoneStrippingLevel, PlayerSettingsEditor.Styles.iPhoneStrippingLevel, new GUILayoutOption[0]);
				}
			}
			if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
			{
				EditorGUILayout.PropertyField(this.m_IPhoneScriptCallOptimization, PlayerSettingsEditor.Styles.iPhoneScriptCallOptimization, new GUILayoutOption[0]);
			}
			if (targetGroup == BuildTargetGroup.Android)
			{
				EditorGUILayout.PropertyField(this.m_AndroidProfiler, PlayerSettingsEditor.Styles.enableInternalProfiler, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
			VertexChannelCompressionFlags vertexChannelCompressionFlags = (VertexChannelCompressionFlags)this.m_VertexChannelCompressionMask.intValue;
			vertexChannelCompressionFlags = (VertexChannelCompressionFlags)EditorGUILayout.EnumMaskPopup(PlayerSettingsEditor.Styles.vertexChannelCompressionMask, vertexChannelCompressionFlags, new GUILayoutOption[0]);
			this.m_VertexChannelCompressionMask.intValue = (int)vertexChannelCompressionFlags;
			if (targetGroup != BuildTargetGroup.PSM)
			{
				EditorGUILayout.PropertyField(this.m_StripUnusedMeshComponents, PlayerSettingsEditor.Styles.stripUnusedMeshComponents, new GUILayoutOption[0]);
			}
			if (targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.m_VideoMemoryForVertexBuffers, PlayerSettingsEditor.Styles.videoMemoryForVertexBuffers, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (this.m_VideoMemoryForVertexBuffers.intValue < 0)
					{
						this.m_VideoMemoryForVertexBuffers.intValue = 0;
					}
					else if (this.m_VideoMemoryForVertexBuffers.intValue > 192)
					{
						this.m_VideoMemoryForVertexBuffers.intValue = 192;
					}
				}
			}
			EditorGUILayout.Space();
		}

		private ApiCompatibilityLevel[] GetAvailableApiCompatibilityLevels(BuildTargetGroup activeBuildTargetGroup)
		{
			ApiCompatibilityLevel[] result;
			if (activeBuildTargetGroup == BuildTargetGroup.WSA || activeBuildTargetGroup == BuildTargetGroup.XboxOne)
			{
				result = PlayerSettingsEditor.allProfiles;
			}
			else
			{
				result = PlayerSettingsEditor.only_2_0_profiles;
			}
			return result;
		}

		private void OtherSectionLoggingGUI()
		{
			GUILayout.Label(PlayerSettingsEditor.Styles.loggingTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Log Type", new GUILayoutOption[0]);
			IEnumerator enumerator = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					StackTraceLogType stackTraceLogType = (StackTraceLogType)enumerator.Current;
					GUILayout.Label(stackTraceLogType.ToString(), new GUILayoutOption[]
					{
						GUILayout.Width(70f)
					});
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
			GUILayout.EndHorizontal();
			IEnumerator enumerator2 = Enum.GetValues(typeof(LogType)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					LogType logType = (LogType)enumerator2.Current;
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(logType.ToString(), new GUILayoutOption[0]);
					IEnumerator enumerator3 = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							StackTraceLogType stackTraceLogType2 = (StackTraceLogType)enumerator3.Current;
							StackTraceLogType stackTraceLogType3 = PlayerSettings.GetStackTraceLogType(logType);
							EditorGUI.BeginChangeCheck();
							bool flag = EditorGUILayout.ToggleLeft(" ", stackTraceLogType3 == stackTraceLogType2, new GUILayoutOption[]
							{
								GUILayout.Width(70f)
							});
							if (EditorGUI.EndChangeCheck() && flag)
							{
								PlayerSettings.SetStackTraceLogType(logType, stackTraceLogType2);
							}
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = (enumerator3 as IDisposable)) != null)
						{
							disposable2.Dispose();
						}
					}
					GUILayout.EndHorizontal();
				}
			}
			finally
			{
				IDisposable disposable3;
				if ((disposable3 = (enumerator2 as IDisposable)) != null)
				{
					disposable3.Dispose();
				}
			}
			GUILayout.EndVertical();
		}

		private static GUIContent[] GetGUIContentsForValues<T>(Dictionary<T, GUIContent> contents, T[] values)
		{
			GUIContent[] array = new GUIContent[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				if (!contents.ContainsKey(values[i]))
				{
					throw new NotImplementedException(string.Format("Missing name for {0}", values[i]));
				}
				array[i] = contents[values[i]];
			}
			return array;
		}

		private bool PlatformSupportsGfxJobModes(BuildTargetGroup targetGroup)
		{
			return targetGroup == BuildTargetGroup.PS4;
		}

		private static GUIContent[] GetNiceScriptingBackendNames(ScriptingImplementation[] scriptingBackends)
		{
			if (PlayerSettingsEditor.m_NiceScriptingBackendNames == null)
			{
				PlayerSettingsEditor.m_NiceScriptingBackendNames = new Dictionary<ScriptingImplementation, GUIContent>
				{
					{
						ScriptingImplementation.Mono2x,
						PlayerSettingsEditor.Styles.scriptingMono2x
					},
					{
						ScriptingImplementation.WinRTDotNET,
						PlayerSettingsEditor.Styles.scriptingWinRTDotNET
					},
					{
						ScriptingImplementation.IL2CPP,
						PlayerSettingsEditor.Styles.scriptingIL2CPP
					}
				};
			}
			return PlayerSettingsEditor.GetGUIContentsForValues<ScriptingImplementation>(PlayerSettingsEditor.m_NiceScriptingBackendNames, scriptingBackends);
		}

		private static GUIContent[] GetNiceApiCompatibilityLevelNames(ApiCompatibilityLevel[] apiCompatibilityLevels)
		{
			if (PlayerSettingsEditor.m_NiceApiCompatibilityLevelNames == null)
			{
				PlayerSettingsEditor.m_NiceApiCompatibilityLevelNames = new Dictionary<ApiCompatibilityLevel, GUIContent>
				{
					{
						ApiCompatibilityLevel.NET_2_0,
						PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_2_0
					},
					{
						ApiCompatibilityLevel.NET_2_0_Subset,
						PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_2_0_Subset
					},
					{
						ApiCompatibilityLevel.NET_4_6,
						PlayerSettingsEditor.Styles.apiCompatibilityLevel_NET_4_6
					}
				};
			}
			return PlayerSettingsEditor.GetGUIContentsForValues<ApiCompatibilityLevel>(PlayerSettingsEditor.m_NiceApiCompatibilityLevelNames, apiCompatibilityLevels);
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
				GUI.FocusControl("");
				string text = EditorGUIUtility.TempContent(browsePanelTitle).text;
				string text2 = (!string.IsNullOrEmpty(dir)) ? (dir.Replace('\\', '/') + "/") : (Directory.GetCurrentDirectory().Replace('\\', '/') + "/");
				string text3;
				if (string.IsNullOrEmpty(extension))
				{
					text3 = EditorUtility.OpenFolderPanel(text, text2, "");
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
			bool flag = string.IsNullOrEmpty(property.stringValue);
			using (new EditorGUI.DisabledScope(flag))
			{
				GUIContent gUIContent;
				if (flag)
				{
					gUIContent = EditorGUIUtility.TextContent("Not selected.");
				}
				else
				{
					gUIContent = EditorGUIUtility.TempContent(property.stringValue);
				}
				EditorGUI.BeginChangeCheck();
				GUILayoutOption[] options = new GUILayoutOption[]
				{
					GUILayout.Width(32f),
					GUILayout.ExpandWidth(true)
				};
				string value = EditorGUILayout.TextArea(gUIContent.text, options);
				if (EditorGUI.EndChangeCheck())
				{
					if (string.IsNullOrEmpty(value))
					{
						property.stringValue = "";
						base.serializedObject.ApplyModifiedProperties();
						GUI.FocusControl("");
						GUIUtility.ExitGUI();
					}
				}
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
			if (targetGroup == BuildTargetGroup.WSA || targetGroup == BuildTargetGroup.PSP2 || targetGroup == BuildTargetGroup.PSM || (settingsExtension != null && settingsExtension.HasPublishSection()))
			{
				if (this.BeginSettingsBox(5, PlayerSettingsEditor.Styles.publishingSettingsTitle))
				{
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
				}
				this.EndSettingsBox();
			}
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
