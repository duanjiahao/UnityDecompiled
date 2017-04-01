using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.BuildReporting;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class BuildPipeline
	{
		public static extern bool isBuildingPlayer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuildTargetGroup GetBuildTargetGroup(BuildTarget platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildTargetGroup GetBuildTargetGroupByName(string platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildTarget GetBuildTargetByName(string platform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetGroupDisplayName(BuildTargetGroup targetPlatformGroup);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetName(BuildTarget targetPlatform);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetEditorTargetName();

		[Obsolete("PushAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PushAssetDependencies();

		[Obsolete("PopAssetDependencies has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PopAssetDependencies();

		private static string[] InvokeCalculateBuildTags(BuildTarget target, BuildTargetGroup group)
		{
			return new string[0];
		}

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
			return BuildPipeline.BuildPlayer(new BuildPlayerOptions
			{
				scenes = EditorBuildSettingsScene.GetActiveSceneList(levels),
				locationPathName = locationPathName,
				target = target,
				options = options
			});
		}

		public static string BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
			return BuildPipeline.BuildPlayer(new BuildPlayerOptions
			{
				scenes = levels,
				locationPathName = locationPathName,
				targetGroup = buildTargetGroup,
				target = target,
				options = options
			});
		}

		public static string BuildPlayer(BuildPlayerOptions buildPlayerOptions)
		{
			return BuildPipeline.BuildPlayer(buildPlayerOptions.scenes, buildPlayerOptions.locationPathName, buildPlayerOptions.assetBundleManifestPath, buildPlayerOptions.targetGroup, buildPlayerOptions.target, buildPlayerOptions.options);
		}

		private static string BuildPlayer(string[] scenes, string locationPathName, string assetBundleManifestPath, BuildTargetGroup buildTargetGroup, BuildTarget target, BuildOptions options)
		{
			string result;
			if (BuildPipeline.isBuildingPlayer)
			{
				result = "Cannot start a new build because there is already a build in progress.";
			}
			else
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown)
				{
					buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
				}
				if (string.IsNullOrEmpty(locationPathName))
				{
					if ((options & BuildOptions.InstallInBuildFolder) == BuildOptions.None || !PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTargetGroup, target))
					{
						result = "The 'locationPathName' parameter for BuildPipeline.BuildPlayer should not be null or empty.";
						return result;
					}
				}
				else if (string.IsNullOrEmpty(Path.GetFileName(locationPathName)))
				{
					string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(buildTargetGroup, target, options);
					if (!string.IsNullOrEmpty(extensionForBuildTarget))
					{
						result = string.Format("For the '{0}' target the 'locationPathName' parameter for BuildPipeline.BuildPlayer should not end with a directory separator.\nProvided path: '{1}', expected a path with the extension '.{2}'.", target, locationPathName, extensionForBuildTarget);
						return result;
					}
				}
				try
				{
					result = BuildPipeline.BuildPlayerInternal(scenes, locationPathName, assetBundleManifestPath, buildTargetGroup, target, options).SummarizeErrors();
				}
				catch (Exception exception)
				{
					BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildPlayer", exception);
					result = "";
				}
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
			BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			return BuildPipeline.BuildStreamedSceneAssetBundle(levels, locationPath, activeBuildTargetGroup, target, out crc, options);
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		internal static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTargetGroup buildTargetGroup, BuildTarget target, out uint crc, BuildOptions options)
		{
			crc = 0u;
			string result;
			try
			{
				BuildReport buildReport = BuildPipeline.BuildPlayerInternal(levels, locationPath, null, buildTargetGroup, target, options | BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.ComputeCRC);
				crc = buildReport.crc;
				string text = buildReport.SummarizeErrors();
				UnityEngine.Object.DestroyImmediate(buildReport, true);
				result = text;
			}
			catch (Exception exception)
			{
				BuildPipeline.LogBuildExceptionAndExit("BuildPipeline.BuildStreamedSceneAssetBundle", exception);
				result = "";
			}
			return result;
		}

		[Obsolete("BuildStreamedSceneAssetBundle has been made obsolete. Please use the new AssetBundle build system introduced in 5.0 and check BuildAssetBundles documentation for details.")]
		public static string BuildStreamedSceneAssetBundle(string[] levels, string locationPath, BuildTarget target, out uint crc)
		{
			return BuildPipeline.BuildStreamedSceneAssetBundle(levels, locationPath, target, out crc, BuildOptions.None);
		}

		private static BuildReport BuildPlayerInternal(string[] levels, string locationPathName, string assetBundleManifestPath, BuildTargetGroup buildTargetGroup, BuildTarget target, BuildOptions options)
		{
			if ((BuildOptions.EnableHeadlessMode & options) != BuildOptions.None && (BuildOptions.Development & options) != BuildOptions.None)
			{
				throw new Exception("Unsupported build setting: cannot build headless development player");
			}
			return BuildPipeline.BuildPlayerInternalNoCheck(levels, locationPathName, assetBundleManifestPath, buildTargetGroup, target, options, false);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern BuildReport BuildPlayerInternalNoCheck(string[] levels, string locationPathName, string assetBundleManifestPath, BuildTargetGroup buildTargetGroup, BuildTarget target, BuildOptions options, bool delayToAfterScriptReload);

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
			BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, out crc, assetBundleOptions, activeBuildTargetGroup, targetPlatform);
		}

		internal static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform)
		{
			crc = 0u;
			bool result;
			try
			{
				result = BuildPipeline.BuildAssetBundleInternal(mainAsset, assets, null, pathName, assetBundleOptions, targetPlatformGroup, targetPlatform, out crc);
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
			BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			return BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames, pathName, out crc, assetBundleOptions, activeBuildTargetGroup, targetPlatform);
		}

		internal static bool BuildAssetBundleExplicitAssetNames(UnityEngine.Object[] assets, string[] assetNames, string pathName, out uint crc, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform)
		{
			crc = 0u;
			bool result;
			try
			{
				result = BuildPipeline.BuildAssetBundleInternal(null, assets, assetNames, pathName, assetBundleOptions, targetPlatformGroup, targetPlatform, out crc);
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool BuildAssetBundleInternal(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string[] assetNames, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform, out uint crc);

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
			BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			return BuildPipeline.BuildAssetBundles(outputPath, assetBundleOptions, activeBuildTargetGroup, targetPlatform);
		}

		internal static AssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform)
		{
			if (!Directory.Exists(outputPath))
			{
				throw new ArgumentException("The output path \"" + outputPath + "\" doesn't exist");
			}
			return BuildPipeline.BuildAssetBundlesInternal(outputPath, assetBundleOptions, targetPlatformGroup, targetPlatform);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleManifest BuildAssetBundlesInternal(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform);

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
			BuildTargetGroup activeBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			return BuildPipeline.BuildAssetBundles(outputPath, builds, assetBundleOptions, activeBuildTargetGroup, targetPlatform);
		}

		internal static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform)
		{
			if (!Directory.Exists(outputPath))
			{
				throw new ArgumentException("The output path \"" + outputPath + "\" doesn't exist");
			}
			if (builds == null)
			{
				throw new ArgumentException("AssetBundleBuild cannot be null.");
			}
			return BuildPipeline.BuildAssetBundlesWithInfoInternal(outputPath, builds, assetBundleOptions, targetPlatformGroup, targetPlatform);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleManifest BuildAssetBundlesWithInfoInternal(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTargetGroup targetPlatformGroup, BuildTarget targetPlatform);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetCRCForAssetBundle(string targetPath, out uint crc);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetHashForAssetBundle(string targetPath, out Hash128 hash);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LicenseCheck(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsBuildTargetSupported(BuildTargetGroup buildTargetGroup, BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsBuildTargetCompatibleWithOS(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetAdvancedLicenseName(BuildTarget target);

		internal static string GetPlaybackEngineDirectory(BuildTarget target, BuildOptions options)
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
			return BuildPipeline.GetPlaybackEngineDirectory(buildTargetGroup, target, options);
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPlaybackEngineDirectory(BuildTargetGroup buildTargetGroup, BuildTarget target, BuildOptions options);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetPlaybackEngineExtensionDirectory(BuildTargetGroup buildTargetGroup, BuildTarget target, BuildOptions options);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPlaybackEngineDirectory(BuildTarget target, BuildOptions options, string playbackEngineDirectory);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildToolsDirectory(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMonoBinDirectory(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMonoLibDirectory(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string CompatibilityProfileToClassLibFolder(ApiCompatibilityLevel compatibilityLevel);

		internal static string GetBuildTargetGroupName(BuildTarget target)
		{
			return BuildPipeline.GetBuildTargetGroupName(BuildPipeline.GetBuildTargetGroup(target));
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetBuildTargetGroupName(BuildTargetGroup buildTargetGroup);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsUnityScriptEvalSupported(BuildTarget target);
	}
}
