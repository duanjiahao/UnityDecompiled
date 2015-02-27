using System;
using UnityEngine;
namespace UnityEditor
{
	internal class ClampVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent x = new GUIContent("X");
			public GUIContent y = new GUIContent("Y");
			public GUIContent z = new GUIContent("Z");
			public GUIContent dampen = new GUIContent("Dampen", "Controls how much the velocity that exceeds the velocity limit should be dampened. A value of 0.5 will dampen the exceeding velocity by 50%.");
			public GUIContent magnitude = new GUIContent("  Speed", "The speed limit of particles over the particle lifetime.");
			public GUIContent separateAxis = new GUIContent("Separate Axis", "If enabled, you can control the velocity limit separately for each axis.");
			public GUIContent space = new GUIContent("  Space", "Specifies if the velocity values are in local space (rotated with the transform) or world space.");
			public string[] spaces = new string[]
			{
				"Local",
				"World"
			};
		}
		private SerializedMinMaxCurve m_X;
		private SerializedMinMaxCurve m_Y;
		private SerializedMinMaxCurve m_Z;
		private SerializedMinMaxCurve m_Magnitude;
		private SerializedProperty m_SeparateAxis;
		private SerializedProperty m_InWorldSpace;
		private SerializedProperty m_Dampen;
		private static ClampVelocityModuleUI.Texts s_Texts;
		public ClampVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ClampVelocityModule", displayName)
		{
			this.m_ToolTip = "Controls the velocity limit and damping of each particle during its lifetime.";
		}
		protected override void Init()
		{
			if (this.m_X != null)
			{
				return;
			}
			if (ClampVelocityModuleUI.s_Texts == null)
			{
				ClampVelocityModuleUI.s_Texts = new ClampVelocityModuleUI.Texts();
			}
			this.m_X = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.x, "x", ModuleUI.kUseSignedRange);
			this.m_Y = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.y, "y", ModuleUI.kUseSignedRange);
			this.m_Z = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.z, "z", ModuleUI.kUseSignedRange);
			this.m_Magnitude = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.magnitude, "magnitude");
			this.m_SeparateAxis = base.GetProperty("separateAxis");
			this.m_InWorldSpace = base.GetProperty("inWorldSpace");
			this.m_Dampen = base.GetProperty("dampen");
		}
		public override void OnInspectorGUI(ParticleSystem s)
		{
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(ClampVelocityModuleUI.s_Texts.separateAxis, this.m_SeparateAxis);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					this.m_Magnitude.RemoveCurveFromEditor();
				}
				else
				{
					this.m_X.RemoveCurveFromEditor();
					this.m_Y.RemoveCurveFromEditor();
					this.m_Z.RemoveCurveFromEditor();
				}
			}
			if (flag)
			{
				base.GUITripleMinMaxCurve(GUIContent.none, ClampVelocityModuleUI.s_Texts.x, this.m_X, ClampVelocityModuleUI.s_Texts.y, this.m_Y, ClampVelocityModuleUI.s_Texts.z, this.m_Z, null);
				ModuleUI.GUIBoolAsPopup(ClampVelocityModuleUI.s_Texts.space, this.m_InWorldSpace, ClampVelocityModuleUI.s_Texts.spaces);
			}
			else
			{
				ModuleUI.GUIMinMaxCurve(ClampVelocityModuleUI.s_Texts.magnitude, this.m_Magnitude);
			}
			ModuleUI.GUIFloat(ClampVelocityModuleUI.s_Texts.dampen, this.m_Dampen);
		}
		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tLimit velocity is enabled.";
		}
	}
}
