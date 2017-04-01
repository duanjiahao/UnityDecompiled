using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ClampVelocityModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");

			public GUIContent dampen = EditorGUIUtility.TextContent("Dampen|Controls how much the velocity that exceeds the velocity limit should be dampened. A value of 0.5 will dampen the exceeding velocity by 50%.");

			public GUIContent magnitude = EditorGUIUtility.TextContent("Speed|The speed limit of particles over the particle lifetime.");

			public GUIContent separateAxes = EditorGUIUtility.TextContent("Separate Axes|If enabled, you can control the velocity limit separately for each axis.");

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

		private SerializedMinMaxCurve m_Magnitude;

		private SerializedProperty m_SeparateAxes;

		private SerializedProperty m_InWorldSpace;

		private SerializedProperty m_Dampen;

		private static ClampVelocityModuleUI.Texts s_Texts;

		public ClampVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ClampVelocityModule", displayName)
		{
			this.m_ToolTip = "Controls the velocity limit and damping of each particle during its lifetime.";
		}

		protected override void Init()
		{
			if (this.m_X == null)
			{
				if (ClampVelocityModuleUI.s_Texts == null)
				{
					ClampVelocityModuleUI.s_Texts = new ClampVelocityModuleUI.Texts();
				}
				this.m_X = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.x, "x");
				this.m_Y = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.y, "y");
				this.m_Z = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.z, "z");
				this.m_Magnitude = new SerializedMinMaxCurve(this, ClampVelocityModuleUI.s_Texts.magnitude, "magnitude");
				this.m_SeparateAxes = base.GetProperty("separateAxis");
				this.m_InWorldSpace = base.GetProperty("inWorldSpace");
				this.m_Dampen = base.GetProperty("dampen");
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(ClampVelocityModuleUI.s_Texts.separateAxes, this.m_SeparateAxes, new GUILayoutOption[0]);
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
			SerializedMinMaxCurve arg_88_0 = this.m_Z;
			MinMaxCurveState state = this.m_X.state;
			this.m_Y.state = state;
			arg_88_0.state = state;
			if (flag)
			{
				base.GUITripleMinMaxCurve(GUIContent.none, ClampVelocityModuleUI.s_Texts.x, this.m_X, ClampVelocityModuleUI.s_Texts.y, this.m_Y, ClampVelocityModuleUI.s_Texts.z, this.m_Z, null, new GUILayoutOption[0]);
				ModuleUI.GUIBoolAsPopup(ClampVelocityModuleUI.s_Texts.space, this.m_InWorldSpace, ClampVelocityModuleUI.s_Texts.spaces, new GUILayoutOption[0]);
			}
			else
			{
				ModuleUI.GUIMinMaxCurve(ClampVelocityModuleUI.s_Texts.magnitude, this.m_Magnitude, new GUILayoutOption[0]);
			}
			ModuleUI.GUIFloat(ClampVelocityModuleUI.s_Texts.dampen, this.m_Dampen, new GUILayoutOption[0]);
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tLimit velocity is enabled.";
		}
	}
}
