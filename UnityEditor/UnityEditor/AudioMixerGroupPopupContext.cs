using System;
using UnityEditor.Audio;

namespace UnityEditor
{
	internal class AudioMixerGroupPopupContext
	{
		public AudioMixerController controller;

		public AudioMixerGroupController[] groups;

		public AudioMixerGroupPopupContext(AudioMixerController controller, AudioMixerGroupController group)
		{
			this.controller = controller;
			this.groups = new AudioMixerGroupController[]
			{
				group
			};
		}

		public AudioMixerGroupPopupContext(AudioMixerController controller, AudioMixerGroupController[] groups)
		{
			this.controller = controller;
			this.groups = groups;
		}
	}
}
