using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSdkVersions minSdkVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidPreferredInstallLocation preferredInstallLocation
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceInternetPermission
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool forceSDCardPermission
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidTVCompatibility
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool androidIsGame
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool androidBannerEnabled
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern AndroidGamepadSupportLevel androidGamepadSupportLevel
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool createWallpaper
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidTargetDevice targetDevice
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidSplashScreenScale splashScreenScale
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoreName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystorePass
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keyaliasPass
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool licenseVerification
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern bool useAPKExpansionFiles
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern AndroidShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern AndroidBanner[] GetAndroidBanners();

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D GetAndroidBannerForHeight(int height);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetAndroidBanners(Texture2D[] banners);
		}

		public sealed class iOS
		{
			public static extern string applicationDisplayName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string buildNumber
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern ScriptCallOptimizationLevel scriptCallOptimization
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSSdkVersion sdkVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSTargetOSVersion targetOSVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSTargetDevice targetDevice
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("use Screen.SetResolution at runtime", true)]
			public static extern iOSTargetResolution targetResolution
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool prerenderedIcon
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresPersistentWiFi
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool requiresFullScreen
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSStatusBarStyle statusBarStyle
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("use appInBackgroundBehavior")]
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

			public static extern iOSAppInBackgroundBehavior appInBackgroundBehavior
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowHTTPDownload
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern iOSShowActivityIndicatorOnLoading showActivityIndicatorOnLoading
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool overrideIPodMusic
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useOnDemandResources
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetAssetBundleVariantsWithDeviceRequirements();

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool CheckAssetBundleVariantHasDeviceRequirements(string name);

			internal static iOSDeviceRequirementGroup GetDeviceRequirementsForAssetBundleVariant(string name)
			{
				if (!PlayerSettings.iOS.CheckAssetBundleVariantHasDeviceRequirements(name))
				{
					return null;
				}
				return new iOSDeviceRequirementGroup(name);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern string[] GetURLSchemes();
		}

		public sealed class Nintendo3DS
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableStereoscopicView
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableSharedListOpt
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool enableVSync
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useExtSaveData
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool compressStaticMem
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string extSaveDataNumber
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int stackSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.Nintendo3DS.TargetPlatform targetPlatform
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.Nintendo3DS.Region region
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.Nintendo3DS.MediaSize mediaSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.Nintendo3DS.LogoStyle logoStyle
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string title
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productCode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string applicationId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}

		public sealed class PS3
		{
			public static extern Texture2D ps3SplashScreen
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoMemoryForVertexBuffers
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoMemoryForAudio
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnableVerboseMemoryStats
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool UseSPUForUmbra
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnableMoveSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool DisableDolbyEncoding
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string titleConfigPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string dlcConfigPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string thumbnailPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string backgroundPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string soundPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTrophyCommId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommunicationPassphrase
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTrophyCommSig
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTrophyPackagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int bootCheckMaxSaveGameSizeKB
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool trialMode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int saveGameSlots
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleSecret
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter1
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter2
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter3
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int applicationParameter4
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string passcode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string monoEnv
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool playerPrefsSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4AppCategory category
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int appType
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4RemotePlayKeyAssignment remotePlayKeyAssignment
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string remotePlayKeyMappingDir
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int playTogetherPlayerCount
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PS4.PS4EnterButtonAssignment enterButtonAssignment
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutPixelFormat
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int videoOutResolution
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationXMLPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PronunciationSIGPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BackgroundImagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string StartupImagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SaveDataImagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SdkOverride
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string BGMPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareFilePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ShareOverlayImagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PrivacyGuardImagePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool patchDayOne
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchPkgPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchLatestPkgPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PatchChangeinfoPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string NPtitleDatPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnSessions
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnPresence
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnFriends
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool pnGameCustomData
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int downloadDataSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int garlicHeapSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool reprojectionSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool useAudio3dBackend
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int audio3dVirtualSpeakerCount
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int socialScreenEnabled
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribUserManagement
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribMoveSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attrib3DSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribShareSupport
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool attribExclusiveVR
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool disableAutoHideSplash
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int attribCpuUsage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] includedModules
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaPowerMode powerMode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool acquireBGM
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool npSupportGBMorGJP
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaTvBootMode tvBootMode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool tvDisableEmu
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool upgradable
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool healthWarning
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("useLibLocation has no effect as of SDK 3.570")]
			public static extern bool useLibLocation
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarOnStartup
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool infoBarColor
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			internal static extern bool useDebugIl2cppLibs
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaEnterButtonAssignment enterButtonAssignment
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int saveDataQuota
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int parentalLevel
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string shortTitle
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string contentID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaAppCategory category
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string masterVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string appVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("AllowTwitterDialog has no effect as of SDK 3.570")]
			public static extern bool AllowTwitterDialog
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int npAgeRating
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npTitleDatPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommunicationsID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsPassphrase
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string npCommsSig
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string paramSfxPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string manualPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaGatePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaBackroundPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string liveAreaTrialPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchChangeInfoPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string patchOriginalPackage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packagePassword
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string keystoneFile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaMemoryExpansionMode memoryExpansionMode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.PSVita.PSVitaDRMType drmType
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int storageType
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mediaCapacity
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productDescription
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthor
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productAuthorEmail
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productLink
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.SamsungTV.SamsungTVProductCategories productCategory
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string productURL
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string signingProfileName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern TizenOSVersion minOSVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				bool result;
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
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);
		}

		public sealed class tvOS
		{
			public static extern tvOSSdkVersion sdkVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern tvOSTargetOSVersion targetOSVersion
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetSmallIconLayers();

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetLargeIconLayers();

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern Texture2D[] GetTopShelfImageLayers();

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetSmallIconLayers(Texture2D[] layers);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetLargeIconLayers(Texture2D[] layers);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void SetTopShelfImageLayers(Texture2D[] layers);
		}

		public sealed class WiiU
		{
			public static extern string titleID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string groupID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonSaveSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountSaveSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string olvAccessKey
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tinCode
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string joinGameModeMask
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int commonBossSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int accountBossSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] addOnUniqueIDs
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsNunchuk
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsClassicController
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsBalanceBoard
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsMotionPlus
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool supportsProController
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool allowScreenCapture
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int controllerCount
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int mainThreadStackSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int loaderThreadStackSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int systemHeapSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern WiiUTVResolution tvResolution
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern Texture2D tvStartupScreen
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern Texture2D gamePadStartupScreen
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern int gamePadMSAA
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string profilerLibraryPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
			Bluetooth
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string packageLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string packageLogo240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern string packageVersionRaw
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string commandLineArgsFile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string certificatePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			internal static extern string certificatePassword
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateSubject
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string certificateIssuer
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			private static extern long certificateNotAfterRaw
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string applicationDescription
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileWideLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeTileSmallLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSmallTile180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeLargeTile180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string storeSplashScreenImageScale180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneAppIcon240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSmallTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneMediumTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneWideTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("Use GetVisualAssetsImage()/SetVisualAssetsImage()")]
			public static extern string phoneSplashScreenImageScale240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string tileShortName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationShowName tileShowName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool mediumTileShowName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool largeTileShowName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool wideTileShowName
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSADefaultTileSize defaultTileSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSACompilationOverrides compilationOverrides
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern PlayerSettings.WSAApplicationForegroundText tileForegroundText
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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

			public static extern bool enableIndependentInputSource
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			[Obsolete("PlayerSettings.enableLowLatencyPresentationAPI is deprecated. It is now always enabled.", false)]
			public static extern bool enableLowLatencyPresentationAPI
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			private static extern bool splashScreenUseBackgroundColor
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
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
					if (certificateNotAfterRaw != 0L)
					{
						return new DateTime?(DateTime.FromFileTime(certificateNotAfterRaw));
					}
					return null;
				}
			}

			public static Color? splashScreenBackgroundColor
			{
				get
				{
					if (PlayerSettings.WSA.splashScreenUseBackgroundColor)
					{
						return new Color?(PlayerSettings.WSA.splashScreenBackgroundColorRaw);
					}
					return null;
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string GetWSAImage(PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWSAImage(string image, PlayerSettings.WSAImageType type, PlayerSettings.WSAImageScale scale);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool SetCertificate(string path, string password);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_tileBackgroundColor(out Color value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_tileBackgroundColor(ref Color value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_splashScreenBackgroundColorRaw(out Color value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_splashScreenBackgroundColorRaw(ref Color value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_get_internalFileTypeAssociations(out PlayerSettings.WSAFileTypeAssociations value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_set_internalFileTypeAssociations(ref PlayerSettings.WSAFileTypeAssociations value);

			internal static string ValidatePackageVersion(string value)
			{
				Regex regex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				if (regex.IsMatch(value))
				{
					return value;
				}
				return "1.0.0.0";
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
					IL_6F:
					if (type != PlayerSettings.WSAImageType.PackageLogo && type != PlayerSettings.WSAImageType.SplashScreenImage)
					{
						throw new Exception("Unknown WSA image type: " + type);
					}
					return;
				}
				goto IL_6F;
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
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				bool result;
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
				return result;
			}
		}

		public sealed class XboxOne
		{
			public static extern XboxOneLoggingLevel defaultLoggingLevel
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ProductId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string UpdateKey
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SandboxId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string ContentId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string TitleId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string SCID
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnableVariableGPU
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool DisableKinectGpuReservation
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool EnablePIXSampling
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string GameOsOverridePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string PackagingOverridePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOneEncryptionLevel PackagingEncryption
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern XboxOnePackageUpdateGranularity PackageUpdateGranularity
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string AppManifestOverridePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern bool IsContentPackage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Version
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string Description
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern string[] SocketNames
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern string[] AllowedProductIds
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
			}

			public static extern uint PersistentLocalStorageSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static extern int monoLoggingLevel
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}

			public static void SetCapability(string capability, bool value)
			{
				PlayerSettings.SetPropertyBool(capability, value, BuildTargetGroup.XboxOne);
			}

			public static bool GetCapability(string capability)
			{
				bool result;
				try
				{
					bool flag = false;
					PlayerSettings.GetPropertyOptionalBool(capability, ref flag, BuildTargetGroup.XboxOne);
					result = flag;
				}
				catch
				{
					result = false;
				}
				return result;
			}

			internal static void SetSupportedLanguage(string language, bool enabled)
			{
				PlayerSettings.SetPropertyBool(language, enabled, BuildTargetGroup.XboxOne);
			}

			internal static bool GetSupportedLanguage(string language)
			{
				bool result;
				try
				{
					bool flag = false;
					PlayerSettings.GetPropertyOptionalBool(language, ref flag, BuildTargetGroup.XboxOne);
					result = flag;
				}
				catch
				{
					result = false;
				}
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveSocketDefinition(string name);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSocketDefinition(string name, string port, int protocol, int[] usages, string templateName, int sessionRequirment, int[] deviceUsages);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void GetSocketDefinition(string name, out string port, out int protocol, out int[] usages, out string templateName, out int sessionRequirment, out int[] deviceUsages);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void RemoveAllowedProductId(string id);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool AddAllowedProductId(string id);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void UpdateAllowedProductId(int idx, string id);

			internal static void InitializeGameRating(string name, int value)
			{
				PlayerSettings.InitializePropertyInt(name, value, BuildTargetGroup.XboxOne);
			}

			public static void SetGameRating(string name, int value)
			{
				PlayerSettings.SetPropertyInt(name, value, BuildTargetGroup.XboxOne);
			}

			public static int GetGameRating(string name)
			{
				int result;
				try
				{
					int num = 0;
					PlayerSettings.GetPropertyOptionalInt(name, ref num, BuildTargetGroup.XboxOne);
					result = num;
				}
				catch
				{
					result = 0;
				}
				return result;
			}
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string productName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showUnitySplashScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern SplashScreenStyle splashScreenStyle
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ColorSpace colorSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultScreenHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultWebScreenHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ResolutionDialogSetting displayResolutionDialog
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsFullScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool defaultIsNativeResolution
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool runInBackground
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool captureSingleScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool usePlayerLog
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool resizableWindow
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool bakeCollisionMeshes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useMacAppStoreValidation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern MacFullscreenMode macFullscreenMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D9FullscreenMode d3d9FullscreenMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern D3D11FullscreenMode d3d11FullscreenMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool virtualRealitySupported
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool singlePassStereoRendering
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool protectGraphicsMemory
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool visibleInBackground
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowFullscreenSwitch
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool forceSingleInstance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("The option alwaysDisplayWatermark is deprecated and is always false.")]
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

		public static extern Texture2D resolutionDialogBanner
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D virtualRealitySplashScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string iPhoneBundleIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string webPlayerTemplate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string[] templateCustomKeys
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string spritePackerPolicy
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keystorePass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string keyaliasPass
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string xboxTitleId
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string xboxImageXexFilePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string xboxSpaFilePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxGenerateSpa
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableGuest
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxDeployKinectResources
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxDeployKinectHeadOrientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxDeployKinectHeadPosition
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Texture2D xboxSplashScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int xboxAdditionalTitleMemorySize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxEnableKinect
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableKinectAutoTracking
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableSpeech
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern uint xboxSpeechDB
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool gpuSkinning
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool graphicsJobs
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool xboxPIXTextureCapture
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool xboxEnableAvatar
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int xboxOneResolution
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool enableInternalProfiler
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ActionOnDotNetUnhandledException actionOnDotNetUnhandledException
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool logObjCUncaughtExceptions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableCrashReportAPI
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string locationUsageDescription
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string bundleIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string bundleVersion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool statusBarHidden
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern StrippingLevel strippingLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripEngineCode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern UIOrientation defaultInterfaceOrientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortrait
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToPortraitUpsideDown
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeRight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowedAutorotateToLandscapeLeft
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useAnimatedAutorotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool use32BitDisplayBuffer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("targetGlesGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs")]
		public static extern TargetGlesGraphics targetGlesGraphics
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("targetIOSGraphics is ignored, use SetGraphicsAPIs/GetGraphicsAPIs")]
		public static extern TargetIOSGraphics targetIOSGraphics
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ApiCompatibilityLevel apiCompatibilityLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool stripUnusedMeshComponents
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool advancedLicense
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string aotOptions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int accelerometerFrequency
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool MTRendering
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool mobileMTRendering
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern RenderingPath renderingPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern RenderingPath mobileRenderingPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use SetGraphicsAPIs/GetGraphicsAPIs instead")]
		public static extern bool useDirect3D11
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool submitAnalytics
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use SetPropertyString(\"VR::devices\", \"stereo\") instead")]
		public static extern bool stereoscopic3D
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPropertyIntInternal(string name, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitializePropertyIntInternal(string name, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPropertyIntInternal(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPropertyBoolInternal(string name, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitializePropertyBoolInternal(string name, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetPropertyBoolInternal(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPropertyStringInternal(string name, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitializePropertyStringInternal(string name, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPropertyStringInternal(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPropertyNameForBuildTargetGroupInternal(BuildTargetGroup target, string name);

		internal static string GetPropertyNameForBuildTarget(BuildTargetGroup target, string name)
		{
			string propertyNameForBuildTargetGroupInternal = PlayerSettings.GetPropertyNameForBuildTargetGroupInternal(target, name);
			if (propertyNameForBuildTargetGroupInternal != string.Empty)
			{
				return propertyNameForBuildTargetGroupInternal;
			}
			throw new ArgumentException("Failed to get property name for the given target.");
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AddEnum(string className, string propertyName, int value, string valueName);

		[ExcludeFromDocs]
		public static void SetPropertyInt(string name, int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyInt(name, value, target);
		}

		public static void SetPropertyInt(string name, int value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.SetPropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		public static void SetPropertyInt(string name, int value, BuildTarget target)
		{
			PlayerSettings.SetPropertyInt(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[ExcludeFromDocs]
		public static int GetPropertyInt(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyInt(name, target);
		}

		public static int GetPropertyInt(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			return PlayerSettings.GetPropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetPropertyOptionalInt(string name, ref int value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[ExcludeFromDocs]
		public static bool GetPropertyOptionalInt(string name, ref int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalInt(name, ref value, target);
		}

		[ExcludeFromDocs]
		internal static void InitializePropertyInt(string name, int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.InitializePropertyInt(name, value, target);
		}

		internal static void InitializePropertyInt(string name, int value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.InitializePropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		[ExcludeFromDocs]
		public static void SetPropertyBool(string name, bool value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyBool(name, value, target);
		}

		public static void SetPropertyBool(string name, bool value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.SetPropertyBoolInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		public static void SetPropertyBool(string name, bool value, BuildTarget target)
		{
			PlayerSettings.SetPropertyBool(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[ExcludeFromDocs]
		public static bool GetPropertyBool(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyBool(name, target);
		}

		public static bool GetPropertyBool(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			return PlayerSettings.GetPropertyBoolInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetPropertyOptionalBool(string name, ref bool value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);

		[ExcludeFromDocs]
		public static bool GetPropertyOptionalBool(string name, ref bool value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalBool(name, ref value, target);
		}

		[ExcludeFromDocs]
		internal static void InitializePropertyBool(string name, bool value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.InitializePropertyBool(name, value, target);
		}

		internal static void InitializePropertyBool(string name, bool value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.InitializePropertyBoolInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		[ExcludeFromDocs]
		public static void SetPropertyString(string name, string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyString(name, value, target);
		}

		public static void SetPropertyString(string name, string value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.SetPropertyStringInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		public static void SetPropertyString(string name, string value, BuildTarget target)
		{
			PlayerSettings.SetPropertyString(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}

		[ExcludeFromDocs]
		public static string GetPropertyString(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyString(name, target);
		}

		public static string GetPropertyString(string name, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			return PlayerSettings.GetPropertyStringInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name));
		}

		[ExcludeFromDocs]
		public static bool GetPropertyOptionalString(string name, ref string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalString(name, ref value, target);
		}

		public static bool GetPropertyOptionalString(string name, ref string value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			string propertyOptionalStringInternal = PlayerSettings.GetPropertyOptionalStringInternal(name, target);
			if (propertyOptionalStringInternal == null)
			{
				return false;
			}
			value = propertyOptionalStringInternal;
			return true;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetPropertyOptionalStringInternal(string name, BuildTargetGroup target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetDirty();

		[ExcludeFromDocs]
		internal static void InitializePropertyString(string name, string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.InitializePropertyString(name, value, target);
		}

		internal static void InitializePropertyString(string name, string value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.InitializePropertyStringInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}

		[ExcludeFromDocs]
		internal static void InitializePropertyEnum(string name, object value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.InitializePropertyEnum(name, value, target);
		}

		internal static void InitializePropertyEnum(string name, object value, [UnityEngine.Internal.DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			string propertyNameForBuildTarget = PlayerSettings.GetPropertyNameForBuildTarget(target, name);
			string[] names = Enum.GetNames(value.GetType());
			Array values = Enum.GetValues(value.GetType());
			for (int i = 0; i < names.Length; i++)
			{
				PlayerSettings.AddEnum("PlayerSettings", propertyNameForBuildTarget, (int)values.GetValue(i), names[i]);
			}
			PlayerSettings.InitializePropertyIntInternal(propertyNameForBuildTarget, (int)value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAspectRatio(AspectRatio aspectRatio);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAspectRatio(AspectRatio aspectRatio, bool enable);

		public static Texture2D[] GetIconsForTargetGroup(BuildTargetGroup platform)
		{
			Texture2D[] iconsForPlatform = PlayerSettings.GetIconsForPlatform(PlayerSettings.GetPlatformName(platform));
			if (iconsForPlatform.Length == 0)
			{
				return new Texture2D[PlayerSettings.GetIconSizesForTargetGroup(platform).Length];
			}
			return iconsForPlatform;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D[] GetIconsForPlatform(string platform);

		public static void SetIconsForTargetGroup(BuildTargetGroup platform, Texture2D[] icons)
		{
			PlayerSettings.SetIconsForPlatform(PlayerSettings.GetPlatformName(platform), icons);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIconsForPlatform(string platform, Texture2D[] icons);

		public static int[] GetIconSizesForTargetGroup(BuildTargetGroup platform)
		{
			return PlayerSettings.GetIconWidthsForPlatform(PlayerSettings.GetPlatformName(platform));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconWidthsForPlatform(string platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconHeightsForPlatform(string platform);

		internal static string GetPlatformName(BuildTargetGroup targetGroup)
		{
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.GetValidPlatforms().Find((BuildPlayerWindow.BuildPlatform p) => p.targetGroup == targetGroup);
			return (buildPlatform != null) ? buildPlatform.name : string.Empty;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForPlatformAtSize(string platform, int width, int height);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GraphicsDeviceType[] GetSupportedGraphicsAPIs(BuildTarget platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsDeviceType[] GetGraphicsAPIs(BuildTarget platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGraphicsAPIs(BuildTarget platform, GraphicsDeviceType[] apis);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetUseDefaultGraphicsAPIs(BuildTarget platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTemplateCustomValue(string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetTemplateCustomValue(string key);

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScriptingDefineSymbolsForGroupInternal(BuildTargetGroup targetGroup, string defines);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetApiCompatibilityInternal(int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StackTraceLogType GetStackTraceLogType(LogType logType);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStackTraceLogType(LogType logType, StackTraceLogType stackTraceType);
	}
}
