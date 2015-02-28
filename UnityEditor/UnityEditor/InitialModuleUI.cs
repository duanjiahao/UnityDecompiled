using System;
using UnityEngine;
namespace UnityEditor
{
	internal class InitialModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent duration = new GUIContent("Duration", "The length of time the Particle System is emitting particles, if the system is looping, this indicates the length of one cycle.");
			public GUIContent looping = new GUIContent("Looping", "If true, the emission cycle will repeat after the duration.");
			public GUIContent prewarm = new GUIContent("Prewarm", "When played a prewarmed system will be in a state as if it had emitted one loop cycle. Can only be used if the system is looping.");
			public GUIContent startDelay = new GUIContent("Start Delay", "Delay in seconds that this Particle System will wait before emitting particles. Cannot be used together with a prewarmed looping system.");
			public GUIContent maxParticles = new GUIContent("Max Particles", "The number of particles in the system will be limited by this number. Emission will be temporarily halted if this is reached.");
			public GUIContent lifetime = new GUIContent("Start Lifetime", "Start lifetime in seconds, particle will die when its lifetime reaches 0.");
			public GUIContent speed = new GUIContent("Start Speed", "The start speed of particles, applied in the starting direction.");
			public GUIContent color = new GUIContent("Start Color", "The start color of particles.");
			public GUIContent size = new GUIContent("Start Size", "The start size of particles.");
			public GUIContent rotation = new GUIContent("Start Rotation", "The start rotation of particles in degrees.");
			public GUIContent autoplay = new GUIContent("Play On Awake", "If enabled, the system will start playing automatically.");
			public GUIContent gravity = new GUIContent("Gravity Modifier", "Scales the gravity defined in Physics Manager");
			public GUIContent inheritvelocity = new GUIContent("Inherit Velocity", "Applies the current directional velocity of the Transform to newly emitted particles.");
			public GUIContent simulationSpace = new GUIContent("Simulation Space", "Makes particle positions simulate in worldspace or local space. In local space they stay relative to the Transform.");
			public string[] simulationSpaces = new string[]
			{
				"World",
				"Local"
			};
		}
		public SerializedProperty m_LengthInSec;
		public SerializedProperty m_Looping;
		public SerializedProperty m_Prewarm;
		public SerializedProperty m_StartDelay;
		public SerializedProperty m_PlayOnAwake;
		public SerializedProperty m_SimulationSpace;
		public SerializedMinMaxCurve m_LifeTime;
		public SerializedMinMaxCurve m_Speed;
		public SerializedMinMaxGradient m_Color;
		public SerializedMinMaxCurve m_Size;
		public SerializedMinMaxCurve m_Rotation;
		public SerializedProperty m_GravityModifier;
		public SerializedProperty m_InheritVelocity;
		public SerializedProperty m_MaxNumParticles;
		private static InitialModuleUI.Texts s_Texts;
		public InitialModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "InitialModule", displayName, ModuleUI.VisibilityState.VisibleAndFoldedOut)
		{
			this.Init();
		}
		public override float GetXAxisScalar()
		{
			return this.m_ParticleSystemUI.GetEmitterDuration();
		}
		protected override void Init()
		{
			if (InitialModuleUI.s_Texts == null)
			{
				InitialModuleUI.s_Texts = new InitialModuleUI.Texts();
			}
			if (this.m_LengthInSec != null)
			{
				return;
			}
			this.m_LengthInSec = base.GetProperty0("lengthInSec");
			this.m_Looping = base.GetProperty0("looping");
			this.m_Prewarm = base.GetProperty0("prewarm");
			this.m_StartDelay = base.GetProperty0("startDelay");
			this.m_PlayOnAwake = base.GetProperty0("playOnAwake");
			this.m_SimulationSpace = base.GetProperty0("moveWithTransform");
			this.m_LifeTime = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.lifetime, "startLifetime");
			this.m_Speed = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.speed, "startSpeed", ModuleUI.kUseSignedRange);
			this.m_Color = new SerializedMinMaxGradient(this, "startColor");
			this.m_Size = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.size, "startSize");
			this.m_Rotation = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.rotation, "startRotation", ModuleUI.kUseSignedRange);
			this.m_Rotation.m_RemapValue = 57.29578f;
			this.m_Rotation.m_DefaultCurveScalar = 3.14159274f;
			this.m_GravityModifier = base.GetProperty("gravityModifier");
			this.m_InheritVelocity = base.GetProperty("inheritVelocity");
			this.m_MaxNumParticles = base.GetProperty("maxNumParticles");
		}
		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (InitialModuleUI.s_Texts == null)
			{
				InitialModuleUI.s_Texts = new InitialModuleUI.Texts();
			}
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.duration, this.m_LengthInSec, "f2");
			this.m_LengthInSec.floatValue = Mathf.Min(100000f, Mathf.Max(0f, this.m_LengthInSec.floatValue));
			bool boolValue = this.m_Looping.boolValue;
			ModuleUI.GUIToggle(InitialModuleUI.s_Texts.looping, this.m_Looping);
			if (this.m_Looping.boolValue && !boolValue && s.time >= this.m_LengthInSec.floatValue)
			{
				s.time = 0f;
			}
			EditorGUI.BeginDisabledGroup(!this.m_Looping.boolValue);
			ModuleUI.GUIToggle(InitialModuleUI.s_Texts.prewarm, this.m_Prewarm);
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(this.m_Prewarm.boolValue && this.m_Looping.boolValue);
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.startDelay, this.m_StartDelay);
			EditorGUI.EndDisabledGroup();
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.lifetime, this.m_LifeTime);
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.speed, this.m_Speed);
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.size, this.m_Size);
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.rotation, this.m_Rotation);
			base.GUIMinMaxGradient(InitialModuleUI.s_Texts.color, this.m_Color);
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.gravity, this.m_GravityModifier);
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.inheritvelocity, this.m_InheritVelocity);
			ModuleUI.GUIBoolAsPopup(InitialModuleUI.s_Texts.simulationSpace, this.m_SimulationSpace, InitialModuleUI.s_Texts.simulationSpaces);
			bool boolValue2 = this.m_PlayOnAwake.boolValue;
			bool flag = ModuleUI.GUIToggle(InitialModuleUI.s_Texts.autoplay, this.m_PlayOnAwake);
			if (boolValue2 != flag)
			{
				this.m_ParticleSystemUI.m_ParticleEffectUI.PlayOnAwakeChanged(flag);
			}
			ModuleUI.GUIInt(InitialModuleUI.s_Texts.maxParticles, this.m_MaxNumParticles);
		}
		public override void UpdateCullingSupportedString(ref string text)
		{
			if (!this.m_SimulationSpace.boolValue)
			{
				text += "\n\tWorld space simulation is used.";
			}
			if (this.m_LifeTime.state == MinMaxCurveState.k_TwoCurves || this.m_LifeTime.state == MinMaxCurveState.k_TwoScalars)
			{
				text += "\n\tStart lifetime is random.";
			}
		}
	}
}
