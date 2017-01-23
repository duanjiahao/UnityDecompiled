using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AudioLowPassFilter : Behaviour
	{
		public extern float cutoffFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimationCurve customCutoffCurve
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float lowpassResonanceQ
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioLowPassFilter.lowpassResonaceQ is obsolete. Use lowpassResonanceQ instead (UnityUpgradable) -> lowpassResonanceQ", true)]
		public float lowpassResonaceQ
		{
			get
			{
				return this.lowpassResonanceQ;
			}
			set
			{
			}
		}
	}
}
