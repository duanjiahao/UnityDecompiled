using System;
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
			if (this.m_Curve == null)
			{
				if (InheritVelocityModuleUI.s_Texts == null)
				{
					InheritVelocityModuleUI.s_Texts = new InheritVelocityModuleUI.Texts();
				}
				this.m_Mode = base.GetProperty("m_Mode");
				this.m_Curve = new SerializedMinMaxCurve(this, GUIContent.none, "m_Curve", ModuleUI.kUseSignedRange);
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (InheritVelocityModuleUI.s_Texts == null)
			{
				InheritVelocityModuleUI.s_Texts = new InheritVelocityModuleUI.Texts();
			}
			ModuleUI.GUIPopup(InheritVelocityModuleUI.s_Texts.mode, this.m_Mode, InheritVelocityModuleUI.s_Texts.modes, new GUILayoutOption[0]);
			ModuleUI.GUIMinMaxCurve(InheritVelocityModuleUI.s_Texts.velocity, this.m_Curve, new GUILayoutOption[0]);
			if (this.m_Curve.scalar.floatValue != 0f)
			{
				ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
				for (int i = 0; i < particleSystems.Length; i++)
				{
					ParticleSystem particleSystem = particleSystems[i];
					Rigidbody componentInParent = particleSystem.GetComponentInParent<Rigidbody>();
					Rigidbody2D componentInParent2 = particleSystem.GetComponentInParent<Rigidbody2D>();
					if (componentInParent != null && !componentInParent.isKinematic)
					{
						EditorGUILayout.HelpBox("Velocity is being driven by RigidBody(" + componentInParent.name + ")", MessageType.Info, true);
					}
					else if (componentInParent2 != null && componentInParent2.bodyType == RigidbodyType2D.Dynamic)
					{
						EditorGUILayout.HelpBox("Velocity is being driven by RigidBody2D(" + componentInParent2.name + ")", MessageType.Info, true);
					}
				}
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
