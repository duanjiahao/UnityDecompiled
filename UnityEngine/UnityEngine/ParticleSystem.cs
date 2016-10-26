using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class ParticleSystem : Component
	{
		public struct Burst
		{
			private float m_Time;

			private short m_MinCount;

			private short m_MaxCount;

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

			public Burst(float _time, short _count)
			{
				this.m_Time = _time;
				this.m_MinCount = _count;
				this.m_MaxCount = _count;
			}

			public Burst(float _time, short _minCount, short _maxCount)
			{
				this.m_Time = _time;
				this.m_MinCount = _minCount;
				this.m_MaxCount = _maxCount;
			}
		}

		public struct MinMaxCurve
		{
			private ParticleSystemCurveMode m_Mode;

			private float m_CurveScalar;

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

			public float curveScalar
			{
				get
				{
					return this.m_CurveScalar;
				}
				set
				{
					this.m_CurveScalar = value;
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
				this.m_CurveScalar = 0f;
				this.m_CurveMin = null;
				this.m_CurveMax = null;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = constant;
			}

			public MinMaxCurve(float scalar, AnimationCurve curve)
			{
				this.m_Mode = ParticleSystemCurveMode.Curve;
				this.m_CurveScalar = scalar;
				this.m_CurveMin = null;
				this.m_CurveMax = curve;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float scalar, AnimationCurve min, AnimationCurve max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoCurves;
				this.m_CurveScalar = scalar;
				this.m_CurveMin = min;
				this.m_CurveMax = max;
				this.m_ConstantMin = 0f;
				this.m_ConstantMax = 0f;
			}

			public MinMaxCurve(float min, float max)
			{
				this.m_Mode = ParticleSystemCurveMode.TwoConstants;
				this.m_CurveScalar = 0f;
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
				if (this.m_Mode == ParticleSystemCurveMode.Constant)
				{
					return this.m_ConstantMax;
				}
				if (this.m_Mode == ParticleSystemCurveMode.TwoConstants)
				{
					return Mathf.Lerp(this.m_ConstantMin, this.m_ConstantMax, lerpFactor);
				}
				float num = this.m_CurveMax.Evaluate(time) * this.m_CurveScalar;
				if (this.m_Mode == ParticleSystemCurveMode.TwoCurves)
				{
					return Mathf.Lerp(this.m_CurveMin.Evaluate(time) * this.m_CurveScalar, num, lerpFactor);
				}
				return num;
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
				if (this.m_Mode == ParticleSystemGradientMode.Color)
				{
					return this.m_ColorMax;
				}
				if (this.m_Mode == ParticleSystemGradientMode.TwoColors)
				{
					return Color.Lerp(this.m_ColorMin, this.m_ColorMax, lerpFactor);
				}
				Color color = this.m_GradientMax.Evaluate(time);
				if (this.m_Mode == ParticleSystemGradientMode.TwoGradients)
				{
					return Color.Lerp(this.m_GradientMin.Evaluate(time), color, lerpFactor);
				}
				return color;
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

			public ParticleSystem.MinMaxCurve rate
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.EmissionModule.GetRate(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.EmissionModule.SetRate(this.m_ParticleSystem, ref value);
				}
			}

			public ParticleSystemEmissionType type
			{
				get
				{
					return (ParticleSystemEmissionType)ParticleSystem.EmissionModule.GetType(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.EmissionModule.SetType(this.m_ParticleSystem, (int)value);
				}
			}

			public int burstCount
			{
				get
				{
					return ParticleSystem.EmissionModule.GetBurstCount(this.m_ParticleSystem);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetBurstCount(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts, int size);

			[WrapperlessIcall]
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

			public bool randomDirection
			{
				get
				{
					return ParticleSystem.ShapeModule.GetRandomDirection(this.m_ParticleSystem);
				}
				set
				{
					ParticleSystem.ShapeModule.SetRandomDirection(this.m_ParticleSystem, value);
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

			internal ShapeModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetShapeType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetShapeType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomDirection(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetRandomDirection(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadius(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadius(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAngle(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetAngle(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLength(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetLength(ParticleSystem system);

			private static void SetBox(ParticleSystem system, Vector3 value)
			{
				ParticleSystem.ShapeModule.INTERNAL_CALL_SetBox(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetBox(ParticleSystem system, ref Vector3 value);

			private static Vector3 GetBox(ParticleSystem system)
			{
				Vector3 result;
				ParticleSystem.ShapeModule.INTERNAL_CALL_GetBox(system, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetBox(ParticleSystem system, out Vector3 value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshShapeType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshShapeType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMesh(ParticleSystem system, Mesh value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Mesh GetMesh(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMeshMaterialIndex(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseMeshColors(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseMeshColors(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNormalOffset(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetNormalOffset(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetArc(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetArc(ParticleSystem system);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetDampen(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
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

			internal InheritVelocityModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetWorldSpace(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetWorldSpace(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRandomized(ParticleSystem system, bool value);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.ColorBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.SizeBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSeparateAxes(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetSeparateAxes(ParticleSystem system);

			private static void SetRange(ParticleSystem system, Vector2 value)
			{
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_SetRange(system, ref value);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);

			private static Vector2 GetRange(ParticleSystem system)
			{
				Vector2 result;
				ParticleSystem.RotationBySpeedModule.INTERNAL_CALL_GetRange(system, out result);
				return result;
			}

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMultiplier(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMultiplier(ParticleSystem system);
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

			public ParticleSystem.MinMaxCurve lifetimeLoss
			{
				get
				{
					ParticleSystem.MinMaxCurve result = default(ParticleSystem.MinMaxCurve);
					ParticleSystem.CollisionModule.GetEnergyLoss(this.m_ParticleSystem, ref result);
					return result;
				}
				set
				{
					ParticleSystem.CollisionModule.SetEnergyLoss(this.m_ParticleSystem, ref value);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMode(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMode(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMinKillSpeed(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMinKillSpeed(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxKillSpeed(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetMaxKillSpeed(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollidesWith(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCollidesWith(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableDynamicColliders(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnableInteriorCollisions(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetMaxCollisionShapes(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetQuality(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetQuality(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVoxelSize(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetVoxelSize(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUsesCollisionMessages(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetPlane(ParticleSystem system, int index, Transform transform);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Transform GetPlane(ParticleSystem system, int index);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetInside(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetInside(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetOutside(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetOutside(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnter(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetEnter(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetExit(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetExit(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRadiusScale(ParticleSystem system, float value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetRadiusScale(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollider(ParticleSystem system, int index, Component collider);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern Component GetCollider(ParticleSystem system, int index);

			[WrapperlessIcall]
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetBirth(ParticleSystem system, int index);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetCollision(ParticleSystem system, int index);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ParticleSystem GetDeath(ParticleSystem system, int index);
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

			internal TextureSheetAnimationModule(ParticleSystem particleSystem)
			{
				this.m_ParticleSystem = particleSystem;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetEnabled(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetEnabled(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesX(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesX(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetNumTilesY(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetNumTilesY(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetAnimationType(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetAnimationType(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUseRandomRow(ParticleSystem system, bool value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool GetUseRandomRow(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetStartFrame(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetCycleCount(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetCycleCount(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetRowIndex(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetRowIndex(ParticleSystem system);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetUVChannelMask(ParticleSystem system, int value);

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetUVChannelMask(ParticleSystem system);
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

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern float GetCurrentSize(ParticleSystem system, ref ParticleSystem.Particle particle);

			private static Vector3 GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle)
			{
				Vector3 result;
				ParticleSystem.Particle.INTERNAL_CALL_GetCurrentSize3D(system, ref particle, out result);
				return result;
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void INTERNAL_CALL_GetCurrentSize3D(ParticleSystem system, ref ParticleSystem.Particle particle, out Vector3 value);

			private static Color32 GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle)
			{
				Color32 result;
				ParticleSystem.Particle.INTERNAL_CALL_GetCurrentColor(system, ref particle, out result);
				return result;
			}

			[WrapperlessIcall]
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

			public Collider collider
			{
				get
				{
					return null;
				}
			}
		}

		internal delegate bool IteratorDelegate(ParticleSystem ps);

		public extern float startDelay
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isPlaying
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isStopped
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPaused
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loop
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool playOnAwake
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float time
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float duration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float playbackSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int particleCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("enableEmission property is deprecated. Use emission.enabled instead.")]
		public extern bool enableEmission
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("emissionRate property is deprecated. Use emission.rate instead.")]
		public extern float emissionRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float startSpeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float startSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public extern float startRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public extern float startLifetime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float gravityModifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int maxParticles
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ParticleSystemSimulationSpace simulationSpace
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ParticleSystemScalingMode scalingMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern uint randomSeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useAutoRandomSeed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		[Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
		public int safeCollisionEventSize
		{
			get
			{
				return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startColor(out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startColor(ref Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_startRotation3D(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_startRotation3D(ref Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetParticles(ParticleSystem.Particle[] particles, int size);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetParticles(ParticleSystem.Particle[] particles);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Simulate(ParticleSystem self, float t, bool restart, bool fixedTimeStep);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Play(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Stop(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Pause(ParticleSystem self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Clear(ParticleSystem self);

		[WrapperlessIcall]
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
		public void Stop()
		{
			bool withChildren = true;
			this.Stop(withChildren);
		}

		public void Stop([DefaultValue("true")] bool withChildren)
		{
			this.IterateParticleSystems(withChildren, (ParticleSystem ps) => ParticleSystem.Internal_Stop(ps));
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

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_EmitOld(ref ParticleSystem.Particle particle);

		public void Emit(ParticleSystem.EmitParams emitParams, int count)
		{
			this.Internal_Emit(ref emitParams, count);
		}

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetupDefaultType(int type);

		internal Matrix4x4 GetLocalToWorldMatrix()
		{
			Matrix4x4 result;
			ParticleSystem.INTERNAL_CALL_GetLocalToWorldMatrix(this, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalToWorldMatrix(ParticleSystem self, out Matrix4x4 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool CountSubEmitterParticles(ref int count);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GenerateRandomSeed();
	}
}
