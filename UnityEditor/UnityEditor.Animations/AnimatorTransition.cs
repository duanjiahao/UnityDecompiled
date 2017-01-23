using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.Animations
{
	public sealed class AnimatorTransition : AnimatorTransitionBase
	{
		public AnimatorTransition()
		{
			AnimatorTransition.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorTransition mono);
	}
}
