using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AudioDistortionFilter : Behaviour
	{
		public extern float distortionLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
