using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
namespace UnityEditor
{
	public sealed class PlayerSettings : UnityEngine.Object
	{
		public sealed class Android
		{
			public static extern bool use24BitDepthBuffer
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
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
			public static extern iOSStatusBarStyle statusBarStyle
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern bool exitOnSuspend
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
		}
		public enum MetroApplicationShowName
		{
			NotSet,
			AllLogos,
			NoLogos,
			StandardLogoOnly,
			WideLogoOnly
		}
		public enum MetroDefaultTileSize
		{
			NotSet,
			Medium,
			Wide
		}
		public enum MetroApplicationForegroundText
		{
			Light = 1,
			Dark
		}
		public enum MetroCompilationOverrides
		{
			None,
			UseNetCore,
			UseNetCorePartially
		}
		public enum MetroCapability
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
			Location
		}
		public sealed class Metro
		{
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
			public static extern string packageLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string packageLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string packageLogo240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static Version packageVersion
			{
				get
				{
					Version result;
					try
					{
						result = new Version(PlayerSettingsEditor.ValidateMetroPackageVersion(PlayerSettings.Metro.packageVersionRaw));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("{0}, the raw string was {1}", ex.Message, PlayerSettings.Metro.packageVersionRaw));
					}
					return result;
				}
				set
				{
					PlayerSettings.Metro.packageVersionRaw = value.ToString();
				}
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
			public static DateTime? certificateNotAfter
			{
				get
				{
					long certificateNotAfterRaw = PlayerSettings.Metro.certificateNotAfterRaw;
					if (certificateNotAfterRaw != 0L)
					{
						return new DateTime?(DateTime.FromFileTime(certificateNotAfterRaw));
					}
					return null;
				}
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
			public static extern string storeTileLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileWideLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileWideLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileWideLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileWideLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileSmallLogo80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileSmallLogo
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileSmallLogo140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeTileSmallLogo180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSmallTile80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSmallTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSmallTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSmallTile180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeLargeTile80
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeLargeTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeLargeTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeLargeTile180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSplashScreenImage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSplashScreenImageScale140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string storeSplashScreenImageScale180
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneAppIcon
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneAppIcon140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneAppIcon240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneSmallTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneSmallTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneSmallTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneMediumTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneMediumTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneMediumTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneWideTile
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneWideTile140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneWideTile240
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneSplashScreenImage
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string phoneSplashScreenImageScale140
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
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
			public static extern PlayerSettings.MetroApplicationShowName tileShowName
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
			public static extern PlayerSettings.MetroDefaultTileSize defaultTileSize
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern PlayerSettings.MetroCompilationOverrides compilationOverrides
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern PlayerSettings.MetroApplicationForegroundText tileForegroundText
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string[] unprocessedPlugins
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
					PlayerSettings.Metro.INTERNAL_get_tileBackgroundColor(out result);
					return result;
				}
				set
				{
					PlayerSettings.Metro.INTERNAL_set_tileBackgroundColor(ref value);
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
			public static extern bool enableLowLatencyPresentationAPI
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static Color? splashScreenBackgroundColor
			{
				get
				{
					if (PlayerSettings.Metro.splashScreenUseBackgroundColor)
					{
						return new Color?(PlayerSettings.Metro.splashScreenBackgroundColorRaw);
					}
					return null;
				}
				set
				{
					PlayerSettings.Metro.splashScreenUseBackgroundColor = value.HasValue;
					if (value.HasValue)
					{
						PlayerSettings.Metro.splashScreenBackgroundColorRaw = value.Value;
					}
				}
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
					PlayerSettings.Metro.INTERNAL_get_splashScreenBackgroundColorRaw(out result);
					return result;
				}
				set
				{
					PlayerSettings.Metro.INTERNAL_set_splashScreenBackgroundColorRaw(ref value);
				}
			}
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
			public static void SetCapability(PlayerSettings.MetroCapability capability, bool enabled)
			{
				PlayerSettings.Metro.InternalSetCapability(capability.ToString(), enabled);
			}
			public static bool GetCapability(PlayerSettings.MetroCapability capability)
			{
				return PlayerSettings.Metro.InternalGetCapability(capability.ToString());
			}
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, bool enabled);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool InternalGetCapability(string name);
		}
		public sealed class WP8
		{
			public static extern string[] unprocessedPlugins
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
		}
		public sealed class BlackBerry
		{
			public static extern string deviceAddress
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string devicePassword
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string tokenPath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string tokenExpires
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string tokenAuthor
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string tokenAuthorId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string authorId
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string cskPassword
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string saveLogPath
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
			public static extern void SetAuthorIDOverride(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool IsAuthorIDOverride();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasSharedPermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetSharedPermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasCameraPermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetCameraPermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasGPSPermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGPSPermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasIdentificationPermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetIdentificationPermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasMicrophonePermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetMicrophonePermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasGamepadSupport();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGamepadSupport(bool enable);
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
			public static extern string certificatePath
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
			}
			public static extern string certificatePassword
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
			public static extern bool HasGPSPermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetGPSPermissions(bool enable);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern bool HasMicrophonePermissions();
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public static extern void SetMicrophonePermissions(bool enable);
		}
		public sealed class PS3
		{
			public static extern int videoMemoryForVertexBuffers
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
			public static extern string deviceAddress
			{
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				get;
				[WrapperlessIcall]
				[MethodImpl(MethodImplOptions.InternalCall)]
				set;
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
		public static extern bool stripPhysics
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
		public static extern bool d3d11ForceExclusiveMode
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
		public static extern int firstStreamedLevelWithResources
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
		public static extern string locationUsageDescription
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
		public static extern string ps3TitleConfigPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3DLCConfigPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3ThumbnailPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3BackgroundPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3SoundPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3TrophyCommId
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3NpCommunicationPassphrase
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3TrophyCommSig
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string ps3TrophyPackagePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int ps3BootCheckMaxSaveGameSizeKB
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool ps3TrialMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern int ps3SaveGameSlots
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2NPTrophyPackPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2NPCommsID
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2NPCommsPassphrase
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2NPCommsSig
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2ParamSfxPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2PackagePassword
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2DLCConfigPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2ThumbnailPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2BackgroundPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2SoundPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2TrophyCommId
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string psp2TrophyPackagePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
		public static extern string shortBundleVersion
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
		public static extern TargetGlesGraphics targetGlesGraphics
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
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
		public static extern bool useDirect3D11
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
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
		public static extern bool GetPropertyOptionalInt(string name, ref int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target);
		[ExcludeFromDocs]
		public static bool GetPropertyOptionalInt(string name, ref int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalInt(name, ref value, target);
		}
		[ExcludeFromDocs]
		public static int GetPropertyInt(string name)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyInt(name, target);
		}
		public static int GetPropertyInt(string name, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			return PlayerSettings.GetPropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name));
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPropertyIntInternal(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPropertyIntInternal(string name, int value);
		public static void SetPropertyInt(string name, int value, BuildTarget target)
		{
			PlayerSettings.SetPropertyInt(name, value, BuildPipeline.GetBuildTargetGroup(target));
		}
		[ExcludeFromDocs]
		public static void SetPropertyInt(string name, int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.SetPropertyInt(name, value, target);
		}
		public static void SetPropertyInt(string name, int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.SetPropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InitializePropertyIntInternal(string name, int value);
		[ExcludeFromDocs]
		internal static void InitializePropertyInt(string name, int value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			PlayerSettings.InitializePropertyInt(name, value, target);
		}
		internal static void InitializePropertyInt(string name, int value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
		{
			PlayerSettings.InitializePropertyIntInternal(PlayerSettings.GetPropertyNameForBuildTarget(target, name), value);
		}
		[ExcludeFromDocs]
		public static bool GetPropertyOptionalString(string name, ref string value)
		{
			BuildTargetGroup target = BuildTargetGroup.Unknown;
			return PlayerSettings.GetPropertyOptionalString(name, ref value, target);
		}
		public static bool GetPropertyOptionalString(string name, ref string value, [DefaultValue("BuildTargetGroup.Unknown")] BuildTargetGroup target)
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
			return PlayerSettings.GetIconSizesForPlatform(PlayerSettings.GetPlatformName(platform));
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIconSizesForPlatform(string platform);
		internal static string GetPlatformName(BuildTargetGroup targetGroup)
		{
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.GetValidPlatforms().Find((BuildPlayerWindow.BuildPlatform p) => p.targetGroup == targetGroup);
			return (buildPlatform != null) ? buildPlatform.name : string.Empty;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetIconForPlatformAtSize(string platform, int size);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetBatchingForPlatform(BuildTarget platform, out int staticBatching, out int dynamicBatching);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBatchingForPlatform(BuildTarget platform, int staticBatching, int dynamicBatching);
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
	}
}
