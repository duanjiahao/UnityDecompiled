using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct Matrix4x4
	{
		public float m00;

		public float m10;

		public float m20;

		public float m30;

		public float m01;

		public float m11;

		public float m21;

		public float m31;

		public float m02;

		public float m12;

		public float m22;

		public float m32;

		public float m03;

		public float m13;

		public float m23;

		public float m33;

		public Matrix4x4 inverse
		{
			get
			{
				return Matrix4x4.Inverse(this);
			}
		}

		public Matrix4x4 transpose
		{
			get
			{
				return Matrix4x4.Transpose(this);
			}
		}

		public extern bool isIdentity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public float determinant
		{
			get
			{
				return Matrix4x4.Determinant(this);
			}
		}

		public float this[int row, int column]
		{
			get
			{
				return this[row + column * 4];
			}
			set
			{
				this[row + column * 4] = value;
			}
		}

		public float this[int index]
		{
			get
			{
				float result;
				switch (index)
				{
				case 0:
					result = this.m00;
					break;
				case 1:
					result = this.m10;
					break;
				case 2:
					result = this.m20;
					break;
				case 3:
					result = this.m30;
					break;
				case 4:
					result = this.m01;
					break;
				case 5:
					result = this.m11;
					break;
				case 6:
					result = this.m21;
					break;
				case 7:
					result = this.m31;
					break;
				case 8:
					result = this.m02;
					break;
				case 9:
					result = this.m12;
					break;
				case 10:
					result = this.m22;
					break;
				case 11:
					result = this.m32;
					break;
				case 12:
					result = this.m03;
					break;
				case 13:
					result = this.m13;
					break;
				case 14:
					result = this.m23;
					break;
				case 15:
					result = this.m33;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
				return result;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.m00 = value;
					break;
				case 1:
					this.m10 = value;
					break;
				case 2:
					this.m20 = value;
					break;
				case 3:
					this.m30 = value;
					break;
				case 4:
					this.m01 = value;
					break;
				case 5:
					this.m11 = value;
					break;
				case 6:
					this.m21 = value;
					break;
				case 7:
					this.m31 = value;
					break;
				case 8:
					this.m02 = value;
					break;
				case 9:
					this.m12 = value;
					break;
				case 10:
					this.m22 = value;
					break;
				case 11:
					this.m32 = value;
					break;
				case 12:
					this.m03 = value;
					break;
				case 13:
					this.m13 = value;
					break;
				case 14:
					this.m23 = value;
					break;
				case 15:
					this.m33 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public static Matrix4x4 zero
		{
			get
			{
				return new Matrix4x4
				{
					m00 = 0f,
					m01 = 0f,
					m02 = 0f,
					m03 = 0f,
					m10 = 0f,
					m11 = 0f,
					m12 = 0f,
					m13 = 0f,
					m20 = 0f,
					m21 = 0f,
					m22 = 0f,
					m23 = 0f,
					m30 = 0f,
					m31 = 0f,
					m32 = 0f,
					m33 = 0f
				};
			}
		}

		public static Matrix4x4 identity
		{
			get
			{
				return new Matrix4x4
				{
					m00 = 1f,
					m01 = 0f,
					m02 = 0f,
					m03 = 0f,
					m10 = 0f,
					m11 = 1f,
					m12 = 0f,
					m13 = 0f,
					m20 = 0f,
					m21 = 0f,
					m22 = 1f,
					m23 = 0f,
					m30 = 0f,
					m31 = 0f,
					m32 = 0f,
					m33 = 1f
				};
			}
		}

		[ThreadAndSerializationSafe]
		public static Matrix4x4 Inverse(Matrix4x4 m)
		{
			Matrix4x4 result;
			Matrix4x4.INTERNAL_CALL_Inverse(ref m, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Inverse(ref Matrix4x4 m, out Matrix4x4 value);

		public static Matrix4x4 Transpose(Matrix4x4 m)
		{
			Matrix4x4 result;
			Matrix4x4.INTERNAL_CALL_Transpose(ref m, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Transpose(ref Matrix4x4 m, out Matrix4x4 value);

		internal static bool Invert(Matrix4x4 inMatrix, out Matrix4x4 dest)
		{
			return Matrix4x4.INTERNAL_CALL_Invert(ref inMatrix, out dest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Invert(ref Matrix4x4 inMatrix, out Matrix4x4 dest);

		public static float Determinant(Matrix4x4 m)
		{
			return Matrix4x4.INTERNAL_CALL_Determinant(ref m);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_Determinant(ref Matrix4x4 m);

		public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
		{
			this = Matrix4x4.TRS(pos, q, s);
		}

		public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
		{
			Matrix4x4 result;
			Matrix4x4.INTERNAL_CALL_TRS(ref pos, ref q, ref s, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_TRS(ref Vector3 pos, ref Quaternion q, ref Vector3 s, out Matrix4x4 value);

		public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float zNear, float zFar)
		{
			Matrix4x4 result;
			Matrix4x4.INTERNAL_CALL_Ortho(left, right, bottom, top, zNear, zFar, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Ortho(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4x4 value);

		public static Matrix4x4 Perspective(float fov, float aspect, float zNear, float zFar)
		{
			Matrix4x4 result;
			Matrix4x4.INTERNAL_CALL_Perspective(fov, aspect, zNear, zFar, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Perspective(float fov, float aspect, float zNear, float zFar, out Matrix4x4 value);

		public override int GetHashCode()
		{
			return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^ this.GetColumn(2).GetHashCode() >> 2 ^ this.GetColumn(3).GetHashCode() >> 1;
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is Matrix4x4))
			{
				result = false;
			}
			else
			{
				Matrix4x4 matrix4x = (Matrix4x4)other;
				result = (this.GetColumn(0).Equals(matrix4x.GetColumn(0)) && this.GetColumn(1).Equals(matrix4x.GetColumn(1)) && this.GetColumn(2).Equals(matrix4x.GetColumn(2)) && this.GetColumn(3).Equals(matrix4x.GetColumn(3)));
			}
			return result;
		}

		public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4
			{
				m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
				m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31,
				m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32,
				m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33,
				m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30,
				m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31,
				m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,
				m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33,
				m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30,
				m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31,
				m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,
				m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33,
				m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30,
				m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31,
				m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,
				m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33
			};
		}

		public static Vector4 operator *(Matrix4x4 lhs, Vector4 v)
		{
			Vector4 result;
			result.x = lhs.m00 * v.x + lhs.m01 * v.y + lhs.m02 * v.z + lhs.m03 * v.w;
			result.y = lhs.m10 * v.x + lhs.m11 * v.y + lhs.m12 * v.z + lhs.m13 * v.w;
			result.z = lhs.m20 * v.x + lhs.m21 * v.y + lhs.m22 * v.z + lhs.m23 * v.w;
			result.w = lhs.m30 * v.x + lhs.m31 * v.y + lhs.m32 * v.z + lhs.m33 * v.w;
			return result;
		}

		public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
		}

		public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return !(lhs == rhs);
		}

		public Vector4 GetColumn(int i)
		{
			return new Vector4(this[0, i], this[1, i], this[2, i], this[3, i]);
		}

		public Vector4 GetRow(int i)
		{
			return new Vector4(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
		}

		public void SetColumn(int i, Vector4 v)
		{
			this[0, i] = v.x;
			this[1, i] = v.y;
			this[2, i] = v.z;
			this[3, i] = v.w;
		}

		public void SetRow(int i, Vector4 v)
		{
			this[i, 0] = v.x;
			this[i, 1] = v.y;
			this[i, 2] = v.z;
			this[i, 3] = v.w;
		}

		public Vector3 MultiplyPoint(Vector3 v)
		{
			Vector3 result;
			result.x = this.m00 * v.x + this.m01 * v.y + this.m02 * v.z + this.m03;
			result.y = this.m10 * v.x + this.m11 * v.y + this.m12 * v.z + this.m13;
			result.z = this.m20 * v.x + this.m21 * v.y + this.m22 * v.z + this.m23;
			float num = this.m30 * v.x + this.m31 * v.y + this.m32 * v.z + this.m33;
			num = 1f / num;
			result.x *= num;
			result.y *= num;
			result.z *= num;
			return result;
		}

		public Vector3 MultiplyPoint3x4(Vector3 v)
		{
			Vector3 result;
			result.x = this.m00 * v.x + this.m01 * v.y + this.m02 * v.z + this.m03;
			result.y = this.m10 * v.x + this.m11 * v.y + this.m12 * v.z + this.m13;
			result.z = this.m20 * v.x + this.m21 * v.y + this.m22 * v.z + this.m23;
			return result;
		}

		public Vector3 MultiplyVector(Vector3 v)
		{
			Vector3 result;
			result.x = this.m00 * v.x + this.m01 * v.y + this.m02 * v.z;
			result.y = this.m10 * v.x + this.m11 * v.y + this.m12 * v.z;
			result.z = this.m20 * v.x + this.m21 * v.y + this.m22 * v.z;
			return result;
		}

		public static Matrix4x4 Scale(Vector3 v)
		{
			return new Matrix4x4
			{
				m00 = v.x,
				m01 = 0f,
				m02 = 0f,
				m03 = 0f,
				m10 = 0f,
				m11 = v.y,
				m12 = 0f,
				m13 = 0f,
				m20 = 0f,
				m21 = 0f,
				m22 = v.z,
				m23 = 0f,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			};
		}

		public override string ToString()
		{
			return UnityString.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", new object[]
			{
				this.m00,
				this.m01,
				this.m02,
				this.m03,
				this.m10,
				this.m11,
				this.m12,
				this.m13,
				this.m20,
				this.m21,
				this.m22,
				this.m23,
				this.m30,
				this.m31,
				this.m32,
				this.m33
			});
		}

		public string ToString(string format)
		{
			return UnityString.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n", new object[]
			{
				this.m00.ToString(format),
				this.m01.ToString(format),
				this.m02.ToString(format),
				this.m03.ToString(format),
				this.m10.ToString(format),
				this.m11.ToString(format),
				this.m12.ToString(format),
				this.m13.ToString(format),
				this.m20.ToString(format),
				this.m21.ToString(format),
				this.m22.ToString(format),
				this.m23.ToString(format),
				this.m30.ToString(format),
				this.m31.ToString(format),
				this.m32.ToString(format),
				this.m33.ToString(format)
			});
		}
	}
}
