using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	internal sealed class Sampler
	{
		internal IntPtr m_Ptr;

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal Sampler()
		{
		}
	}
}
