using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.Animations
{
	public sealed class AnimatorTransition : AnimatorTransitionBase
	{
		public AnimatorTransition()
		{
			AnimatorTransition.Internal_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorTransition mono);
	}
}
