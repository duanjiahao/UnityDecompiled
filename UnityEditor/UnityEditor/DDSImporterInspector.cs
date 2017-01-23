using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(DDSImporter))]
	internal class DDSImporterInspector : AssetImporterInspector
	{
		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			SerializedProperty serializedProperty = base.serializedObject.FindProperty("m_IsReadable");
			EditorGUI.showMixedValue = serializedProperty.hasMultipleDifferentValues;
			bool boolValue = EditorGUILayout.Toggle("IsReadable", serializedProperty.boolValue, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				serializedProperty.boolValue = boolValue;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			base.ApplyRevertGUI();
			GUILayout.EndHorizontal();
		}
	}
}
