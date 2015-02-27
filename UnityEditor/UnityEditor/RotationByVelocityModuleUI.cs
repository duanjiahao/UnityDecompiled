using System;
using UnityEngine;
namespace UnityEditor
{
	internal class RotationByVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent velocityRange = new GUIContent("Speed Range", "Remaps speed in the defined range to an angular velocity.");
			public GUIContent rotation = new GUIContent("Angular Velocity", "Controls the angular velocity of each particle based on its speed.");
		}
		private static RotationByVelocityModuleUI.Texts s_Texts;
		private SerializedMinMaxCurve m_Curve;
		private SerializedProperty m_Range;
		public RotationByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationBySpeedModule", displayName)
		{
			this.m_ToolTip = "Controls the angular velocity of each particle based on its speed.";
		}
		protected override void Init()
		{
			if (this.m_Curve != null)
			{
				return;
			}
			if (RotationByVelocityModuleUI.s_Texts == null)
			{
				RotationByVelocityModuleUI.s_Texts = new RotationByVelocityModuleUI.Texts();
			}
			this.m_Curve = new SerializedMinMaxCurve(this, RotationByVelocityModuleUI.s_Texts.rotation, ModuleUI.kUseSignedRange);
			this.m_Curve.m_RemapValue = 57.29578f;
			this.m_Range = base.GetProperty("range");
		}
		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (RotationByVelocityModuleUI.s_Texts == null)
			{
				RotationByVelocityModuleUI.s_Texts = new RotationByVelocityModuleUI.Texts();
			}
			ModuleUI.GUIMinMaxCurve(RotationByVelocityModuleUI.s_Texts.rotation, this.m_Curve);
			ModuleUI.GUIMinMaxRange(RotationByVelocityModuleUI.s_Texts.velocityRange, this.m_Range);
		}
		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tRotation by Speed is enabled.";
		}
	}
}
