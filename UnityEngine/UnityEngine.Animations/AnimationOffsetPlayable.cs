using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	[RequiredByNativeCode]
	internal struct AnimationOffsetPlayable : IPlayable, IEquatable<AnimationOffsetPlayable>
	{
		private PlayableHandle m_Handle;

		private static readonly AnimationOffsetPlayable m_NullPlayable = new AnimationOffsetPlayable(PlayableHandle.Null);

		public static AnimationOffsetPlayable Null
		{
			get
			{
				return AnimationOffsetPlayable.m_NullPlayable;
			}
		}

		internal AnimationOffsetPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOfType<AnimationOffsetPlayable>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationOffsetPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		public static AnimationOffsetPlayable Create(PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
		{
			PlayableHandle handle = AnimationOffsetPlayable.CreateHandle(graph, position, rotation, inputCount);
			return new AnimationOffsetPlayable(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AnimationOffsetPlayable.CreateHandleInternal(graph, position, rotation, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(inputCount);
				result = @null;
			}
			return result;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator Playable(AnimationOffsetPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator AnimationOffsetPlayable(Playable playable)
		{
			return new AnimationOffsetPlayable(playable.GetHandle());
		}

		public bool Equals(AnimationOffsetPlayable other)
		{
			return this.Equals(other.GetHandle());
		}

		private static bool CreateHandleInternal(PlayableGraph graph, Vector3 position, Quaternion rotation, ref PlayableHandle handle)
		{
			return AnimationOffsetPlayable.INTERNAL_CALL_CreateHandleInternal(ref graph, ref position, ref rotation, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateHandleInternal(ref PlayableGraph graph, ref Vector3 position, ref Quaternion rotation, ref PlayableHandle handle);

		public Vector3 GetPosition()
		{
			return AnimationOffsetPlayable.GetPositionInternal(ref this.m_Handle);
		}

		public void SetPosition(Vector3 value)
		{
			AnimationOffsetPlayable.SetPositionInternal(ref this.m_Handle, value);
		}

		public Quaternion GetRotation()
		{
			return AnimationOffsetPlayable.GetRotationInternal(ref this.m_Handle);
		}

		public void SetRotation(Quaternion value)
		{
			AnimationOffsetPlayable.SetRotationInternal(ref this.m_Handle, value);
		}

		private static Vector3 GetPositionInternal(ref PlayableHandle handle)
		{
			Vector3 result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetPositionInternal(ref handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPositionInternal(ref PlayableHandle handle, out Vector3 value);

		private static void SetPositionInternal(ref PlayableHandle handle, Vector3 value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetPositionInternal(ref handle, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPositionInternal(ref PlayableHandle handle, ref Vector3 value);

		private static Quaternion GetRotationInternal(ref PlayableHandle handle)
		{
			Quaternion result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetRotationInternal(ref handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRotationInternal(ref PlayableHandle handle, out Quaternion value);

		private static void SetRotationInternal(ref PlayableHandle handle, Quaternion value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetRotationInternal(ref handle, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRotationInternal(ref PlayableHandle handle, ref Quaternion value);
	}
}
