using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(NavMeshObstacle))]
	internal class NavMeshObstacleInspector : Editor
	{
		private SerializedProperty m_Radius;
		private SerializedProperty m_Height;
		private SerializedProperty m_MoveThreshold;
		private SerializedProperty m_Carve;
		private void OnEnable()
		{
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_MoveThreshold = base.serializedObject.FindProperty("m_MoveThreshold");
			this.m_Carve = base.serializedObject.FindProperty("m_Carve");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			if (!Application.HasProLicense())
			{
				EditorGUILayout.HelpBox("This is only available in the Pro version of Unity.", MessageType.Warning);
				GUI.enabled = false;
			}
			EditorGUILayout.PropertyField(this.m_MoveThreshold, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Carve, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
