using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class LightProbeVisualization
	{
		internal enum LightProbeVisualizationMode
		{
			OnlyProbesUsedBySelection,
			AllProbesNoCells,
			AllProbesWithCells,
			None
		}

		public static extern LightProbeVisualization.LightProbeVisualizationMode lightProbeVisualizationMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showInterpolationWeights
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showOcclusions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool dynamicUpdateLightProbes
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static void DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, Color baseColor, Color selectedColor, float scale, Transform cloudTransform)
		{
			LightProbeVisualization.INTERNAL_CALL_DrawPointCloud(unselectedPositions, selectedPositions, ref baseColor, ref selectedColor, scale, cloudTransform);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, ref Color baseColor, ref Color selectedColor, float scale, Transform cloudTransform);

		internal static void DrawTetrahedra(bool shouldRecalculateTetrahedra, Vector3 cameraPosition)
		{
			LightProbeVisualization.INTERNAL_CALL_DrawTetrahedra(shouldRecalculateTetrahedra, ref cameraPosition);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawTetrahedra(bool shouldRecalculateTetrahedra, ref Vector3 cameraPosition);
	}
}
