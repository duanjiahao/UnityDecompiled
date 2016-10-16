using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	public struct SphericalHarmonicsL2
	{
		private float shr0;

		private float shr1;

		private float shr2;

		private float shr3;

		private float shr4;

		private float shr5;

		private float shr6;

		private float shr7;

		private float shr8;

		private float shg0;

		private float shg1;

		private float shg2;

		private float shg3;

		private float shg4;

		private float shg5;

		private float shg6;

		private float shg7;

		private float shg8;

		private float shb0;

		private float shb1;

		private float shb2;

		private float shb3;

		private float shb4;

		private float shb5;

		private float shb6;

		private float shb7;

		private float shb8;

		public float this[int rgb, int coefficient]
		{
			get
			{
				switch (rgb * 9 + coefficient)
				{
				case 0:
					return this.shr0;
				case 1:
					return this.shr1;
				case 2:
					return this.shr2;
				case 3:
					return this.shr3;
				case 4:
					return this.shr4;
				case 5:
					return this.shr5;
				case 6:
					return this.shr6;
				case 7:
					return this.shr7;
				case 8:
					return this.shr8;
				case 9:
					return this.shg0;
				case 10:
					return this.shg1;
				case 11:
					return this.shg2;
				case 12:
					return this.shg3;
				case 13:
					return this.shg4;
				case 14:
					return this.shg5;
				case 15:
					return this.shg6;
				case 16:
					return this.shg7;
				case 17:
					return this.shg8;
				case 18:
					return this.shb0;
				case 19:
					return this.shb1;
				case 20:
					return this.shb2;
				case 21:
					return this.shb3;
				case 22:
					return this.shb4;
				case 23:
					return this.shb5;
				case 24:
					return this.shb6;
				case 25:
					return this.shb7;
				case 26:
					return this.shb8;
				default:
					throw new IndexOutOfRangeException("Invalid index!");
				}
			}
			set
			{
				switch (rgb * 9 + coefficient)
				{
				case 0:
					this.shr0 = value;
					break;
				case 1:
					this.shr1 = value;
					break;
				case 2:
					this.shr2 = value;
					break;
				case 3:
					this.shr3 = value;
					break;
				case 4:
					this.shr4 = value;
					break;
				case 5:
					this.shr5 = value;
					break;
				case 6:
					this.shr6 = value;
					break;
				case 7:
					this.shr7 = value;
					break;
				case 8:
					this.shr8 = value;
					break;
				case 9:
					this.shg0 = value;
					break;
				case 10:
					this.shg1 = value;
					break;
				case 11:
					this.shg2 = value;
					break;
				case 12:
					this.shg3 = value;
					break;
				case 13:
					this.shg4 = value;
					break;
				case 14:
					this.shg5 = value;
					break;
				case 15:
					this.shg6 = value;
					break;
				case 16:
					this.shg7 = value;
					break;
				case 17:
					this.shg8 = value;
					break;
				case 18:
					this.shb0 = value;
					break;
				case 19:
					this.shb1 = value;
					break;
				case 20:
					this.shb2 = value;
					break;
				case 21:
					this.shb3 = value;
					break;
				case 22:
					this.shb4 = value;
					break;
				case 23:
					this.shb5 = value;
					break;
				case 24:
					this.shb6 = value;
					break;
				case 25:
					this.shb7 = value;
					break;
				case 26:
					this.shb8 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid index!");
				}
			}
		}

		public void Clear()
		{
			SphericalHarmonicsL2.ClearInternal(ref this);
		}

		private static void ClearInternal(ref SphericalHarmonicsL2 sh)
		{
			SphericalHarmonicsL2.INTERNAL_CALL_ClearInternal(ref sh);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearInternal(ref SphericalHarmonicsL2 sh);

		public void AddAmbientLight(Color color)
		{
			SphericalHarmonicsL2.AddAmbientLightInternal(color, ref this);
		}

		private static void AddAmbientLightInternal(Color color, ref SphericalHarmonicsL2 sh)
		{
			SphericalHarmonicsL2.INTERNAL_CALL_AddAmbientLightInternal(ref color, ref sh);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddAmbientLightInternal(ref Color color, ref SphericalHarmonicsL2 sh);

		public void AddDirectionalLight(Vector3 direction, Color color, float intensity)
		{
			Color color2 = color * (2f * intensity);
			SphericalHarmonicsL2.AddDirectionalLightInternal(direction, color2, ref this);
		}

		private static void AddDirectionalLightInternal(Vector3 direction, Color color, ref SphericalHarmonicsL2 sh)
		{
			SphericalHarmonicsL2.INTERNAL_CALL_AddDirectionalLightInternal(ref direction, ref color, ref sh);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddDirectionalLightInternal(ref Vector3 direction, ref Color color, ref SphericalHarmonicsL2 sh);

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.shr0.GetHashCode();
			num = num * 23 + this.shr1.GetHashCode();
			num = num * 23 + this.shr2.GetHashCode();
			num = num * 23 + this.shr3.GetHashCode();
			num = num * 23 + this.shr4.GetHashCode();
			num = num * 23 + this.shr5.GetHashCode();
			num = num * 23 + this.shr6.GetHashCode();
			num = num * 23 + this.shr7.GetHashCode();
			num = num * 23 + this.shr8.GetHashCode();
			num = num * 23 + this.shg0.GetHashCode();
			num = num * 23 + this.shg1.GetHashCode();
			num = num * 23 + this.shg2.GetHashCode();
			num = num * 23 + this.shg3.GetHashCode();
			num = num * 23 + this.shg4.GetHashCode();
			num = num * 23 + this.shg5.GetHashCode();
			num = num * 23 + this.shg6.GetHashCode();
			num = num * 23 + this.shg7.GetHashCode();
			num = num * 23 + this.shg8.GetHashCode();
			num = num * 23 + this.shb0.GetHashCode();
			num = num * 23 + this.shb1.GetHashCode();
			num = num * 23 + this.shb2.GetHashCode();
			num = num * 23 + this.shb3.GetHashCode();
			num = num * 23 + this.shb4.GetHashCode();
			num = num * 23 + this.shb5.GetHashCode();
			num = num * 23 + this.shb6.GetHashCode();
			num = num * 23 + this.shb7.GetHashCode();
			return num * 23 + this.shb8.GetHashCode();
		}

		public override bool Equals(object other)
		{
			if (!(other is SphericalHarmonicsL2))
			{
				return false;
			}
			SphericalHarmonicsL2 rhs = (SphericalHarmonicsL2)other;
			return this == rhs;
		}

		public static SphericalHarmonicsL2 operator *(SphericalHarmonicsL2 lhs, float rhs)
		{
			return new SphericalHarmonicsL2
			{
				shr0 = lhs.shr0 * rhs,
				shr1 = lhs.shr1 * rhs,
				shr2 = lhs.shr2 * rhs,
				shr3 = lhs.shr3 * rhs,
				shr4 = lhs.shr4 * rhs,
				shr5 = lhs.shr5 * rhs,
				shr6 = lhs.shr6 * rhs,
				shr7 = lhs.shr7 * rhs,
				shr8 = lhs.shr8 * rhs,
				shg0 = lhs.shg0 * rhs,
				shg1 = lhs.shg1 * rhs,
				shg2 = lhs.shg2 * rhs,
				shg3 = lhs.shg3 * rhs,
				shg4 = lhs.shg4 * rhs,
				shg5 = lhs.shg5 * rhs,
				shg6 = lhs.shg6 * rhs,
				shg7 = lhs.shg7 * rhs,
				shg8 = lhs.shg8 * rhs,
				shb0 = lhs.shb0 * rhs,
				shb1 = lhs.shb1 * rhs,
				shb2 = lhs.shb2 * rhs,
				shb3 = lhs.shb3 * rhs,
				shb4 = lhs.shb4 * rhs,
				shb5 = lhs.shb5 * rhs,
				shb6 = lhs.shb6 * rhs,
				shb7 = lhs.shb7 * rhs,
				shb8 = lhs.shb8 * rhs
			};
		}

		public static SphericalHarmonicsL2 operator *(float lhs, SphericalHarmonicsL2 rhs)
		{
			return new SphericalHarmonicsL2
			{
				shr0 = rhs.shr0 * lhs,
				shr1 = rhs.shr1 * lhs,
				shr2 = rhs.shr2 * lhs,
				shr3 = rhs.shr3 * lhs,
				shr4 = rhs.shr4 * lhs,
				shr5 = rhs.shr5 * lhs,
				shr6 = rhs.shr6 * lhs,
				shr7 = rhs.shr7 * lhs,
				shr8 = rhs.shr8 * lhs,
				shg0 = rhs.shg0 * lhs,
				shg1 = rhs.shg1 * lhs,
				shg2 = rhs.shg2 * lhs,
				shg3 = rhs.shg3 * lhs,
				shg4 = rhs.shg4 * lhs,
				shg5 = rhs.shg5 * lhs,
				shg6 = rhs.shg6 * lhs,
				shg7 = rhs.shg7 * lhs,
				shg8 = rhs.shg8 * lhs,
				shb0 = rhs.shb0 * lhs,
				shb1 = rhs.shb1 * lhs,
				shb2 = rhs.shb2 * lhs,
				shb3 = rhs.shb3 * lhs,
				shb4 = rhs.shb4 * lhs,
				shb5 = rhs.shb5 * lhs,
				shb6 = rhs.shb6 * lhs,
				shb7 = rhs.shb7 * lhs,
				shb8 = rhs.shb8 * lhs
			};
		}

		public static SphericalHarmonicsL2 operator +(SphericalHarmonicsL2 lhs, SphericalHarmonicsL2 rhs)
		{
			return new SphericalHarmonicsL2
			{
				shr0 = lhs.shr0 + rhs.shr0,
				shr1 = lhs.shr1 + rhs.shr1,
				shr2 = lhs.shr2 + rhs.shr2,
				shr3 = lhs.shr3 + rhs.shr3,
				shr4 = lhs.shr4 + rhs.shr4,
				shr5 = lhs.shr5 + rhs.shr5,
				shr6 = lhs.shr6 + rhs.shr6,
				shr7 = lhs.shr7 + rhs.shr7,
				shr8 = lhs.shr8 + rhs.shr8,
				shg0 = lhs.shg0 + rhs.shg0,
				shg1 = lhs.shg1 + rhs.shg1,
				shg2 = lhs.shg2 + rhs.shg2,
				shg3 = lhs.shg3 + rhs.shg3,
				shg4 = lhs.shg4 + rhs.shg4,
				shg5 = lhs.shg5 + rhs.shg5,
				shg6 = lhs.shg6 + rhs.shg6,
				shg7 = lhs.shg7 + rhs.shg7,
				shg8 = lhs.shg8 + rhs.shg8,
				shb0 = lhs.shb0 + rhs.shb0,
				shb1 = lhs.shb1 + rhs.shb1,
				shb2 = lhs.shb2 + rhs.shb2,
				shb3 = lhs.shb3 + rhs.shb3,
				shb4 = lhs.shb4 + rhs.shb4,
				shb5 = lhs.shb5 + rhs.shb5,
				shb6 = lhs.shb6 + rhs.shb6,
				shb7 = lhs.shb7 + rhs.shb7,
				shb8 = lhs.shb8 + rhs.shb8
			};
		}

		public static bool operator ==(SphericalHarmonicsL2 lhs, SphericalHarmonicsL2 rhs)
		{
			return lhs.shr0 == rhs.shr0 && lhs.shr1 == rhs.shr1 && lhs.shr2 == rhs.shr2 && lhs.shr3 == rhs.shr3 && lhs.shr4 == rhs.shr4 && lhs.shr5 == rhs.shr5 && lhs.shr6 == rhs.shr6 && lhs.shr7 == rhs.shr7 && lhs.shr8 == rhs.shr8 && lhs.shg0 == rhs.shg0 && lhs.shg1 == rhs.shg1 && lhs.shg2 == rhs.shg2 && lhs.shg3 == rhs.shg3 && lhs.shg4 == rhs.shg4 && lhs.shg5 == rhs.shg5 && lhs.shg6 == rhs.shg6 && lhs.shg7 == rhs.shg7 && lhs.shg8 == rhs.shg8 && lhs.shb0 == rhs.shb0 && lhs.shb1 == rhs.shb1 && lhs.shb2 == rhs.shb2 && lhs.shb3 == rhs.shb3 && lhs.shb4 == rhs.shb4 && lhs.shb5 == rhs.shb5 && lhs.shb6 == rhs.shb6 && lhs.shb7 == rhs.shb7 && lhs.shb8 == rhs.shb8;
		}

		public static bool operator !=(SphericalHarmonicsL2 lhs, SphericalHarmonicsL2 rhs)
		{
			return !(lhs == rhs);
		}
	}
}
