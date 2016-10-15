using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SizeByVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent velocityRange = EditorGUIUtility.TextContent("Speed Range|Remaps speed in the defined range to a size.");

			public GUIContent size = EditorGUIUtility.TextContent("Size|Controls the size of each particle based on its speed.");

			public GUIContent separateAxes = new GUIContent("Separate Axes", "If enabled, you can control the angular velocity limit separately for each axis.");

			public GUIContent x = new GUIContent("X");

			public GUIContent y = new GUIContent("Y");

			public GUIContent z = new GUIContent("Z");
		}

		private static SizeByVelocityModuleUI.Texts s_Texts;

		private SerializedMinMaxCurve m_X;

		private SerializedMinMaxCurve m_Y;

		private SerializedMinMaxCurve m_Z;

		private SerializedProperty m_SeparateAxes;

		private SerializedProperty m_Range;

		public SizeByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeBySpeedModule", displayName)
		{
			this.m_ToolTip = "Controls the size of each particle based on its speed.";
		}

		protected override void Init()
		{
			if (this.m_X != null)
			{
				return;
			}
			if (SizeByVelocityModuleUI.s_Texts == null)
			{
				SizeByVelocityModuleUI.s_Texts = new SizeByVelocityModuleUI.Texts();
			}
			this.m_X = new SerializedMinMaxCurve(this, SizeByVelocityModuleUI.s_Texts.x, "curve");
			this.m_X.m_AllowConstant = false;
			this.m_Y = new SerializedMinMaxCurve(this, SizeByVelocityModuleUI.s_Texts.y, "y");
			this.m_Y.m_AllowConstant = false;
			this.m_Z = new SerializedMinMaxCurve(this, SizeByVelocityModuleUI.s_Texts.z, "z");
			this.m_Z.m_AllowConstant = false;
			this.m_SeparateAxes = base.GetProperty("separateAxes");
			this.m_Range = base.GetProperty("range");
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (SizeByVelocityModuleUI.s_Texts == null)
			{
				SizeByVelocityModuleUI.s_Texts = new SizeByVelocityModuleUI.Texts();
			}
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(SizeByVelocityModuleUI.s_Texts.separateAxes, this.m_SeparateAxes);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					this.m_X.RemoveCurveFromEditor();
				}
				else
				{
					this.m_X.RemoveCurveFromEditor();
					this.m_Y.RemoveCurveFromEditor();
					this.m_Z.RemoveCurveFromEditor();
				}
			}
			SerializedMinMaxCurve arg_8F_0 = this.m_Z;
			MinMaxCurveState state = this.m_X.state;
			this.m_Y.state = state;
			arg_8F_0.state = state;
			MinMaxCurveState state2 = this.m_Z.state;
			if (flag)
			{
				this.m_X.m_DisplayName = SizeByVelocityModuleUI.s_Texts.x;
				base.GUITripleMinMaxCurve(GUIContent.none, SizeByVelocityModuleUI.s_Texts.x, this.m_X, SizeByVelocityModuleUI.s_Texts.y, this.m_Y, SizeByVelocityModuleUI.s_Texts.z, this.m_Z, null);
			}
			else
			{
				this.m_X.m_DisplayName = SizeByVelocityModuleUI.s_Texts.size;
				ModuleUI.GUIMinMaxCurve(SizeByVelocityModuleUI.s_Texts.size, this.m_X);
			}
			using (new EditorGUI.DisabledScope(state2 == MinMaxCurveState.k_Scalar || state2 == MinMaxCurveState.k_TwoScalars))
			{
				ModuleUI.GUIMinMaxRange(SizeByVelocityModuleUI.s_Texts.velocityRange, this.m_Range);
			}
		}
	}
}
