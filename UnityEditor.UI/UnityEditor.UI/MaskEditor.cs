using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(Mask), true)]
	public class MaskEditor : Editor
	{
		private SerializedProperty m_ShowMaskGraphic;

		protected virtual void OnEnable()
		{
			this.m_ShowMaskGraphic = base.serializedObject.FindProperty("m_ShowMaskGraphic");
		}

		public override void OnInspectorGUI()
		{
			Graphic component = (base.target as Mask).GetComponent<Graphic>();
			if (component && !component.IsActive())
			{
				EditorGUILayout.HelpBox("Masking disabled due to Graphic component being disabled.", MessageType.Warning);
			}
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_ShowMaskGraphic, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
