using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ColorModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent color = EditorGUIUtility.TextContent("Color|Controls the color of each particle during its lifetime.");
		}

		private static ColorModuleUI.Texts s_Texts;

		private SerializedMinMaxGradient m_Gradient;

		private SerializedProperty m_Scale;

		public ColorModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ColorModule", displayName)
		{
			this.m_ToolTip = "Controls the color of each particle during its lifetime.";
		}

		protected override void Init()
		{
			if (this.m_Gradient == null)
			{
				this.m_Gradient = new SerializedMinMaxGradient(this);
				this.m_Gradient.m_AllowColor = false;
				this.m_Gradient.m_AllowRandomBetweenTwoColors = false;
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (ColorModuleUI.s_Texts == null)
			{
				ColorModuleUI.s_Texts = new ColorModuleUI.Texts();
			}
			base.GUIMinMaxGradient(ColorModuleUI.s_Texts.color, this.m_Gradient, false, new GUILayoutOption[0]);
		}
	}
}
