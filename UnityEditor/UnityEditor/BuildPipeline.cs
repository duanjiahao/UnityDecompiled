using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
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
		internal static extern BuildTargetGroup GetBuildTargetGroup(BuildTarget platform);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildTargetGroup GetBuildTargetGroupByName(string platform);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetGroupDisplayName(BuildTargetGroup targetPlatformGroup);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetName(BuildTarget targetPlatform);
		[WrapperlessIcall]
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
		public static string BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
		{
			string result;
			try
			{
				uint num;
				result = BuildPipeline.BuildPlayerInternal(levels, locationPathName, target, options, out num);
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
				result = BuildPipeline.BuildPlayerInternal(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes, out crc);
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
		private static string BuildPlayerInternal(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, out uint crc)
		{
			crc = 0u;
			if (!InternalEditorUtility.HasProFeaturesEnabled())
			{
				Debug.LogError("Building Player from editor scripts requires Unity PRO");
				return "Building Player from editor scripts requires Unity PRO";
			}
			if ((BuildOptions.EnableHeadlessMode & options) != BuildOptions.None && (BuildOptions.Development & options) != BuildOptions.None)
			{
				return "Unsupported build setting: cannot build headless development player";
			}
			return BuildPipeline.BuildPlayerInternalNoCheck(levels, locationPathName, target, options, false, out crc);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string BuildPlayerInternalNoCheck(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, bool delayToAfterScriptReload, out uint crc);
		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out num, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions);
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
		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions);
		}
		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out num, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions);
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
		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		[Obsolete("BuildAssetBundleExplicitAssetNames has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc)
		{
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions);
		}
		internal static bool DoesBuildTargetSupportIl2cpp(BuildTarget target)
		{
			return BuildPipeline.GetPlaybackEngineDirectory(target, BuildOptions.Il2CPP) != string.Empty;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BuildAssetBundleInternal(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform, out uint crc);
		[ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;
			return BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, targetPlatform);
		}
		public static AssetBundleManifest BuildAssetBundles(string outputPath, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
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
		[ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundles(outputPath, builds, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;
			return BuildPipeline.BuildAssetBundles(outputPath, builds, assetBundleOptions, targetPlatform);
		}
		public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, [DefaultValue("BuildAssetBundleOptions.None")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LicenseCheck(BuildTarget target);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsBuildTargetSupported(BuildTarget target);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetAdvancedLicenseName(BuildTarget target);
		[WrapperlessIcall]
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
