using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class OcclusionPortal : Component
	{
		public extern bool open
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
