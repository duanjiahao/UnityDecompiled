using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(NavMeshAgent))]
	internal class NavMeshAgentInspector : Editor
	{
		private SerializedProperty m_WalkableMask;
		private SerializedProperty m_Radius;
		private SerializedProperty m_Speed;
		private SerializedProperty m_Acceleration;
		private SerializedProperty m_AngularSpeed;
		private SerializedProperty m_StoppingDistance;
		private SerializedProperty m_AutoTraverseOffMeshLink;
		private SerializedProperty m_AutoBraking;
		private SerializedProperty m_AutoRepath;
		private SerializedProperty m_Height;
		private SerializedProperty m_BaseOffset;
		private SerializedProperty m_ObstacleAvoidanceType;
		private SerializedProperty m_AvoidancePriority;
		private void OnEnable()
		{
			this.m_WalkableMask = base.serializedObject.FindProperty("m_WalkableMask");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_Speed = base.serializedObject.FindProperty("m_Speed");
			this.m_Acceleration = base.serializedObject.FindProperty("m_Acceleration");
			this.m_AngularSpeed = base.serializedObject.FindProperty("m_AngularSpeed");
			this.m_StoppingDistance = base.serializedObject.FindProperty("m_StoppingDistance");
			this.m_AutoTraverseOffMeshLink = base.serializedObject.FindProperty("m_AutoTraverseOffMeshLink");
			this.m_AutoBraking = base.serializedObject.FindProperty("m_AutoBraking");
			this.m_AutoRepath = base.serializedObject.FindProperty("m_AutoRepath");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_BaseOffset = base.serializedObject.FindProperty("m_BaseOffset");
			this.m_ObstacleAvoidanceType = base.serializedObject.FindProperty("m_ObstacleAvoidanceType");
			this.m_AvoidancePriority = base.serializedObject.FindProperty("avoidancePriority");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Speed, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Acceleration, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AngularSpeed, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_StoppingDistance, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoTraverseOffMeshLink, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoBraking, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoRepath, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BaseOffset, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ObstacleAvoidanceType, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AvoidancePriority, new GUILayoutOption[0]);
			string[] navMeshLayerNames = GameObjectUtility.GetNavMeshLayerNames();
			int intValue = this.m_WalkableMask.intValue;
			int num = 0;
			for (int i = 0; i < navMeshLayerNames.Length; i++)
			{
				int navMeshLayerFromName = GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[i]);
				if ((1 << navMeshLayerFromName & intValue) > 0)
				{
					num |= 1 << i;
				}
			}
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.layerMaskField);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_WalkableMask.hasMultipleDifferentValues;
			int num2 = EditorGUI.MaskField(rect, "NavMesh Walkable", num, navMeshLayerNames, EditorStyles.layerMaskField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				if (num2 == -1)
				{
					this.m_WalkableMask.intValue = -1;
				}
				else
				{
					int num3 = 0;
					for (int j = 0; j < navMeshLayerNames.Length; j++)
					{
						if ((num2 >> j & 1) > 0)
						{
							num3 |= 1 << GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[j]);
						}
					}
					this.m_WalkableMask.intValue = num3;
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
