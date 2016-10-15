using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.BuildReporting;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public sealed class BuildPipeline
	{
		public static extern bool isBuildingPlayer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuildTargetGroup GetBuildTargetGroup(BuildTarget platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildTargetGroup GetBuildTargetGroupByName(string platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildTarget GetBuildTargetByName(string platform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetGroupDisplayName(BuildTargetGroup targetPlatformGroup);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetName(BuildTarget targetPlatform);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetEditorTargetName();

		[Obsolete("PushAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PushAssetDependencies();

		[Obsolete("PopAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PopAssetDependencies();

		private static void LogBuildExceptionAndExit(string buildFunctionName, Exception exception)
		{
			Debug.LogErrorFormat("Internal Error in {0}:", new object[]
			{
				buildFunctionName
			});
			Debug.LogException(exception);
			EditorApplication.Exit(1);
		}

		public static string BuildPlayer(EditorBuildSettingsScene[] levels, string locationPathName, BuildTarget target, BuildOptions options)
		{
			string[] levels2 = null;
			if (levels != null)
			{
				levels2 = (from l in levels
				where l.enabled
				select l.path).ToArray<string>();
			}
			return BuildPipeline.BuildPlayer(levels2, locationPathName, target, options);
		}

		public static string BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
		{
			if (string.IsNullOrEmpty(locationPathName))
			{
				if ((options & BuildOptions.InstallInBuildFolder) == BuildOptions.None || !PostprocessBuildPlayer.SupportsInstallInBuildFolder(target))
				{
					return "The 'locationPathName' parameter for BuildPipeline.BuildPlayer should not be null or empty.";
				}
			}
			else if (string.IsNullOrEmpty(Path.GetFileName(locationPathName)))
			{
				string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(target, options);
				if (!string.IsNullOrEmpty(extensionForBuildTarget))
				{
					return string.Format("For the '{0}' target the 'locationPathName' parameter for BuildPipeline.BuildPlayer should not end with a directory separator.\nProvided path: '{1}', expected a path with the extension '.{2}'.", target, locationPathName, extensionForBuildTarget);
				}
			}
			string result;
			try
			{
				result = BuildPipeline.BuildPlayerInternal(levels, locationPathName, target, options).SummarizeErrors();
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildPlayer", exception);
				result = string.Empty;
			}
			return result;
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, BuildOptions options)
		{
			return BuildPipeline.BuildPlayer(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes);
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target)
		{
			return BuildPipeline.BuildPlayer(levels, locationPath, target, BuildOptions.BuildAdditionalStreamedScenes);
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc, BuildOptions options)
		{
			crc = 0u;
			string result;
			try
			{
				BuildReport buildReport = BuildPipeline.BuildPlayerInternal(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.ComputeCRC);
				crc = buildReport.crc;
				string text = buildReport.SummarizeErrors();
				UnityEngine.Object.DestroyImmediate(buildReport, true);
				result = text;
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildStreamedSceneAssetBundle", exception);
				result = string.Empty;
			}
			return result;
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc)
		{
			return BuildPipeline.BuildStreamedSceneAssetBundle(levels, locationPath, target, out crc, BuildOptions.None);
		}

		private static BuildReport BuildPlayerInternal(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
		{
			if ((BuildOptions.EnableHeadlessMode & options) != BuildOptions.None && (BuildOptions.Development & options) != BuildOptions.None)
			{
				throw new Exception("Unsupported build setting: cannot build headless development player");
			}
			if (target == BuildTarget.WSAPlayer && EditorUserBuildSettings.wsaSDK == WSASDK.SDK80)
			{
				throw new Exception("Windows SDK 8.0 is no longer supported, please switch to Windows SDK 8.1");
			}
			return BuildPipeline.BuildPlayerInternalNoCheck(levels, locationPathName, target, options, false);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildReport BuildPlayerInternalNoCheck(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, bool delayToAfterScriptReload);

		[Obsolete("WebPlayer has been removed in 5.4", true)]
		private static bool WebPlayerAssetBundlesAreNoLongerSupported()
		{
			throw new InvalidOperationException("WebPlayer asset bundles can no longer be built in 5.4+");
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out num, assetBundleOptions, targetPlatform);
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName)
		{
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			crc = 0u;
			bool result;
			try
			{
				result = BuildPipeline.BuildAssetBundleInternal(mainAsset, assets, null, pathName, assetBundleOptions, targetPlatform, out crc);
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundle", exception);
				result = false;
			}
			return result;
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			crc = 0u;
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc)
		{
			crc = 0u;
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out num, assetBundleOptions, targetPlatform);
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName)
		{
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			crc = 0u;
			bool result;
			try
			{
				result = BuildPipeline.BuildAssetBundleInternal(null, assets, assetNames, pathName, assetBundleOptions, targetPlatform, out crc);
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundleExplicitAssetNames", exception);
				result = false;
			}
			return result;
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			crc = 0u;
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.", true)]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc)
		{
			crc = 0u;
			return BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BuildAssetBundleInternal(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform, out uint crc);

		[Obsolete("BuildAssetBundles signature has changed. Please specify the targetPlatform parameter", true), ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;
			return BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions);
		}

		[Obsolete("BuildAssetBundles signature has changed. Please specify the targetPlatform parameter", true)]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions)
		{
			BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
			return null;
		}

		public static AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			if (!Directory.Exists(outputPath))
			{
				Debug.LogError("The output path \"" + outputPath + "\" doesn't exist");
				return null;
			}
			AssetBundleManifest result;
			try
			{
				result = BuildPipeline.BuildAssetBundlesInternal(outputPath, assetBundleOptions, targetPlatform);
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundles", exception);
				result = null;
			}
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleManifest BuildAssetBundlesInternal(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform);

		[Obsolete("BuildAssetBundles signature has changed. Please specify the targetPlatform parameter", true), ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;
			return BuildPipeline.BuildAssetBundles(outputPath, builds, assetBundleOptions);
		}

		[Obsolete("BuildAssetBundles signature has changed. Please specify the targetPlatform parameter", true)]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions)
		{
			BuildPipeline.WebPlayerAssetBundlesAreNoLongerSupported();
			return null;
		}

		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			if (!Directory.Exists(outputPath))
			{
				Debug.LogError("The output path \"" + outputPath + "\" doesn't exist");
				return null;
			}
			if (builds == null)
			{
				Debug.LogError("AssetBundleBuild cannot be null.");
				return null;
			}
			AssetBundleManifest result;
			try
			{
				result = BuildPipeline.BuildAssetBundlesWithInfoInternal(outputPath, builds, assetBundleOptions, targetPlatform);
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildAssetBundles", exception);
				result = null;
			}
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleManifest BuildAssetBundlesWithInfoInternal(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetCRCForAssetBundle(string targetPath, out uint crc);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetHashForAssetBundle(string targetPath, out Hash128 hash);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LicenseCheck(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsBuildTargetSupported(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetAdvancedLicenseName(BuildTarget target);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPlaybackEngineDirectory(BuildTarget target, BuildOptions options);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPlaybackEngineDirectory(BuildTarget target, BuildOptions options, string playbackEngineDirectory);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildToolsDirectory(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMonoBinDirectory(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMonoLibDirectory(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMonoProfileLibDirectory(BuildTarget target, string profile);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetGroupName(BuildTarget target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsUnityScriptEvalSupported(BuildTarget target);
	}
}
