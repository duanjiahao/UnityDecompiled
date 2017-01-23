using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ForceModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");

			public GUIContent randomizePerFrame = EditorGUIUtility.TextContent("Randomize|Randomize force every frame. Only available when using random between two constants or random between two curves.");

			public GUIContent space = EditorGUIUtility.TextContent("Space|Specifies if the force values are in local space (rotated with the transform) or world space.");

			public string[] spaces = new string[]
			{
				"Local",
				"World"
			};
		}

		private SerializedMinMaxCurve m_X;

		private SerializedMinMaxCurve m_Y;

		private SerializedMinMaxCurve m_Z;

		private SerializedProperty m_RandomizePerFrame;

		private SerializedProperty m_InWorldSpace;

		private static ForceModuleUI.Texts s_Texts;

		public ForceModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ForceModule", displayName)
		{
			this.m_ToolTip = "Controls the force of each particle during its lifetime.";
		}

		protected override void Init()
		{
			if (this.m_X == null)
			{
				if (ForceModuleUI.s_Texts == null)
				{
					ForceModuleUI.s_Texts = new ForceModuleUI.Texts();
				}
				this.m_X = new SerializedMinMaxCurve(this, ForceModuleUI.s_Texts.x, "x", ModuleUI.kUseSignedRange);
				this.m_Y = new SerializedMinMaxCurve(this, ForceModuleUI.s_Texts.y, "y", ModuleUI.kUseSignedRange);
				this.m_Z = new SerializedMinMaxCurve(this, ForceModuleUI.s_Texts.z, "z", ModuleUI.kUseSignedRange);
				this.m_RandomizePerFrame = base.GetProperty("randomizePerFrame");
				this.m_InWorldSpace = base.GetProperty("inWorldSpace");
			}
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (ForceModuleUI.s_Texts == null)
			{
				ForceModuleUI.s_Texts = new ForceModuleUI.Texts();
			}
			MinMaxCurveState state = this.m_X.state;
			base.GUITripleMinMaxCurve(GUIContent.none, ForceModuleUI.s_Texts.x, this.m_X, ForceModuleUI.s_Texts.y, this.m_Y, ForceModuleUI.s_Texts.z, this.m_Z, this.m_RandomizePerFrame, new GUILayoutOption[0]);
			ModuleUI.GUIBoolAsPopup(ForceModuleUI.s_Texts.space, this.m_InWorldSpace, ForceModuleUI.s_Texts.spaces, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(state != MinMaxCurveState.k_TwoScalars && state != MinMaxCurveState.k_TwoCurves))
			{
				ModuleUI.GUIToggle(ForceModuleUI.s_Texts.randomizePerFrame, this.m_RandomizePerFrame, new GUILayoutOption[0]);
			}
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			this.Init();
			if (!this.m_X.SupportsProcedural() || !this.m_Y.SupportsProcedural() || !this.m_Z.SupportsProcedural())
			{
				text += "\n\tLifetime force curves use too many keys.";
			}
			if (this.m_RandomizePerFrame.boolValue)
			{
				text += "\n\tLifetime force curves use random per frame.";
			}
		}
	}
}
