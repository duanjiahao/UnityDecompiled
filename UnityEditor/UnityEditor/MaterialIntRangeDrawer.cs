using System;
using UnityEngine;

namespace UnityEditor
{
	internal class MaterialIntRangeDrawer : MaterialPropertyDrawer
	{
		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			if (prop.type != MaterialProperty.PropType.Range)
			{
				GUIContent label2 = EditorGUIUtility.TempContent("IntRange used on a non-range property: " + prop.name, EditorGUIUtility.GetHelpIcon(MessageType.Warning));
				EditorGUI.LabelField(position, label2, EditorStyles.helpBox);
			}
			else
			{
				MaterialEditor.DoIntRangeProperty(position, prop, label);
			}
		}
	}
}
