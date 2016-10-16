using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Experimental.Director
{
	public class DirectorPlayer : Behaviour
	{
		public void Play(Playable pStruct)
		{
			this.PlayStructInternal(pStruct);
		}

		private void PlayStructInternal(Playable pStruct)
		{
			DirectorPlayer.INTERNAL_CALL_PlayStructInternal(this, ref pStruct);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PlayStructInternal(DirectorPlayer self, ref Playable pStruct);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTime(double time);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern double GetTime();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTimeUpdateMode(DirectorUpdateMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern DirectorUpdateMode GetTimeUpdateMode();
	}
}
