using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class SerializedMinMaxGradient
	{
		public SerializedProperty m_MaxGradient;

		public SerializedProperty m_MinGradient;

		public SerializedProperty m_MaxColor;

		public SerializedProperty m_MinColor;

		private SerializedProperty m_MinMaxState;

		public bool m_AllowColor;

		public bool m_AllowGradient;

		public bool m_AllowRandomBetweenTwoColors;

		public bool m_AllowRandomBetweenTwoGradients;

		public bool m_AllowRandomColor;

		public MinMaxGradientState state
		{
			get
			{
				return (MinMaxGradientState)this.m_MinMaxState.intValue;
			}
			set
			{
				this.SetMinMaxState(value);
			}
		}

		public bool stateHasMultipleDifferentValues
		{
			get
			{
				return this.m_MinMaxState.hasMultipleDifferentValues;
			}
		}

		public SerializedMinMaxGradient(SerializedModule m)
		{
			this.Init(m, "gradient");
		}

		public SerializedMinMaxGradient(SerializedModule m, string name)
		{
			this.Init(m, name);
		}

		private void Init(SerializedModule m, string name)
		{
			this.m_MaxGradient = m.GetProperty(name, "maxGradient");
			this.m_MinGradient = m.GetProperty(name, "minGradient");
			this.m_MaxColor = m.GetProperty(name, "maxColor");
			this.m_MinColor = m.GetProperty(name, "minColor");
			this.m_MinMaxState = m.GetProperty(name, "minMaxState");
			this.m_AllowColor = true;
			this.m_AllowGradient = true;
			this.m_AllowRandomBetweenTwoColors = true;
			this.m_AllowRandomBetweenTwoGradients = true;
			this.m_AllowRandomColor = false;
		}

		private void SetMinMaxState(MinMaxGradientState newState)
		{
			if (newState != this.state)
			{
				this.m_MinMaxState.intValue = (int)newState;
			}
		}

		public static Color GetGradientAsColor(SerializedProperty gradientProp)
		{
			Gradient gradientValue = gradientProp.gradientValue;
			return gradientValue.constantColor;
		}

		public static void SetGradientAsColor(SerializedProperty gradientProp, Color color)
		{
			Gradient gradientValue = gradientProp.gradientValue;
			gradientValue.constantColor = color;
			GradientPreviewCache.ClearCache();
		}
	}
}
