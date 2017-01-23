using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Tree : Component
	{
		public extern ScriptableObject data
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasSpeedTreeWind
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
