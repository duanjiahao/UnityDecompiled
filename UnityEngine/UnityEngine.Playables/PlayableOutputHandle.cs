using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[UsedByNativeCode]
	public struct PlayableOutputHandle
	{
		internal IntPtr m_Handle;

		internal int m_Version;

		public static PlayableOutputHandle Null
		{
			get
			{
				return new PlayableOutputHandle
				{
					m_Version = 2147483647
				};
			}
		}

		internal bool IsValid()
		{
			return PlayableOutputHandle.IsValidInternal(ref this);
		}

		internal static bool IsValidInternal(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_IsValidInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsValidInternal(ref PlayableOutputHandle handle);

		internal static Type GetPlayableOutputTypeOf(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_GetPlayableOutputTypeOf(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetPlayableOutputTypeOf(ref PlayableOutputHandle handle);

		internal bool IsPlayableOutputOfType<T>()
		{
			return PlayableOutputHandle.GetPlayableOutputTypeOf(ref this) == typeof(T);
		}

		internal UnityEngine.Object GetReferenceObject()
		{
			return PlayableOutputHandle.GetInternalReferenceObject(ref this);
		}

		internal void SetReferenceObject(UnityEngine.Object value)
		{
			PlayableOutputHandle.SetInternalReferenceObject(ref this, value);
		}

		internal static UnityEngine.Object GetInternalReferenceObject(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_GetInternalReferenceObject(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_GetInternalReferenceObject(ref PlayableOutputHandle handle);

		internal static void SetInternalReferenceObject(ref PlayableOutputHandle handle, UnityEngine.Object target)
		{
			PlayableOutputHandle.INTERNAL_CALL_SetInternalReferenceObject(ref handle, target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInternalReferenceObject(ref PlayableOutputHandle handle, UnityEngine.Object target);

		internal UnityEngine.Object GetUserData()
		{
			return PlayableOutputHandle.GetInternalUserData(ref this);
		}

		internal void SetUserData(UnityEngine.Object value)
		{
			PlayableOutputHandle.SetInternalUserData(ref this, value);
		}

		internal static UnityEngine.Object GetInternalUserData(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_GetInternalUserData(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_GetInternalUserData(ref PlayableOutputHandle handle);

		internal static void SetInternalUserData(ref PlayableOutputHandle handle, [Writable] UnityEngine.Object target)
		{
			PlayableOutputHandle.INTERNAL_CALL_SetInternalUserData(ref handle, target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInternalUserData(ref PlayableOutputHandle handle, [Writable] UnityEngine.Object target);

		internal PlayableHandle GetSourcePlayable()
		{
			return PlayableOutputHandle.GetSourcePlayableInternal(ref this);
		}

		internal void SetSourcePlayable(PlayableHandle value)
		{
			PlayableOutputHandle.SetSourcePlayableInternal(ref this, ref value);
		}

		internal static PlayableHandle GetSourcePlayableInternal(ref PlayableOutputHandle handle)
		{
			PlayableHandle result;
			PlayableOutputHandle.INTERNAL_CALL_GetSourcePlayableInternal(ref handle, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSourcePlayableInternal(ref PlayableOutputHandle handle, out PlayableHandle value);

		internal static void SetSourcePlayableInternal(ref PlayableOutputHandle handle, ref PlayableHandle target)
		{
			PlayableOutputHandle.INTERNAL_CALL_SetSourcePlayableInternal(ref handle, ref target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetSourcePlayableInternal(ref PlayableOutputHandle handle, ref PlayableHandle target);

		internal int GetSourceInputPort()
		{
			return PlayableOutputHandle.GetSourceInputPortInternal(ref this);
		}

		internal void SetSourceInputPort(int value)
		{
			PlayableOutputHandle.SetSourceInputPortInternal(ref this, value);
		}

		internal static int GetSourceInputPortInternal(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_GetSourceInputPortInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetSourceInputPortInternal(ref PlayableOutputHandle handle);

		internal static void SetSourceInputPortInternal(ref PlayableOutputHandle handle, int port)
		{
			PlayableOutputHandle.INTERNAL_CALL_SetSourceInputPortInternal(ref handle, port);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetSourceInputPortInternal(ref PlayableOutputHandle handle, int port);

		internal float GetWeight()
		{
			return PlayableOutputHandle.GetWeightInternal(ref this);
		}

		internal void SetWeight(float value)
		{
			PlayableOutputHandle.SetWeightInternal(ref this, value);
		}

		internal static void SetWeightInternal(ref PlayableOutputHandle handle, float weight)
		{
			PlayableOutputHandle.INTERNAL_CALL_SetWeightInternal(ref handle, weight);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetWeightInternal(ref PlayableOutputHandle handle, float weight);

		internal static float GetWeightInternal(ref PlayableOutputHandle handle)
		{
			return PlayableOutputHandle.INTERNAL_CALL_GetWeightInternal(ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetWeightInternal(ref PlayableOutputHandle handle);

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode() ^ this.m_Version.GetHashCode();
		}

		public static bool operator ==(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return PlayableOutputHandle.CompareVersion(lhs, rhs);
		}

		public static bool operator !=(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return !PlayableOutputHandle.CompareVersion(lhs, rhs);
		}

		public override bool Equals(object p)
		{
			return p is PlayableOutputHandle && PlayableOutputHandle.CompareVersion(this, (PlayableOutputHandle)p);
		}

		internal static bool CompareVersion(PlayableOutputHandle lhs, PlayableOutputHandle rhs)
		{
			return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
		}
	}
}
