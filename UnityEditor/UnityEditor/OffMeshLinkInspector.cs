using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(OffMeshLink))]
	internal class OffMeshLinkInspector : Editor
	{
		private SerializedProperty m_NavMeshLayer;
		private SerializedProperty m_Start;
		private SerializedProperty m_End;
		private SerializedProperty m_CostOverride;
		private SerializedProperty m_BiDirectional;
		private SerializedProperty m_Activated;
		private SerializedProperty m_AutoUpdatePositions;
		private void OnEnable()
		{
			this.m_NavMeshLayer = base.serializedObject.FindProperty("m_NavMeshLayer");
			this.m_Start = base.serializedObject.FindProperty("m_Start");
			this.m_End = base.serializedObject.FindProperty("m_End");
			this.m_CostOverride = base.serializedObject.FindProperty("m_CostOverride");
			this.m_BiDirectional = base.serializedObject.FindProperty("m_BiDirectional");
			this.m_Activated = base.serializedObject.FindProperty("m_Activated");
			this.m_AutoUpdatePositions = base.serializedObject.FindProperty("m_AutoUpdatePositions");
		}
		public override void OnInspectorGUI()
		{
			if (!Application.HasProLicense())
			{
				EditorGUILayout.HelpBox("This is only available in the Pro version of Unity.", MessageType.Warning);
				GUI.enabled = false;
			}
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Start, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_End, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_CostOverride, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_BiDirectional, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Activated, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_AutoUpdatePositions, new GUILayoutOption[0]);
			this.SelectNavMeshLayer();
			base.serializedObject.ApplyModifiedProperties();
		}
		private void SelectNavMeshLayer()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_NavMeshLayer.hasMultipleDifferentValues;
			string[] navMeshLayerNames = GameObjectUtility.GetNavMeshLayerNames();
			int intValue = this.m_NavMeshLayer.intValue;
			int selectedIndex = -1;
			for (int i = 0; i < navMeshLayerNames.Length; i++)
			{
				if (GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[i]) == intValue)
				{
					selectedIndex = i;
					break;
				}
			}
			int num = EditorGUILayout.Popup("Navigation Layer", selectedIndex, navMeshLayerNames, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				int navMeshLayerFromName = GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[num]);
				this.m_NavMeshLayer.intValue = navMeshLayerFromName;
			}
		}
	}
}
