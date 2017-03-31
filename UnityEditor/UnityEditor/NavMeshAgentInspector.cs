using System;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(NavMeshAgent))]
	internal class NavMeshAgentInspector : Editor
	{
		private class Styles
		{
			public readonly GUIContent m_AgentSteeringHeader = new GUIContent("Steering");

			public readonly GUIContent m_AgentAvoidanceHeader = new GUIContent("Obstacle Avoidance");

			public readonly GUIContent m_AgentPathFindingHeader = new GUIContent("Path Finding");
		}

		private SerializedProperty m_AgentTypeID;

		private SerializedProperty m_Radius;

		private SerializedProperty m_Height;

		private SerializedProperty m_WalkableMask;

		private SerializedProperty m_Speed;

		private SerializedProperty m_Acceleration;

		private SerializedProperty m_AngularSpeed;

		private SerializedProperty m_StoppingDistance;

		private SerializedProperty m_AutoTraverseOffMeshLink;

		private SerializedProperty m_AutoBraking;

		private SerializedProperty m_AutoRepath;

		private SerializedProperty m_BaseOffset;

		private SerializedProperty m_ObstacleAvoidanceType;

		private SerializedProperty m_AvoidancePriority;

		private static NavMeshAgentInspector.Styles s_Styles;

		private void OnEnable()
		{
			this.m_AgentTypeID = base.serializedObject.FindProperty("m_AgentTypeID");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_WalkableMask = base.serializedObject.FindProperty("m_WalkableMask");
			this.m_Speed = base.serializedObject.FindProperty("m_Speed");
			this.m_Acceleration = base.serializedObject.FindProperty("m_Acceleration");
			this.m_AngularSpeed = base.serializedObject.FindProperty("m_AngularSpeed");
			this.m_StoppingDistance = base.serializedObject.FindProperty("m_StoppingDistance");
			this.m_AutoTraverseOffMeshLink = base.serializedObject.FindProperty("m_AutoTraverseOffMeshLink");
			this.m_AutoBraking = base.serializedObject.FindProperty("m_AutoBraking");
			this.m_AutoRepath = base.serializedObject.FindProperty("m_AutoRepath");
			this.m_BaseOffset = base.serializedObject.FindProperty("m_BaseOffset");
			this.m_ObstacleAvoidanceType = base.serializedObject.FindProperty("m_ObstacleAvoidanceType");
			this.m_AvoidancePriority = base.serializedObject.FindProperty("avoidancePriority");
		}

		public override void OnInspectorGUI()
		{
			if (NavMeshAgentInspector.s_Styles == null)
			{
				NavMeshAgentInspector.s_Styles = new NavMeshAgentInspector.Styles();
			}
			base.serializedObject.Update();
			NavMeshAgentInspector.AgentTypePopupInternal("Agent Type", this.m_AgentTypeID);
			EditorGUILayout.PropertyField(this.m_BaseOffset, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(NavMeshAgentInspector.s_Styles.m_AgentSteeringHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Speed, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AngularSpeed, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Acceleration, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_StoppingDistance, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoBraking, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(NavMeshAgentInspector.s_Styles.m_AgentAvoidanceHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ObstacleAvoidanceType, GUIContent.Temp("Quality"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AvoidancePriority, GUIContent.Temp("Priority"), new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(NavMeshAgentInspector.s_Styles.m_AgentPathFindingHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoTraverseOffMeshLink, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoRepath, new GUILayoutOption[0]);
			string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
			long longValue = this.m_WalkableMask.longValue;
			int num = 0;
			for (int i = 0; i < navMeshAreaNames.Length; i++)
			{
				int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]);
				if ((1L << (navMeshAreaFromName & 31) & longValue) != 0L)
				{
					num |= 1 << i;
				}
			}
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.layerMaskField);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_WalkableMask.hasMultipleDifferentValues;
			int num2 = EditorGUI.MaskField(rect, "Area Mask", num, navMeshAreaNames, EditorStyles.layerMaskField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				if (num2 == -1)
				{
					this.m_WalkableMask.longValue = (long)((ulong)-1);
				}
				else
				{
					uint num3 = 0u;
					for (int j = 0; j < navMeshAreaNames.Length; j++)
					{
						if ((num2 >> j & 1) != 0)
						{
							num3 |= 1u << GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[j]);
						}
					}
					this.m_WalkableMask.longValue = (long)((ulong)num3);
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private static void AgentTypePopupInternal(string labelName, SerializedProperty agentTypeID)
		{
			int num = -1;
			int settingsCount = NavMesh.GetSettingsCount();
			string[] array = new string[settingsCount + 2];
			for (int i = 0; i < settingsCount; i++)
			{
				int agentTypeID2 = NavMesh.GetSettingsByIndex(i).agentTypeID;
				string settingsNameFromID = NavMesh.GetSettingsNameFromID(agentTypeID2);
				array[i] = settingsNameFromID;
				if (agentTypeID2 == agentTypeID.intValue)
				{
					num = i;
				}
			}
			array[settingsCount] = "";
			array[settingsCount + 1] = "Open Agent Settings...";
			if (num == -1)
			{
				EditorGUILayout.HelpBox("Agent Type invalid.", MessageType.Warning);
			}
			Rect controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, new GUILayoutOption[0]);
			EditorGUI.BeginProperty(controlRect, GUIContent.none, agentTypeID);
			EditorGUI.BeginChangeCheck();
			num = EditorGUI.Popup(controlRect, labelName, num, array);
			if (EditorGUI.EndChangeCheck())
			{
				if (num >= 0 && num < settingsCount)
				{
					int agentTypeID3 = NavMesh.GetSettingsByIndex(num).agentTypeID;
					agentTypeID.intValue = agentTypeID3;
				}
				else if (num == settingsCount + 1)
				{
					NavMeshEditorHelpers.OpenAgentSettings(-1);
				}
			}
			EditorGUI.EndProperty();
		}
	}
}
