using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(WheelCollider))]
	internal class WheelColliderEditor : Editor
	{
		private SerializedProperty m_Center;

		private SerializedProperty m_Radius;

		private SerializedProperty m_SuspensionDistance;

		private SerializedProperty m_SuspensionSpring;

		private SerializedProperty m_ForceAppPointDistance;

		private SerializedProperty m_Mass;

		private SerializedProperty m_WheelDampingRate;

		private SerializedProperty m_ForwardFriction;

		private SerializedProperty m_SidewaysFriction;

		public void OnEnable()
		{
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_SuspensionDistance = base.serializedObject.FindProperty("m_SuspensionDistance");
			this.m_SuspensionSpring = base.serializedObject.FindProperty("m_SuspensionSpring");
			this.m_Mass = base.serializedObject.FindProperty("m_Mass");
			this.m_ForceAppPointDistance = base.serializedObject.FindProperty("m_ForceAppPointDistance");
			this.m_WheelDampingRate = base.serializedObject.FindProperty("m_WheelDampingRate");
			this.m_ForwardFriction = base.serializedObject.FindProperty("m_ForwardFriction");
			this.m_SidewaysFriction = base.serializedObject.FindProperty("m_SidewaysFriction");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Mass, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_WheelDampingRate, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SuspensionDistance, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_ForceAppPointDistance, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			StructPropertyGUILayout.GenericStruct(this.m_SuspensionSpring, new GUILayoutOption[0]);
			StructPropertyGUILayout.GenericStruct(this.m_ForwardFriction, new GUILayoutOption[0]);
			StructPropertyGUILayout.GenericStruct(this.m_SidewaysFriction, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
