using System;
using UnityEngine;

namespace UnityEditor
{
	internal class InitialModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent duration = EditorGUIUtility.TextContent("Duration|The length of time the Particle System is emitting particles. If the system is looping, this indicates the length of one cycle.");

			public GUIContent looping = EditorGUIUtility.TextContent("Looping|If true, the emission cycle will repeat after the duration.");

			public GUIContent prewarm = EditorGUIUtility.TextContent("Prewarm|When played a prewarmed system will be in a state as if it had emitted one loop cycle. Can only be used if the system is looping.");

			public GUIContent startDelay = EditorGUIUtility.TextContent("Start Delay|Delay in seconds that this Particle System will wait before emitting particles. Cannot be used together with a prewarmed looping system.");

			public GUIContent maxParticles = EditorGUIUtility.TextContent("Max Particles|The number of particles in the system will be limited by this number. Emission will be temporarily halted if this is reached.");

			public GUIContent lifetime = EditorGUIUtility.TextContent("Start Lifetime|Start lifetime in seconds, particle will die when its lifetime reaches 0.");

			public GUIContent speed = EditorGUIUtility.TextContent("Start Speed|The start speed of particles, applied in the starting direction.");

			public GUIContent color = EditorGUIUtility.TextContent("Start Color|The start color of particles.");

			public GUIContent size3D = EditorGUIUtility.TextContent("3D Start Size|If enabled, you can control the size separately for each axis.");

			public GUIContent size = EditorGUIUtility.TextContent("Start Size|The start size of particles.");

			public GUIContent rotation3D = EditorGUIUtility.TextContent("3D Start Rotation|If enabled, you can control the rotation separately for each axis.");

			public GUIContent rotation = EditorGUIUtility.TextContent("Start Rotation|The start rotation of particles in degrees.");

			public GUIContent randomizeRotationDirection = EditorGUIUtility.TextContent("Randomize Rotation Direction|Cause some particles to spin in the opposite direction. (Set between 0 and 1, where a higher value causes more to flip)");

			public GUIContent autoplay = EditorGUIUtility.TextContent("Play On Awake*|If enabled, the system will start playing automatically. Note that this setting is shared between all Particle Systems in the current particle effect.");

			public GUIContent gravity = EditorGUIUtility.TextContent("Gravity Modifier|Scales the gravity defined in Physics Manager");

			public GUIContent scalingMode = EditorGUIUtility.TextContent("Scaling Mode|Should we use the combined scale from our entire hierarchy, just this particle node, or just apply scale to the shape module?");

			public GUIContent simulationSpace = EditorGUIUtility.TextContent("Simulation Space|Makes particle positions simulate in worldspace or local space. In local space they stay relative to the Transform.");

			public GUIContent autoRandomSeed = EditorGUIUtility.TextContent("Auto Random Seed|Simulate differently each time the effect is played.");

			public GUIContent randomSeed = EditorGUIUtility.TextContent("Random Seed|Randomize the look of the Particle System. Using the same seed will make the Particle System play identically each time.");

			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");

