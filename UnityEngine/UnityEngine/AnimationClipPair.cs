using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Obsolete("This class is not used anymore.  See AnimatorOverrideController.GetOverrides() and AnimatorOverrideController.ApplyOverrides()")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationClipPair
	{
		public AnimationClip originalClip;

		public AnimationClip overrideClip;
	}
}
