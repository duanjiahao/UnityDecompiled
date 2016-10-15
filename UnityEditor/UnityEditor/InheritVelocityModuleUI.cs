using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class InheritVelocityModuleUI : ModuleUI
	{
		private enum Modes
		{
			Initial,
			Current
		}

		private class Texts
		{
			public GUIContent mode = EditorGUIUtility.TextContent("Mode|Specifies whether the emitter velocity is inherited as a one-shot when a particle is born, always using the current emitter velocity, or using the emitter velocity when the particle was born.");

			public GUIContent velocity = EditorGUIUtility.TextContent("Multiplier|Controls the amount of emitter velocity inherited during each particle's lifetime.");

			public string[] modes = new string[]
			{
				"Initial",
				"Current"
			};
		}

		private SerializedProperty m_Mode;

		private SerializedMinMaxCurve m_Curve;

		private static InheritVelocityModuleUI.Texts s_Texts;

		public InheritVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "InheritVelocityModule", displayName)
		{
			this.m_ToolTip = "Controls the velocity inherited from the emitter, for each particle.";
		}

		protected override void Init()
		{
			if (this.m_Curve != null)
			{
				return;
			}
			if (InheritVelocityModuleUI.s_Texts == null)
			{
				InheritVelocityModuleUI.s_Texts = new InheritVelocityModuleUI.Texts();
			}
			this.m_Mode = base.GetProperty("m_Mode");
			this.m_Curve = new SerializedMinMaxCurve(this, GUIContent.none, "m_Curve", ModuleUI.kUseSignedRange);
		}

		private bool CanInheritVelocity(ParticleSystem s)
		{
			Rigidbody[] componentsInParent = s.GetComponentsInParent<Rigidbody>();
			Rigidbody2D[] componentsInParent2 = s.GetComponentsInParent<Rigidbody2D>();
			if (!componentsInParent.Any((Rigidbody o) => !o.isKinematic && o.interpolation == RigidbodyInterpolation.None))
			{
				if (!componentsInParent2.Any((Rigidbody2D o) => !o.isKinematic && o.interpolation == RigidbodyInterpolation2D.None))
				{
					return true;
				}
			}
			return false;
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (InheritVelocityModuleUI.s_Texts == null)
			{
				InheritVelocityModuleUI.s_Texts = new InheritVelocityModuleUI.Texts();
			}
			ModuleUI.GUIPopup(InheritVelocityModuleUI.s_Texts.mode, this.m_Mode, InheritVelocityModuleUI.s_Texts.modes);
			ModuleUI.GUIMinMaxCurve(InheritVelocityModuleUI.s_Texts.velocity, this.m_Curve);
			if (this.m_Curve.scalar.floatValue != 0f && !this.CanInheritVelocity(s))
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("Inherit velocity requires interpolation enabled on the rigidbody to function correctly.");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
			}
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			this.Init();
			if (!this.m_Curve.SupportsProcedural())
			{
				text += "\n\tInherited velocity curves use too many keys.";
			}
		}
	}
}