			public string[] simulationSpaces = new string[]
			{
				"World",
				"Local"
			};
		}

		public SerializedProperty m_LengthInSec;

		public SerializedProperty m_Looping;

		public SerializedProperty m_Prewarm;

		public SerializedMinMaxCurve m_StartDelay;

		public SerializedProperty m_PlayOnAwake;

		public SerializedProperty m_SimulationSpace;

		public SerializedProperty m_ScalingMode;

		public SerializedMinMaxCurve m_LifeTime;

		public SerializedMinMaxCurve m_Speed;

		public SerializedMinMaxGradient m_Color;

		public SerializedProperty m_Size3D;

		public SerializedMinMaxCurve m_SizeX;

		public SerializedMinMaxCurve m_SizeY;

		public SerializedMinMaxCurve m_SizeZ;

		public SerializedProperty m_Rotation3D;

		public SerializedMinMaxCurve m_RotationX;

		public SerializedMinMaxCurve m_RotationY;

		public SerializedMinMaxCurve m_RotationZ;

		public SerializedProperty m_RandomizeRotationDirection;

		public SerializedProperty m_GravityModifier;

		public SerializedProperty m_MaxNumParticles;

		public SerializedProperty m_AutoRandomSeed;

		public SerializedProperty m_RandomSeed;

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
			this.m_StartDelay = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.startDelay, "startDelay", false, true);
			this.m_StartDelay.m_AllowCurves = false;
			this.m_PlayOnAwake = base.GetProperty0("playOnAwake");
			this.m_SimulationSpace = base.GetProperty0("moveWithTransform");
			this.m_ScalingMode = base.GetProperty0("scalingMode");
			this.m_AutoRandomSeed = base.GetProperty0("autoRandomSeed");
			this.m_RandomSeed = base.GetProperty0("randomSeed");
			this.m_LifeTime = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.lifetime, "startLifetime");
			this.m_Speed = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.speed, "startSpeed", ModuleUI.kUseSignedRange);
			this.m_Color = new SerializedMinMaxGradient(this, "startColor");
			this.m_Size3D = base.GetProperty("size3D");
			this.m_SizeX = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.x, "startSize");
			this.m_SizeY = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.y, "startSizeY");
			this.m_SizeZ = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.z, "startSizeZ");
			this.m_Rotation3D = base.GetProperty("rotation3D");
			this.m_RotationX = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.x, "startRotationX", ModuleUI.kUseSignedRange);
			this.m_RotationY = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.y, "startRotationY", ModuleUI.kUseSignedRange);
			this.m_RotationZ = new SerializedMinMaxCurve(this, InitialModuleUI.s_Texts.z, "startRotation", ModuleUI.kUseSignedRange);
			this.m_RotationX.m_RemapValue = 57.29578f;
			this.m_RotationY.m_RemapValue = 57.29578f;
			this.m_RotationZ.m_RemapValue = 57.29578f;
			this.m_RotationX.m_DefaultCurveScalar = 3.14159274f;
			this.m_RotationY.m_DefaultCurveScalar = 3.14159274f;
			this.m_RotationZ.m_DefaultCurveScalar = 3.14159274f;
			this.m_RandomizeRotationDirection = base.GetProperty("randomizeRotationDirection");
			this.m_GravityModifier = base.GetProperty("gravityModifier");
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
			using (new EditorGUI.DisabledScope(!this.m_Looping.boolValue))
			{
				ModuleUI.GUIToggle(InitialModuleUI.s_Texts.prewarm, this.m_Prewarm);
			}
			using (new EditorGUI.DisabledScope(this.m_Prewarm.boolValue && this.m_Looping.boolValue))
			{
				ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.startDelay, this.m_StartDelay);
			}
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.lifetime, this.m_LifeTime);
			ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.speed, this.m_Speed);
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(InitialModuleUI.s_Texts.size3D, this.m_Size3D);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					this.m_SizeX.RemoveCurveFromEditor();
				}
				else
				{
					this.m_SizeX.RemoveCurveFromEditor();
					this.m_SizeY.RemoveCurveFromEditor();
					this.m_SizeZ.RemoveCurveFromEditor();
				}
			}
			if (flag)
			{
				this.m_SizeX.m_DisplayName = InitialModuleUI.s_Texts.x;
				base.GUITripleMinMaxCurve(GUIContent.none, InitialModuleUI.s_Texts.x, this.m_SizeX, InitialModuleUI.s_Texts.y, this.m_SizeY, InitialModuleUI.s_Texts.z, this.m_SizeZ, null);
			}
			else
			{
				this.m_SizeX.m_DisplayName = InitialModuleUI.s_Texts.size;
				ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.size, this.m_SizeX);
			}
			EditorGUI.BeginChangeCheck();
			bool flag2 = ModuleUI.GUIToggle(InitialModuleUI.s_Texts.rotation3D, this.m_Rotation3D);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag2)
				{
					this.m_RotationZ.RemoveCurveFromEditor();
				}
				else
				{
					this.m_RotationX.RemoveCurveFromEditor();
					this.m_RotationY.RemoveCurveFromEditor();
					this.m_RotationZ.RemoveCurveFromEditor();
				}
			}
			if (flag2)
			{
				this.m_RotationZ.m_DisplayName = InitialModuleUI.s_Texts.z;
				base.GUITripleMinMaxCurve(GUIContent.none, InitialModuleUI.s_Texts.x, this.m_RotationX, InitialModuleUI.s_Texts.y, this.m_RotationY, InitialModuleUI.s_Texts.z, this.m_RotationZ, null);
			}
			else
			{
				this.m_RotationZ.m_DisplayName = InitialModuleUI.s_Texts.rotation;
				ModuleUI.GUIMinMaxCurve(InitialModuleUI.s_Texts.rotation, this.m_RotationZ);
			}
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.randomizeRotationDirection, this.m_RandomizeRotationDirection);
			base.GUIMinMaxGradient(InitialModuleUI.s_Texts.color, this.m_Color);
			ModuleUI.GUIFloat(InitialModuleUI.s_Texts.gravity, this.m_GravityModifier);
			ModuleUI.GUIBoolAsPopup(InitialModuleUI.s_Texts.simulationSpace, this.m_SimulationSpace, InitialModuleUI.s_Texts.simulationSpaces);
			ModuleUI.GUIPopup(InitialModuleUI.s_Texts.scalingMode, this.m_ScalingMode, new string[]
			{
				"Hierarchy",
				"Local",
				"Shape"
			});
			bool boolValue2 = this.m_PlayOnAwake.boolValue;
			bool flag3 = ModuleUI.GUIToggle(InitialModuleUI.s_Texts.autoplay, this.m_PlayOnAwake);
			if (boolValue2 != flag3)
			{
				this.m_ParticleSystemUI.m_ParticleEffectUI.PlayOnAwakeChanged(flag3);
			}
			ModuleUI.GUIInt(InitialModuleUI.s_Texts.maxParticles, this.m_MaxNumParticles);
			if (!ModuleUI.GUIToggle(InitialModuleUI.s_Texts.autoRandomSeed, this.m_AutoRandomSeed))
			{
				bool flag4 = this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner is ParticleSystemInspector;
				if (flag4)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					ModuleUI.GUIInt(InitialModuleUI.s_Texts.randomSeed, this.m_RandomSeed);
					if (GUILayout.Button("Reseed", EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					}))
					{
						this.m_RandomSeed.intValue = this.m_ParticleSystemUI.m_ParticleSystem.GenerateRandomSeed();
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					ModuleUI.GUIInt(InitialModuleUI.s_Texts.randomSeed, this.m_RandomSeed);
					if (GUILayout.Button("Reseed", EditorStyles.miniButton, new GUILayoutOption[0]))
					{
						this.m_RandomSeed.intValue = this.m_ParticleSystemUI.m_ParticleSystem.GenerateRandomSeed();
					}
				}
			}
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			if (!this.m_SimulationSpace.boolValue)
			{
				text += "\n\tWorld space simulation is used.";
			}
		}
	}
}
