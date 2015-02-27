using System;
using UnityEngine;
namespace UnityEditor
{
	internal class SizeByVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent velocityRange = new GUIContent("Speed Range", "Remaps speed in the defined range to a size.");
			public GUIContent size = new GUIContent("Size", "Controls the size of each particle based on its speed.");
		}
		private static SizeByVelocityModuleUI.Texts s_Texts;
		private SerializedMinMaxCurve m_Curve;
		private SerializedProperty m_Range;
		public SizeByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeBySpeedModule", displayName)
		{
			this.m_ToolTip = "Controls the size of each particle based on its speed.";
		}
		protected override void Init()
		{
			if (this.m_Curve != null)
			{
				return;
			}
			if (SizeByVelocityModuleUI.s_Texts == null)
			{
				SizeByVelocityModuleUI.s_Texts = new SizeByVelocityModuleUI.Texts();
			}
			this.m_Curve = new SerializedMinMaxCurve(this, SizeByVelocityModuleUI.s_Texts.size);
			this.m_Curve.m_AllowConstant = false;
			this.m_Range = base.GetProperty("range");
		}
		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (SizeByVelocityModuleUI.s_Texts == null)
			{
				SizeByVelocityModuleUI.s_Texts = new SizeByVelocityModuleUI.Texts();
			}
			ModuleUI.GUIMinMaxCurve(SizeByVelocityModuleUI.s_Texts.size, this.m_Curve);
			ModuleUI.GUIMinMaxRange(SizeByVelocityModuleUI.s_Texts.velocityRange, this.m_Range);
		}
	}
}
