using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class SerializedMinMaxCurve
	{
		public SerializedProperty scalar;

		public SerializedProperty maxCurve;

		public SerializedProperty minCurve;

		public SerializedProperty minCurveFirstKeyValue;

		public SerializedProperty maxCurveFirstKeyValue;

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
				return this.maxCurveFirstKeyValue.floatValue * this.scalar.floatValue;
			}
			set
			{
				value = this.ClampValueToMaxAllowed(value);
				if (!this.signedRange)
				{
					value = Mathf.Max(value, 0f);
				}
				float minConstant = this.minConstant;
				float num = Mathf.Abs(minConstant);
				float num2 = Mathf.Abs(value);
				float num3 = (num2 <= num) ? num : num2;
				if (num3 != this.scalar.floatValue)
				{
					this.SetScalarAndNormalizedConstants(num3, minConstant, value);
				}
				else
				{
					this.SetNormalizedConstant(this.maxCurve, value);
				}
			}
		}

		public float minConstant
		{
			get
			{
				return this.minCurveFirstKeyValue.floatValue * this.scalar.floatValue;
			}
			set
			{
				value = this.ClampValueToMaxAllowed(value);
				if (!this.signedRange)
				{
					value = Mathf.Max(value, 0f);
				}
				float maxConstant = this.maxConstant;
				float num = Mathf.Abs(value);
				float num2 = Mathf.Abs(maxConstant);
				float num3 = (num2 <= num) ? num : num2;
				if (num3 != this.scalar.floatValue)
				{
					this.SetScalarAndNormalizedConstants(num3, value, maxConstant);
				}
				else
				{
					this.SetNormalizedConstant(this.minCurve, value);
				}
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
			this.maxCurve = ((!useProp0) ? m.GetProperty(this.m_Name, "maxCurve") : m.GetProperty0(this.m_Name, "maxCurve"));
			this.maxCurveFirstKeyValue = this.maxCurve.FindPropertyRelative("m_Curve.Array.data[0].value");
			this.minCurve = ((!useProp0) ? m.GetProperty(this.m_Name, "minCurve") : m.GetProperty0(this.m_Name, "minCurve"));
			this.minCurveFirstKeyValue = this.minCurve.FindPropertyRelative("m_Curve.Array.data[0].value");
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

		public void SetScalarAndNormalizedConstants(float newScalar, float totalMin, float totalMax)
		{
			this.scalar.floatValue = newScalar;
			this.SetNormalizedConstant(this.minCurve, totalMin);
			this.SetNormalizedConstant(this.maxCurve, totalMax);
		}

		private void SetNormalizedConstant(SerializedProperty curve, float totalValue)
		{
			float num = this.scalar.floatValue;
			num = Mathf.Max(num, 0.0001f);
			float value = totalValue / num;
			this.SetCurveConstant(curve, value);
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

		public void SetMinMaxState(MinMaxCurveState newState, bool addToCurveEditor = true)
		{
			if (!this.stateHasMultipleDifferentValues)
			{
				if (newState == this.state)
				{
					return;
				}
			}
			MinMaxCurveState state = this.state;
			ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
			if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
			{
				particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
			}
			switch (newState)
			{
			case MinMaxCurveState.k_Scalar:
				this.InitSingleScalar(state);
				break;
			case MinMaxCurveState.k_Curve:
				this.InitSingleCurve(state);
				break;
			case MinMaxCurveState.k_TwoCurves:
				this.InitDoubleCurves(state);
				break;
			case MinMaxCurveState.k_TwoScalars:
				this.InitDoubleScalars(state);
				break;
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

		private void InitSingleScalar(MinMaxCurveState oldState)
		{
			if (oldState == MinMaxCurveState.k_Curve || oldState == MinMaxCurveState.k_TwoCurves || oldState == MinMaxCurveState.k_TwoScalars)
			{
				float maxKeyValue = this.GetMaxKeyValue(this.maxCurve.animationCurveValue.keys);
				this.scalar.floatValue *= maxKeyValue;
			}
			this.SetCurveConstant(this.maxCurve, 1f);
		}

		private void InitDoubleScalars(MinMaxCurveState oldState)
		{
			this.minConstant = this.GetAverageKeyValue(this.minCurve.animationCurveValue.keys) * this.scalar.floatValue;
			switch (oldState)
			{
			case MinMaxCurveState.k_Scalar:
				this.maxConstant = this.scalar.floatValue;
				break;
			case MinMaxCurveState.k_Curve:
			case MinMaxCurveState.k_TwoCurves:
				this.maxConstant = this.GetAverageKeyValue(this.maxCurve.animationCurveValue.keys) * this.scalar.floatValue;
				break;
			default:
				Debug.LogError("Enum not handled!");
				break;
			}
			this.SetCurveRequirements();
		}

		private void InitSingleCurve(MinMaxCurveState oldState)
		{
			if (oldState != MinMaxCurveState.k_Scalar)
			{
				if (oldState != MinMaxCurveState.k_TwoScalars && oldState != MinMaxCurveState.k_TwoCurves)
				{
				}
			}
			else
			{
				this.SetCurveConstant(this.maxCurve, this.GetNormalizedValueFromScalar());
			}
			this.SetCurveRequirements();
		}

		private void InitDoubleCurves(MinMaxCurveState oldState)
		{
			if (oldState != MinMaxCurveState.k_Scalar)
			{
				if (oldState != MinMaxCurveState.k_TwoScalars)
				{
					if (oldState != MinMaxCurveState.k_Curve)
					{
					}
				}
			}
			else
			{
				this.SetCurveConstant(this.maxCurve, this.GetNormalizedValueFromScalar());
			}
			this.SetCurveRequirements();
		}

		private float GetNormalizedValueFromScalar()
		{
			float result;
			if (this.scalar.floatValue < 0f)
			{
				result = -1f;
			}
			else if (this.scalar.floatValue > 0f)
			{
				result = 1f;
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		private void SetCurveRequirements()
		{
			this.scalar.floatValue = Mathf.Abs(this.scalar.floatValue);
			if (this.scalar.floatValue == 0f)
			{
				this.scalar.floatValue = this.m_DefaultCurveScalar;
			}
		}

		private void SetCurveConstant(SerializedProperty curve, float value)
		{
			curve.animationCurveValue = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, value)
			});
		}

		private float GetAverageKeyValue(Keyframe[] keyFrames)
		{
			float num = 0f;
			for (int i = 0; i < keyFrames.Length; i++)
			{
				Keyframe keyframe = keyFrames[i];
				num += keyframe.value;
			}
			return num / (float)keyFrames.Length;
		}

		private float GetMaxKeyValue(Keyframe[] keyFrames)
		{
			float num = float.NegativeInfinity;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < keyFrames.Length; i++)
			{
				Keyframe keyframe = keyFrames[i];
				if (keyframe.value > num)
				{
					num = keyframe.value;
				}
				if (keyframe.value < num2)
				{
					num2 = keyframe.value;
				}
			}
			float result;
			if (Mathf.Abs(num2) > num)
			{
				result = num2;
			}
			else
			{
				result = num;
			}
			return result;
		}

		private bool IsCurveConstant(Keyframe[] keyFrames, out float constantValue)
		{
			bool result;
			if (keyFrames.Length == 0)
			{
				constantValue = 0f;
				result = false;
			}
			else
			{
				constantValue = keyFrames[0].value;
				for (int i = 1; i < keyFrames.Length; i++)
				{
					if (Mathf.Abs(constantValue - keyFrames[i].value) > 1E-05f)
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		public string GetUniqueCurveName()
		{
			return SerializedModule.Concat(this.m_Module.GetUniqueModuleName(this.m_Module.serializedObject.targetObject), this.m_Name);
		}

		public bool SupportsProcedural()
		{
			bool flag = AnimationUtility.CurveSupportsProcedural(this.maxCurve.animationCurveValue);
			bool result;
			if (this.state != MinMaxCurveState.k_TwoCurves && this.state != MinMaxCurveState.k_TwoScalars)
			{
				result = flag;
			}
			else
			{
				result = (flag && AnimationUtility.CurveSupportsProcedural(this.minCurve.animationCurveValue));
			}
			return result;
		}
	}
}
