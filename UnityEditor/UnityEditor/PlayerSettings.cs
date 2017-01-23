using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class PlayerSettings : UnityEngine.Object
	{
		public sealed class PSM
		{
		}

		public sealed class Android
		{
			public static extern bool disableDepthAndStencilBuffers
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("This has been replaced by disableDepthAndStencilBuffers")]
			public static bool use24BitDepthBuffer
			{
				get
				{
					return !PlayerSettings.Android.disableDepthAndStencilBuffers;
				}
				set
				{
				}
			}

			public static extern int bundleVersionCode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSdkVersions minSdkVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidPreferredInstallLocation preferredInstallLocation
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceInternetPermission
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceSDCardPermission
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidTVCompatibility
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidIsGame
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool androidBannerEnabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern AndroidGamepadSupportLevel androidGamepadSupportLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool createWallpaper
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidTargetDevice targetDevice
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSplashScreenScale splashScreenScale
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoreName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystorePass
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasPass
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool licenseVerification
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool useAPKExpansionFiles
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern AndroidBanner[] GetAndroidBanners();

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D GetAndroidBannerForHeight(int height);

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetAndroidBanners(Texture2D[] banners);
		}

		public sealed class iOS
		{
			public static extern string applicationDisplayName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string buildNumber
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern ScriptCallOptimizationLevel scriptCallOptimization
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSSdkVersion sdkVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("targetOSVersion is obsolete, use targetOSVersionString")]
			public static extern iOSTargetOSVersion targetOSVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string targetOSVersionString
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSTargetDevice targetDevice
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool prerenderedIcon
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresPersistentWiFi
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresFullScreen
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSStatusBarStyle statusBarStyle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSAppInBackgroundBehavior appInBackgroundBehavior
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSBackgroundMode backgroundModes
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceHardShadowsOnMetal
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowHTTPDownload
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appleDeveloperTeamID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string cameraUsageDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string locationUsageDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string microphoneUsageDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useOnDemandResources
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("exitOnSuspend is deprecated, use appInBackgroundBehavior", false)]
			public static bool exitOnSuspend
			{
				get
				{
					return PlayerSettings.iOS.appInBackgroundBehavior == iOSAppInBackgroundBehavior.Exit;
				}
				set
				{
					PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Exit;
				}
			}

			[Obsolete("Use Screen.SetResolution at runtime", true)]
			public static iOSTargetResolution targetResolution
			{
				get
				{
					return iOSTargetResolution.Native;
				}
				set
				{
				}
			}

			[Obsolete("Use PlayerSettings.muteOtherAudioSources instead (UnityUpgradable) -> UnityEditor.PlayerSettings.muteOtherAudioSources", false)]
			public static bool overrideIPodMusic
			{
				get
				{
					return PlayerSettings.muteOtherAudioSources;
				}
				set
				{
					PlayerSettings.muteOtherAudioSources = value;
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetAssetBundleVariantsWithDeviceRequirements();

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool CheckAssetBundleVariantHasDeviceRequirements(string name);

			internal static iOSDeviceRequirementGroup GetDeviceRequirementsForAssetBundleVariant(string name)
			{
				iOSDeviceRequirementGroup result;
				if (!PlayerSettings.iOS.CheckAssetBundleVariantHasDeviceRequirements(name))
				{
					result = null;
				}
				else
				{
					result = new iOSDeviceRequirementGroup(name);
				}
				return result;
			}

			internal static void RemoveDeviceRequirementsForAssetBundleVariant(string name)
			{
				iOSDeviceRequirementGroup deviceRequirementsForAssetBundleVariant = PlayerSettings.iOS.GetDeviceRequirementsForAssetBundleVariant(name);
				for (int i = 0; i < deviceRequirementsForAssetBundleVariant.count; i++)
				{
					deviceRequirementsForAssetBundleVariant.RemoveAt(0);
				}
			}

			internal static iOSDeviceRequirementGroup AddDeviceRequirementsForAssetBundleVariant(string name)
			{
				return new iOSDeviceRequirementGroup(name);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetURLSchemes();
		}

		public sealed class N3DS
		{
			public enum TargetPlatform
			{
				Nintendo3DS = 1,
				NewNintendo3DS
			}

			public enum Region
			{
				Japan = 1,
				America,
				Europe,
				China,
				Korea,
				Taiwan,
				All
			}

			public enum MediaSize
			{
				_128MB,
				_256MB,
				_512MB,
				_1GB,
				_2GB
			}

			public enum LogoStyle
			{
				Nintendo,
				Distributed,
				iQue,
				Licensed
			}

			public static extern bool disableDepthAndStencilBuffers
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableStereoscopicView
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableSharedListOpt
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableVSync
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useExtSaveData
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool compressStaticMem
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string extSaveDataNumber
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int stackSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.TargetPlatform targetPlatform
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.Region region
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.MediaSize mediaSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.LogoStyle logoStyle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string title
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productCode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string applicationId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class PS4
		{
			public enum PS4AppCategory
			{
				Application,
				Patch
			}

			public enum PS4RemotePlayKeyAssignment
			{
				None = -1,
				PatternA,
				PatternB,
				PatternC,
				PatternD,
				PatternE,
				PatternF,
				PatternG,
				PatternH
			}

			public enum PS4EnterButtonAssignment
			{
				CircleButton,
				CrossButton
			}

			public static extern string npTrophyPackPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleSecret
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter1
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter2
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter3
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter4
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string passcode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string monoEnv
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool playerPrefsSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool restrictedAudioUsageRights
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useResolutionFallback
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4AppCategory category
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int appType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4RemotePlayKeyAssignment remotePlayKeyAssignment
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string remotePlayKeyMappingDir
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int playTogetherPlayerCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4EnterButtonAssignment enterButtonAssignment
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutPixelFormat
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutInitialWidth
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutReprojectionRate
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("videoOutResolution is deprecated. Use PlayerSettings.PS4.videoOutInitialWidth and PlayerSettings.PS4.videoOutReprojectionRate to control initial display resolution and reprojection rate.")]
			public static extern int videoOutResolution
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationXMLPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationSIGPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BackgroundImagePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string StartupImagePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SaveDataImagePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SdkOverride
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BGMPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareFilePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareOverlayImagePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PrivacyGuardImagePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool patchDayOne
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchPkgPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchLatestPkgPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchChangeinfoPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string NPtitleDatPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnSessions
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnPresence
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnFriends
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnGameCustomData
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int downloadDataSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int garlicHeapSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool reprojectionSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useAudio3dBackend
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int audio3dVirtualSpeakerCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int scriptOptimizationLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int socialScreenEnabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribUserManagement
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribMoveSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attrib3DSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribShareSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribExclusiveVR
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableAutoHideSplash
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int attribCpuUsage
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] includedModules
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class PSVita
		{
			public enum PSVitaPowerMode
			{
				ModeA,
				ModeB,
				ModeC
			}

			public enum PSVitaTvBootMode
			{
				Default,
				PSVitaBootablePSVitaTvBootable,
				PSVitaBootablePSVitaTvNotBootable
			}

			public enum PSVitaEnterButtonAssignment
			{
				Default,
				CircleButton,
				CrossButton
			}

			public enum PSVitaAppCategory
			{
				Application,
				ApplicationPatch
			}

			public enum PSVitaMemoryExpansionMode
			{
				None,
				ExpandBy29MB,
				ExpandBy77MB,
				ExpandBy109MB
			}

			public enum PSVitaDRMType
			{
				PaidFor,
				Free
			}

			public static extern string npTrophyPackPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaPowerMode powerMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool acquireBGM
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool npSupportGBMorGJP
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaTvBootMode tvBootMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool tvDisableEmu
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool upgradable
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool healthWarning
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("useLibLocation has no effect as of SDK 3.570")]
			public static extern bool useLibLocation
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarOnStartup
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarColor
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaEnterButtonAssignment enterButtonAssignment
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int saveDataQuota
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string shortTitle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaAppCategory category
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("AllowTwitterDialog has no effect as of SDK 3.570")]
			public static extern bool AllowTwitterDialog
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleDatPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommunicationsID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsPassphrase
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsSig
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string manualPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaGatePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaBackroundPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaTrialPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchChangeInfoPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchOriginalPackage
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packagePassword
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoneFile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaMemoryExpansionMode memoryExpansionMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaDRMType drmType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int storageType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mediaCapacity
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class SamsungTV
		{
			public enum SamsungTVProductCategories
			{
				Games,
				Videos,
				Sports,
				Lifestyle,
				Information,
				Education,
				Kids
			}

			public static extern string deviceAddress
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthor
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthorEmail
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productLink
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SamsungTV.SamsungTVProductCategories productCategory
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public struct SplashScreenLogo
		{
			private const float k_MinLogoTime = 2f;

			private static Sprite s_UnityLogo;

			private Sprite m_Logo;

			private float m_Duration;

			public Sprite logo
			{
				get
				{
					return this.m_Logo;
				}
				set
				{
					this.m_Logo = value;
				}
			}

			public static Sprite unityLogo
			{
				get
				{
					return PlayerSettings.SplashScreenLogo.s_UnityLogo;
				}
			}

			public float duration
			{
				get
				{
					return Mathf.Max(this.m_Duration, 2f);
				}
				set
				{
					this.m_Duration = Mathf.Max(value, 2f);
				}
			}

			static SplashScreenLogo()
			{
				PlayerSettings.SplashScreenLogo.s_UnityLogo = Resources.GetBuiltinResource<Sprite>("UnitySplash-cube.png");
			}

			[ExcludeFromDocs]
			public static PlayerSettings.SplashScreenLogo Create(float duration)
			{
				Sprite logo = null;
				return PlayerSettings.SplashScreenLogo.Create(duration, logo);
			}

			[ExcludeFromDocs]
			public static PlayerSettings.SplashScreenLogo Create()
			{
				Sprite logo = null;
				float duration = 2f;
				return PlayerSettings.SplashScreenLogo.Create(duration, logo);
			}

			public static PlayerSettings.SplashScreenLogo Create([UnityEngine.Internal.DefaultValue("k_MinLogoTime")] float duration, [UnityEngine.Internal.DefaultValue("null")] Sprite logo)
			{
				return new PlayerSettings.SplashScreenLogo
				{
					m_Duration = duration,
					m_Logo = logo
				};
			}

			[ExcludeFromDocs]
			public static PlayerSettings.SplashScreenLogo CreateWithUnityLogo()
			{
				float duration = 2f;
				return PlayerSettings.SplashScreenLogo.CreateWithUnityLogo(duration);
			}

			public static PlayerSettings.SplashScreenLogo CreateWithUnityLogo([UnityEngine.Internal.DefaultValue("k_MinLogoTime")] float duration)
			{
				return new PlayerSettings.SplashScreenLogo
				{
					m_Duration = duration,
					m_Logo = PlayerSettings.SplashScreenLogo.s_UnityLogo
				};
			}
		}

		public sealed class SplashScreen
		{
			public enum AnimationMode
			{
				Static,
				Dolly,
				Custom
			}

			public enum DrawMode
			{
				UnityLogoBelow,
				AllSequential
			}

			public enum UnityLogoStyle
			{
				DarkOnLight,
				LightOnDark
			}

			public static extern PlayerSettings.SplashScreen.AnimationMode animationMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float animationBackgroundZoom
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float animationLogoZoom
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Sprite background
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Sprite backgroundPortrait
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static Color backgroundColor
			{
				get
				{
					Color result;
					PlayerSettings.SplashScreen.INTERNAL_get_backgroundColor(out result);
					return result;
				}
				set
				{
					PlayerSettings.SplashScreen.INTERNAL_set_backgroundColor(ref value);
				}
			}

			public static extern PlayerSettings.SplashScreen.DrawMode drawMode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SplashScreenLogo[] logos
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float overlayOpacity
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool show
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool showUnityLogo
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SplashScreen.UnityLogoStyle unityLogoStyle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_backgroundColor(out Color value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_backgroundColor(ref Color value);
		}

		public enum TizenCapability
		{
			Location,
			DataSharing,
			NetworkGet,
			WifiDirect,
			CallHistoryRead,
			Power,
			ContactWrite,
			MessageWrite,
			ContentWrite,
			Push,
			AccountRead,
			ExternalStorage,
			Recorder,
			PackageManagerInfo,
			NFCCardEmulation,
			CalendarWrite,
			WindowPrioritySet,
			VolumeSet,
			CallHistoryWrite,
			AlarmSet,
			Call,
			Email,
			ContactRead,
			Shortcut,
			KeyManager,
			LED,
			NetworkProfile,
			AlarmGet,
			Display,
			CalendarRead,
			NFC,
			AccountWrite,
			Bluetooth,
			Notification,
			NetworkSet,
			ExternalStorageAppData,
			Download,
			Telephony,
			MessageRead,
			MediaStorage,
			Internet,
			Camera,
			Haptic,
			AppManagerLaunch,
			SystemSettings
		}

		public sealed class Tizen
		{
			public static extern string productDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productURL
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string signingProfileName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string deploymentTarget
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int deploymentTargetType
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern TizenOSVersion minOSVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern TizenShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static void SetCapability(PlayerSettings.TizenCapability capability, bool value)
			{
				PlayerSettings.Tizen.InternalSetCapability(capability.ToString(), value.ToString());
			}

			public static bool GetCapability(PlayerSettings.TizenCapability capability)
			{
				string text = PlayerSettings.Tizen.InternalGetCapability(capability.ToString());
				bool result;
				if (string.IsNullOrEmpty(text))
				{
					result = false;
				}
				else
				{
					try
					{
						result = (bool)TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(text);
					}
					catch
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Failed to parse value  ('",
							capability.ToString(),
							",",
							text,
							"') to bool type."
						}));
						result = false;
					}
				}
				return result;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);
		}

		public sealed class tvOS
		{
			public static extern tvOSSdkVersion sdkVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern tvOSTargetOSVersion targetOSVersion
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string targetOSVersionString
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requireExtendedGameController
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetSmallIconLayers();

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetLargeIconLayers();

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetTopShelfImageLayers();

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetTopShelfImageWideLayers();

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetSmallIconLayers(Texture2D[] layers);

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetLargeIconLayers(Texture2D[] layers);

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetTopShelfImageLayers(Texture2D[] layers);

			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetTopShelfImageWideLayers(Texture2D[] layers);
		}

		public sealed class WebGL
		{
			public static extern int memorySize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WebGLExceptionSupport exceptionSupport
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool dataCaching
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string emscriptenArgs
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string modulesDirectory
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string template
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool analyzeBuildSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useEmbeddedResources
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useWasm
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WebGLCompressionFormat compressionFormat
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool debugSymbols
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class WiiU
		{
			public static extern string titleID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string groupID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonSaveSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountSaveSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string olvAccessKey
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tinCode
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameModeMask
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonBossSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountBossSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] addOnUniqueIDs
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsNunchuk
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsClassicController
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsBalanceBoard
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsMotionPlus
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsProController
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowScreenCapture
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int controllerCount
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mainThreadStackSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int loaderThreadStackSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int systemHeapSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WiiUTVResolution tvResolution
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Texture2D tvStartupScreen
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern Texture2D gamePadStartupScreen
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int gamePadMSAA
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string profilerLibraryPath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool drcBufferDisabled
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public enum WSAApplicationShowName
		{
			NotSet,
			AllLogos,
			NoLogos,
			StandardLogoOnly,
			WideLogoOnly
		}

		public enum WSADefaultTileSize
		{
			NotSet,
			Medium,
			Wide
		}

		public enum WSAApplicationForegroundText
		{
			Light = 1,
			Dark
		}

		public enum WSACompilationOverrides
		{
			None,
			UseNetCore,
			UseNetCorePartially
		}

		public enum WSACapability
		{
			EnterpriseAuthentication,
			InternetClient,
			InternetClientServer,
			MusicLibrary,
			PicturesLibrary,
			PrivateNetworkClientServer,
			RemovableStorage,
			SharedUserCertificates,
			VideosLibrary,
			WebCam,
			Proximity,
			Microphone,
			Location,
			HumanInterfaceDevice,
			AllJoyn,
			BlockedChatMessages,
			Chat,
			CodeGeneration,
			Objects3D,
			PhoneCall,
			UserAccountInformation,
			VoipCall,
			Bluetooth,
			SpatialPerception,
			InputInjectionBrokered
		}

		public enum WSAImageScale
		{
			_80 = 80,
			_100 = 100,
			_125 = 125,
			_140 = 140,
			_150 = 150,
			_180 = 180,
			_200 = 200,
			_240 = 240,
			_400 = 400,
			Target16 = 16,
			Target24 = 24,
			Target32 = 32,
			Target48 = 48,
			Target256 = 256
		}

		public enum WSAImageType
		{
			PackageLogo = 1,
			SplashScreenImage,
			StoreTileLogo = 11,
			StoreTileWideLogo,
			StoreTileSmallLogo,
			StoreSmallTile,
			StoreLargeTile,
			PhoneAppIcon = 21,
			PhoneSmallTile,
			PhoneMediumTile,
			PhoneWideTile,
			PhoneSplashScreen,
			UWPSquare44x44Logo = 31,
			UWPSquare71x71Logo,
			UWPSquare150x150Logo,
			UWPSquare310x310Logo,
			UWPWide310x150Logo
		}

		public enum WSAInputSource
		{
			CoreWindow,
			IndependentInputSource,
			SwapChainPanel
		}

		[RequiredByNativeCode]
		public struct WSASupportedFileType
		{
			public string contentType;

			public string fileType;
		}

		[RequiredByNativeCode]
		public struct WSAFileTypeAssociations
		{
			public string name;

			public PlayerSettings.WSASupportedFileType[] supportedFileTypes;
		}

		public sealed class WSA
		{
			public static class Declarations
			{
				public static string protocolName
				{
					get
					{
						return PlayerSettings.WSA.internalProtocolName;
					}
					set
					{
						PlayerSettings.WSA.internalProtocolName = value;
					}
				}

				public static PlayerSettings.WSAFileTypeAssociations fileTypeAssociations
				{
					get
					{
						return PlayerSettings.WSA.internalFileTypeAssociations;
					}
					set
					{
						PlayerSettings.WSA.internalFileTypeAssociations = value;
					}
				}
			}

			public static extern string packageName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packageLogo
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern string packageVersionRaw
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string commandLineArgsFile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string certificatePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			internal static extern string certificatePassword
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateSubject
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateIssuer
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			private static extern long certificateNotAfterRaw
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string applicationDescription
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo80
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo80
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo80
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile80
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile80
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImage
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale180
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImage
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale140
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale240
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tileShortName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationShowName tileShowName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool mediumTileShowName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool largeTileShowName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool wideTileShowName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSADefaultTileSize defaultTileSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSACompilationOverrides compilationOverrides
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationForegroundText tileForegroundText
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static Color tileBackgroundColor
			{
				get
				{
					Color result;
					PlayerSettings.WSA.INTERNAL_get_tileBackgroundColor(out result);
					return result;
				}
				set
				{
					PlayerSettings.WSA.INTERNAL_set_tileBackgroundColor(ref value);
				}
			}

			[Obsolete("PlayerSettings.WSA.enableIndependentInputSource is deprecated. Use PlayerSettings.WSA.inputSource.", false)]
			public static bool enableIndependentInputSource
			{
				get
				{
					return PlayerSettings.WSA.inputSource == PlayerSettings.WSAInputSource.IndependentInputSource;
				}
				set
				{
					PlayerSettings.WSA.inputSource = ((!value) ? PlayerSettings.WSAInputSource.CoreWindow : PlayerSettings.WSAInputSource.IndependentInputSource);
				}
			}

			public static extern PlayerSettings.WSAInputSource inputSource
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("PlayerSettings.enableLowLatencyPresentationAPI is deprecated. It is now always enabled.", false)]
			public static extern bool enableLowLatencyPresentationAPI
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern bool splashScreenUseBackgroundColor
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static Color splashScreenBackgroundColorRaw
			{
				get
				{
					Color result;
					PlayerSettings.WSA.INTERNAL_get_splashScreenBackgroundColorRaw(out result);
					return result;
				}
				set
				{
					PlayerSettings.WSA.INTERNAL_set_splashScreenBackgroundColorRaw(ref value);
				}
			}

			internal static extern string internalProtocolName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static PlayerSettings.WSAFileTypeAssociations internalFileTypeAssociations
			{
				get
				{
					PlayerSettings.WSAFileTypeAssociations result;
					PlayerSettings.WSA.INTERNAL_get_internalFileTypeAssociations(out result);
					return result;
				}
				set
				{
					PlayerSettings.WSA.INTERNAL_set_internalFileTypeAssociations(ref value);
				}
			}

			public static Version packageVersion
			{
				get
				{
					Version result;
					try
					{
						result = new Version(PlayerSettings.WSA.ValidatePackageVersion(PlayerSettings.WSA.packageVersionRaw));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("{0}, the raw string was {1}", ex.Message, PlayerSettings.WSA.packageVersionRaw));
					}
					return result;
				}
				set
				{
					PlayerSettings.WSA.packageVersionRaw = value.ToString();
				}
			}

			public static DateTime? certificateNotAfter
			{
				get
				{
					long certificateNotAfterRaw = PlayerSettings.WSA.certificateNotAfterRaw;
					DateTime? result;
					if (certificateNotAfterRaw != 0L)
					{
						result = new DateTime?(DateTime.FromFileTime(certificateNotAfterRaw));
					}
					else
					{
						result = null;
					}
					return result;
				}
			}

			public static Color? splashScreenBackgroundColor
			{
				get
				{
					Color? result;
					if (PlayerSettings.WSA.splashScreenUseBackgroundColor)
					{
						result = new Color?(PlayerSettings.WSA.splashScreenBackgroundColorRaw);
					}
					else
					{
						result = null;
					}
					return result;
				}
				set
				{
					PlayerSettings.WSA.splashScreenUseBackgroundColor = value.HasValue;
					if (value.HasValue)
					{
						PlayerSettings.WSA.splashScreenBackgroundColorRaw = value.Value;
					}
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetWSAImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWSAImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool SetCertificate(string path, string password);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_tileBackgroundColor(out Color value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_tileBackgroundColor(ref Color value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_splashScreenBackgroundColorRaw(out Color value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_splashScreenBackgroundColorRaw(ref Color value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_internalFileTypeAssociations(out PlayerSettings.WSAFileTypeAssociations value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_internalFileTypeAssociations(ref PlayerSettings.WSAFileTypeAssociations value);

			internal static string ValidatePackageVersion(string value)
			{
				Regex regex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				string result;
				if (regex.IsMatch(value))
				{
					result = value;
				}
				else
				{
					result = "1.0.0.0";
				}
				return result;
			}

			private static void ValidateWSAImageType(PlayerSettings.WSAImageType type)
			{
				switch (type)
				{
				case PlayerSettings.WSAImageType.StoreTileLogo:
				case PlayerSettings.WSAImageType.StoreTileWideLogo:
				case PlayerSettings.WSAImageType.StoreTileSmallLogo:
				case PlayerSettings.WSAImageType.StoreSmallTile:
				case PlayerSettings.WSAImageType.StoreLargeTile:
				case PlayerSettings.WSAImageType.PhoneAppIcon:
				case PlayerSettings.WSAImageType.PhoneSmallTile:
				case PlayerSettings.WSAImageType.PhoneMediumTile:
				case PlayerSettings.WSAImageType.PhoneWideTile:
				case PlayerSettings.WSAImageType.PhoneSplashScreen:
				case PlayerSettings.WSAImageType.UWPSquare44x44Logo:
				case PlayerSettings.WSAImageType.UWPSquare71x71Logo:
				case PlayerSettings.WSAImageType.UWPSquare150x150Logo:
				case PlayerSettings.WSAImageType.UWPSquare310x310Logo:
				case PlayerSettings.WSAImageType.UWPWide310x150Logo:
					return;
				case (PlayerSettings.WSAImageType)16:
				case (PlayerSettings.WSAImageType)17:
				case (PlayerSettings.WSAImageType)18:
				case (PlayerSettings.WSAImageType)19:
				case (PlayerSettings.WSAImageType)20:
				case (PlayerSettings.WSAImageType)26:
				case (PlayerSettings.WSAImageType)27:
				case (PlayerSettings.WSAImageType)28:
				case (PlayerSettings.WSAImageType)29:
				case (PlayerSettings.WSAImageType)30:
					IL_6E:
					if (type != PlayerSettings.WSAImageType.PackageLogo && type != PlayerSettings.WSAImageType.SplashScreenImage)
					{
						throw new Exception("Unknown WSA image type: " + type);
					}
					return;
				}
				goto IL_6E;
			}

			private static void ValidateWSAImageScale(PlayerSettings.WSAImageScale scale)
			{
				if (scale != PlayerSettings.WSAImageScale.Target16 && scale != PlayerSettings.WSAImageScale.Target24 && scale != PlayerSettings.WSAImageScale.Target32 && scale != PlayerSettings.WSAImageScale.Target48 && scale != PlayerSettings.WSAImageScale._80 && scale != PlayerSettings.WSAImageScale._100 && scale != PlayerSettings.WSAImageScale._125 && scale != PlayerSettings.WSAImageScale._140 && scale != PlayerSettings.WSAImageScale._150 && scale != PlayerSettings.WSAImageScale._180 && scale != PlayerSettings.WSAImageScale._200 && scale != PlayerSettings.WSAImageScale._240 && scale != PlayerSettings.WSAImageScale.Target256 && scale != PlayerSettings.WSAImageScale._400)
				{
					throw new Exception("Unknown image scale: " + scale);
				}
			}

			public static string GetVisualAssetsImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
			{
				PlayerSettings.WSA.ValidateWSAImageType(type);
				PlayerSettings.WSA.ValidateWSAImageScale(scale);
				return PlayerSettings.WSA.GetWSAImage(type, scale);
			}

			public static void SetVisualAssetsImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale)
			{
				PlayerSettings.WSA.ValidateWSAImageType(type);
				PlayerSettings.WSA.ValidateWSAImageScale(scale);
				PlayerSettings.WSA.SetWSAImage(image, type, scale);
			}

			public static void SetCapability(PlayerSettings.WSACapability capability, bool value)
			{
				PlayerSettings.WSA.InternalSetCapability(capability.ToString(), value.ToString());
			}

			public static bool GetCapability(PlayerSettings.WSACapability capability)
			{
				string text = PlayerSettings.WSA.InternalGetCapability(capability.ToString());
				bool result;
				if (string.IsNullOrEmpty(text))
				{
					result = false;
				}
				else
				{
					try
					{
						result = (bool)TypeDescriptor.GetConverter(typeof(bool)).ConvertFromString(text);
					}
					catch
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Failed to parse value  ('",
							capability.ToString(),
							",",
							text,
							"') to bool type."
						}));
						result = false;
					}
				}
				return result;
			}
		}

		public sealed class XboxOne
		{
			public static extern XboxOneLoggingLevel defaultLoggingLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ProductId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string UpdateKey
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SandboxId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ContentId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string TitleId
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SCID
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnableVariableGPU
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool DisableKinectGpuReservation
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnablePIXSampling
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string GameOsOverridePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PackagingOverridePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOneEncryptionLevel PackagingEncryption
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOnePackageUpdateGranularity PackageUpdateGranularity
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string AppManifestOverridePath
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool IsContentPackage
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Version
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Description
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] SocketNames
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string[] AllowedProductIds
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern uint PersistentLocalStorageSize
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int monoLoggingLevel
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetCapability(string capability, bool value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool GetCapability(string capability);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSupportedLanguage(string language, bool enabled);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool GetSupportedLanguage(string language);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveSocketDefinition(string name);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSocketDefinition(string name, string port, int protocol, int[] usages, string templateName, int sessionRequirment, int[] deviceUsages);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void GetSocketDefinition(string name, out string port, out int protocol, out int[] usages, out string templateName, out int sessionRequirment, out int[] deviceUsages);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveAllowedProductId(string id);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool AddAllowedProductId(string id);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void UpdateAllowedProductId(int idx, string id);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGameRating(string name, int value);

			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern int GetGameRating(string name);
		}

		private static SerializedObject _serializedObject;

		internal static readonly char[] defineSplits = new char[]
		{
			';',
			',',
			' '
		};

		public static extern string companyName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string productName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use PlayerSettings.SplashScreen.show instead")]
		public static extern bool showUnitySplashScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use PlayerSettings.SplashScreen.unityLogoStyle instead")]
		public static extern SplashScreenStyle splashScreenStyle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static string cloudProjectId
		{
			get
			{
				return PlayerSettings.cloudProjectIdRaw;
			}
			internal set
			{
				PlayerSettings.cloudProjectIdRaw = value;
			}
		}

		private static extern string cloudProjectIdRaw
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Guid productGUID
		{
			get
			{
				return new Guid(PlayerSettings.productGUIDRaw);
			}
		}

		private static extern byte[] productGUIDRaw
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ColorSpace colorSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ResolutionDialogSetting displayResolutionDialog
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsFullScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsNativeResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool runInBackground
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool captureSingleScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool usePlayerLog
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool resizableWindow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool bakeCollisionMeshes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useMacAppStoreValidation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern MacFullscreenMode macFullscreenMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D9FullscreenMode d3d9FullscreenMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D11FullscreenMode d3d11FullscreenMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool virtualRealitySupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("singlePassStereoRendering will be deprecated. Use stereoRenderingPath instead.")]
		public static extern bool singlePassStereoRendering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern StereoRenderingPath stereoRenderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool protectGraphicsMemory
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool visibleInBackground
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowFullscreenSwitch
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool forceSingleInstance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool openGLRequireES31
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool openGLRequireES31AEP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D resolutionDialogBanner
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D virtualRealitySplashScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string iPhoneBundleIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string webPlayerTemplate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string[] templateCustomKeys
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string spritePackerPolicy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keystorePass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keyaliasPass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string xboxTitleId
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string xboxImageXexFilePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string xboxSpaFilePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxGenerateSpa
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableGuest
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxDeployKinectResources
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxDeployKinectHeadOrientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxDeployKinectHeadPosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D xboxSplashScreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int xboxAdditionalTitleMemorySize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxEnableKinect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableKinectAutoTracking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableSpeech
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern uint xboxSpeechDB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gpuSkinning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool graphicsJobs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxPIXTextureCapture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableAvatar
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int xboxOneResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool enableInternalProfiler
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ActionOnDotNetUnhandledException actionOnDotNetUnhandledException
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool logObjCUncaughtExceptions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableCrashReportAPI
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string bundleIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string bundleVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool statusBarHidden
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern StrippingLevel strippingLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripEngineCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UIOrientation defaultInterfaceOrientation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortrait
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortraitUpsideDown
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeRight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeLeft
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useAnimatedAutorotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool use32BitDisplayBuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ApiCompatibilityLevel apiCompatibilityLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripUnusedMeshComponents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool advancedLicense
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string aotOptions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D defaultCursor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Vector2 cursorHotspot
		{
			get
			{
				Vector2 result;
				PlayerSettings.INTERNAL_get_cursorHotspot(out result);
				return result;
			}
			set
			{
				PlayerSettings.INTERNAL_set_cursorHotspot(ref value);
			}
		}

		public static extern int accelerometerFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool MTRendering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool mobileMTRendering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use UnityEditor.PlayerSettings.SetGraphicsAPIs/GetGraphicsAPIs instead")]
		public static extern bool useDirect3D11
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool submitAnalytics
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use VREditor.GetStereoDeviceEnabled instead")]
		public static extern bool stereoscopic3D
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool muteOtherAudioSources
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("The option alwaysDisplayWatermark is deprecated and is always false", true)]
		public static bool alwaysDisplayWatermark
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("Use AssetBundles instead for streaming data", true)]
		public static int firstStreamedLevelWithResources
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("targetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
		public static TargetGlesGraphics targetGlesGraphics
		{
			get
			{
				return TargetGlesGraphics.Automatic;
			}
			set
			{
			}
		}

		[Obsolete("targetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs APIs", false)]
		public static TargetIOSGraphics targetIOSGraphics
		{
			get
			{
				return TargetIOSGraphics.Automatic;
			}
			set
			{
			}
		}

		[Obsolete("Use PlayerSettings.iOS.locationUsageDescription instead (UnityUpgradable) -> UnityEditor.PlayerSettings/iOS.locationUsageDescription", false)]
		public static string locationUsageDescription
		{
			get
			{
				return PlayerSettings.iOS.locationUsageDescription;
			}
			set
			{
				PlayerSettings.iOS.locationUsageDescription = value;
			}
		}

		[Obsolete("renderingPath is ignored, use UnityEditor.Rendering.TierSettings with UnityEditor.Rendering.SetTierSettings/GetTierSettings instead", false)]
		public static RenderingPath renderingPath
		{
			get
			{
				return EditorGraphicsSettings.GetCurrentTierSettings().renderingPath;
			}
			set
			{
			}
		}

		[Obsolete("mobileRenderingPath is ignored, use UnityEditor.Rendering.TierSettings with UnityEditor.Rendering.SetTierSettings/GetTierSettings instead", false)]
		public static RenderingPath mobileRenderingPath
		{
			get
			{
				return EditorGraphicsSettings.GetCurrentTierSettings().renderingPath;
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object InternalGetPlayerSettingsObject();

		internal static SerializedObject GetSerializedObject()
		{
			if (PlayerSettings._serializedObject == null)
			{
				PlayerSettings._serializedObject = new SerializedObject(PlayerSettings.InternalGetPlayerSettingsObject());
			}
			return PlayerSettings._serializedObject;
		}

		internal static SerializedProperty FindProperty(string name)
		{
			SerializedProperty serializedProperty = PlayerSettings.GetSerializedObject().FindProperty(name);
			if (serializedProperty == null)
			{
				Debug.LogError("Failed to find:" + name);
			}
			return serializedProperty;
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPropertyInt(string name, int value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static void SetPropertyInt(string name, int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyInt(name, value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static void SetPropertyInt(string name, int value, BuildTarget target)
		{
			PlayerSettings.SetPropertyInt(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPropertyInt(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static int GetPropertyInt(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyInt(name, target);
		}

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static bool GetPropertyOptionalInt(string name, ref int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalInt(name, ref value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static bool GetPropertyOptionalInt(string name, ref int value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			value = PlayerSettings.GetPropertyInt(name, target);
			return true;
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPropertyBool(string name, bool value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static void SetPropertyBool(string name, bool value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyBool(name, value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static void SetPropertyBool(string name, bool value, BuildTarget target)
		{
			PlayerSettings.SetPropertyBool(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetPropertyBool(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static bool GetPropertyBool(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyBool(name, target);
		}

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static bool GetPropertyOptionalBool(string name, ref bool value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalBool(name, ref value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static bool GetPropertyOptionalBool(string name, ref bool value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			value = PlayerSettings.GetPropertyBool(name, target);
			return true;
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPropertyString(string name, string value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static void SetPropertyString(string name, string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyString(name, value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static void SetPropertyString(string name, string value, BuildTarget target)
		{
			PlayerSettings.SetPropertyString(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[Obsolete("Use explicit API instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetPropertyString(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static string GetPropertyString(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyString(name, target);
		}

		[Obsolete("Use explicit API instead."), ExcludeFromDocs]
		public static bool GetPropertyOptionalString(string name, ref string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalString(name, ref value, target);
		}

		[Obsolete("Use explicit API instead.")]
		public static bool GetPropertyOptionalString(string name, ref string value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			value = PlayerSettings.GetPropertyString(name, target);
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCloudServiceEnabled(string serviceKey, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetCloudServiceEnabled(string serviceKey);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAspectRatio(AspectRatio aspectRatio);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAspectRatio(AspectRatio aspectRatio, bool enable);

		public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform)
		{
			Texture2D[] iconsForPlatform = PlayerSettings.GetIconsForPlatform(PlayerSettings.GetPlatformName(platform));
			Texture2D[] result;
			if (iconsForPlatform.Length == 0)
			{
				result = new Texture2D[PlayerSettings.GetIconSizesForTargetGroup(platform).Length];
			}
			else
			{
				result = iconsForPlatform;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D[] GetIconsForPlatform(string platform);

		public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons)
		{
			PlayerSettings.SetIconsForPlatform(PlayerSettings.GetPlatformName(platform), icons);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconsForPlatform(string platform, Texture2D[] icons);

		public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform)
		{
			return PlayerSettings.GetIconWidthsForPlatform(PlayerSettings.GetPlatformName(platform));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconWidthsForPlatform(string platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconHeightsForPlatform(string platform);

		internal static string GetPlatformName(BuildTargetGroup targetGroup)
		{
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.GetValidPlatforms().Find((BuildPlayerWindow.BuildPlatform p) => p.targetGroup == targetGroup);
			return (buildPlatform != null) ? buildPlatform.name : string.Empty;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForPlatformAtSize(string platform, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GraphicsDeviceType[] GetSupportedGraphicsAPIs(BuildTarget platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsDeviceType[] GetGraphicsAPIs(BuildTarget platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGraphicsAPIs(BuildTarget platform, GraphicsDeviceType[] apis);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetUseDefaultGraphicsAPIs(BuildTarget platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTemplateCustomValue(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetTemplateCustomValue(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup);

		public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines)
		{
			if (!string.IsNullOrEmpty(defines))
			{
				defines = string.Join(";", defines.Split(PlayerSettings.defineSplits, StringSplitOptions.RemoveEmptyEntries));
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroupInternal(targetGroup, defines);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScriptingDefineSymbolsForGroupInternal(BuildTargetGroup targetGroup, string defines);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetArchitecture(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetArchitecture(BuildTargetGroup targetGroup, int architecture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ScriptingImplementation GetScriptingBackend(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetScriptingBackend(BuildTargetGroup targetGroup, ScriptingImplementation backend);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIncrementalIl2CppBuild(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIncrementalIl2CppBuild(BuildTargetGroup targetGroup, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAdditionalIl2CppArgs();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAdditionalIl2CppArgs(string additionalArgs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetApiCompatibilityInternal(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_cursorHotspot(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_cursorHotspot(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);
	}
}
