using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Text), true)]
	public class TextEditor : GraphicEditor
	{
		private SerializedProperty m_Text;

		private SerializedProperty m_FontData;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_Text = base.serializedObject.FindProperty("m_Text");
			this.m_FontData = base.serializedObject.FindProperty("m_FontData");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Text, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_FontData, new GUILayoutOption[0]);
			base.AppearanceControlsGUI();
			base.RaycastControlsGUI();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
