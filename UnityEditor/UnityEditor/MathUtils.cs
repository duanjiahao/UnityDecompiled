using System;
using UnityEngine;

namespace UnityEditor
{
	public class MathUtils
	{
		private const int kMaxDecimals = 15;

		internal static float ClampToFloat(double value)
		{
			if (double.IsPositiveInfinity(value))
			{
				return float.PositiveInfinity;
			}
			if (double.IsNegativeInfinity(value))
			{
				return float.NegativeInfinity;
			}
			if (value < -3.4028234663852886E+38)
			{
				return -3.40282347E+38f;
			}
			if (value > 3.4028234663852886E+38)
			{
				return 3.40282347E+38f;
			}
			return (float)value;
		}

		internal static int ClampToInt(long value)
		{
			if (value < -2147483648L)
			{
				return -2147483648;
			}
			if (value > 2147483647L)
			{
				return 2147483647;
			}
			return (int)value;
		}

		internal static float RoundToMultipleOf(float value, float roundingValue)
		{
			if (roundingValue == 0f)
			{
				return value;
			}
			return Mathf.Round(value / roundingValue) * roundingValue;
		}

		internal static float GetClosestPowerOfTen(float positiveNumber)
		{
			if (positiveNumber <= 0f)
			{
				return 1f;
			}
			return Mathf.Pow(10f, (float)Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
		}

		internal static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
		{
			return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, 15);
		}

		internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
		{
			return (int)Math.Max(0.0, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
		}

		internal static float RoundBasedOnMinimumDifference(float valueToRound, float minDifference)
		{
			if (minDifference == 0f)
			{
				return MathUtils.DiscardLeastSignificantDecimal(valueToRound);
			}
			return (float)Math.Round((double)valueToRound, MathUtils.GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
		}

		internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
		{
			if (minDifference == 0.0)
			{
				return MathUtils.DiscardLeastSignificantDecimal(valueToRound);
			}
			return Math.Round(valueToRound, MathUtils.GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
		}

		internal static float DiscardLeastSignificantDecimal(float v)
		{
			int digits = Mathf.Clamp((int)(5f - Mathf.Log10(Mathf.Abs(v))), 0, 15);
			return (float)Math.Round((double)v, digits, MidpointRounding.AwayFromZero);
		}

		internal static double DiscardLeastSignificantDecimal(double v)
		{
			int digits = Math.Max(0, (int)(5.0 - Math.Log10(Math.Abs(v))));
			double result;
			try
			{
				result = Math.Round(v, digits);
			}
			catch (ArgumentOutOfRangeException)
			{
				result = 0.0;
			}
			return result;
		}

		public static float GetQuatLength(Quaternion q)
		{
			return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}

		public static Quaternion GetQuatConjugate(Quaternion q)
		{
			return new Quaternion(-q.x, -q.y, -q.z, q.w);
		}

		public static Matrix4x4 OrthogonalizeMatrix(Matrix4x4 m)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			Vector3 vector = m.GetColumn(0);
			Vector3 vector2 = m.GetColumn(1);
			Vector3 normalized = m.GetColumn(2).normalized;
			vector = Vector3.Cross(vector2, normalized).normalized;
			vector2 = Vector3.Cross(normalized, vector).normalized;
			identity.SetColumn(0, vector);
			identity.SetColumn(1, vector2);
			identity.SetColumn(2, normalized);
			return identity;
		}

		public static void QuaternionNormalize(ref Quaternion q)
		{
			float num = 1f / Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
			q.x *= num;
			q.y *= num;
			q.z *= num;
			q.w *= num;
		}

		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			Quaternion result = default(Quaternion);
			result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
			result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
			result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
			result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
			result.x *= Mathf.Sign(result.x * (m[2, 1] - m[1, 2]));
			result.y *= Mathf.Sign(result.y * (m[0, 2] - m[2, 0]));
			result.z *= Mathf.Sign(result.z * (m[1, 0] - m[0, 1]));
			MathUtils.QuaternionNormalize(ref result);
			return result;
		}

		public static Quaternion GetQuatLog(Quaternion q)
		{
			Quaternion result = q;
			result.w = 0f;
			if (Mathf.Abs(q.w) < 1f)
			{
				float num = Mathf.Acos(q.w);
				float num2 = Mathf.Sin(num);
				if ((double)Mathf.Abs(num2) > 0.0001)
				{
					float num3 = num / num2;
					result.x = q.x * num3;
					result.y = q.y * num3;
					result.z = q.z * num3;
				}
			}
			return result;
		}

		public static Quaternion GetQuatExp(Quaternion q)
		{
			Quaternion result = q;
			float num = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
			float num2 = Mathf.Sin(num);
			result.w = Mathf.Cos(num);
			if ((double)Mathf.Abs(num2) > 0.0001)
			{
				float num3 = num2 / num;
				result.x = num3 * q.x;
				result.y = num3 * q.y;
				result.z = num3 * q.z;
			}
			return result;
		}

		public static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
		{
			float t2 = 2f * t * (1f - t);
			Quaternion p = MathUtils.Slerp(q0, q1, t);
			Quaternion q2 = MathUtils.Slerp(a0, a1, t);
			Quaternion result = MathUtils.Slerp(p, q2, t2);
			float num = Mathf.Sqrt(result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w);
			result.x /= num;
			result.y /= num;
			result.z /= num;
			result.w /= num;
			return result;
		}

		public static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
		{
			Quaternion quatConjugate = MathUtils.GetQuatConjugate(q1);
			Quaternion quatLog = MathUtils.GetQuatLog(quatConjugate * q0);
			Quaternion quatLog2 = MathUtils.GetQuatLog(quatConjugate * q2);
			Quaternion q3 = new Quaternion(-0.25f * (quatLog.x + quatLog2.x), -0.25f * (quatLog.y + quatLog2.y), -0.25f * (quatLog.z + quatLog2.z), -0.25f * (quatLog.w + quatLog2.w));
			return q1 * MathUtils.GetQuatExp(q3);
		}

		public static float Ease(float t, float k1, float k2)
		{
			float num = k1 * 2f / 3.14159274f + k2 - k1 + (1f - k2) * 2f / 3.14159274f;
			float num2;
			if (t < k1)
			{
				num2 = k1 * 0.636619747f * (Mathf.Sin(t / k1 * 3.14159274f / 2f - 1.57079637f) + 1f);
			}
			else if (t < k2)
			{
				num2 = 2f * k1 / 3.14159274f + t - k1;
			}
			else
			{
				num2 = 2f * k1 / 3.14159274f + k2 - k1 + (1f - k2) * 0.636619747f * Mathf.Sin((t - k2) / (1f - k2) * 3.14159274f / 2f);
			}
			return num2 / num;
		}

		public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
		{
			float num = Quaternion.Dot(p, q);
			Quaternion result;
			if ((double)(1f + num) > 1E-05)
			{
				float num4;
				float num5;
				if ((double)(1f - num) > 1E-05)
				{
					float num2 = Mathf.Acos(num);
					float num3 = 1f / Mathf.Sin(num2);
					num4 = Mathf.Sin((1f - t) * num2) * num3;
					num5 = Mathf.Sin(t * num2) * num3;
				}
				else
				{
					num4 = 1f - t;
					num5 = t;
				}
				result.x = num4 * p.x + num5 * q.x;
				result.y = num4 * p.y + num5 * q.y;
				result.z = num4 * p.z + num5 * q.z;
				result.w = num4 * p.w + num5 * q.w;
			}
			else
			{
				float num6 = Mathf.Sin((1f - t) * 3.14159274f * 0.5f);
				float num7 = Mathf.Sin(t * 3.14159274f * 0.5f);
				result.x = num6 * p.x - num7 * p.y;
				result.y = num6 * p.y + num7 * p.x;
				result.z = num6 * p.z - num7 * p.w;
				result.w = p.z;
			}
			return result;
		}

		public static object IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, bool bidirectional)
		{
			Vector3 lhs = v1 - v0;
			Vector3 vector = v2 - v0;
			Vector3 vector2 = Vector3.Cross(lhs, vector);
			float num = Vector3.Dot(-ray.direction, vector2);
			if (num <= 0f)
			{
				return null;
			}
			Vector3 vector3 = ray.origin - v0;
			float num2 = Vector3.Dot(vector3, vector2);
			if (num2 < 0f && !bidirectional)
			{
				return null;
			}
			Vector3 rhs = Vector3.Cross(-ray.direction, vector3);
			float num3 = Vector3.Dot(vector, rhs);
			if (num3 < 0f || num3 > num)
			{
				return null;
			}
			float num4 = -Vector3.Dot(lhs, rhs);
			if (num4 < 0f || num3 + num4 > num)
			{
				return null;
			}
			float num5 = 1f / num;
			num2 *= num5;
			num3 *= num5;
			num4 *= num5;
			float x = 1f - num3 - num4;
			return new RaycastHit
			{
				point = ray.origin + num2 * ray.direction,
				distance = num2,
				barycentricCoordinate = new Vector3(x, num3, num4),
				normal = Vector3.Normalize(vector2)
			};
		}

