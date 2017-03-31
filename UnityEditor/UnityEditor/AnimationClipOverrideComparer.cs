using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class AnimationClipOverrideComparer : IComparer<KeyValuePair<AnimationClip, AnimationClip>>
	{
		public int Compare(KeyValuePair<AnimationClip, AnimationClip> x, KeyValuePair<AnimationClip, AnimationClip> y)
		{
			return string.Compare(x.Key.name, y.Key.name, StringComparison.OrdinalIgnoreCase);
		}
	}
}
