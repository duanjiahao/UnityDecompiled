using System;
using System.Collections.Generic;

namespace UnityEditor.Audio
{
	internal class MixerGroupControllerCompareByName : IComparer<AudioMixerGroupController>
	{
		public int Compare(AudioMixerGroupController x, AudioMixerGroupController y)
		{
			return StringComparer.InvariantCultureIgnoreCase.Compare(x.GetDisplayString(), y.GetDisplayString());
		}
	}
}
