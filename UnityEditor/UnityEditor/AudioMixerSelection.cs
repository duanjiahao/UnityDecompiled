using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Audio;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioMixerSelection
	{
		private AudioMixerController m_Controller;

		public List<AudioMixerGroupController> ChannelStripSelection
		{
			get;
			private set;
		}

		public AudioMixerSelection(AudioMixerController controller)
		{
			this.m_Controller = controller;
			this.ChannelStripSelection = new List<AudioMixerGroupController>();
			this.SyncToUnitySelection();
		}

		public void SyncToUnitySelection()
		{
			if (this.m_Controller != null)
			{
				this.RefreshCachedChannelStripSelection();
			}
		}

		public void SetChannelStrips(List<AudioMixerGroupController> newSelection)
		{
			Selection.objects = newSelection.ToArray();
		}

		public void SetSingleChannelStrip(AudioMixerGroupController group)
		{
			Selection.objects = new AudioMixerGroupController[]
			{
				group
			};
		}

		public void ToggleChannelStrip(AudioMixerGroupController group)
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>(Selection.objects);
			if (list.Contains(group))
			{
				list.Remove(group);
			}
			else
			{
				list.Add(group);
			}
			Selection.objects = list.ToArray();
		}

		public void ClearChannelStrips()
		{
			Selection.objects = new UnityEngine.Object[0];
		}

		public bool HasSingleChannelStripSelection()
		{
			return this.ChannelStripSelection.Count == 1;
		}

		private void RefreshCachedChannelStripSelection()
		{
			UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(AudioMixerGroupController), SelectionMode.Deep);
			this.ChannelStripSelection = new List<AudioMixerGroupController>();
			List<AudioMixerGroupController> allAudioGroupsSlow = this.m_Controller.GetAllAudioGroupsSlow();
			foreach (AudioMixerGroupController current in allAudioGroupsSlow)
			{
				if (filtered.Contains(current))
				{
					this.ChannelStripSelection.Add(current);
				}
			}
		}

		public void Sanitize()
		{
			this.RefreshCachedChannelStripSelection();
		}
	}
}
