using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Tree : Component
	{
		public extern ScriptableObject data
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasSpeedTreeWind
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
