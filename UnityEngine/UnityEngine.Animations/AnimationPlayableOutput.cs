using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	public struct AnimationPlayableOutput : IPlayableOutput
	{
		private PlayableOutputHandle m_Handle;

		public static AnimationPlayableOutput Null
		{
			get
			{
				return new AnimationPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		internal AnimationPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<AnimationPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
		{
			PlayableOutputHandle handle;
			AnimationPlayableOutput result;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out handle))
			{
				result = AnimationPlayableOutput.Null;
			}
			else
			{
				AnimationPlayableOutput animationPlayableOutput = new AnimationPlayableOutput(handle);
				animationPlayableOutput.SetTarget(target);
				result = animationPlayableOutput;
			}
			return result;
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(AnimationPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator AnimationPlayableOutput(PlayableOutput output)
		{
			return new AnimationPlayableOutput(output.GetHandle());
		}

		public Animator GetTarget()
		{
			return AnimationPlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		public void SetTarget(Animator value)
		{
			AnimationPlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		private static Animator InternalGetTarget(ref PlayableOutputHandle handle)
		{
			return AnimationPlayableOutput.INTERNAL_CALL_InternalGetTarget(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Animator INTERNAL_CALL_InternalGetTarget(ref PlayableOutputHandle handle);

		private static void InternalSetTarget(ref PlayableOutputHandle handle, Animator target)
		{
			AnimationPlayableOutput.INTERNAL_CALL_InternalSetTarget(ref handle, target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSetTarget(ref PlayableOutputHandle handle, Animator target);
	}
}
