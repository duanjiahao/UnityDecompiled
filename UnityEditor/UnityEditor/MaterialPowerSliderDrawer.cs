using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialPowerSliderDrawer : MaterialPropertyDrawer
	{
		private readonly float power;

		public MaterialPowerSliderDrawer(float power)
		{
			this.power = power;
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			if (prop.type != MaterialProperty.PropType.Range)
			{
				return 40f;
			}
			return base.GetPropertyHeight(prop, label, editor);
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			if (prop.type != MaterialProperty.PropType.Range)
			{
				GUIContent label2 = EditorGUIUtility.TempContent("PowerSlider used on a non-range property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
				EditorGUI.LabelField(position, label2, EditorStyles.helpBox);
				return;
			}
			MaterialEditor.DoPowerRangeProperty(position, prop, label, this.power);
		}
	}
}
