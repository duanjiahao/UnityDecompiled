using System;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Lightmapping
	{
		internal enum ConcurrentJobsType
		{
			Min,
			Low,
			High
		}

		public enum GIWorkflowMode
		{
			Iterative,
			OnDemand,
			Legacy
		}

		public delegate void OnCompletedFunction();

		public static Lightmapping.OnCompletedFunction completed;

		public static extern Lightmapping.GIWorkflowMode giWorkflowMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool realtimeGI
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool bakedGI
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float indirectOutputScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float bounceBoost
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool openRLEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern Lightmapping.ConcurrentJobsType concurrentJobsType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern long diskCacheSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string diskCachePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool enlightenForceWhiteAlbedo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool enlightenForceUpdates
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern FilterMode filterMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isRunning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float buildProgress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern LightingDataAsset lightingDataAsset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("lightmapSnapshot has been deprecated. Use lightingDataAsset instead (UnityUpgradable) -> lightingDataAsset", true)]
		public static LightmapSnapshot lightmapSnapshot
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearPrecompSetIsDone();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDiskCache();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateCachePath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PrintStateToConsole();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeAsync();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Bake();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Cancel();

		private static void Internal_CallCompletedFunctions()
		{
			if (Lightmapping.completed != null)
			{
				Lightmapping.completed();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Clear();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLightingDataAsset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Tetrahedralize(Vector3[] positions, out int[] outIndices, out Vector3[] outPositions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeReflectionProbe(ReflectionProbe probe, string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool BakeReflectionProbeSnapshot(ReflectionProbe probe);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool BakeAllReflectionProbesSnapshots();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetTerrainGIChunks(Terrain terrain, ref int numChunksX, ref int numChunksY);

		public static void BakeMultipleScenes(string[] paths)
		{
			if (paths.Length != 0)
			{
				for (int i = 0; i < paths.Length; i++)
				{
					for (int j = i + 1; j < paths.Length; j++)
					{
						if (paths[i] == paths[j])
						{
							throw new Exception("no duplication of scenes is allowed");
						}
					}
				}
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					SceneSetup[] sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();
					EditorSceneManager.OpenScene(paths[0]);
					for (int k = 1; k < paths.Length; k++)
					{
						EditorSceneManager.OpenScene(paths[k], OpenSceneMode.Additive);
					}
					Lightmapping.Bake();
					EditorSceneManager.SaveOpenScenes();
					EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
				}
			}
		}

		[Obsolete("BakeSelectedAsync has been deprecated. Use BakeAsync instead (UnityUpgradable) -> BakeAsync()", true)]
		public static bool BakeSelectedAsync()
		{
			return false;
		}

		[Obsolete("BakeSelected has been deprecated. Use Bake instead (UnityUpgradable) -> Bake()", true)]
		public static bool BakeSelected()
		{
			return false;
		}

		[Obsolete("BakeLightProbesOnlyAsync has been deprecated. Use BakeAsync instead (UnityUpgradable) -> BakeAsync()", true)]
		public static bool BakeLightProbesOnlyAsync()
		{
			return false;
		}

		[Obsolete("BakeLightProbesOnly has been deprecated. Use Bake instead (UnityUpgradable) -> Bake()", true)]
		public static bool BakeLightProbesOnly()
		{
			return false;
		}
	}
}
