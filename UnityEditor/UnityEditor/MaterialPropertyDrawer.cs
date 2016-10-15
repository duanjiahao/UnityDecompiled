using System;
using UnityEngine;

namespace UnityEditor
{
	public abstract class MaterialPropertyDrawer
	{
		public virtual void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			this.OnGUI(position, prop, label.text, editor);
		}

		public virtual void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{
			EditorGUI.LabelField(position, new GUIContent(label), EditorGUIUtility.TempContent("No GUI Implemented"));
		}

		public virtual float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return 16f;
		}

		public virtual void Apply(MaterialProperty prop)
		{
		}
	}
}
