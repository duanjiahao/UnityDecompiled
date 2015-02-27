using System;
using UnityEngine;
namespace UnityEditor
{
	internal class LightProbeGUI
	{
		private const int kLightProbeCoefficientCount = 27;
		private const float kDuplicateEpsilonSq = 0.1f;
		public void DisplayControls(SceneView sceneView)
		{
			LightmapVisualization.showLightProbeLocations = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.ShowProbes"), LightmapVisualization.showLightProbeLocations, new GUILayoutOption[0]);
			LightmapVisualization.showLightProbeCells = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.ShowCells"), LightmapVisualization.showLightProbeCells, new GUILayoutOption[0]);
		}
	}
}
