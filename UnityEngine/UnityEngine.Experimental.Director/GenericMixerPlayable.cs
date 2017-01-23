using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	public struct GenericMixerPlayable
	{
		internal Playable handle;

		internal Playable node
		{
			get
			{
				return this.handle;
			}
		}

		public static GenericMixerPlayable Create()
		{
			GenericMixerPlayable result = default(GenericMixerPlayable);
			GenericMixerPlayable.InternalCreate(ref result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(ref GenericMixerPlayable playable);

		public void Destroy()
		{
			this.handle.Destroy();
		}

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
		}

		public static implicit operator Playable(GenericMixerPlayable s)
		{
			return new Playable
			{
				m_Handle = s.handle.m_Handle,
				m_Version = s.handle.m_Version
			};
		}
	}
}
