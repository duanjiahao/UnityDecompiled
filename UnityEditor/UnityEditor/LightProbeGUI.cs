using System;
using UnityEngine;
namespace UnityEditor
{
	internal class LightProbeGUI
	{
		public void DisplayControls(SceneView sceneView)
		{
			LightmapVisualization.showLightProbeLocations = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.ShowProbes"), LightmapVisualization.showLightProbeLocations, new GUILayoutOption[0]);
			EditorGUI.indentLevel++;
			LightmapVisualization.showLightProbeCells = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("LightmapEditor.DisplayControls.ShowCells"), LightmapVisualization.showLightProbeCells, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
		}
	}
}
