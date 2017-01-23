using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class MovieTexture : Texture
	{
		public extern AudioClip audioClip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool loop
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isReadyToPlay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float duration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void Play()
		{
			MovieTexture.INTERNAL_CALL_Play(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Play(MovieTexture self);

		public void Stop()
		{
			MovieTexture.INTERNAL_CALL_Stop(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Stop(MovieTexture self);

		public void Pause()
		{
			MovieTexture.INTERNAL_CALL_Pause(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Pause(MovieTexture self);
	}
}
