using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomPropertyDrawer(typeof(AnimationTriggers), true)]
	public class AnimationTriggersDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
		{
			Rect position = rect;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = prop.FindPropertyRelative("m_NormalTrigger");
			SerializedProperty property2 = prop.FindPropertyRelative("m_HighlightedTrigger");
			SerializedProperty property3 = prop.FindPropertyRelative("m_PressedTrigger");
			SerializedProperty property4 = prop.FindPropertyRelative("m_DisabledTrigger");
			EditorGUI.PropertyField(position, property);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property2);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property3);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property4);
		}

		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 4f * EditorGUIUtility.singleLineHeight + 3f * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
