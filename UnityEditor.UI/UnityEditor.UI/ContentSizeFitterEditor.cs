using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(ContentSizeFitter), true)]
	public class ContentSizeFitterEditor : SelfControllerEditor
	{
		private SerializedProperty m_HorizontalFit;

		private SerializedProperty m_VerticalFit;

		protected virtual void OnEnable()
		{
			this.m_HorizontalFit = base.serializedObject.FindProperty("m_HorizontalFit");
			this.m_VerticalFit = base.serializedObject.FindProperty("m_VerticalFit");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_HorizontalFit, true, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_VerticalFit, true, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
		}
	}
}
