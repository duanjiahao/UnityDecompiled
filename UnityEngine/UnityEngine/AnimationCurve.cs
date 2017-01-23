using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationCurve
	{
		internal IntPtr m_Ptr;

		public Keyframe[] keys
		{
			get
			{
				return this.GetKeys();
			}
			set
			{
				this.SetKeys(value);
			}
		}

		public Keyframe this[int index]
		{
			get
			{
				return this.GetKey_Internal(index);
			}
		}

		public extern int length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern WrapMode preWrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern WrapMode postWrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public AnimationCurve(params Keyframe[] keys)
		{
			this.Init(keys);
		}

		[RequiredByNativeCode]
		public AnimationCurve()
		{
			this.Init(null);
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup();

		~AnimationCurve()
		{
			this.Cleanup();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float Evaluate(float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int AddKey(float time, float value);

		public int AddKey(Keyframe key)
		{
			return this.AddKey_Internal(key);
		}

		private int AddKey_Internal(Keyframe key)
		{
			return AnimationCurve.INTERNAL_CALL_AddKey_Internal(this, ref key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_AddKey_Internal(AnimationCurve self, ref Keyframe key);

		public int MoveKey(int index, Keyframe key)
		{
			return AnimationCurve.INTERNAL_CALL_MoveKey(this, index, ref key);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_MoveKey(AnimationCurve self, int index, ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveKey(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetKeys(Keyframe[] keys);

		private Keyframe GetKey_Internal(int index)
		{
			Keyframe result;
			AnimationCurve.INTERNAL_CALL_GetKey_Internal(this, index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetKey_Internal(AnimationCurve self, int index, out Keyframe value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Keyframe[] GetKeys();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothTangents(int index, float weight);

		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			float num = (valueEnd - valueStart) / (timeEnd - timeStart);
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(timeStart, valueStart, 0f, num),
				new Keyframe(timeEnd, valueEnd, num, 0f)
			};
			return new AnimationCurve(keys);
		}

		public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(timeStart, valueStart, 0f, 0f),
				new Keyframe(timeEnd, valueEnd, 0f, 0f)
			};
			return new AnimationCurve(keys);
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Init(Keyframe[] keys);
	}
}
