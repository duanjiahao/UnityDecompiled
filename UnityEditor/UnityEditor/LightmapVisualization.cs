using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class LightmapVisualization
	{
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useLightmaps
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showLightProbes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showLightProbeLocations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showLightProbeCells
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool dynamicUpdateLightProbes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetLightmapLODLevelScale(Renderer renderer);

		internal static void DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, Color baseColor, Color selectedColor, float scale, Transform cloudTransform)
		{
			LightmapVisualization.INTERNAL_CALL_DrawPointCloud(unselectedPositions, selectedPositions, ref baseColor, ref selectedColor, scale, cloudTransform);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, ref Color baseColor, ref Color selectedColor, float scale, Transform cloudTransform);

		internal static void DrawTetrahedra(bool shouldRecalculateTetrahedra, Vector3 cameraPosition)
		{
			LightmapVisualization.INTERNAL_CALL_DrawTetrahedra(shouldRecalculateTetrahedra, ref cameraPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawTetrahedra(bool shouldRecalculateTetrahedra, ref Vector3 cameraPosition);
	}
}
