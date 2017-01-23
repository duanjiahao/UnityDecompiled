using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class RuntimeAnimatorController : Object
	{
		public extern AnimationClip[] animationClips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
