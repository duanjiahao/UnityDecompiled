using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomPropertyDrawer(typeof(SpriteState), true)]
	public class SpriteStateDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
		{
			Rect position = rect;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = prop.FindPropertyRelative("m_HighlightedSprite");
			SerializedProperty property2 = prop.FindPropertyRelative("m_PressedSprite");
			SerializedProperty property3 = prop.FindPropertyRelative("m_DisabledSprite");
			EditorGUI.PropertyField(position, property);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property2);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property3);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 3f * EditorGUIUtility.singleLineHeight + 2f * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
