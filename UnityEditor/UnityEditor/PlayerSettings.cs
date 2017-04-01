using System;
using System.ComponentModel;
using System.IO;
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSdkVersions minSdkVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSdkVersions targetSdkVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidPreferredInstallLocation preferredInstallLocation
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceInternetPermission
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceSDCardPermission
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidTVCompatibility
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidIsGame
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool androidBannerEnabled
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern AndroidGamepadSupportLevel androidGamepadSupportLevel
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool createWallpaper
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidTargetDevice targetDevice
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSplashScreenScale splashScreenScale
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoreName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystorePass
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasPass
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool licenseVerification
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool useAPKExpansionFiles
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern AndroidBanner[] GetAndroidBanners();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D GetAndroidBannerForHeight(int height);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetAndroidBanners(Texture2D[] banners);
		}

		public static class VRCardboard
		{
			public static extern int depthFormat
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public static class VRDaydream
		{
			public static extern Texture2D daydreamIcon
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Texture2D daydreamIconBackground
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int depthFormat
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class Facebook
		{
			public static extern string sdkVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class iOS
		{
			public static extern string applicationDisplayName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static string buildNumber
			{
				get
				{
					return PlayerSettings.GetBuildNumber(BuildTargetGroup.iPhone);
				}
				set
				{
					PlayerSettings.SetBuildNumber(BuildTargetGroup.iPhone, value);
				}
			}

			public static extern ScriptCallOptimizationLevel scriptCallOptimization
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSSdkVersion sdkVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("targetOSVersion is obsolete, use targetOSVersionString")]
			public static extern iOSTargetOSVersion targetOSVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string targetOSVersionString
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSTargetDevice targetDevice
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool prerenderedIcon
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresPersistentWiFi
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresFullScreen
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSStatusBarStyle statusBarStyle
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSAppInBackgroundBehavior appInBackgroundBehavior
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSBackgroundMode backgroundModes
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceHardShadowsOnMetal
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowHTTPDownload
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appleDeveloperTeamID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string iOSManualProvisioningProfileID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tvOSManualProvisioningProfileID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool appleEnableAutomaticSigning
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string cameraUsageDescription
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string locationUsageDescription
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string microphoneUsageDescription
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useOnDemandResources
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetAssetBundleVariantsWithDeviceRequirements();

			[GeneratedByOldBindingsGenerator]
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

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetURLSchemes();
		}

		public sealed class macOS
		{
			public static string buildNumber
			{
				get
				{
					return PlayerSettings.GetBuildNumber(BuildTargetGroup.Standalone);
				}
				set
				{
					PlayerSettings.SetBuildNumber(BuildTargetGroup.Standalone, value);
				}
			}
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableStereoscopicView
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableSharedListOpt
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableVSync
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useExtSaveData
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool compressStaticMem
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string extSaveDataNumber
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int stackSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.TargetPlatform targetPlatform
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.Region region
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.MediaSize mediaSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.N3DS.LogoStyle logoStyle
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string title
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productCode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string applicationId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class PS4
		{
			public enum PS4AppCategory
			{
				Application,
				Patch,
				Remaster
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

			public enum PlayStationVREyeToEyeDistanceSettings
			{
				PerUser,
				ForceDefault,
				DynamicModeAtRuntime
			}

			public static extern string npTrophyPackPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleSecret
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter1
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter2
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter3
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter4
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string passcode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string monoEnv
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool playerPrefsSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool restrictedAudioUsageRights
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useResolutionFallback
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4AppCategory category
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int appType
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4RemotePlayKeyAssignment remotePlayKeyAssignment
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string remotePlayKeyMappingDir
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int playTogetherPlayerCount
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4EnterButtonAssignment enterButtonAssignment
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutPixelFormat
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutInitialWidth
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutBaseModeInitialWidth
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutReprojectionRate
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("videoOutResolution is deprecated. Use PlayerSettings.PS4.videoOutInitialWidth and PlayerSettings.PS4.videoOutReprojectionRate to control initial display resolution and reprojection rate.")]
			public static extern int videoOutResolution
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationXMLPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationSIGPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BackgroundImagePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string StartupImagePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SaveDataImagePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SdkOverride
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BGMPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareFilePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareOverlayImagePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PrivacyGuardImagePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool patchDayOne
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchPkgPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchLatestPkgPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchChangeinfoPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string NPtitleDatPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnSessions
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnPresence
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnFriends
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnGameCustomData
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int downloadDataSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int garlicHeapSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int proGarlicHeapSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool reprojectionSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useAudio3dBackend
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int audio3dVirtualSpeakerCount
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int scriptOptimizationLevel
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int socialScreenEnabled
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribUserManagement
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribMoveSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attrib3DSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribShareSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribExclusiveVR
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableAutoHideSplash
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int attribCpuUsage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool videoRecordingFeaturesUsed
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool contentSearchFeaturesUsed
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PlayStationVREyeToEyeDistanceSettings attribEyeToEyeDistanceSettingVR
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] includedModules
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaPowerMode powerMode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool acquireBGM
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool npSupportGBMorGJP
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaTvBootMode tvBootMode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool tvDisableEmu
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool upgradable
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool healthWarning
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("useLibLocation has no effect as of SDK 3.570")]
			public static extern bool useLibLocation
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarOnStartup
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarColor
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaEnterButtonAssignment enterButtonAssignment
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int saveDataQuota
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string shortTitle
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaAppCategory category
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("AllowTwitterDialog has no effect as of SDK 3.570")]
			public static extern bool AllowTwitterDialog
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleDatPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommunicationsID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsPassphrase
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsSig
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string manualPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaGatePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaBackroundPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaTrialPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchChangeInfoPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchOriginalPackage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packagePassword
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoneFile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaMemoryExpansionMode memoryExpansionMode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaDRMType drmType
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int storageType
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mediaCapacity
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productDescription
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthor
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthorEmail
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productLink
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SamsungTV.SamsungTVProductCategories productCategory
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float animationBackgroundZoom
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float animationLogoZoom
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Sprite background
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Sprite backgroundPortrait
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SplashScreenLogo[] logos
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern float overlayOpacity
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool show
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool showUnityLogo
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SplashScreen.UnityLogoStyle unityLogoStyle
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_backgroundColor(out Color value);

			[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productURL
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string signingProfileName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string deploymentTarget
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int deploymentTargetType
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern TizenOSVersion minOSVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern TizenShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);
		}

		public sealed class tvOS
		{
			public static extern tvOSSdkVersion sdkVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static string buildNumber
			{
				get
				{
					return PlayerSettings.GetBuildNumber(BuildTargetGroup.tvOS);
				}
				set
				{
					PlayerSettings.SetBuildNumber(BuildTargetGroup.tvOS, value);
				}
			}

			public static extern tvOSTargetOSVersion targetOSVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string targetOSVersionString
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requireExtendedGameController
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetSmallIconLayers();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetLargeIconLayers();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetTopShelfImageLayers();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetTopShelfImageWideLayers();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetSmallIconLayers(Texture2D[] layers);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetLargeIconLayers(Texture2D[] layers);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetTopShelfImageLayers(Texture2D[] layers);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetTopShelfImageWideLayers(Texture2D[] layers);
		}

		public sealed class WebGL
		{
			public static extern int memorySize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WebGLExceptionSupport exceptionSupport
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool dataCaching
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string emscriptenArgs
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string modulesDirectory
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string template
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool analyzeBuildSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useEmbeddedResources
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useWasm
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WebGLCompressionFormat compressionFormat
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool nameFilesAsHashes
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool debugSymbols
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class WiiU
		{
			public static extern string titleID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string groupID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonSaveSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountSaveSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string olvAccessKey
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tinCode
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameModeMask
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonBossSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountBossSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] addOnUniqueIDs
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsNunchuk
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsClassicController
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsBalanceBoard
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsMotionPlus
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsProController
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowScreenCapture
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int controllerCount
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mainThreadStackSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int loaderThreadStackSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int systemHeapSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WiiUTVResolution tvResolution
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Texture2D tvStartupScreen
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern Texture2D gamePadStartupScreen
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int gamePadMSAA
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string profilerLibraryPath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool drcBufferDisabled
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class Switch
		{
			public enum Languages
			{
				AmericanEnglish,
				BritishEnglish,
				Japanese,
				French,
				German,
				LatinAmericanSpanish,
				Spanish,
				Italian,
				Dutch,
				CanadianFrench,
				Portuguese,
				Russian,
				Korean,
				Taiwanese,
				Chinese
			}

			public enum StartupUserAccount
			{
				None,
				Required,
				RequiredWithNetworkServiceAccountAvailable
			}

			public enum TouchScreenUsage
			{
				Supported,
				Required,
				None
			}

			public enum LogoHandling
			{
				Auto,
				Manual
			}

			public enum LogoType
			{
				LicensedByNintendo,
				DistributedByNintendo,
				Nintendo
			}

			public enum ApplicationAttribute
			{
				None,
				Demo
			}

			public enum RatingCategories
			{
				CERO,
				GRACGCRB,
				GSRMR,
				ESRB,
				ClassInd,
				USK,
				PEGI,
				PEGIPortugal,
				PEGIBBFC,
				Russian,
				ACB,
				OFLC
			}

			public static extern int socketMemoryPoolSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int socketAllocatorPoolSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int socketConcurrencyLimit
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useSwitchCPUProfiler
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string applicationID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string nsoDependencies
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] titleNames
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string[] publisherNames
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern Texture2D[] icons
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern Texture2D[] smallIcons
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static string manualHTMLPath
			{
				get
				{
					string manualHTMLPath = PlayerSettings.Switch.GetManualHTMLPath();
					string result;
					if (string.IsNullOrEmpty(manualHTMLPath))
					{
						result = "";
					}
					else
					{
						string text = manualHTMLPath;
						if (!Path.IsPathRooted(text))
						{
							text = Path.GetFullPath(text);
						}
						if (!Directory.Exists(text))
						{
							result = "";
						}
						else
						{
							result = text;
						}
					}
					return result;
				}
			}

			public static string accessibleURLPath
			{
				get
				{
					string accessibleURLPath = PlayerSettings.Switch.GetAccessibleURLPath();
					string result;
					if (string.IsNullOrEmpty(accessibleURLPath))
					{
						result = "";
					}
					else
					{
						string text = accessibleURLPath;
						if (!Path.IsPathRooted(text))
						{
							text = Path.GetFullPath(text);
						}
						if (!Directory.Exists(text))
						{
							result = "";
						}
						else
						{
							result = text;
						}
					}
					return result;
				}
			}

			public static string legalInformationPath
			{
				get
				{
					string text = PlayerSettings.Switch.GetLegalInformationPath();
					string result;
					if (string.IsNullOrEmpty(text))
					{
						result = "";
					}
					else
					{
						if (!Path.IsPathRooted(text))
						{
							text = Path.GetFullPath(text);
						}
						result = text;
					}
					return result;
				}
			}

			public static extern int mainThreadStackSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string presenceGroupId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern PlayerSettings.Switch.LogoHandling logoHandling
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string releaseVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string displayVersion
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern PlayerSettings.Switch.StartupUserAccount startupUserAccount
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern PlayerSettings.Switch.TouchScreenUsage touchScreenUsage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int supportedLanguages
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern PlayerSettings.Switch.LogoType logoType
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string applicationErrorCodeCategory
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int userAccountSaveDataSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int userAccountSaveDataJournalSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern PlayerSettings.Switch.ApplicationAttribute attribute
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int cardSpecSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int cardSpecClock
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int ratingsMask
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string[] localCommunicationIds
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool isUnderParentalControl
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool isAllowsScreenshot
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool isDataLossConfirmation
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetManualHTMLPath();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetAccessibleURLPath();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetLegalInformationPath();

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern int GetRatingAge(PlayerSettings.Switch.RatingCategories category);
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packageLogo
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern string packageVersionRaw
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string commandLineArgsFile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string certificatePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			internal static extern string certificatePassword
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateSubject
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateIssuer
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			private static extern long certificateNotAfterRaw
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string applicationDescription
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo80
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo80
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo80
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile80
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile80
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale180
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale140
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale240
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tileShortName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationShowName tileShowName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool mediumTileShowName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool largeTileShowName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool wideTileShowName
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSADefaultTileSize defaultTileSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSACompilationOverrides compilationOverrides
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationForegroundText tileForegroundText
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("PlayerSettings.enableLowLatencyPresentationAPI is deprecated. It is now always enabled.", false)]
			public static extern bool enableLowLatencyPresentationAPI
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern bool splashScreenUseBackgroundColor
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
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

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetWSAImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWSAImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool SetCertificate(string path, string password);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_tileBackgroundColor(out Color value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_tileBackgroundColor(ref Color value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_splashScreenBackgroundColorRaw(out Color value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_splashScreenBackgroundColorRaw(ref Color value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_internalFileTypeAssociations(out PlayerSettings.WSAFileTypeAssociations value);

			[GeneratedByOldBindingsGenerator]
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
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ProductId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string UpdateKey
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("SandboxId is obsolete please remove")]
			public static extern string SandboxId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ContentId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string TitleId
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SCID
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnableVariableGPU
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool Enable7thCore
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool DisableKinectGpuReservation
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnablePIXSampling
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string GameOsOverridePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PackagingOverridePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOneEncryptionLevel PackagingEncryption
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOnePackageUpdateGranularity PackageUpdateGranularity
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string AppManifestOverridePath
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool IsContentPackage
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Version
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Description
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] SocketNames
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string[] AllowedProductIds
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern uint PersistentLocalStorageSize
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int monoLoggingLevel
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern ScriptCompiler scriptCompiler
			{
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[GeneratedByOldBindingsGenerator]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetCapability(string capability, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool GetCapability(string capability);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSupportedLanguage(string language, bool enabled);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool GetSupportedLanguage(string language);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveSocketDefinition(string name);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSocketDefinition(string name, string port, int protocol, int[] usages, string templateName, int sessionRequirment, int[] deviceUsages);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void GetSocketDefinition(string name, out string port, out int protocol, out int[] usages, out string templateName, out int sessionRequirment, out int[] deviceUsages);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveAllowedProductId(string id);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool AddAllowedProductId(string id);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void UpdateAllowedProductId(int idx, string id);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGameRating(string name, int value);

			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string productName
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use PlayerSettings.SplashScreen.show instead")]
		public static extern bool showUnitySplashScreen
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use PlayerSettings.SplashScreen.unityLogoStyle instead")]
		public static extern SplashScreenStyle splashScreenStyle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ColorSpace colorSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ResolutionDialogSetting displayResolutionDialog
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsFullScreen
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsNativeResolution
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool runInBackground
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool captureSingleScreen
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool usePlayerLog
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool resizableWindow
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool bakeCollisionMeshes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useMacAppStoreValidation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern MacFullscreenMode macFullscreenMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D9FullscreenMode d3d9FullscreenMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D11FullscreenMode d3d11FullscreenMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool virtualRealitySupported
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("singlePassStereoRendering will be deprecated. Use stereoRenderingPath instead.")]
		public static extern bool singlePassStereoRendering
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern StereoRenderingPath stereoRenderingPath
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool protectGraphicsMemory
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useHDRDisplay
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool visibleInBackground
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowFullscreenSwitch
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool forceSingleInstance
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool openGLRequireES31
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool openGLRequireES31AEP
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D resolutionDialogBanner
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D virtualRealitySplashScreen
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("iPhoneBundleIdentifier is deprecated. Use PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS) instead.")]
		public static string iPhoneBundleIdentifier
		{
			get
			{
				return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iPhone);
			}
			set
			{
				PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iPhone, value);
			}
		}

		internal static extern string webPlayerTemplate
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string[] templateCustomKeys
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string spritePackerPolicy
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keystorePass
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keyaliasPass
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern string xboxTitleId
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern string xboxImageXexFilePath
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern string xboxSpaFilePath
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxGenerateSpa
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxEnableGuest
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxDeployKinectResources
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxDeployKinectHeadOrientation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxDeployKinectHeadPosition
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern Texture2D xboxSplashScreen
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern int xboxAdditionalTitleMemorySize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxEnableKinect
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxEnableKinectAutoTracking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern bool xboxEnableSpeech
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Xbox 360 has been removed in >=5.5")]
		public static extern uint xboxSpeechDB
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gpuSkinning
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool graphicsJobs
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern GraphicsJobMode graphicsJobMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxPIXTextureCapture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableAvatar
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int xboxOneResolution
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool enableInternalProfiler
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ActionOnDotNetUnhandledException actionOnDotNetUnhandledException
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool logObjCUncaughtExceptions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableCrashReportAPI
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static string applicationIdentifier
		{
			get
			{
				return PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup);
			}
			set
			{
				PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, value);
			}
		}

		public static extern string bundleVersion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool statusBarHidden
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern StrippingLevel strippingLevel
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripEngineCode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UIOrientation defaultInterfaceOrientation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortrait
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortraitUpsideDown
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeRight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeLeft
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useAnimatedAutorotation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool use32BitDisplayBuffer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("apiCompatibilityLevel is deprecated. Use PlayerSettings.GetApiCompatibilityLevel()/PlayerSettings.SetApiCompatibilityLevel() instead.")]
		public static extern ApiCompatibilityLevel apiCompatibilityLevel
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripUnusedMeshComponents
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool advancedLicense
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string aotOptions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D defaultCursor
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool MTRendering
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool mobileMTRendering
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use UnityEditor.PlayerSettings.SetGraphicsAPIs/GetGraphicsAPIs instead")]
		public static extern bool useDirect3D11
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool submitAnalytics
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use VREditor.GetStereoDeviceEnabled instead")]
		public static extern bool stereoscopic3D
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool muteOtherAudioSources
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use PlayerSettings.applicationIdentifier instead (UnityUpgradable) -> UnityEditor.PlayerSettings.applicationIdentifier", true)]
		public static string bundleIdentifier
		{
			get
			{
				return PlayerSettings.applicationIdentifier;
			}
			set
			{
				PlayerSettings.applicationIdentifier = value;
			}
		}

		[GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[Obsolete("Use explicit API instead."), GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDirty();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCloudServiceEnabled(string serviceKey, bool enabled);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetCloudServiceEnabled(string serviceKey);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAspectRatio(AspectRatio aspectRatio);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D[] GetIconsForPlatform(string platform);

		public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons)
		{
			PlayerSettings.SetIconsForPlatform(PlayerSettings.GetPlatformName(platform), icons);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconsForPlatform(string platform, Texture2D[] icons);

		public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform)
		{
			return PlayerSettings.GetIconWidthsForPlatform(PlayerSettings.GetPlatformName(platform));
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconWidthsForPlatform(string platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconHeightsForPlatform(string platform);

		internal static string GetPlatformName(BuildTargetGroup targetGroup)
		{
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.GetValidPlatforms().Find((BuildPlayerWindow.BuildPlatform p) => p.targetGroup == targetGroup);
			return (buildPlatform != null) ? buildPlatform.name : string.Empty;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForPlatformAtSize(string platform, int width, int height);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GraphicsDeviceType[] GetSupportedGraphicsAPIs(BuildTarget platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsDeviceType[] GetGraphicsAPIs(BuildTarget platform);

		public static void SetGraphicsAPIs(BuildTarget platform, GraphicsDeviceType[] apis)
		{
			PlayerSettings.SetGraphicsAPIsImpl(platform, apis);
			PlayerSettingsEditor.SyncPlatformAPIsList(platform);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGraphicsAPIsImpl(BuildTarget platform, GraphicsDeviceType[] apis);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetUseDefaultGraphicsAPIs(BuildTarget platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTemplateCustomValue(string key, string value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetTemplateCustomValue(string key);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScriptingDefineSymbolsForGroupInternal(BuildTargetGroup targetGroup, string defines);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetArchitecture(BuildTargetGroup targetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetArchitecture(BuildTargetGroup targetGroup, int architecture);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ScriptingImplementation GetScriptingBackend(BuildTargetGroup targetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetApplicationIdentifier(BuildTargetGroup targetGroup, string identifier);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetApplicationIdentifier(BuildTargetGroup targetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBuildNumber(BuildTargetGroup targetGroup, string buildNumber);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildNumber(BuildTargetGroup targetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetScriptingBackend(BuildTargetGroup targetGroup, ScriptingImplementation backend);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetIncrementalIl2CppBuild(BuildTargetGroup targetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIncrementalIl2CppBuild(BuildTargetGroup targetGroup, bool enabled);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAdditionalIl2CppArgs();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAdditionalIl2CppArgs(string additionalArgs);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ApiCompatibilityLevel GetApiCompatibilityLevel(BuildTargetGroup buildTargetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetApiCompatibilityLevel(BuildTargetGroup buildTargetGroup, ApiCompatibilityLevel value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_cursorHotspot(out Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_cursorHotspot(ref Vector2 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);
	}
}
