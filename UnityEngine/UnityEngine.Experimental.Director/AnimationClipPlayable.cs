using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct AnimationClipPlayable
	{
		internal AnimationPlayable handle;

		internal Playable node
		{
			get
			{
				return this.handle.node;
			}
		}

		public PlayState state
		{
			get
			{
				return Playables.GetPlayStateValidated(this, base.GetType());
			}
			set
			{
				Playables.SetPlayStateValidated(this, value, base.GetType());
			}
		}

		public double time
		{
			get
			{
				return Playables.GetTimeValidated(this, base.GetType());
			}
			set
			{
				Playables.SetTimeValidated(this, value, base.GetType());
			}
		}

		public double duration
		{
			get
			{
				return Playables.GetDurationValidated(this, base.GetType());
			}
			set
			{
				Playables.SetDurationValidated(this, value, base.GetType());
			}
		}

		public int outputCount
		{
			get
			{
				return Playables.GetOutputCountValidated(this, base.GetType());
			}
		}

		public AnimationClip clip
		{
			get
			{
				return AnimationClipPlayable.GetAnimationClip(ref this);
			}
		}

		public float speed
		{
			get
			{
				return AnimationClipPlayable.GetSpeed(ref this);
			}
			set
			{
				AnimationClipPlayable.SetSpeed(ref this, value);
			}
		}

		public bool applyFootIK
		{
			get
			{
				return AnimationClipPlayable.GetApplyFootIK(ref this);
			}
			set
			{
				AnimationClipPlayable.SetApplyFootIK(ref this, value);
			}
		}

		internal bool removeStartOffset
		{
			get
			{
				return AnimationClipPlayable.GetRemoveStartOffset(ref this);
			}
			set
			{
				AnimationClipPlayable.SetRemoveStartOffset(ref this, value);
			}
		}

		public static AnimationClipPlayable Create(AnimationClip clip)
		{
			AnimationClipPlayable result = default(AnimationClipPlayable);
			AnimationClipPlayable.InternalCreate(clip, ref result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(AnimationClip clip, ref AnimationClipPlayable that);

		public void Destroy()
		{
			this.node.Destroy();
		}

		public static bool operator ==(AnimationClipPlayable x, Playable y)
		{
			return Playables.Equals(x, y);
		}

		public static bool operator !=(AnimationClipPlayable x, Playable y)
		{
			return !Playables.Equals(x, y);
		}

		public override bool Equals(object p)
		{
			return Playables.Equals(this, p);
		}

		public override int GetHashCode()
		{
			return this.node.GetHashCode();
		}

		public static implicit operator Playable(AnimationClipPlayable b)
		{
			return b.node;
		}

		public static implicit operator AnimationPlayable(AnimationClipPlayable b)
		{
			return b.handle;
		}

		public bool IsValid()
		{
			return Playables.IsValid(this);
		}

		public Playable GetOutput(int outputPort)
		{
			return Playables.GetOutputValidated(this, outputPort, base.GetType());
		}

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip GetAnimationClip(ref AnimationClipPlayable that);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetSpeed(ref AnimationClipPlayable that);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSpeed(ref AnimationClipPlayable that, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetApplyFootIK(ref AnimationClipPlayable that);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetApplyFootIK(ref AnimationClipPlayable that, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetRemoveStartOffset(ref AnimationClipPlayable that);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRemoveStartOffset(ref AnimationClipPlayable that, bool value);
	}
}
