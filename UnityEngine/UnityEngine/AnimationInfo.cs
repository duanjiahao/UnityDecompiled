using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use AnimatorClipInfo instead (UnityUpgradable) -> AnimatorClipInfo", true)]
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct AnimationInfo
	{
		public AnimationClip clip
		{
			get
			{
				return null;
			}
		}

		public float weight
		{
			get
			{
				return 0f;
			}
		}
	}
}
