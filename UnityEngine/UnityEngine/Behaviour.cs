using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Behaviour : Component
	{
		public extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isActiveAndEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
