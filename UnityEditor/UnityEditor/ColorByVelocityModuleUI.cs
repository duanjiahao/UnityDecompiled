using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ColorByVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent color = EditorGUIUtility.TextContent("Color|Controls the color of each particle based on its speed.");

			public GUIContent velocityRange = EditorGUIUtility.TextContent("Speed Range|Remaps speed in the defined range to a color.");
		}

		private static ColorByVelocityModuleUI.Texts s_Texts;

		private SerializedMinMaxGradient m_Gradient;

		private SerializedProperty m_Range;

		private SerializedProperty m_Scale;

		public ColorByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ColorBySpeedModule", displayName)
		{
			this.m_ToolTip = "Controls the color of each particle based on its speed.";
		}

		protected override void Init()
		{
			if (this.m_Gradient == null)
			{
				this.m_Gradient = new SerializedMinMaxGradient(this);
				this.m_Gradient.m_AllowColor = false;
				this.m_Gradient.m_AllowRandomBetweenTwoColors = false;
				this.m_Range = base.GetProperty("range");
			}
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (ColorByVelocityModuleUI.s_Texts == null)
			{
				ColorByVelocityModuleUI.s_Texts = new ColorByVelocityModuleUI.Texts();
			}
			base.GUIMinMaxGradient(ColorByVelocityModuleUI.s_Texts.color, this.m_Gradient, new GUILayoutOption[0]);
			ModuleUI.GUIMinMaxRange(ColorByVelocityModuleUI.s_Texts.velocityRange, this.m_Range, new GUILayoutOption[0]);
		}
	}
}
