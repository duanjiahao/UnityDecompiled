using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Random
	{
		[Serializable]
		public struct State
		{
			[SerializeField]
			private int s0;

			[SerializeField]
			private int s1;

			[SerializeField]
			private int s2;

			[SerializeField]
			private int s3;
		}

		[Obsolete("Deprecated. Use InitState() function or Random.state property instead.")]
		public static extern int seed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Random.State state
		{
			get
			{
				Random.State result;
				Random.INTERNAL_get_state(out result);
				return result;
			}
			set
			{
				Random.INTERNAL_set_state(ref value);
			}
		}

		public static extern float value
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Vector3 insideUnitSphere
		{
			get
			{
				Vector3 result;
				Random.INTERNAL_get_insideUnitSphere(out result);
				return result;
			}
		}

		public static Vector2 insideUnitCircle
		{
			get
			{
				Vector2 result;
				Random.GetRandomUnitCircle(out result);
				return result;
			}
		}

		public static Vector3 onUnitSphere
		{
			get
			{
				Vector3 result;
				Random.INTERNAL_get_onUnitSphere(out result);
				return result;
			}
		}

		public static Quaternion rotation
		{
			get
			{
				Quaternion result;
				Random.INTERNAL_get_rotation(out result);
				return result;
			}
		}

		public static Quaternion rotationUniform
		{
			get
			{
				Quaternion result;
				Random.INTERNAL_get_rotationUniform(out result);
				return result;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitState(int seed);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_state(out Random.State value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_state(ref Random.State value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float Range(float min, float max);

		public static int Range(int min, int max)
		{
			return Random.RandomRangeInt(min, max);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RandomRangeInt(int min, int max);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_insideUnitSphere(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRandomUnitCircle(out Vector2 output);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_onUnitSphere(out Vector3 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rotation(out Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rotationUniform(out Quaternion value);

		[Obsolete("Use Random.Range instead")]
		public static float RandomRange(float min, float max)
		{
			return Random.Range(min, max);
		}

		[Obsolete("Use Random.Range instead")]
		public static int RandomRange(int min, int max)
		{
			return Random.Range(min, max);
		}

		public static Color ColorHSV()
		{
			return Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax)
		{
			return Random.ColorHSV(hueMin, hueMax, 0f, 1f, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax)
		{
			return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, 0f, 1f, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax)
		{
			return Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, 1f, 1f);
		}

		public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax)
		{
			float h = Mathf.Lerp(hueMin, hueMax, Random.value);
			float s = Mathf.Lerp(saturationMin, saturationMax, Random.value);
			float v = Mathf.Lerp(valueMin, valueMax, Random.value);
			Color result = Color.HSVToRGB(h, s, v, true);
			result.a = Mathf.Lerp(alphaMin, alphaMax, Random.value);
			return result;
		}
	}
}
