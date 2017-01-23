using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Skybox : Behaviour
	{
		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
