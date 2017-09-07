using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class SerializedMinMaxCurve
	{
		public SerializedProperty scalar;

		public SerializedProperty minScalar;

		public SerializedProperty maxCurve;

		public SerializedProperty minCurve;

		private SerializedProperty minMaxState;

		public ModuleUI m_Module;

		private string m_Name;

		public GUIContent m_DisplayName;

		private bool m_SignedRange;

		public float m_DefaultCurveScalar;

		public float m_RemapValue;

		public bool m_AllowConstant;

		public bool m_AllowRandom;

		public bool m_AllowCurves;

		public float m_MaxAllowedScalar = float.PositiveInfinity;

		public MinMaxCurveState state
		{
			get
			{
				return (MinMaxCurveState)this.minMaxState.intValue;
			}
			set
			{
				this.SetMinMaxState(value, true);
			}
		}

		public bool stateHasMultipleDifferentValues
		{
			get
			{
				return this.minMaxState.hasMultipleDifferentValues;
			}
		}

		public bool signedRange
		{
			get
			{
				return this.m_SignedRange;
			}
		}

		public float maxConstant
		{
			get
			{
				return this.scalar.floatValue;
			}
			set
			{
				value = this.ClampValueToMaxAllowed(value);
				if (!this.signedRange)
				{
					value = Mathf.Max(value, 0f);
				}
				this.scalar.floatValue = value;
			}
		}

		public float minConstant
		{
			get
			{
				return this.minScalar.floatValue;
			}
			set
			{
				value = this.ClampValueToMaxAllowed(value);
				if (!this.signedRange)
				{
					value = Mathf.Max(value, 0f);
				}
				this.minScalar.floatValue = value;
			}
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName)
		{
			this.Init(m, displayName, "curve", false, false, true);
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name)
		{
			this.Init(m, displayName, name, false, false, true);
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, bool signedRange)
		{
			this.Init(m, displayName, "curve", signedRange, false, true);
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name, bool signedRange)
		{
			this.Init(m, displayName, name, signedRange, false, true);
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name, bool signedRange, bool useProp0)
		{
			this.Init(m, displayName, name, signedRange, useProp0, true);
		}

		public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name, bool signedRange, bool useProp0, bool addCurveIfNeeded)
		{
			this.Init(m, displayName, name, signedRange, useProp0, addCurveIfNeeded);
		}

		private void Init(ModuleUI m, GUIContent displayName, string uniqueName, bool signedRange, bool useProp0, bool addCurveIfNeeded)
		{
			this.m_Module = m;
			this.m_DisplayName = displayName;
			this.m_Name = uniqueName;
			this.m_SignedRange = signedRange;
			this.m_RemapValue = 1f;
			this.m_DefaultCurveScalar = 1f;
			this.m_AllowConstant = true;
			this.m_AllowRandom = true;
			this.m_AllowCurves = true;
			this.scalar = ((!useProp0) ? m.GetProperty(this.m_Name, "scalar") : m.GetProperty0(this.m_Name, "scalar"));
			this.minScalar = ((!useProp0) ? m.GetProperty(this.m_Name, "minScalar") : m.GetProperty0(this.m_Name, "minScalar"));
			this.maxCurve = ((!useProp0) ? m.GetProperty(this.m_Name, "maxCurve") : m.GetProperty0(this.m_Name, "maxCurve"));
			this.minCurve = ((!useProp0) ? m.GetProperty(this.m_Name, "minCurve") : m.GetProperty0(this.m_Name, "minCurve"));
			this.minMaxState = ((!useProp0) ? m.GetProperty(this.m_Name, "minMaxState") : m.GetProperty0(this.m_Name, "minMaxState"));
			if (addCurveIfNeeded)
			{
				if (this.state == MinMaxCurveState.k_Curve || this.state == MinMaxCurveState.k_TwoCurves)
				{
					if (this.m_Module.m_ParticleSystemUI.m_ParticleEffectUI.IsParticleSystemUIVisible(this.m_Module.m_ParticleSystemUI))
					{
						m.GetParticleSystemCurveEditor().AddCurveDataIfNeeded(this.GetUniqueCurveName(), this.CreateCurveData(Color.black));
					}
				}
			}
			m.AddToModuleCurves(this.maxCurve);
		}

		private float ClampValueToMaxAllowed(float val)
		{
			float result;
			if (Mathf.Abs(val) > this.m_MaxAllowedScalar)
			{
				result = this.m_MaxAllowedScalar * Mathf.Sign(val);
			}
			else
			{
				result = val;
			}
			return result;
		}

		public Vector2 GetAxisScalars()
		{
			return new Vector2(this.m_Module.GetXAxisScalar(), this.scalar.floatValue * this.m_RemapValue);
		}

		public void SetAxisScalars(Vector2 axisScalars)
		{
			float num = (this.m_RemapValue != 0f) ? this.m_RemapValue : 1f;
			this.scalar.floatValue = axisScalars.y / num;
		}

		public void RemoveCurveFromEditor()
		{
			ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
			if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
			{
				particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
			}
		}

		public bool OnCurveAreaMouseDown(int button, Rect drawRect, Rect curveRanges)
		{
			bool result;
			if (button == 0)
			{
				this.ToggleCurveInEditor();
				result = true;
			}
			else if (button == 1)
			{
				SerializedProperty serializedProperty = this.GetMinCurve();
				AnimationCurveContextMenu.Show(drawRect, (this.maxCurve == null) ? null : this.maxCurve.Copy(), (serializedProperty == null) ? null : serializedProperty.Copy(), (this.scalar == null) ? null : this.scalar.Copy(), curveRanges, this.m_Module.GetParticleSystemCurveEditor());
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public ParticleSystemCurveEditor.CurveData CreateCurveData(Color color)
		{
			return new ParticleSystemCurveEditor.CurveData(this.GetUniqueCurveName(), this.m_DisplayName, this.GetMinCurve(), this.maxCurve, color, this.m_SignedRange, new CurveWrapper.GetAxisScalarsCallback(this.GetAxisScalars), new CurveWrapper.SetAxisScalarsCallback(this.SetAxisScalars), this.m_Module.foldout);
		}

		private SerializedProperty GetMinCurve()
		{
			return (this.state != MinMaxCurveState.k_TwoCurves) ? null : this.minCurve;
		}

		public void ToggleCurveInEditor()
		{
			ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
			if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
			{
				particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
			}
			else
			{
				particleSystemCurveEditor.AddCurve(this.CreateCurveData(particleSystemCurveEditor.GetAvailableColor()));
			}
		}

		public void SetMinMaxState(MinMaxCurveState newState, bool addToCurveEditor)
		{
			if (this.stateHasMultipleDifferentValues)
			{
				Debug.LogError("SetMinMaxState is not allowed with multiple different values");
			}
			else if (newState != this.state)
			{
				MinMaxCurveState state = this.state;
				ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
				if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
				{
					particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
				}
				if (newState != MinMaxCurveState.k_Curve)
				{
					if (newState == MinMaxCurveState.k_TwoCurves)
					{
						this.SetCurveRequirements();
					}
				}
				else
				{
					this.SetCurveRequirements();
				}
				this.minMaxState.intValue = (int)newState;
				if (addToCurveEditor)
				{
					switch (newState)
					{
					case MinMaxCurveState.k_Scalar:
					case MinMaxCurveState.k_TwoScalars:
						break;
					case MinMaxCurveState.k_Curve:
					case MinMaxCurveState.k_TwoCurves:
						particleSystemCurveEditor.AddCurve(this.CreateCurveData(particleSystemCurveEditor.GetAvailableColor()));
						break;
					default:
						Debug.LogError("Unhandled enum value");
						break;
					}
				}
				AnimationCurvePreviewCache.ClearCache();
			}
		}

		private void SetCurveRequirements()
		{
			this.scalar.floatValue = Mathf.Abs(this.scalar.floatValue);
			if (this.scalar.floatValue == 0f)
			{
				this.scalar.floatValue = this.m_DefaultCurveScalar;
			}
		}

		public string GetUniqueCurveName()
		{
			return SerializedModule.Concat(this.m_Module.GetUniqueModuleName(this.m_Module.serializedObject.targetObject), this.m_Name);
		}

		private static bool AnimationCurveSupportsProcedural(AnimationCurve curve, ref string failureReason)
		{
			bool result;
			switch (AnimationUtility.IsValidPolynomialCurve(curve))
			{
			case AnimationUtility.PolynomialValid.Valid:
				result = true;
				return result;
			case AnimationUtility.PolynomialValid.InvalidPreWrapMode:
				failureReason = "Unsupported curve pre-wrap mode. Loop and ping-pong do not support procedural mode.";
				break;
			case AnimationUtility.PolynomialValid.InvalidPostWrapMode:
				failureReason = "Unsupported curve post-wrap mode. Loop and ping-pong do not support procedural mode.";
				break;
			case AnimationUtility.PolynomialValid.TooManySegments:
				failureReason = "Curve uses too many keys. Procedural mode does not support more than " + AnimationUtility.GetMaxNumPolynomialSegmentsSupported() + " keys";
				if (curve.keys[0].time != 0f || curve.keys[curve.keys.Length - 1].time != 1f)
				{
					failureReason += " (Additional keys are added to curves that do not start at 0, or do not end at 1)";
				}
				failureReason += ".";
				break;
			}
			result = false;
			return result;
		}

		public bool SupportsProcedural(ref string failureReason)
		{
			string text = "Max Curve: ";
			bool flag = SerializedMinMaxCurve.AnimationCurveSupportsProcedural(this.maxCurve.animationCurveValue, ref text);
			if (!flag)
			{
				failureReason = text;
			}
			bool result;
			if (this.state != MinMaxCurveState.k_TwoCurves && this.state != MinMaxCurveState.k_TwoScalars)
			{
				result = flag;
			}
			else
			{
				string str = "Min Curve: ";
				bool flag2 = SerializedMinMaxCurve.AnimationCurveSupportsProcedural(this.minCurve.animationCurveValue, ref str);
				if (flag2)
				{
					failureReason += str;
				}
				result = (flag && flag2);
			}
			return result;
		}
	}
}
