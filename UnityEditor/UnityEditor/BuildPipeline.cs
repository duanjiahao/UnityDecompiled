using System;
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
		internal static extern string GetBuildTargetGroupDisplayName(BuildTargetGroup targetPlatformGroup);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PushAssetDependencies();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PopAssetDependencies();
		private static void LogBuildExceptionAndExit(string buildFunctionName, Exception exception)
		{
			Debug.LogError(string.Format("Internal Error in {0}:", buildFunctionName));
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
		[ExcludeFromDocs]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target)
		{
			BuildOptions options = BuildOptions.None;
			return BuildPipeline.BuildStreamedSceneAssetBundle(levels, locationPath, target, options);
		}
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, [DefaultValue("0")] BuildOptions options)
		{
			return BuildPipeline.BuildPlayer(levels, locationPath, target, options | BuildOptions.BuildAdditionalStreamedScenes);
		}
		[ExcludeFromDocs]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc)
		{
			BuildOptions options = BuildOptions.None;
			return BuildPipeline.BuildStreamedSceneAssetBundle(levels, locationPath, target, out crc, options);
		}
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc, [DefaultValue("0")] BuildOptions options)
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
		private static string BuildPlayerInternal(string[] levels, string locationPathName, BuildTarget target, BuildOptions options, out uint crc)
		{
			crc = 0u;
			if (!InternalEditorUtility.HasPro())
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
		[ExcludeFromDocs]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions, targetPlatform);
		}
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, [DefaultValue("BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out num, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, [DefaultValue("BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
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
		[ExcludeFromDocs]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, assetBundleOptions, targetPlatform);
		}
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, [DefaultValue("BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
		{
			uint num;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out num, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		[ExcludeFromDocs]
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc)
		{
			BuildTarget targetPlatform = BuildTarget.WebPlayer;
			BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions, targetPlatform);
		}
		public static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, [DefaultValue("BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets")] BuildAssetBundleOptions assetBundleOptions, [DefaultValue("BuildTarget.WebPlayer")] BuildTarget targetPlatform)
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BuildAssetBundleInternal(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform, out uint crc);
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
