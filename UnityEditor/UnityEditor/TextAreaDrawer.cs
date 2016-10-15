using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(TextAreaAttribute))]
	internal sealed class TextAreaDrawer : PropertyDrawer
	{
		private const int kLineHeight = 13;

		private Vector2 m_ScrollPosition = default(Vector2);

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				label = EditorGUI.BeginProperty(position, label, property);
				Rect labelPosition = position;
				labelPosition.height = 16f;
				position.yMin += labelPosition.height;
				EditorGUI.HandlePrefixLabel(position, labelPosition, label);
				EditorGUI.BeginChangeCheck();
				string stringValue = EditorGUI.ScrollableTextAreaInternal(position, property.stringValue, ref this.m_ScrollPosition, EditorStyles.textArea);
				if (EditorGUI.EndChangeCheck())
				{
					property.stringValue = stringValue;
				}
				EditorGUI.EndProperty();
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use TextAreaDrawer with string.");
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			TextAreaAttribute textAreaAttribute = base.attribute as TextAreaAttribute;
			string stringValue = property.stringValue;
			float num = EditorStyles.textArea.CalcHeight(GUIContent.Temp(stringValue), EditorGUIUtility.contextWidth);
			int num2 = Mathf.CeilToInt(num / 13f);
			num2 = Mathf.Clamp(num2, textAreaAttribute.minLines, textAreaAttribute.maxLines);
			return 32f + (float)((num2 - 1) * 13);
		}
	}
}
