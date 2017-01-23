using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Behaviour : Component
	{
		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isActiveAndEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
