using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(DelayedAttribute))]
	internal sealed class DelayedDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Float)
			{
				EditorGUI.DelayedFloatField(position, property, label);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUI.DelayedIntField(position, property, label);
			}
			else if (property.propertyType == SerializedPropertyType.String)
			{
				EditorGUI.DelayedTextField(position, property, label);
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use Delayed with float, int, or string.");
			}
		}
	}
}
