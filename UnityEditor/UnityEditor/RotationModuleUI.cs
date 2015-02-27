using System;
using UnityEngine;
namespace UnityEditor
{
	internal class RotationModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent rotation = new GUIContent("Angular Velocity", "Controls the angular velocity of each particle during its lifetime.");
		}
		private SerializedMinMaxCurve m_Curve;
		private static RotationModuleUI.Texts s_Texts;
		public RotationModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationModule", displayName)
		{
			this.m_ToolTip = "Controls the angular velocity of each particle during its lifetime.";
		}
		protected override void Init()
		{
			if (this.m_Curve != null)
			{
				return;
			}
			if (RotationModuleUI.s_Texts == null)
			{
				RotationModuleUI.s_Texts = new RotationModuleUI.Texts();
			}
			this.m_Curve = new SerializedMinMaxCurve(this, RotationModuleUI.s_Texts.rotation, ModuleUI.kUseSignedRange);
			this.m_Curve.m_RemapValue = 57.29578f;
		}
		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (RotationModuleUI.s_Texts == null)
			{
				RotationModuleUI.s_Texts = new RotationModuleUI.Texts();
			}
			ModuleUI.GUIMinMaxCurve(RotationModuleUI.s_Texts.rotation, this.m_Curve);
		}
		public override void UpdateCullingSupportedString(ref string text)
		{
			this.Init();
			if (!this.m_Curve.SupportsProcedural())
			{
				text += "\n\tLifetime rotation curve uses too many keys.";
			}
		}
	}
}
