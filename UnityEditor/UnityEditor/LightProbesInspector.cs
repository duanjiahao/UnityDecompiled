using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(LightProbes))]
	internal class LightProbesInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			LightProbes lightProbes = this.target as LightProbes;
			GUIStyle wordWrappedMiniLabel = EditorStyles.wordWrappedMiniLabel;
			GUILayout.Label("Light probe count: " + lightProbes.count, wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.Label("Cell count: " + lightProbes.cellCount, wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}
	}
}
