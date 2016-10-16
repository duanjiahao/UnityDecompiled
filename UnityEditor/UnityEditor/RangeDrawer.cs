using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(RangeAttribute))]
	internal sealed class RangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			RangeAttribute rangeAttribute = (RangeAttribute)base.attribute;
			if (property.propertyType == SerializedPropertyType.Float)
			{
				EditorGUI.Slider(position, property, rangeAttribute.min, rangeAttribute.max, label);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUI.IntSlider(position, property, (int)rangeAttribute.min, (int)rangeAttribute.max, label);
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
			}
		}
	}
}
