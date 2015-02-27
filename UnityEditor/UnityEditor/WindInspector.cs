using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(WindZone))]
	internal class WindInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			GUI.enabled = true;
			WindZone windZone = this.target as WindZone;
			if (windZone == null)
			{
				GUILayout.Label("Error: Not a WindZode", new GUILayoutOption[0]);
				return;
			}
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_Mode"), new GUILayoutOption[0]);
			GUI.enabled = (windZone.mode == WindZoneMode.Spherical);
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_Radius"), new GUILayoutOption[0]);
			GUI.enabled = true;
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_WindMain"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_WindTurbulence"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_WindPulseMagnitude"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_WindPulseFrequency"), new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
