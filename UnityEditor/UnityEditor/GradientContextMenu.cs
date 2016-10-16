using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class GradientContextMenu
	{
		private SerializedProperty m_Prop1;

		private GradientContextMenu(SerializedProperty prop1)
		{
			this.m_Prop1 = prop1;
		}

		internal static void Show(SerializedProperty prop)
		{
			GUIContent content = new GUIContent("Copy");
			GUIContent content2 = new GUIContent("Paste");
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(content, false, new GenericMenu.MenuFunction(new GradientContextMenu(prop).Copy));
			if (ParticleSystemClipboard.HasSingleGradient())
			{
				genericMenu.AddItem(content2, false, new GenericMenu.MenuFunction(new GradientContextMenu(prop).Paste));
			}
			else
			{
				genericMenu.AddDisabledItem(content2);
			}
			genericMenu.ShowAsContext();
		}

		private void Copy()
		{
			Gradient gradient = (this.m_Prop1 == null) ? null : this.m_Prop1.gradientValue;
			ParticleSystemClipboard.CopyGradient(gradient, null);
		}

		private void Paste()
		{
			ParticleSystemClipboard.PasteGradient(this.m_Prop1, null);
			GradientPreviewCache.ClearCache();
		}
	}
}
