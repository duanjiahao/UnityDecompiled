using System;
using System.Collections.Generic;

namespace UnityEngine.Assertions.Comparers
{
	public class FloatComparer : IEqualityComparer<float>
	{
		private readonly float m_Error;

		private readonly bool m_Relative;

		public static readonly FloatComparer s_ComparerWithDefaultTolerance = new FloatComparer(1E-05f);

		public const float kEpsilon = 1E-05f;

		public FloatComparer() : this(1E-05f, false)
		{
		}

		public FloatComparer(bool relative) : this(1E-05f, relative)
		{
		}

		public FloatComparer(float error) : this(error, false)
		{
		}

		public FloatComparer(float error, bool relative)
		{
			this.m_Error = error;
			this.m_Relative = relative;
		}

		public bool Equals(float a, float b)
		{
			return (!this.m_Relative) ? FloatComparer.AreEqual(a, b, this.m_Error) : FloatComparer.AreEqualRelative(a, b, this.m_Error);
		}

		public int GetHashCode(float obj)
		{
			return base.GetHashCode();
		}

		public static bool AreEqual(float expected, float actual, float error)
		{
			return Math.Abs(actual - expected) <= error;
		}

		public static bool AreEqualRelative(float expected, float actual, float error)
		{
			bool result;
			if (expected == actual)
			{
				result = true;
			}
			else
			{
				float num = Math.Abs(expected);
				float num2 = Math.Abs(actual);
				float num3 = Math.Abs((actual - expected) / ((num <= num2) ? num2 : num));
				result = (num3 <= error);
			}
			return result;
		}
	}
}
