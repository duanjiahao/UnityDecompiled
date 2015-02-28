using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Internal;
namespace UnityEditor
{
	public sealed class PlayerSettings : UnityEngine.Object
	{
		public sealed class XboxOne
		{
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
				Application
			}
			public enum PS4RemotePlayKeyAssignment
			{
				None = -1,
				PatternA,
				PatternB,
				PatternC,
				PatternD
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
			public static extern string NPtitleDatPath
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
			HumanInterfaceDevice
		}
		public sealed class WSA
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
					long certificateNotAfterRaw = PlayerSettings.WSA.certificateNotAfterRaw;
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
			internal static string ValidatePackageVersion(string value)
			{
				Regex regex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				if (regex.IsMatch(value))
				{
					return value;
				}
				return "1.0.0.0";
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void InternalSetCapability(string name, string value);
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern string InternalGetCapability(string name);
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
		public sealed class PSM
		{
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
		internal static extern bool submitAnalytics
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
