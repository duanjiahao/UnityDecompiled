using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Animations
{
	public sealed class AnimatorTransition : AnimatorTransitionBase
	{
		public AnimatorTransition()
		{
			AnimatorTransition.Internal_Create(this);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorTransition mono);
	}
}
