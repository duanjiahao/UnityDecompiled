using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationClipPair
	{
		public AnimationClip originalClip;

		public AnimationClip overrideClip;
	}
}
