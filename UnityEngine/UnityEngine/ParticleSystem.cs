using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	public sealed class ParticleSystem : Component
	{
		[Obsolete("ParticleSystem.CollisionEvent has been deprecated. Use ParticleCollisionEvent instead (UnityUpgradable) -> ParticleCollisionEvent", true)]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CollisionEvent
		{
			public Vector3 intersection
			{
				get
				{
					return default(Vector3);
				}
			}

			public Vector3 normal
			{
				get
				{
					return default(Vector3);
				}
			}

			public Vector3 velocity
			{
				get
				{
					return default(Vector3);
				}
			}

			public Component collider
			{
				get
				{
					return null;
				}
			}
		}

		public struct Burst
		{
			private float m_Time;

			private short m_MinCount;

			private short m_MaxCount;

			private int m_RepeatCount;

			private float m_RepeatInterval;

			public float time
			{
				get
				{
					return this.m_Time;
				}
				set
				{
					this.m_Time = value;
				}
			}

			public short minCount
			{
				get
				{
					return this.m_MinCount;
				}
				set
				{
					this.m_MinCount = value;
				}
			}

			public short maxCount
			{
				get
				{
					return this.m_MaxCount;
				}
				set
				{
					this.m_MaxCount = value;
				}
			}

			public int cycleCount
			{
				get
				{
					return this.m_RepeatCount + 1;
				}
				set
				{
					this.m_RepeatCount = value - 1;
				}
			}

			public float repeatInterval
			{
				get
				{
					return this.m_RepeatInterval;
				}
				set
				{
					this.m_RepeatInterval = value;
				}
			}

			public Burst(float _time, short _count)
			{
				this.m_Time = _time;
				this.m_MinCount = _count;
				this.m_MaxCount = _count;
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
			}

			public Burst(float _time, short _minCount, short _maxCount)
			{
				this.m_Time = _time;
				this.m_MinCount = _minCount;
				this.m_MaxCount = _maxCount;
				this.m_RepeatCount = 0;
				this.m_RepeatInterval = 0f;
			}

			public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
			{
				this.m_Time = _time;
				this.m_MinCount = _minCount;
				this.m_MaxCount = _maxCount;
				this.m_RepeatCount = _cycleCount - 1;
				this.m_RepeatInterval = _repeatInterval;
			}
		}

		public struct MinMaxCurve
		{
			private ParticleSystemCurveMode m_Mode;

			private float m_CurveMultiplier;

			private AnimationCurve m_CurveMin;

			private AnimationCurve m_CurveMax;

			private float m_ConstantMin;

			private float m_ConstantMax;

			public ParticleSystemCurveMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			[Obsolete("Please use MinMaxCurve.curveMultiplier instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/MinMaxCurve.curveMultiplier")]
			public float curveScalar
			{
				get
				{
					return this.m_CurveMultiplier;
				}
				set
				{
					this.m_CurveMultiplier = value;
				}
			}

			public float curveMultiplier
			{
				get
				{
					return this.m_CurveMultiplier;
				}
				set
				{
					this.m_CurveMultiplier = value;
				}
			}

			public AnimationCurve curveMax
			{
				get
				{
					return this.m_CurveMax;
				}
				set
				{
					this.m_CurveMax = value;
				}
			}

			public AnimationCurve curveMin
			{
				get
				{
					return this.m_CurveMin;
				}
				set
				{
					this.m_CurveMin = value;
				}
			}

			public float constantMax
			{
				get
				{
					return this.m_ConstantMax;
				}
				set
				{
					this.m_ConstantMax = value;
				}
			}

			public float constantMin
			{
				get
				{
					return this.m_ConstantMin;
				}
				set
				{
					this.m_ConstantMin = value;
				}
			}

			public float constant
			{
				get
				{
					return this.m_ConstantMax;
				}
				set
				{
					this.m_ConstantMax = value;
				}
			}

			public AnimationCurve curve
			{
				get
				{
					return this.m_CurveMax;
				}
				set
				{
					this.m_CurveMax = value;
				}
			}

			public MinMaxCurve(float constant)
			{
				this.m_Mode = ParticleSystemCurveMode.Constant;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = constant;
			}

			public MinMaxCurve(float multiplier, AnimationCurve curve)
			{
				this.m_Mode = ParticleSystemCurveMode.Curve;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = null;
				this.m_CurveMax = curve;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoCurves;
				this.m_CurveMultiplier = multiplier;
				this.m_CurveMin = min;
				this.m_CurveMax = max;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float min, float max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoConstants;
				this.m_CurveMultiplier = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = min;
				this.m_ConstantMax = max;
			}

			public float Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

			public float Evaluate(float time, float lerpFactor)
			{
				time = Mathf.Clamp(time, 0f, 1f);
				lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
				float result;
				if (this.m_Mode == ParticleSystemCurveMode.Constant)
				{
					result = this.m_ConstantMax;
				}
				else if (this.m_Mode == ParticleSystemCurveMode.TwoConstants)
				{
					result = Mathf.Lerp(this.m_ConstantMin, this.m_ConstantMax, lerpFactor);
				}
				else
				{
					float num = this.m_CurveMax.Evaluate(time) * this.m_CurveMultiplier;
					if (this.m_Mode == ParticleSystemCurveMode.TwoCurves)
					{
						result = Mathf.Lerp(this.m_CurveMin.Evaluate(time) * this.m_CurveMultiplier, num, lerpFactor);
					}
					else
					{
						result = num;
					}
				}
				return result;
			}

			public static implicit operator ParticleSystem.MinMaxCurve(float constant)
			{
				return new ParticleSystem.MinMaxCurve(constant);
			}
		}

		public struct MinMaxGradient
		{
			private ParticleSystemGradientMode m_Mode;

			private Gradient m_GradientMin;

			private Gradient m_GradientMax;

			private Color m_ColorMin;

			private Color m_ColorMax;

			public ParticleSystemGradientMode mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					this.m_Mode = value;
				}
			}

			public Gradient gradientMax
			{
				get
				{
					return this.m_GradientMax;
				}
				set
				{
					this.m_GradientMax = value;
				}
			}

			public Gradient gradientMin
			{
				get
				{
					return this.m_GradientMin;
				}
				set
				{
					this.m_GradientMin = value;
				}
			}

			public Color colorMax
			{
				get
				{
					return this.m_ColorMax;
				}
				set
				{
					this.m_ColorMax = value;
				}
			}

			public Color colorMin
			{
				get
				{
					return this.m_ColorMin;
				}
				set
				{
					this.m_ColorMin = value;
				}
			}

			public Color color
			{
				get
				{
					return this.m_ColorMax;
				}
				set
				{
					this.m_ColorMax = value;
				}
			}

			public Gradient gradient
			{
				get
				{
					return this.m_GradientMax;
				}
				set
				{
					this.m_GradientMax = value;
				}
			}

			public MinMaxGradient(Color color)
			{
				this.m_Mode = ParticleSystemGradientMode.Color;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = color;
			}

			public MinMaxGradient(Gradient gradient)
			{
				this.m_Mode = ParticleSystemGradientMode.Gradient;
				this.m_GradientMin = null;
				this.m_GradientMax = gradient;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			public MinMaxGradient(Color min, Color max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoColors;
				this.m_GradientMin = null;
				this.m_GradientMax = null;
				this.m_ColorMin = min;
				this.m_ColorMax = max;
			}

			public MinMaxGradient(Gradient min, Gradient max)
			{
				this.m_Mode = ParticleSystemGradientMode.TwoGradients;
				this.m_GradientMin = min;
				this.m_GradientMax = max;
				this.m_ColorMin = Color.black;
				this.m_ColorMax = Color.black;
			}

			public Color Evaluate(float time)
			{
				return this.Evaluate(time, 1f);
			}

			public Color Evaluate(float time, float lerpFactor)
			{
				time = Mathf.Clamp(time, 0f, 1f);
				lerpFactor = Mathf.Clamp(lerpFactor, 0f, 1f);
				Color result;
				if (this.m_Mode == ParticleSystemGradientMode.Color)
				{
					result = this.m_ColorMax;
				}
				else if (this.m_Mode == ParticleSystemGradientMode.TwoColors)
				{
					result = Color.Lerp(this.m_ColorMin, this.m_ColorMax, lerpFactor);
				}
				else
				{
					Color color = this.m_GradientMax.Evaluate(time);
					if (this.m_Mode == ParticleSystemGradientMode.TwoGradients)
					{
						result = Color.Lerp(this.m_GradientMin.Evaluate(time), color, lerpFactor);
					}
					else
					{
						result = color;
					}
				}
				return result;
			}

			public static implicit operator ParticleSystem.MinMaxGradient(Color color)
			{
				return new ParticleSystem.MinMaxGradient(color);
			}

			public static implicit operator ParticleSystem.MinMaxGradient(Gradient gradient)
			{
				return new ParticleSystem.MinMaxGradient(gradient);
			}
		}

		public struct MainModule
		{
			private ParticleSystem m_ParticleSystem;

			public float duration
			{
				get
				{
					return ParticleSystem.MainModule.GetDuration(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetDuration(this.m_ParticleSystem, value);
				}
			}

			public bool loop
			{
				get
				{
					return ParticleSystem.MainModule.GetLoop(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetLoop(this.m_ParticleSystem, value);
				}
			}

			public bool prewarm
			{
				get
				{
					return ParticleSystem.MainModule.GetPrewarm(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetPrewarm(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startDelay
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartDelay(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartDelay(this.m_ParticleSystem, ref value);
				}
			}

			public float startDelayMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartDelayMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartDelayMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startLifetime
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartLifetime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartLifetime(this.m_ParticleSystem, ref value);
				}
			}

			public float startLifetimeMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartLifetimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartLifetimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSpeed(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSpeed(this.m_ParticleSystem, ref value);
				}
			}

			public float startSpeedMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool startSize3D
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSize3D(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSize3D(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startSize
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeX(this.m_ParticleSystem, ref value);
				}
			}

			public float startSizeMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startSizeX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeX(this.m_ParticleSystem, ref value);
				}
			}

			public float startSizeXMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startSizeY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeY(this.m_ParticleSystem, ref value);
				}
			}

			public float startSizeYMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startSizeZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartSizeZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeZ(this.m_ParticleSystem, ref value);
				}
			}

			public float startSizeZMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartSizeZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartSizeZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool startRotation3D
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotation3D(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotation3D(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startRotation
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZ(this.m_ParticleSystem, ref value);
				}
			}

			public float startRotationMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startRotationX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationX(this.m_ParticleSystem, ref value);
				}
			}

			public float startRotationXMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startRotationY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationY(this.m_ParticleSystem, ref value);
				}
			}

			public float startRotationYMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startRotationZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetStartRotationZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZ(this.m_ParticleSystem, ref value);
				}
			}

			public float startRotationZMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetStartRotationZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetStartRotationZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float randomizeRotationDirection
			{
				get
				{
					return ParticleSystem.MainModule.GetRandomizeRotationDirection(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetRandomizeRotationDirection(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxGradient startColor
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.MainModule.GetStartColor(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetStartColor(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve gravityModifier
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.MainModule.GetGravityModifier(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.MainModule.SetGravityModifier(this.m_ParticleSystem, ref value);
				}
			}

			public float gravityModifierMultiplier
			{
				get
				{
					return ParticleSystem.MainModule.GetGravityModifierMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetGravityModifierMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemSimulationSpace simulationSpace
			{
				get
				{
					return ParticleSystem.MainModule.GetSimulationSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetSimulationSpace(this.m_ParticleSystem, value);
				}
			}

			public Transform customSimulationSpace
			{
				get
				{
					return ParticleSystem.MainModule.GetCustomSimulationSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetCustomSimulationSpace(this.m_ParticleSystem, value);
				}
			}

			public float simulationSpeed
			{
				get
				{
					return ParticleSystem.MainModule.GetSimulationSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetSimulationSpeed(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemScalingMode scalingMode
			{
				get
				{
					return ParticleSystem.MainModule.GetScalingMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetScalingMode(this.m_ParticleSystem, value);
				}
			}

			public bool playOnAwake
			{
				get
				{
					return ParticleSystem.MainModule.GetPlayOnAwake(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetPlayOnAwake(this.m_ParticleSystem, value);
				}
			}

			public int maxParticles
			{
				get
				{
					return ParticleSystem.MainModule.GetMaxParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.MainModule.SetMaxParticles(this.m_ParticleSystem, value);
				}
			}

			internal MainModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDuration(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDuration(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLoop(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetLoop(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPrewarm(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetPrewarm(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartDelay(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartDelayMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartDelayMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartLifetimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartLifetimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSize3D(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetStartSize3D(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartSizeZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartSizeZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartSizeZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotation3D(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetStartRotation3D(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartRotationZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartRotationZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartRotationZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomizeRotationDirection(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRandomizeRotationDirection(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetGravityModifier(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetGravityModifierMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetGravityModifierMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSimulationSpace(ParticleSystem system, ParticleSystemSimulationSpace value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemSimulationSpace GetSimulationSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCustomSimulationSpace(ParticleSystem system, Transform value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetCustomSimulationSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSimulationSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetSimulationSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScalingMode(ParticleSystem system, ParticleSystemScalingMode value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystemScalingMode GetScalingMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlayOnAwake(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetPlayOnAwake(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxParticles(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxParticles(ParticleSystem system);
		}

		public struct EmissionModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.EmissionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve rateOverTime
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRateOverTime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTime(this.m_ParticleSystem, ref value);
				}
			}

			public float rateOverTimeMultiplier
			{
				get
				{
					return ParticleSystem.EmissionModule.GetRateOverTimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve rateOverDistance
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRateOverDistance(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverDistance(this.m_ParticleSystem, ref value);
				}
			}

			public float rateOverDistanceMultiplier
			{
				get
				{
					return ParticleSystem.EmissionModule.GetRateOverDistanceMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverDistanceMultiplier(this.m_ParticleSystem, value);
				}
			}

			public int burstCount
			{
				get
				{
					return ParticleSystem.EmissionModule.GetBurstCount(this.m_ParticleSystem);
				}
			}

			[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.")]
			public ParticleSystemEmissionType type
			{
				get
				{
					return ParticleSystemEmissionType.Time;
				}
				set
				{
				}
			}

			[Obsolete("rate property is deprecated. Use rateOverTime or rateOverDistance instead.")]
			public ParticleSystem.MinMaxCurve rate
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRateOverTime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTime(this.m_ParticleSystem, ref value);
				}
			}

			[Obsolete("rateMultiplier property is deprecated. Use rateOverTimeMultiplier or rateOverDistanceMultiplier instead.")]
			public float rateMultiplier
			{
				get
				{
					return ParticleSystem.EmissionModule.GetRateOverTimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetRateOverTimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			internal EmissionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void SetBursts(ParticleSystem.Burst[] bursts)
			{
				ParticleSystem.EmissionModule.SetBursts(this.m_ParticleSystem, bursts, bursts.Length);
			}

			public void SetBursts(ParticleSystem.Burst[] bursts, int size)
			{
				ParticleSystem.EmissionModule.SetBursts(this.m_ParticleSystem, bursts, size);
			}

			public int GetBursts(ParticleSystem.Burst[] bursts)
			{
				return ParticleSystem.EmissionModule.GetBursts(this.m_ParticleSystem, bursts);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBurstCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRateOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverTimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRateOverTimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRateOverDistance(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRateOverDistanceMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRateOverDistanceMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts, int size);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts);
		}

		public struct ShapeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.ShapeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemShapeType shapeType
			{
				get
				{
					return (ParticleSystemShapeType)ParticleSystem.ShapeModule.GetShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetShapeType(this.m_ParticleSystem, (int)value);
				}
			}

			public float randomDirectionAmount
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomDirectionAmount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomDirectionAmount(this.m_ParticleSystem, value);
				}
			}

			public float sphericalDirectionAmount
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSphericalDirectionAmount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSphericalDirectionAmount(this.m_ParticleSystem, value);
				}
			}

			public bool alignToDirection
			{
				get
				{
					return ParticleSystem.ShapeModule.GetAlignToDirection(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetAlignToDirection(this.m_ParticleSystem, value);
				}
			}

			public float radius
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadius(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadius(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemShapeMultiModeValue radiusMode
			{
				get
				{
					return (ParticleSystemShapeMultiModeValue)ParticleSystem.ShapeModule.GetRadiusMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusMode(this.m_ParticleSystem, (int)value);
				}
			}

			public float radiusSpread
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusSpread(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpread(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve radiusSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ShapeModule.GetRadiusSpeed(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpeed(this.m_ParticleSystem, ref value);
				}
			}

			public float radiusSpeedMultiplier
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRadiusSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRadiusSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float angle
			{
				get
				{
					return ParticleSystem.ShapeModule.GetAngle(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetAngle(this.m_ParticleSystem, value);
				}
			}

			public float length
			{
				get
				{
					return ParticleSystem.ShapeModule.GetLength(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetLength(this.m_ParticleSystem, value);
				}
			}

			public Vector3 box
			{
				get
				{
					return ParticleSystem.ShapeModule.GetBox(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetBox(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemMeshShapeType meshShapeType
			{
				get
				{
					return (ParticleSystemMeshShapeType)ParticleSystem.ShapeModule.GetMeshShapeType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshShapeType(this.m_ParticleSystem, (int)value);
				}
			}

			public Mesh mesh
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMesh(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMesh(this.m_ParticleSystem, value);
				}
			}

			public MeshRenderer meshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			public SkinnedMeshRenderer skinnedMeshRenderer
			{
				get
				{
					return ParticleSystem.ShapeModule.GetSkinnedMeshRenderer(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetSkinnedMeshRenderer(this.m_ParticleSystem, value);
				}
			}

			public bool useMeshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			public int meshMaterialIndex
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshMaterialIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshMaterialIndex(this.m_ParticleSystem, value);
				}
			}

			public bool useMeshColors
			{
				get
				{
					return ParticleSystem.ShapeModule.GetUseMeshColors(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetUseMeshColors(this.m_ParticleSystem, value);
				}
			}

			public float normalOffset
			{
				get
				{
					return ParticleSystem.ShapeModule.GetNormalOffset(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetNormalOffset(this.m_ParticleSystem, value);
				}
			}

			public float meshScale
			{
				get
				{
					return ParticleSystem.ShapeModule.GetMeshScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetMeshScale(this.m_ParticleSystem, value);
				}
			}

			public float arc
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArc(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArc(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemShapeMultiModeValue arcMode
			{
				get
				{
					return (ParticleSystemShapeMultiModeValue)ParticleSystem.ShapeModule.GetArcMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcMode(this.m_ParticleSystem, (int)value);
				}
			}

			public float arcSpread
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArcSpread(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpread(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve arcSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ShapeModule.GetArcSpeed(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpeed(this.m_ParticleSystem, ref value);
				}
			}

			public float arcSpeedMultiplier
			{
				get
				{
					return ParticleSystem.ShapeModule.GetArcSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetArcSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			[Obsolete("randomDirection property is deprecated. Use randomDirectionAmount instead.")]
			public bool randomDirection
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomDirectionAmount(this.m_ParticleSystem) >= 0.5f;
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomDirectionAmount(this.m_ParticleSystem, 1f);
				}
			}

			internal ShapeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetShapeType(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetShapeType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomDirectionAmount(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRandomDirectionAmount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSphericalDirectionAmount(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetSphericalDirectionAmount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAlignToDirection(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetAlignToDirection(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadius(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadius(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusMode(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRadiusMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpread(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusSpread(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRadiusSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAngle(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetAngle(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLength(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLength(ParticleSystem system);

			private static void SetBox(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetBox(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetBox(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetBox(ParticleSystem system)
			{
				Vector3 result;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetBox(system, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetBox(ParticleSystem system, out Vector3 value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshShapeType(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshShapeType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMesh(ParticleSystem system, Mesh value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Mesh GetMesh(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshMaterialIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshColors(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshColors(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNormalOffset(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetNormalOffset(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMeshScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArc(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArc(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcMode(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetArcMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpread(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArcSpread(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetArcSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArcSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArcSpeedMultiplier(ParticleSystem system);
		}

		public struct VelocityOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.VelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.VelocityOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.VelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.VelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);
		}

		public struct LimitVelocityOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve limitX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float limitXMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve limitY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public float limitYMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve limitZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float limitZMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve limit
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LimitVelocityOverLifetimeModule.GetMagnitude(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMagnitude(this.m_ParticleSystem, ref value);
				}
			}

			public float limitMultiplier
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetMagnitudeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetMagnitudeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float dampen
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetDampen(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetDampen(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.LimitVelocityOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.LimitVelocityOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.LimitVelocityOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitudeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMagnitudeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampen(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);
		}

		public struct InheritVelocityModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemInheritVelocityMode mode
			{
				get
				{
					return (ParticleSystemInheritVelocityMode)ParticleSystem.InheritVelocityModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetMode(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystem.MinMaxCurve curve
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.InheritVelocityModule.GetCurve(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetCurve(this.m_ParticleSystem, ref value);
				}
			}

			public float curveMultiplier
			{
				get
				{
					return ParticleSystem.InheritVelocityModule.GetCurveMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.InheritVelocityModule.SetCurveMultiplier(this.m_ParticleSystem, value);
				}
			}

			internal InheritVelocityModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurveMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetCurveMultiplier(ParticleSystem system);
		}

		public struct ForceOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.ForceOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemSimulationSpace space
			{
				get
				{
					return (!ParticleSystem.ForceOverLifetimeModule.GetWorldSpace(this.m_ParticleSystem)) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
				}
			}

			public bool randomized
			{
				get
				{
					return ParticleSystem.ForceOverLifetimeModule.GetRandomized(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ForceOverLifetimeModule.SetRandomized(this.m_ParticleSystem, value);
				}
			}

			internal ForceOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomized(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRandomized(ParticleSystem system);
		}

		public struct ColorOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorOverLifetimeModule.GetColor(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ColorOverLifetimeModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			internal ColorOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
		}

		public struct ColorBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxGradient color
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.ColorBySpeedModule.GetColor(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetColor(this.m_ParticleSystem, ref value);
				}
			}

			public Vector2 range
			{
				get
				{
					return ParticleSystem.ColorBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ColorBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal ColorBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		public struct SizeOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float sizeMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.SizeOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			internal SizeOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);
		}

		public struct SizeBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve size
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float sizeMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.SizeBySpeedModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			public Vector2 range
			{
				get
				{
					return ParticleSystem.SizeBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SizeBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal SizeBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		public struct RotationOverLifetimeModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationOverLifetimeModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationOverLifetimeModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationOverLifetimeModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			internal RotationOverLifetimeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);
		}

		public struct RotationBySpeedModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve x
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetX(this.m_ParticleSystem, ref value);
				}
			}

			public float xMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve y
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetY(this.m_ParticleSystem, ref value);
				}
			}

			public float yMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve z
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.RotationBySpeedModule.GetZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetZ(this.m_ParticleSystem, ref value);
				}
			}

			public float zMultiplier
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			public Vector2 range
			{
				get
				{
					return ParticleSystem.RotationBySpeedModule.GetRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.RotationBySpeedModule.SetRange(this.m_ParticleSystem, value);
				}
			}

			internal RotationBySpeedModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
		}

		public struct ExternalForcesModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public float multiplier
			{
				get
				{
					return ParticleSystem.ExternalForcesModule.GetMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ExternalForcesModule.SetMultiplier(this.m_ParticleSystem, value);
				}
			}

			internal ExternalForcesModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMultiplier(ParticleSystem system);
		}

		public struct NoiseModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.NoiseModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public bool separateAxes
			{
				get
				{
					return ParticleSystem.NoiseModule.GetSeparateAxes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetSeparateAxes(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve strength
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthX(this.m_ParticleSystem, ref value);
				}
			}

			public float strengthMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve strengthX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthX(this.m_ParticleSystem, ref value);
				}
			}

			public float strengthXMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve strengthY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthY(this.m_ParticleSystem, ref value);
				}
			}

			public float strengthYMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve strengthZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetStrengthZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthZ(this.m_ParticleSystem, ref value);
				}
			}

			public float strengthZMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetStrengthZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetStrengthZMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float frequency
			{
				get
				{
					return ParticleSystem.NoiseModule.GetFrequency(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetFrequency(this.m_ParticleSystem, value);
				}
			}

			public bool damping
			{
				get
				{
					return ParticleSystem.NoiseModule.GetDamping(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetDamping(this.m_ParticleSystem, value);
				}
			}

			public int octaveCount
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveCount(this.m_ParticleSystem, value);
				}
			}

			public float octaveMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float octaveScale
			{
				get
				{
					return ParticleSystem.NoiseModule.GetOctaveScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetOctaveScale(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemNoiseQuality quality
			{
				get
				{
					return (ParticleSystemNoiseQuality)ParticleSystem.NoiseModule.GetQuality(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetQuality(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystem.MinMaxCurve scrollSpeed
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetScrollSpeed(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetScrollSpeed(this.m_ParticleSystem, ref value);
				}
			}

			public float scrollSpeedMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetScrollSpeedMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetScrollSpeedMultiplier(this.m_ParticleSystem, value);
				}
			}

			public bool remapEnabled
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve remap
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapX(this.m_ParticleSystem, ref value);
				}
			}

			public float remapMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve remapX
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapX(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapX(this.m_ParticleSystem, ref value);
				}
			}

			public float remapXMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapXMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapXMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve remapY
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapY(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapY(this.m_ParticleSystem, ref value);
				}
			}

			public float remapYMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapYMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapYMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve remapZ
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.NoiseModule.GetRemapZ(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapZ(this.m_ParticleSystem, ref value);
				}
			}

			public float remapZMultiplier
			{
				get
				{
					return ParticleSystem.NoiseModule.GetRemapZMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.NoiseModule.SetRemapZMultiplier(this.m_ParticleSystem, value);
				}
			}

			internal NoiseModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStrengthZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStrengthZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStrengthZMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrequency(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFrequency(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDamping(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetDamping(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveCount(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetOctaveCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOctaveMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOctaveScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetOctaveScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetScrollSpeed(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetScrollSpeedMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetScrollSpeedMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRemapEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRemapZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapXMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapXMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapYMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapYMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRemapZMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRemapZMultiplier(ParticleSystem system);
		}

		public struct CollisionModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemCollisionType type
			{
				get
				{
					return (ParticleSystemCollisionType)ParticleSystem.CollisionModule.GetType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetType(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystemCollisionMode mode
			{
				get
				{
					return (ParticleSystemCollisionMode)ParticleSystem.CollisionModule.GetMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMode(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystem.MinMaxCurve dampen
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetDampen(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetDampen(this.m_ParticleSystem, ref value);
				}
			}

			public float dampenMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetDampenMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetDampenMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve bounce
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetBounce(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetBounce(this.m_ParticleSystem, ref value);
				}
			}

			public float bounceMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetBounceMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetBounceMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve lifetimeLoss
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetLifetimeLoss(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetLifetimeLoss(this.m_ParticleSystem, ref value);
				}
			}

			public float lifetimeLossMultiplier
			{
				get
				{
					return ParticleSystem.CollisionModule.GetLifetimeLossMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetLifetimeLossMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float minKillSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMinKillSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMinKillSpeed(this.m_ParticleSystem, value);
				}
			}

			public float maxKillSpeed
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxKillSpeed(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMaxKillSpeed(this.m_ParticleSystem, value);
				}
			}

			public LayerMask collidesWith
			{
				get
				{
					return ParticleSystem.CollisionModule.GetCollidesWith(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetCollidesWith(this.m_ParticleSystem, value);
				}
			}

			public bool enableDynamicColliders
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableDynamicColliders(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableDynamicColliders(this.m_ParticleSystem, value);
				}
			}

			public bool enableInteriorCollisions
			{
				get
				{
					return ParticleSystem.CollisionModule.GetEnableInteriorCollisions(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnableInteriorCollisions(this.m_ParticleSystem, value);
				}
			}

			public int maxCollisionShapes
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxCollisionShapes(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetMaxCollisionShapes(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemCollisionQuality quality
			{
				get
				{
					return (ParticleSystemCollisionQuality)ParticleSystem.CollisionModule.GetQuality(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetQuality(this.m_ParticleSystem, (int)value);
				}
			}

			public float voxelSize
			{
				get
				{
					return ParticleSystem.CollisionModule.GetVoxelSize(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetVoxelSize(this.m_ParticleSystem, value);
				}
			}

			public float radiusScale
			{
				get
				{
					return ParticleSystem.CollisionModule.GetRadiusScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetRadiusScale(this.m_ParticleSystem, value);
				}
			}

			public bool sendCollisionMessages
			{
				get
				{
					return ParticleSystem.CollisionModule.GetUsesCollisionMessages(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CollisionModule.SetUsesCollisionMessages(this.m_ParticleSystem, value);
				}
			}

			public int maxPlaneCount
			{
				get
				{
					return ParticleSystem.CollisionModule.GetMaxPlaneCount(this.m_ParticleSystem);
				}
			}

			internal CollisionModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void SetPlane(int index, Transform transform)
			{
				ParticleSystem.CollisionModule.SetPlane(this.m_ParticleSystem, index, transform);
			}

			public Transform GetPlane(int index)
			{
				return ParticleSystem.CollisionModule.GetPlane(this.m_ParticleSystem, index);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampenMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampenMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounceMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetBounceMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLifetimeLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeLossMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLifetimeLossMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinKillSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinKillSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxKillSpeed(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMaxKillSpeed(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollidesWith(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCollidesWith(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableDynamicColliders(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableInteriorCollisions(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxCollisionShapes(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVoxelSize(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetVoxelSize(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUsesCollisionMessages(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlane(ParticleSystem system, int index, Transform transform);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetPlane(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxPlaneCount(ParticleSystem system);
		}

		public struct TriggerModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.TriggerModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemOverlapAction inside
			{
				get
				{
					return (ParticleSystemOverlapAction)ParticleSystem.TriggerModule.GetInside(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetInside(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystemOverlapAction outside
			{
				get
				{
					return (ParticleSystemOverlapAction)ParticleSystem.TriggerModule.GetOutside(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetOutside(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystemOverlapAction enter
			{
				get
				{
					return (ParticleSystemOverlapAction)ParticleSystem.TriggerModule.GetEnter(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetEnter(this.m_ParticleSystem, (int)value);
				}
			}

			public ParticleSystemOverlapAction exit
			{
				get
				{
					return (ParticleSystemOverlapAction)ParticleSystem.TriggerModule.GetExit(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetExit(this.m_ParticleSystem, (int)value);
				}
			}

			public float radiusScale
			{
				get
				{
					return ParticleSystem.TriggerModule.GetRadiusScale(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TriggerModule.SetRadiusScale(this.m_ParticleSystem, value);
				}
			}

			public int maxColliderCount
			{
				get
				{
					return ParticleSystem.TriggerModule.GetMaxColliderCount(this.m_ParticleSystem);
				}
			}

			internal TriggerModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void SetCollider(int index, Component collider)
			{
				ParticleSystem.TriggerModule.SetCollider(this.m_ParticleSystem, index, collider);
			}

			public Component GetCollider(int index)
			{
				return ParticleSystem.TriggerModule.GetCollider(this.m_ParticleSystem, index);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInside(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetInside(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOutside(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetOutside(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnter(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetEnter(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetExit(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetExit(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollider(ParticleSystem system, int index, Component collider);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Component GetCollider(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxColliderCount(ParticleSystem system);
		}

		public struct SubEmittersModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public int subEmittersCount
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetSubEmittersCount(this.m_ParticleSystem);
				}
			}

			[Obsolete("birth0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem birth0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 0, value);
				}
			}

			[Obsolete("birth1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem birth1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetBirth(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetBirth(this.m_ParticleSystem, 1, value);
				}
			}

			[Obsolete("collision0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem collision0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 0, value);
				}
			}

			[Obsolete("collision1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem collision1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetCollision(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetCollision(this.m_ParticleSystem, 1, value);
				}
			}

			[Obsolete("death0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem death0
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 0);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 0, value);
				}
			}

			[Obsolete("death1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.")]
			public ParticleSystem death1
			{
				get
				{
					return ParticleSystem.SubEmittersModule.GetDeath(this.m_ParticleSystem, 1);
				}
				set
				{
					ParticleSystem.SubEmittersModule.SetDeath(this.m_ParticleSystem, 1, value);
				}
			}

			internal SubEmittersModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
			{
				ParticleSystem.SubEmittersModule.AddSubEmitter(this.m_ParticleSystem, subEmitter, (int)type, (int)properties);
			}

			public void RemoveSubEmitter(int index)
			{
				ParticleSystem.SubEmittersModule.RemoveSubEmitter(this.m_ParticleSystem, index);
			}

			public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterSystem(this.m_ParticleSystem, index, subEmitter);
			}

			public void SetSubEmitterType(int index, ParticleSystemSubEmitterType type)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterType(this.m_ParticleSystem, index, (int)type);
			}

			public void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties)
			{
				ParticleSystem.SubEmittersModule.SetSubEmitterProperties(this.m_ParticleSystem, index, (int)properties);
			}

			public ParticleSystem GetSubEmitterSystem(int index)
			{
				return ParticleSystem.SubEmittersModule.GetSubEmitterSystem(this.m_ParticleSystem, index);
			}

			public ParticleSystemSubEmitterType GetSubEmitterType(int index)
			{
				return (ParticleSystemSubEmitterType)ParticleSystem.SubEmittersModule.GetSubEmitterType(this.m_ParticleSystem, index);
			}

			public ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index)
			{
				return (ParticleSystemSubEmitterProperties)ParticleSystem.SubEmittersModule.GetSubEmitterProperties(this.m_ParticleSystem, index);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmittersCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetBirth(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetCollision(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetDeath(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void AddSubEmitter(ParticleSystem system, ParticleSystem subEmitter, int type, int properties);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void RemoveSubEmitter(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterSystem(ParticleSystem system, int index, ParticleSystem subEmitter);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterType(ParticleSystem system, int index, int type);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubEmitterProperties(ParticleSystem system, int index, int properties);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetSubEmitterSystem(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmitterType(ParticleSystem system, int index);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubEmitterProperties(ParticleSystem system, int index);
		}

		public struct TextureSheetAnimationModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public int numTilesX
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesX(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesX(this.m_ParticleSystem, value);
				}
			}

			public int numTilesY
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetNumTilesY(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetNumTilesY(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemAnimationType animation
			{
				get
				{
					return (ParticleSystemAnimationType)ParticleSystem.TextureSheetAnimationModule.GetAnimationType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetAnimationType(this.m_ParticleSystem, (int)value);
				}
			}

			public bool useRandomRow
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetUseRandomRow(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetUseRandomRow(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve frameOverTime
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TextureSheetAnimationModule.GetFrameOverTime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFrameOverTime(this.m_ParticleSystem, ref value);
				}
			}

			public float frameOverTimeMultiplier
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFrameOverTimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFrameOverTimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve startFrame
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TextureSheetAnimationModule.GetStartFrame(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetStartFrame(this.m_ParticleSystem, ref value);
				}
			}

			public float startFrameMultiplier
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetStartFrameMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetStartFrameMultiplier(this.m_ParticleSystem, value);
				}
			}

			public int cycleCount
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetCycleCount(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetCycleCount(this.m_ParticleSystem, value);
				}
			}

			public int rowIndex
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetRowIndex(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetRowIndex(this.m_ParticleSystem, value);
				}
			}

			public UVChannelFlags uvChannelMask
			{
				get
				{
					return (UVChannelFlags)ParticleSystem.TextureSheetAnimationModule.GetUVChannelMask(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetUVChannelMask(this.m_ParticleSystem, (int)value);
				}
			}

			public float flipU
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFlipU(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFlipU(this.m_ParticleSystem, value);
				}
			}

			public float flipV
			{
				get
				{
					return ParticleSystem.TextureSheetAnimationModule.GetFlipV(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TextureSheetAnimationModule.SetFlipV(this.m_ParticleSystem, value);
				}
			}

			internal TextureSheetAnimationModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesX(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesX(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesY(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesY(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAnimationType(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetAnimationType(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomRow(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomRow(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFrameOverTimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartFrameMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetStartFrameMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCycleCount(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCycleCount(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRowIndex(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRowIndex(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUVChannelMask(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetUVChannelMask(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFlipU(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFlipU(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFlipV(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetFlipV(ParticleSystem system);
		}

		public struct LightsModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.LightsModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public float ratio
			{
				get
				{
					return ParticleSystem.LightsModule.GetRatio(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetRatio(this.m_ParticleSystem, value);
				}
			}

			public bool useRandomDistribution
			{
				get
				{
					return ParticleSystem.LightsModule.GetUseRandomDistribution(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetUseRandomDistribution(this.m_ParticleSystem, value);
				}
			}

			public Light light
			{
				get
				{
					return ParticleSystem.LightsModule.GetLightPrefab(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetLightPrefab(this.m_ParticleSystem, value);
				}
			}

			public bool useParticleColor
			{
				get
				{
					return ParticleSystem.LightsModule.GetUseParticleColor(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetUseParticleColor(this.m_ParticleSystem, value);
				}
			}

			public bool sizeAffectsRange
			{
				get
				{
					return ParticleSystem.LightsModule.GetSizeAffectsRange(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetSizeAffectsRange(this.m_ParticleSystem, value);
				}
			}

			public bool alphaAffectsIntensity
			{
				get
				{
					return ParticleSystem.LightsModule.GetAlphaAffectsIntensity(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetAlphaAffectsIntensity(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve range
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LightsModule.GetRange(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LightsModule.SetRange(this.m_ParticleSystem, ref value);
				}
			}

			public float rangeMultiplier
			{
				get
				{
					return ParticleSystem.LightsModule.GetRangeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetRangeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve intensity
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.LightsModule.GetIntensity(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.LightsModule.SetIntensity(this.m_ParticleSystem, ref value);
				}
			}

			public float intensityMultiplier
			{
				get
				{
					return ParticleSystem.LightsModule.GetIntensityMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetIntensityMultiplier(this.m_ParticleSystem, value);
				}
			}

			public int maxLights
			{
				get
				{
					return ParticleSystem.LightsModule.GetMaxLights(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.LightsModule.SetMaxLights(this.m_ParticleSystem, value);
				}
			}

			internal LightsModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRatio(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRatio(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomDistribution(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomDistribution(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLightPrefab(ParticleSystem system, Light value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Light GetLightPrefab(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseParticleColor(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseParticleColor(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsRange(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsRange(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAlphaAffectsIntensity(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetAlphaAffectsIntensity(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRange(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRangeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRangeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetIntensity(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetIntensityMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetIntensityMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxLights(ParticleSystem system, int value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxLights(ParticleSystem system);
		}

		public struct TrailModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.TrailModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			public float ratio
			{
				get
				{
					return ParticleSystem.TrailModule.GetRatio(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetRatio(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxCurve lifetime
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TrailModule.GetLifetime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TrailModule.SetLifetime(this.m_ParticleSystem, ref value);
				}
			}

			public float lifetimeMultiplier
			{
				get
				{
					return ParticleSystem.TrailModule.GetLifetimeMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetLifetimeMultiplier(this.m_ParticleSystem, value);
				}
			}

			public float minVertexDistance
			{
				get
				{
					return ParticleSystem.TrailModule.GetMinVertexDistance(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetMinVertexDistance(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystemTrailTextureMode textureMode
			{
				get
				{
					return (ParticleSystemTrailTextureMode)ParticleSystem.TrailModule.GetTextureMode(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetTextureMode(this.m_ParticleSystem, (float)value);
				}
			}

			public bool worldSpace
			{
				get
				{
					return ParticleSystem.TrailModule.GetWorldSpace(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetWorldSpace(this.m_ParticleSystem, value);
				}
			}

			public bool dieWithParticles
			{
				get
				{
					return ParticleSystem.TrailModule.GetDieWithParticles(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetDieWithParticles(this.m_ParticleSystem, value);
				}
			}

			public bool sizeAffectsWidth
			{
				get
				{
					return ParticleSystem.TrailModule.GetSizeAffectsWidth(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetSizeAffectsWidth(this.m_ParticleSystem, value);
				}
			}

			public bool sizeAffectsLifetime
			{
				get
				{
					return ParticleSystem.TrailModule.GetSizeAffectsLifetime(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetSizeAffectsLifetime(this.m_ParticleSystem, value);
				}
			}

			public bool inheritParticleColor
			{
				get
				{
					return ParticleSystem.TrailModule.GetInheritParticleColor(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetInheritParticleColor(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxGradient colorOverLifetime
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.TrailModule.GetColorOverLifetime(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TrailModule.SetColorOverLifetime(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystem.MinMaxCurve widthOverTrail
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.TrailModule.GetWidthOverTrail(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TrailModule.SetWidthOverTrail(this.m_ParticleSystem, ref value);
				}
			}

			public float widthOverTrailMultiplier
			{
				get
				{
					return ParticleSystem.TrailModule.GetWidthOverTrailMultiplier(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.TrailModule.SetWidthOverTrailMultiplier(this.m_ParticleSystem, value);
				}
			}

			public ParticleSystem.MinMaxGradient colorOverTrail
			{
				get
				{
					ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
					ParticleSystem.TrailModule.GetColorOverTrail(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.TrailModule.SetColorOverTrail(this.m_ParticleSystem, ref value);
				}
			}

			internal TrailModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRatio(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRatio(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLifetime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLifetimeMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLifetimeMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinVertexDistance(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinVertexDistance(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetTextureMode(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetTextureMode(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDieWithParticles(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetDieWithParticles(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsWidth(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsWidth(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSizeAffectsLifetime(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSizeAffectsLifetime(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInheritParticleColor(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetInheritParticleColor(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColorOverLifetime(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetWidthOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWidthOverTrailMultiplier(ParticleSystem system, float value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetWidthOverTrailMultiplier(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColorOverTrail(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
		}

		public struct CustomDataModule
		{
			private ParticleSystem m_ParticleSystem;

			public bool enabled
			{
				get
				{
					return ParticleSystem.CustomDataModule.GetEnabled(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.CustomDataModule.SetEnabled(this.m_ParticleSystem, value);
				}
			}

			internal CustomDataModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			public void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode)
			{
				ParticleSystem.CustomDataModule.SetMode(this.m_ParticleSystem, (int)stream, (int)mode);
			}

			public ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream)
			{
				return (ParticleSystemCustomDataMode)ParticleSystem.CustomDataModule.GetMode(this.m_ParticleSystem, (int)stream);
			}

			public void SetVectorComponentCount(ParticleSystemCustomData stream, int count)
			{
				ParticleSystem.CustomDataModule.SetVectorComponentCount(this.m_ParticleSystem, (int)stream, count);
			}

			public int GetVectorComponentCount(ParticleSystemCustomData stream)
			{
				return ParticleSystem.CustomDataModule.GetVectorComponentCount(this.m_ParticleSystem, (int)stream);
			}

			public void SetVector(ParticleSystemCustomData stream, int component, ParticleSystem.MinMaxCurve curve)
			{
				ParticleSystem.CustomDataModule.SetVector(this.m_ParticleSystem, (int)stream, component, ref curve);
			}

			public ParticleSystem.MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
			{
				ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
				ParticleSystem.CustomDataModule.GetVector(this.m_ParticleSystem, (int)stream, component, ref result);
				return result;
			}

			public void SetColor(ParticleSystemCustomData stream, ParticleSystem.MinMaxGradient gradient)
			{
				ParticleSystem.CustomDataModule.SetColor(this.m_ParticleSystem, (int)stream, ref gradient);
			}

			public ParticleSystem.MinMaxGradient GetColor(ParticleSystemCustomData stream)
			{
				ParticleSystem.MinMaxGradient result = default(ParticleSystem.MinMaxGradient);
				ParticleSystem.CustomDataModule.GetColor(this.m_ParticleSystem, (int)stream, ref result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int stream, int mode);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVectorComponentCount(ParticleSystem system, int stream, int count);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system, int stream);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVectorComponentCount(ParticleSystem system, int stream);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetVector(ParticleSystem system, int stream, int component, ref ParticleSystem.MinMaxCurve curve);

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, int stream, ref ParticleSystem.MinMaxGradient gradient);
		}

		[RequiredByNativeCode("particleSystemParticle", Optional = true)]
		public struct Particle
		{
			private Vector3 m_Position;

			private Vector3 m_Velocity;

			private Vector3 m_AnimatedVelocity;

			private Vector3 m_InitialVelocity;

			private Vector3 m_AxisOfRotation;

			private Vector3 m_Rotation;

			private Vector3 m_AngularVelocity;

			private Vector3 m_StartSize;

			private Color32 m_StartColor;

			private uint m_RandomSeed;

			private float m_Lifetime;

			private float m_StartLifetime;

			private float m_EmitAccumulator0;

			private float m_EmitAccumulator1;

			public Vector3 position
			{
				get
				{
					return this.m_Position;
				}
				set
				{
					this.m_Position = value;
				}
			}

			public Vector3 velocity
			{
				get
				{
					return this.m_Velocity;
				}
				set
				{
					this.m_Velocity = value;
				}
			}

			[Obsolete("Please use Particle.remainingLifetime instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/Particle.remainingLifetime")]
			public float lifetime
			{
				get
				{
					return this.m_Lifetime;
				}
				set
				{
					this.m_Lifetime = value;
				}
			}

			public float remainingLifetime
			{
				get
				{
					return this.m_Lifetime;
				}
				set
				{
					this.m_Lifetime = value;
				}
			}

			public float startLifetime
			{
				get
				{
					return this.m_StartLifetime;
				}
				set
				{
					this.m_StartLifetime = value;
				}
			}

			public float startSize
			{
				get
				{
					return this.m_StartSize.x;
				}
				set
				{
					this.m_StartSize = new Vector3(value, value, value);
				}
			}

			public Vector3 startSize3D
			{
				get
				{
					return this.m_StartSize;
				}
				set
				{
					this.m_StartSize = value;
				}
			}

			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_AxisOfRotation;
				}
				set
				{
					this.m_AxisOfRotation = value;
				}
			}

			public float rotation
			{
				get
				{
					return this.m_Rotation.z * 57.29578f;
				}
				set
				{
					this.m_Rotation = new Vector3(0f, 0f, value * 0.0174532924f);
				}
			}

			public Vector3 rotation3D
			{
				get
				{
					return this.m_Rotation * 57.29578f;
				}
				set
				{
					this.m_Rotation = value * 0.0174532924f;
				}
			}

			public float angularVelocity
			{
				get
				{
					return this.m_AngularVelocity.z * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity.z = value * 0.0174532924f;
				}
			}

			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_AngularVelocity * 57.29578f;
				}
				set
				{
					this.m_AngularVelocity = value * 0.0174532924f;
				}
			}

			public Color32 startColor
			{
				get
				{
					return this.m_StartColor;
				}
				set
				{
					this.m_StartColor = value;
				}
			}

			[Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
			public float randomValue
			{
				get
				{
					return BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
				}
				set
				{
					this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
				}
			}

			public uint randomSeed
			{
				get
				{
					return this.m_RandomSeed;
				}
				set
				{
					this.m_RandomSeed = value;
				}
			}

			[Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.")]
			public float size
			{
				get
				{
					return this.m_StartSize.x;
				}
				set
				{
					this.m_StartSize = new Vector3(value, value, value);
				}
			}

			[Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.")]
			public Color32 color
			{
				get
				{
					return this.m_StartColor;
				}
				set
				{
					this.m_StartColor = value;
				}
			}

			public float GetCurrentSize(ParticleSystem system)
			{
				return ParticleSystem.Particle.GetCurrentSize(system, ref this);
			}

			public Vector3 GetCurrentSize3D(ParticleSystem system)
			{
				return ParticleSystem.Particle.GetCurrentSize3D(system, ref this);
			}

			public Color32 GetCurrentColor(ParticleSystem system)
			{
				return ParticleSystem.Particle.GetCurrentColor(system, ref this);
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetCurrentSize(ParticleSystem system, ref ParticleSystem.Particle particle);

			private static Vector3 GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle)
			{
				Vector3 result;
				ParticleSystem.Particle.INTERNAL_CALL_GetCurrentSize3D(system, ref particle, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle, out Vector3 value);

			private static Color32 GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle)
			{
				Color32 result;
				ParticleSystem.Particle.INTERNAL_CALL_GetCurrentColor(system, ref particle, out result);
				return result;
			}

			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle, out Color32 value);
		}

		public struct EmitParams
		{
			internal ParticleSystem.Particle m_Particle;

			internal bool m_PositionSet;

			internal bool m_VelocitySet;

			internal bool m_AxisOfRotationSet;

			internal bool m_RotationSet;

			internal bool m_AngularVelocitySet;

			internal bool m_StartSizeSet;

			internal bool m_StartColorSet;

			internal bool m_RandomSeedSet;

			internal bool m_StartLifetimeSet;

			internal bool m_ApplyShapeToPosition;

			public Vector3 position
			{
				get
				{
					return this.m_Particle.position;
				}
				set
				{
					this.m_Particle.position = value;
					this.m_PositionSet = true;
				}
			}

			public bool applyShapeToPosition
			{
				get
				{
					return this.m_ApplyShapeToPosition;
				}
				set
				{
					this.m_ApplyShapeToPosition = value;
				}
			}

			public Vector3 velocity
			{
				get
				{
					return this.m_Particle.velocity;
				}
				set
				{
					this.m_Particle.velocity = value;
					this.m_VelocitySet = true;
				}
			}

			public float startLifetime
			{
				get
				{
					return this.m_Particle.startLifetime;
				}
				set
				{
					this.m_Particle.startLifetime = value;
					this.m_StartLifetimeSet = true;
				}
			}

			public float startSize
			{
				get
				{
					return this.m_Particle.startSize;
				}
				set
				{
					this.m_Particle.startSize = value;
					this.m_StartSizeSet = true;
				}
			}

			public Vector3 startSize3D
			{
				get
				{
					return this.m_Particle.startSize3D;
				}
				set
				{
					this.m_Particle.startSize3D = value;
					this.m_StartSizeSet = true;
				}
			}

			public Vector3 axisOfRotation
			{
				get
				{
					return this.m_Particle.axisOfRotation;
				}
				set
				{
					this.m_Particle.axisOfRotation = value;
					this.m_AxisOfRotationSet = true;
				}
			}

			public float rotation
			{
				get
				{
					return this.m_Particle.rotation;
				}
				set
				{
					this.m_Particle.rotation = value;
					this.m_RotationSet = true;
				}
			}

			public Vector3 rotation3D
			{
				get
				{
					return this.m_Particle.rotation3D;
				}
				set
				{
					this.m_Particle.rotation3D = value;
					this.m_RotationSet = true;
				}
			}

			public float angularVelocity
			{
				get
				{
					return this.m_Particle.angularVelocity;
				}
				set
				{
					this.m_Particle.angularVelocity = value;
					this.m_AngularVelocitySet = true;
				}
			}

			public Vector3 angularVelocity3D
			{
				get
				{
					return this.m_Particle.angularVelocity3D;
				}
				set
				{
					this.m_Particle.angularVelocity3D = value;
					this.m_AngularVelocitySet = true;
				}
			}

			public Color32 startColor
			{
				get
				{
					return this.m_Particle.startColor;
				}
				set
				{
					this.m_Particle.startColor = value;
					this.m_StartColorSet = true;
				}
			}

			public uint randomSeed
			{
				get
				{
					return this.m_Particle.randomSeed;
				}
				set
				{
					this.m_Particle.randomSeed = value;
					this.m_RandomSeedSet = true;
				}
			}

			public void ResetPosition()
			{
				this.m_PositionSet = false;
			}

			public void ResetVelocity()
			{
				this.m_VelocitySet = false;
			}

			public void ResetAxisOfRotation()
			{
				this.m_AxisOfRotationSet = false;
			}

			public void ResetRotation()
			{
				this.m_RotationSet = false;
			}

			public void ResetAngularVelocity()
			{
				this.m_AngularVelocitySet = false;
			}

			public void ResetStartSize()
			{
				this.m_StartSizeSet = false;
			}

			public void ResetStartColor()
			{
				this.m_StartColorSet = false;
			}

			public void ResetRandomSeed()
			{
				this.m_RandomSeedSet = false;
			}

			public void ResetStartLifetime()
			{
				this.m_StartLifetimeSet = false;
			}
		}

		internal delegate bool IteratorDelegate(ParticleSystem ps);

		[Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
		public int safeCollisionEventSize
		{
			get
			{
				return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);
			}
		}

		[Obsolete("startDelay property is deprecated. Use main.startDelay or main.startDelayMultiplier instead.")]
		public extern float startDelay
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isPlaying
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isEmitting
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isStopped
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPaused
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("loop property is deprecated. Use main.loop instead.")]
		public extern bool loop
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("playOnAwake property is deprecated. Use main.playOnAwake instead.")]
		public extern bool playOnAwake
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float time
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("duration property is deprecated. Use main.duration instead.")]
		public extern float duration
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("playbackSpeed property is deprecated. Use main.simulationSpeed instead.")]
		public extern float playbackSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int particleCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("enableEmission property is deprecated. Use emission.enabled instead.")]
		public bool enableEmission
		{
			get
			{
				return this.emission.enabled;
			}
			set
			{
				this.emission.enabled = value;
			}
		}

		[Obsolete("emissionRate property is deprecated. Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.")]
		public float emissionRate
		{
			get
			{
				return this.emission.rateOverTimeMultiplier;
			}
			set
			{
				this.emission.rateOverTime = value;
			}
		}

		[Obsolete("startSpeed property is deprecated. Use main.startSpeed or main.startSpeedMultiplier instead.")]
		public extern float startSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("startSize property is deprecated. Use main.startSize or main.startSizeMultiplier instead.")]
		public extern float startSize
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("startColor property is deprecated. Use main.startColor instead.")]
		public Color startColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_startColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_startColor(ref value);
			}
		}

		[Obsolete("startRotation property is deprecated. Use main.startRotation or main.startRotationMultiplier instead.")]
		public extern float startRotation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("startRotation3D property is deprecated. Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).")]
		public Vector3 startRotation3D
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_startRotation3D(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_startRotation3D(ref value);
			}
		}

		[Obsolete("startLifetime property is deprecated. Use main.startLifetime or main.startLifetimeMultiplier instead.")]
		public extern float startLifetime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("gravityModifier property is deprecated. Use main.gravityModifier or main.gravityModifierMultiplier instead.")]
		public extern float gravityModifier
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("maxParticles property is deprecated. Use main.maxParticles instead.")]
		public extern int maxParticles
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("simulationSpace property is deprecated. Use main.simulationSpace instead.")]
		public extern ParticleSystemSimulationSpace simulationSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("scalingMode property is deprecated. Use main.scalingMode instead.")]
		public extern ParticleSystemScalingMode scalingMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern uint randomSeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useAutoRandomSeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public ParticleSystem.MainModule main
		{
			get
			{
				return new ParticleSystem.MainModule(this);
			}
		}

		public ParticleSystem.EmissionModule emission
		{
			get
			{
				return new ParticleSystem.EmissionModule(this);
			}
		}

		public ParticleSystem.ShapeModule shape
		{
			get
			{
				return new ParticleSystem.ShapeModule(this);
			}
		}

		public ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime
		{
			get
			{
				return new ParticleSystem.VelocityOverLifetimeModule(this);
			}
		}

		public ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime
		{
			get
			{
				return new ParticleSystem.LimitVelocityOverLifetimeModule(this);
			}
		}

		public ParticleSystem.InheritVelocityModule inheritVelocity
		{
			get
			{
				return new ParticleSystem.InheritVelocityModule(this);
			}
		}

		public ParticleSystem.ForceOverLifetimeModule forceOverLifetime
		{
			get
			{
				return new ParticleSystem.ForceOverLifetimeModule(this);
			}
		}

		public ParticleSystem.ColorOverLifetimeModule colorOverLifetime
		{
			get
			{
				return new ParticleSystem.ColorOverLifetimeModule(this);
			}
		}

		public ParticleSystem.ColorBySpeedModule colorBySpeed
		{
			get
			{
				return new ParticleSystem.ColorBySpeedModule(this);
			}
		}

		public ParticleSystem.SizeOverLifetimeModule sizeOverLifetime
		{
			get
			{
				return new ParticleSystem.SizeOverLifetimeModule(this);
			}
		}

		public ParticleSystem.SizeBySpeedModule sizeBySpeed
		{
			get
			{
				return new ParticleSystem.SizeBySpeedModule(this);
			}
		}

		public ParticleSystem.RotationOverLifetimeModule rotationOverLifetime
		{
			get
			{
				return new ParticleSystem.RotationOverLifetimeModule(this);
			}
		}

		public ParticleSystem.RotationBySpeedModule rotationBySpeed
		{
			get
			{
				return new ParticleSystem.RotationBySpeedModule(this);
			}
		}

		public ParticleSystem.ExternalForcesModule externalForces
		{
			get
			{
				return new ParticleSystem.ExternalForcesModule(this);
			}
		}

		public ParticleSystem.NoiseModule noise
		{
			get
			{
				return new ParticleSystem.NoiseModule(this);
			}
		}

		public ParticleSystem.CollisionModule collision
		{
			get
			{
				return new ParticleSystem.CollisionModule(this);
			}
		}

		public ParticleSystem.TriggerModule trigger
		{
			get
			{
				return new ParticleSystem.TriggerModule(this);
			}
		}

		public ParticleSystem.SubEmittersModule subEmitters
		{
			get
			{
				return new ParticleSystem.SubEmittersModule(this);
			}
		}

		public ParticleSystem.TextureSheetAnimationModule textureSheetAnimation
		{
			get
			{
				return new ParticleSystem.TextureSheetAnimationModule(this);
			}
		}

		public ParticleSystem.LightsModule lights
		{
			get
			{
				return new ParticleSystem.LightsModule(this);
			}
		}

		public ParticleSystem.TrailModule trails
		{
			get
			{
				return new ParticleSystem.TrailModule(this);
			}
		}

		public ParticleSystem.CustomDataModule customData
		{
			get
			{
				return new ParticleSystem.CustomDataModule(this);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startColor(ref Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startRotation3D(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startRotation3D(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetParticles(ParticleSystem.Particle[] particles, int size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetParticles(ParticleSystem.Particle[] particles);

		public void SetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			this.SetCustomParticleDataInternal(customData, (int)streamIndex);
		}

		public int GetCustomParticleData(List<Vector4> customData, ParticleSystemCustomData streamIndex)
		{
			return this.GetCustomParticleDataInternal(customData, (int)streamIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetCustomParticleDataInternal(object customData, int streamIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetCustomParticleDataInternal(object customData, int streamIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Simulate(ParticleSystem self, float t, bool restart, bool fixedTimeStep);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Play(ParticleSystem self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Stop(ParticleSystem self, ParticleSystemStopBehavior stopBehavior);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Pause(ParticleSystem self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Clear(ParticleSystem self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_IsAlive(ParticleSystem self);

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren, bool restart)
		{
			bool fixedTimeStep = true;
			this.Simulate(t, withChildren, restart, fixedTimeStep);
		}

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren)
		{
			bool fixedTimeStep = true;
			bool restart = true;
			this.Simulate(t, withChildren, restart, fixedTimeStep);
		}

		[ExcludeFromDocs]
		public void Simulate(float t)
		{
			bool fixedTimeStep = true;
			bool restart = true;
			bool withChildren = true;
			this.Simulate(t, withChildren, restart, fixedTimeStep);
		}

		public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Simulate(ps, t, restart, fixedTimeStep));
		}

		[ExcludeFromDocs]
		public void Play()
		{
			bool withChildren = true;
			this.Play(withChildren);
		}

		public void Play([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Play(ps));
		}

		[ExcludeFromDocs]
		public void Stop(bool withChildren)
		{
			ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;
			this.Stop(withChildren, stopBehavior);
		}

		[ExcludeFromDocs]
		public void Stop()
		{
			ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;
			bool withChildren = true;
			this.Stop(withChildren, stopBehavior);
		}

		public void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Stop(ps, stopBehavior));
		}

		[ExcludeFromDocs]
		public void Pause()
		{
			bool withChildren = true;
			this.Pause(withChildren);
		}

		public void Pause([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Pause(ps));
		}

		[ExcludeFromDocs]
		public void Clear()
		{
			bool withChildren = true;
			this.Clear(withChildren);
		}

		public void Clear([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Clear(ps));
		}

		[ExcludeFromDocs]
		public bool IsAlive()
		{
			bool withChildren = true;
			return this.IsAlive(withChildren);
		}

		public bool IsAlive([DefaultValue("true")] bool withChildren)
		{
			return this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_IsAlive(ps));
		}

		public void Emit(int count)
		{
			ParticleSystem.INTERNAL_CALL_Emit(this, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);

		[Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
		public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
		{
			ParticleSystem.Particle particle = default(ParticleSystem.Particle);
			particle.position = position;
			particle.velocity = velocity;
			particle.lifetime = lifetime;
			particle.startLifetime = lifetime;
			particle.startSize = size;
			particle.rotation3D = Vector3.zero;
			particle.angularVelocity3D = Vector3.zero;
			particle.startColor = color;
			particle.randomSeed = 5u;
			this.Internal_EmitOld(ref particle);
		}

		[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
		public void Emit(ParticleSystem.Particle particle)
		{
			this.Internal_EmitOld(ref particle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_EmitOld(ref ParticleSystem.Particle particle);

		public void Emit(ParticleSystem.EmitParams emitParams, int count)
		{
			this.Internal_Emit(ref emitParams, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Emit(ref ParticleSystem.EmitParams emitParams, int count);

		internal bool IterateParticleSystems(bool recurse, ParticleSystem.IteratorDelegate func)
		{
			bool flag = func(this);
			if (recurse)
			{
				flag |= ParticleSystem.IterateParticleSystemsRecursive(base.transform, func);
			}
			return flag;
		}

		private static bool IterateParticleSystemsRecursive(Transform transform, ParticleSystem.IteratorDelegate func)
		{
			bool flag = false;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				ParticleSystem component = child.gameObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					flag = func(component);
					if (flag)
					{
						break;
					}
					ParticleSystem.IterateParticleSystemsRecursive(child, func);
				}
			}
			return flag;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetupDefaultType(int type);

		internal Matrix4x4 GetLocalToWorldMatrix()
		{
			Matrix4x4 result;
			ParticleSystem.INTERNAL_CALL_GetLocalToWorldMatrix(this, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalToWorldMatrix(ParticleSystem self, out Matrix4x4 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GenerateNoisePreviewTexture(Texture2D dst);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CountSubEmitterParticles(ref int count);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CheckVertexStreamsMatchShader(bool hasTangent, bool hasColor, int texCoordChannelCount, Material material, ref bool tangentError, ref bool colorError, ref bool uvError);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetMaxTexCoordStreams();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GenerateRandomSeed();
	}
}