		public static Vector3 ClosestPtSegmentRay(Vector3 p1, Vector3 q1, Ray ray, out float squaredDist, out float s, out Vector3 closestRay)
		{
			Vector3 origin = ray.origin;
			Vector3 point = ray.GetPoint(10000f);
			Vector3 vector = q1 - p1;
			Vector3 vector2 = point - origin;
			Vector3 rhs = p1 - origin;
			float num = Vector3.Dot(vector, vector);
			float num2 = Vector3.Dot(vector2, vector2);
			float num3 = Vector3.Dot(vector2, rhs);
			if (num <= Mathf.Epsilon && num2 <= Mathf.Epsilon)
			{
				squaredDist = Vector3.Dot(p1 - origin, p1 - origin);
				s = 0f;
				closestRay = origin;
				return p1;
			}
			float num4;
			if (num <= Mathf.Epsilon)
			{
				s = 0f;
				num4 = num3 / num2;
				num4 = Mathf.Clamp(num4, 0f, 1f);
			}
			else
			{
				float num5 = Vector3.Dot(vector, rhs);
				if (num2 <= Mathf.Epsilon)
				{
					num4 = 0f;
					s = Mathf.Clamp(-num5 / num, 0f, 1f);
				}
				else
				{
					float num6 = Vector3.Dot(vector, vector2);
					float num7 = num * num2 - num6 * num6;
					if (num7 != 0f)
					{
						s = Mathf.Clamp((num6 * num3 - num5 * num2) / num7, 0f, 1f);
					}
					else
					{
						s = 0f;
					}
					num4 = (num6 * s + num3) / num2;
					if (num4 < 0f)
					{
						num4 = 0f;
						s = Mathf.Clamp(-num5 / num, 0f, 1f);
					}
					else if (num4 > 1f)
					{
						num4 = 1f;
						s = Mathf.Clamp((num6 - num5) / num, 0f, 1f);
					}
				}
			}
			Vector3 vector3 = p1 + vector * s;
			Vector3 vector4 = origin + vector2 * num4;
			squaredDist = Vector3.Dot(vector3 - vector4, vector3 - vector4);
			closestRay = vector4;
			return vector3;
		}

		public static bool IntersectRaySphere(Ray ray, Vector3 sphereOrigin, float sphereRadius, ref float t, ref Vector3 q)
		{
			Vector3 vector = ray.origin - sphereOrigin;
			float num = Vector3.Dot(vector, ray.direction);
			float num2 = Vector3.Dot(vector, vector) - sphereRadius * sphereRadius;
			if (num2 > 0f && num > 0f)
			{
				return false;
			}
			float num3 = num * num - num2;
			if (num3 < 0f)
			{
				return false;
			}
			t = -num - Mathf.Sqrt(num3);
			if (t < 0f)
			{
				t = 0f;
			}
			q = ray.origin + t * ray.direction;
			return true;
		}

		public static bool ClosestPtRaySphere(Ray ray, Vector3 sphereOrigin, float sphereRadius, ref float t, ref Vector3 q)
		{
			Vector3 vector = ray.origin - sphereOrigin;
			float num = Vector3.Dot(vector, ray.direction);
			float num2 = Vector3.Dot(vector, vector) - sphereRadius * sphereRadius;
			if (num2 > 0f && num > 0f)
			{
				t = 0f;
				q = ray.origin;
				return true;
			}
			float num3 = num * num - num2;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			t = -num - Mathf.Sqrt(num3);
			if (t < 0f)
			{
				t = 0f;
			}
			q = ray.origin + t * ray.direction;
			return true;
		}
	}
}
