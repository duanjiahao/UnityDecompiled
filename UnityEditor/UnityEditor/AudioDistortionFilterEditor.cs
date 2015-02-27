using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(AudioDistortionFilter))]
	internal class AudioDistortionFilterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (!Application.HasAdvancedLicense())
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUIContent content = new GUIContent("This is only available in the Pro version of Unity.");
				GUILayout.Label(content, EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
			}
			base.OnInspectorGUI();
		}
	}
}
