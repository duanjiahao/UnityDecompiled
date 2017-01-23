using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Toggle), true)]
	public class ToggleEditor : SelectableEditor
	{
		private SerializedProperty m_OnValueChangedProperty;

		private SerializedProperty m_TransitionProperty;

		private SerializedProperty m_GraphicProperty;

		private SerializedProperty m_GroupProperty;

		private SerializedProperty m_IsOnProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_TransitionProperty = base.serializedObject.FindProperty("toggleTransition");
			this.m_GraphicProperty = base.serializedObject.FindProperty("graphic");
			this.m_GroupProperty = base.serializedObject.FindProperty("m_Group");
			this.m_IsOnProperty = base.serializedObject.FindProperty("m_IsOn");
			this.m_OnValueChangedProperty = base.serializedObject.FindProperty("onValueChanged");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_IsOnProperty, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GraphicProperty, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GroupProperty, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_OnValueChangedProperty, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
