using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class AudioLowPassFilter : Behaviour
	{
		public extern float cutoffFrequency
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float lowpassResonanceQ
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("lowpassResonaceQ is obsolete. Use lowpassResonanceQ instead (UnityUpgradable).", true)]
		public extern float lowpassResonaceQ
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
