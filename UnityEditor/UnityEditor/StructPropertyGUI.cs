using System;
using UnityEngine;

namespace UnityEditor
{
	internal class StructPropertyGUI
	{
		internal static void GenericStruct(Rect position, SerializedProperty property)
		{
			GUI.Label(EditorGUI.IndentedRect(position), property.displayName, EditorStyles.label);
			position.y += 16f;
			StructPropertyGUI.DoChildren(position, property);
		}

		private static void DoChildren(Rect position, SerializedProperty property)
		{
			position.height = 16f;
			EditorGUI.indentLevel++;
			SerializedProperty serializedProperty = property.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			serializedProperty.NextVisible(true);
			while (!SerializedProperty.EqualContents(serializedProperty, endProperty))
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
