using System;
using UnityEngine;

namespace UnityEditor
{
	internal class StructPropertyGUI
	{
		internal static void JointSpring(Rect position, SerializedProperty property)
		{
			StructPropertyGUI.GenericStruct(position, property);
		}

		internal static void WheelFrictionCurve(Rect position, SerializedProperty property)
		{
			StructPropertyGUI.GenericStruct(position, property);
		}

		internal static void GenericStruct(Rect position, SerializedProperty property)
		{
			GUI.Label(EditorGUI.IndentedRect(position), property.displayName, EditorStyles.label);
			position.y += 16f;
			StructPropertyGUI.DoChildren(position, property);
		}

		private static void DoChildren(Rect position, SerializedProperty property)
		{
			float num = (float)property.depth;
			position.height = 16f;
			EditorGUI.indentLevel++;
			SerializedProperty serializedProperty = property.Copy();
			serializedProperty.NextVisible(true);
			while ((float)serializedProperty.depth == num + 1f)
			{
				EditorGUI.PropertyField(position, serializedProperty);
				position.y += 16f;
				if (!serializedProperty.NextVisible(false))
				{
					break;
				}
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
	}
}
