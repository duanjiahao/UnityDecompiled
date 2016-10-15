using System;
using UnityEngine;

namespace UnityEditor
{
	internal class Collider3DEditorBase : ColliderEditorBase
	{
		protected SerializedProperty m_Material;

		protected SerializedProperty m_IsTrigger;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
