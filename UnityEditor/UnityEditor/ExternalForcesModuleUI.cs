using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ExternalForcesModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent multiplier = EditorGUIUtility.TextContent("Multiplier|Used to scale the force applied to this particle system.");
		}

		private SerializedProperty m_Multiplier;

		private static ExternalForcesModuleUI.Texts s_Texts;

		public ExternalForcesModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ExternalForcesModule", displayName)
		{
			this.m_ToolTip = "Controls the wind zones that each particle is affected by.";
		}

		protected override void Init()
		{
			if (this.m_Multiplier == null)
			{
				if (ExternalForcesModuleUI.s_Texts == null)
				{
					ExternalForcesModuleUI.s_Texts = new ExternalForcesModuleUI.Texts();
				}
				this.m_Multiplier = base.GetProperty("multiplier");
			}
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (ExternalForcesModuleUI.s_Texts == null)
			{
				ExternalForcesModuleUI.s_Texts = new ExternalForcesModuleUI.Texts();
			}
			ModuleUI.GUIFloat(ExternalForcesModuleUI.s_Texts.multiplier, this.m_Multiplier, new GUILayoutOption[0]);
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tExternal Forces is enabled.";
		}
	}
}
