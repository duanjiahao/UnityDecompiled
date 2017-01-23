using System;
using UnityEngine;

namespace UnityEditor
{
	internal class StructPropertyGUILayout
	{
		internal static void GenericStruct(SerializedProperty property, params GUILayoutOption[] options)
		{
			float num = 16f + 16f * (float)StructPropertyGUILayout.GetChildrenCount(property);
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, num, num, EditorStyles.layerMaskField, options);
			StructPropertyGUI.GenericStruct(rect, property);
		}

		internal static int GetChildrenCount(SerializedProperty property)
		{
			int num = 0;
			SerializedProperty serializedProperty = property.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			while (!SerializedProperty.EqualContents(serializedProperty, endProperty))
			{
				num++;
				serializedProperty.NextVisible(true);
			}
			return num;
		}
	}
}
