using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Effector2D : Behaviour
	{
		public extern bool useColliderMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int colliderMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern bool requiresCollider
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool designedForTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool designedForNonTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
