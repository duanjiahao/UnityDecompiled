using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPropertyDrawer(typeof(ColorUsageAttribute))]
	internal sealed class ColorUsageDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ColorUsageAttribute colorUsageAttribute = (ColorUsageAttribute)base.attribute;
			ColorPickerHDRConfig hdrConfig = ColorPickerHDRConfig.Temp(colorUsageAttribute.minBrightness, colorUsageAttribute.maxBrightness, colorUsageAttribute.minExposureValue, colorUsageAttribute.maxExposureValue);
			EditorGUI.BeginChangeCheck();
			Color colorValue = EditorGUI.ColorField(position, label, property.colorValue, true, colorUsageAttribute.showAlpha, colorUsageAttribute.hdr, hdrConfig);
			if (EditorGUI.EndChangeCheck())
			{
				property.colorValue = colorValue;
			}
		}
	}
}
