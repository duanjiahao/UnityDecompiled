using System;
using UnityEngine;

namespace UnityEditor
{
	internal class VelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");

			public GUIContent space = EditorGUIUtility.TextContent("Space|Specifies if the velocity values are in local space (rotated with the transform) or world space.");

			public string[] spaces = new string[]
			{
				"Local",
				"World"
			};
		}

		private SerializedMinMaxCurve m_X;

		private SerializedMinMaxCurve m_Y;

		private SerializedMinMaxCurve m_Z;

		private SerializedProperty m_InWorldSpace;

		private static VelocityModuleUI.Texts s_Texts;

		public VelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "VelocityModule", displayName)
		{
			this.m_ToolTip = "Controls the velocity of each particle during its lifetime.";
		}

		protected override void Init()
		{
			if (this.m_X != null)
			{
				return;
			}
			if (VelocityModuleUI.s_Texts == null)
			{
				VelocityModuleUI.s_Texts = new VelocityModuleUI.Texts();
			}
			this.m_X = new SerializedMinMaxCurve(this, VelocityModuleUI.s_Texts.x, "x", ModuleUI.kUseSignedRange);
			this.m_Y = new SerializedMinMaxCurve(this, VelocityModuleUI.s_Texts.y, "y", ModuleUI.kUseSignedRange);
			this.m_Z = new SerializedMinMaxCurve(this, VelocityModuleUI.s_Texts.z, "z", ModuleUI.kUseSignedRange);
			this.m_InWorldSpace = base.GetProperty("inWorldSpace");
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (VelocityModuleUI.s_Texts == null)
			{
				VelocityModuleUI.s_Texts = new VelocityModuleUI.Texts();
			}
			base.GUITripleMinMaxCurve(GUIContent.none, VelocityModuleUI.s_Texts.x, this.m_X, VelocityModuleUI.s_Texts.y, this.m_Y, VelocityModuleUI.s_Texts.z, this.m_Z, null);
			ModuleUI.GUIBoolAsPopup(VelocityModuleUI.s_Texts.space, this.m_InWorldSpace, VelocityModuleUI.s_Texts.spaces);
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			this.Init();
			if (!this.m_X.SupportsProcedural() || !this.m_Y.SupportsProcedural() || !this.m_Z.SupportsProcedural())
			{
				text += "\n\tLifetime velocity curves use too many keys.";
			}
		}
	}
}
