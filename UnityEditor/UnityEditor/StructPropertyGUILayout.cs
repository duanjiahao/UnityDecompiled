using System;
using UnityEngine;

namespace UnityEditor
{
	internal class StructPropertyGUILayout
	{
		internal static void JointSpring(SerializedProperty property, params GUILayoutOption[] options)
		{
			StructPropertyGUILayout.GenericStruct(property, options);
		}

		internal static void WheelFrictionCurve(SerializedProperty property, params GUILayoutOption[] options)
		{
			StructPropertyGUILayout.GenericStruct(property, options);
		}

		internal static void GenericStruct(SerializedProperty property, params GUILayoutOption[] options)
		{
			float num = 16f + 16f * (float)StructPropertyGUILayout.GetChildrenCount(property);
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, num, num, EditorStyles.layerMaskField, options);
			StructPropertyGUI.GenericStruct(rect, property);
		}

		internal static int GetChildrenCount(SerializedProperty property)
		{
			int depth = property.depth;
			SerializedProperty serializedProperty = property.Copy();
			serializedProperty.NextVisible(true);
			int num = 0;
			while (serializedProperty.depth == depth + 1)
			{
				num++;
				if (!serializedProperty.NextVisible(false))
				{
					break;
				}
			}
			return num;
		}
	}
}
