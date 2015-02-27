using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MeshRenderer))]
	internal class MeshRendererEditor : Editor
	{
		private SerializedProperty m_UseLightProbes;
		private SerializedProperty m_LightProbeAnchor;
		public void OnEnable()
		{
			this.m_UseLightProbes = base.serializedObject.FindProperty("m_UseLightProbes");
			this.m_LightProbeAnchor = base.serializedObject.FindProperty("m_LightProbeAnchor");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Editor.DrawPropertiesExcluding(base.serializedObject, new string[]
			{
				"m_UseLightProbes",
				"m_LightProbeAnchor"
			});
			EditorGUILayout.PropertyField(this.m_UseLightProbes, new GUILayoutOption[0]);
			if (this.m_UseLightProbes.boolValue)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_LightProbeAnchor, new GUIContent("Anchor Override", this.m_LightProbeAnchor.tooltip), new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
