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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PlayStructInternal(DirectorPlayer self, ref Playable pStruct);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTime(double time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern double GetTime();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTimeUpdateMode(DirectorUpdateMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern DirectorUpdateMode GetTimeUpdateMode();
	}
}
