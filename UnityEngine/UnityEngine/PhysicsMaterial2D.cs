using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class PhysicsMaterial2D : Object
	{
		public extern float bounciness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float friction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public PhysicsMaterial2D()
		{
			PhysicsMaterial2D.Internal_Create(this, null);
		}

		public PhysicsMaterial2D(string name)
		{
			PhysicsMaterial2D.Internal_Create(this, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] PhysicsMaterial2D mat, string name);
	}
}
