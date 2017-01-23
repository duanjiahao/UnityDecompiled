using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AnimationClip : Motion
	{
		public extern float length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern float startTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern float stopTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float frameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern WrapMode wrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Bounds localBounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_localBounds(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_localBounds(ref value);
			}
		}

		public new extern bool legacy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool humanMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public AnimationEvent[] events
		{
			get
			{
				return (AnimationEvent[])this.GetEventsInternal();
			}
			set
			{
				this.SetEventsInternal(value);
			}
		}

		public AnimationClip()
		{
			AnimationClip.Internal_CreateAnimationClip(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SampleAnimation(GameObject go, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCurve(string relativePath, Type type, string propertyName, AnimationCurve curve);

		public void EnsureQuaternionContinuity()
		{
			AnimationClip.INTERNAL_CALL_EnsureQuaternionContinuity(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_EnsureQuaternionContinuity(AnimationClip self);

		public void ClearCurves()
		{
			AnimationClip.INTERNAL_CALL_ClearCurves(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearCurves(AnimationClip self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_localBounds(out Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_localBounds(ref Bounds value);

		public void AddEvent(AnimationEvent evt)
		{
			if (evt == null)
			{
				throw new ArgumentNullException("evt");
			}
			this.AddEventInternal(evt);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddEventInternal(object evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetEventsInternal(Array value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Array GetEventsInternal();
	}
}
