using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	internal sealed class AnimationOffsetPlayable : AnimationPlayable
	{
		public Vector3 position
		{
			get
			{
				return AnimationOffsetPlayable.GetPosition(ref this.handle);
			}
			set
			{
				AnimationOffsetPlayable.SetPosition(ref this.handle, value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				return AnimationOffsetPlayable.GetRotation(ref this.handle);
			}
			set
			{
				AnimationOffsetPlayable.SetRotation(ref this.handle, value);
			}
		}

		private static Vector3 GetPosition(ref PlayableHandle handle)
		{
			Vector3 result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetPosition(ref handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPosition(ref PlayableHandle handle, out Vector3 value);

		private static void SetPosition(ref PlayableHandle handle, Vector3 value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetPosition(ref handle, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPosition(ref PlayableHandle handle, ref Vector3 value);

		private static Quaternion GetRotation(ref PlayableHandle handle)
		{
			Quaternion result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetRotation(ref handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRotation(ref PlayableHandle handle, out Quaternion value);

		private static void SetRotation(ref PlayableHandle handle, Quaternion value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetRotation(ref handle, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRotation(ref PlayableHandle handle, ref Quaternion value);
	}
}
