using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class OcclusionPortal : Component
	{
		public extern bool open
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
