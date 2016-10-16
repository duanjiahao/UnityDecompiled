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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool realtimeGI
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool bakedGI
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float indirectOutputScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float bounceBoost
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool openRLEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern Lightmapping.ConcurrentJobsType concurrentJobsType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern long diskCacheSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern string diskCachePath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool enlightenForceWhiteAlbedo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern bool enlightenForceUpdates
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern FilterMode filterMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isRunning
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float buildProgress
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern LightingDataAsset lightingDataAsset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearPrecompSetIsDone();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDiskCache();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateCachePath();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PrintStateToConsole();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeAsync();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Bake();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeSelectedAsync();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeSelected();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeLightProbesOnlyAsync();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeLightProbesOnly();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Cancel();

		private static void Internal_CallCompletedFunctions()
		{
			if (Lightmapping.completed != null)
			{
				Lightmapping.completed();
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Clear();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLightingDataAsset();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Tetrahedralize(Vector3[] positions, out int[] outIndices, out Vector3[] outPositions);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BakeReflectionProbe(ReflectionProbe probe, string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool BakeReflectionProbeSnapshot(ReflectionProbe probe);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool BakeAllReflectionProbesSnapshots();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetTerrainGIChunks(Terrain terrain, ref int numChunksX, ref int numChunksY);

		public static void BakeMultipleScenes(string[] paths)
		{
			if (paths.Length == 0)
			{
				return;
			}
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
			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				return;
			}
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
