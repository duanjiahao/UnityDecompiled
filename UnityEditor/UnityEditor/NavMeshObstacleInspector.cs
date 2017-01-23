using System;
using UnityEngine;
using UnityEngine.AI;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(NavMeshObstacle))]
	internal class NavMeshObstacleInspector : Editor
	{
		private SerializedProperty m_Shape;

		private SerializedProperty m_Center;

		private SerializedProperty m_Extents;

		private SerializedProperty m_Carve;

		private SerializedProperty m_MoveThreshold;

		private SerializedProperty m_TimeToStationary;

		private SerializedProperty m_CarveOnlyStationary;

		private void OnEnable()
		{
			this.m_Shape = base.serializedObject.FindProperty("m_Shape");
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Extents = base.serializedObject.FindProperty("m_Extents");
			this.m_Carve = base.serializedObject.FindProperty("m_Carve");
			this.m_MoveThreshold = base.serializedObject.FindProperty("m_MoveThreshold");
			this.m_TimeToStationary = base.serializedObject.FindProperty("m_TimeToStationary");
			this.m_CarveOnlyStationary = base.serializedObject.FindProperty("m_CarveOnlyStationary");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_Shape, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				base.serializedObject.ApplyModifiedProperties();
				(base.target as NavMeshObstacle).FitExtents();
				base.serializedObject.Update();
			}
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			if (this.m_Shape.enumValueIndex == 0)
			{
				EditorGUI.BeginChangeCheck();
				float num = EditorGUILayout.FloatField("Radius", this.m_Extents.vector3Value.x, new GUILayoutOption[0]);
				float num2 = EditorGUILayout.FloatField("Height", this.m_Extents.vector3Value.y * 2f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_Extents.vector3Value = new Vector3(num, num2 / 2f, num);
				}
			}
			else if (this.m_Shape.enumValueIndex == 1)
			{
				EditorGUI.BeginChangeCheck();
				Vector3 vector = this.m_Extents.vector3Value * 2f;
				vector = EditorGUILayout.Vector3Field("Size", vector, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_Extents.vector3Value = vector / 2f;
				}
			}
			EditorGUILayout.PropertyField(this.m_Carve, new GUILayoutOption[0]);
			if (this.m_Carve.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_MoveThreshold, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_TimeToStationary, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CarveOnlyStationary, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
