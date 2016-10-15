using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(OffMeshLink))]
	internal class OffMeshLinkInspector : Editor
	{
		private SerializedProperty m_AreaIndex;

		private SerializedProperty m_Start;

		private SerializedProperty m_End;

		private SerializedProperty m_CostOverride;

		private SerializedProperty m_BiDirectional;

		private SerializedProperty m_Activated;

		private SerializedProperty m_AutoUpdatePositions;

		private void OnEnable()
		{
			this.m_AreaIndex = base.serializedObject.FindProperty("m_AreaIndex");
			this.m_Start = base.serializedObject.FindProperty("m_Start");
			this.m_End = base.serializedObject.FindProperty("m_End");
			this.m_CostOverride = base.serializedObject.FindProperty("m_CostOverride");
			this.m_BiDirectional = base.serializedObject.FindProperty("m_BiDirectional");
			this.m_Activated = base.serializedObject.FindProperty("m_Activated");
			this.m_AutoUpdatePositions = base.serializedObject.FindProperty("m_AutoUpdatePositions");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Start, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_End, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_CostOverride, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BiDirectional, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Activated, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoUpdatePositions, new GUILayoutOption[0]);
			this.SelectNavMeshArea();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void SelectNavMeshArea()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_AreaIndex.hasMultipleDifferentValues;
			string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
			int intValue = this.m_AreaIndex.intValue;
			int selectedIndex = -1;
			for (int i = 0; i < navMeshAreaNames.Length; i++)
			{
				if (GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]) == intValue)
				{
					selectedIndex = i;
					break;
				}
			}
			int num = EditorGUILayout.Popup("Navigation Area", selectedIndex, navMeshAreaNames, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[num]);
				this.m_AreaIndex.intValue = navMeshAreaFromName;
			}
		}
	}
}
