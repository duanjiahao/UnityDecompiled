using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Button), true)]
	public class ButtonEditor : SelectableEditor
	{
		private SerializedProperty m_OnClickProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_OnClickProperty = base.serializedObject.FindProperty("m_OnClick");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_OnClickProperty, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
