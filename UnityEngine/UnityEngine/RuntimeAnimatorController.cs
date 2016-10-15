using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class RuntimeAnimatorController : Object
	{
		public extern AnimationClip[] animationClips
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
