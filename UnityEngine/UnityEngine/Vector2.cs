using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct Vector2
	{
		public float x;

		public float y;

		public const float kEpsilon = 1E-05f;

		public float this[int index]
		{
			get
			{
				float result;
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					result = this.y;
				}
				else
				{
					result = this.x;
				}
				return result;
			}
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}

		public Vector2 normalized
		{
			get
			{
				Vector2 result = new Vector2(this.x, this.y);
				result.Normalize();
				return result;
			}
		}

		public float magnitude
		{
			get
			{
				return Mathf.Sqrt(this.x * this.x + this.y * this.y);
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public static Vector2 zero
		{
			get
			{
				return new Vector2(0f, 0f);
			}
		}

		public static Vector2 one
		{
			get
			{
				return new Vector2(1f, 1f);
			}
		}

		public static Vector2 up
		{
			get
			{
				return new Vector2(0f, 1f);
			}
		}

		public static Vector2 down
		{
			get
			{
				return new Vector2(0f, -1f);
			}
		}

		public static Vector2 left
		{
			get
			{
				return new Vector2(-1f, 0f);
			}
		}

		public static Vector2 right
		{
			get
			{
				return new Vector2(1f, 0f);
			}
		}

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public void Set(float newX, float newY)
		{
			this.x = newX;
			this.y = newY;
		}

		public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
		{
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}

		public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
		{
			Vector2 a = target - current;
			float magnitude = a.magnitude;
			Vector2 result;
			if (magnitude <= maxDistanceDelta || magnitude == 0f)
			{
				result = target;
			}
			else
			{
				result = current + a / magnitude * maxDistanceDelta;
			}
			return result;
		}

		public static Vector2 Scale(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public void Scale(Vector2 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		public void Normalize()
		{
			float magnitude = this.magnitude;
			if (magnitude > 1E-05f)
			{
				this /= magnitude;
			}
			else
			{
				this = Vector2.zero;
			}
		}

		public override string ToString()
		{
			return UnityString.Format("({0:F1}, {1:F1})", new object[]
			{
				this.x,
				this.y
			});
		}

		public string ToString(string format)
		{
			return UnityString.Format("({0}, {1})", new object[]
			{
				this.x.ToString(format),
				this.y.ToString(format)
			});
		}

		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is Vector2))
			{
				result = false;
			}
			else
			{
				Vector2 vector = (Vector2)other;
				result = (this.x.Equals(vector.x) && this.y.Equals(vector.y));
			}
			return result;
		}

		public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
		{
			return -2f * Vector2.Dot(inNormal, inDirection) * inNormal + inDirection;
		}

		public static float Dot(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		public static float Angle(Vector2 from, Vector2 to)
		{
			return Mathf.Acos(Mathf.Clamp(Vector2.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			return (a - b).magnitude;
		}

		public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
		{
			Vector2 result;
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				result = vector.normalized * maxLength;
			}
			else
			{
				result = vector;
			}
			return result;
		}

		public static float SqrMagnitude(Vector2 a)
		{
			return a.x * a.x + a.y * a.y;
		}

		public float SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}

		public static Vector2 Min(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
		}

		public static Vector2 Max(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
		}

		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			Vector2 vector = current - target;
			Vector2 vector2 = target;
			float maxLength = maxSpeed * smoothTime;
			vector = Vector2.ClampMagnitude(vector, maxLength);
			target = current - vector;
			Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3) * d;
			Vector2 vector4 = target + (vector + vector3) * d;
			if (Vector2.Dot(vector2 - current, vector4 - vector2) > 0f)
			{
				vector4 = vector2;
				currentVelocity = (vector4 - vector2) / deltaTime;
			}
			return vector4;
		}

		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator -(Vector2 a)
		{
			return new Vector2(-a.x, -a.y);
		}

		public static Vector2 operator *(Vector2 a, float d)
		{
			return new Vector2(a.x * d, a.y * d);
		}

		public static Vector2 operator *(float d, Vector2 a)
		{
			return new Vector2(a.x * d, a.y * d);
		}

		public static Vector2 operator /(Vector2 a, float d)
		{
			return new Vector2(a.x / d, a.y / d);
		}

		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return (lhs - rhs).sqrMagnitude < 9.99999944E-11f;
		}

		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return (lhs - rhs).sqrMagnitude >= 9.99999944E-11f;
		}

		public static implicit operator Vector2(Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}
	}
}
