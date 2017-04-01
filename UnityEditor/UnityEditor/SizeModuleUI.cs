using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SizeModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent size = EditorGUIUtility.TextContent("Size|Controls the size of each particle during its lifetime.");

			public GUIContent separateAxes = EditorGUIUtility.TextContent("Separate Axes|If enabled, you can control the angular velocity limit separately for each axis.");

			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");
		}

		private SerializedMinMaxCurve m_X;

		private SerializedMinMaxCurve m_Y;

		private SerializedMinMaxCurve m_Z;

		private SerializedProperty m_SeparateAxes;

		private static SizeModuleUI.Texts s_Texts;

		public SizeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeModule", displayName)
		{
			this.m_ToolTip = "Controls the size of each particle during its lifetime.";
		}

		protected override void Init()
		{
			if (this.m_X == null)
			{
				if (SizeModuleUI.s_Texts == null)
				{
					SizeModuleUI.s_Texts = new SizeModuleUI.Texts();
				}
				this.m_SeparateAxes = base.GetProperty("separateAxes");
				this.m_X = new SerializedMinMaxCurve(this, SizeModuleUI.s_Texts.x, "curve");
				this.m_Y = new SerializedMinMaxCurve(this, SizeModuleUI.s_Texts.y, "y", false, false, this.m_SeparateAxes.boolValue);
				this.m_Z = new SerializedMinMaxCurve(this, SizeModuleUI.s_Texts.z, "z", false, false, this.m_SeparateAxes.boolValue);
				this.m_X.m_AllowConstant = false;
				this.m_Y.m_AllowConstant = false;
				this.m_Z.m_AllowConstant = false;
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (SizeModuleUI.s_Texts == null)
			{
				SizeModuleUI.s_Texts = new SizeModuleUI.Texts();
			}
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(SizeModuleUI.s_Texts.separateAxes, this.m_SeparateAxes, new GUILayoutOption[0]);
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
			if (!this.m_X.stateHasMultipleDifferentValues)
			{
				this.m_Z.SetMinMaxState(this.m_X.state, flag);
				this.m_Y.SetMinMaxState(this.m_X.state, flag);
			}
			if (flag)
			{
				this.m_X.m_DisplayName = SizeModuleUI.s_Texts.x;
				base.GUITripleMinMaxCurve(GUIContent.none, SizeModuleUI.s_Texts.x, this.m_X, SizeModuleUI.s_Texts.y, this.m_Y, SizeModuleUI.s_Texts.z, this.m_Z, null, new GUILayoutOption[0]);
			}
			else
			{
				this.m_X.m_DisplayName = SizeModuleUI.s_Texts.size;
				ModuleUI.GUIMinMaxCurve(SizeModuleUI.s_Texts.size, this.m_X, new GUILayoutOption[0]);
			}
		}
	}
}
