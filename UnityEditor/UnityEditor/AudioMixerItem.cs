using System;
using UnityEditor.Audio;

namespace UnityEditor
{
	internal class AudioMixerItem : TreeViewItem
	{
		private const string kSuspendedText = " - Inactive";

		public AudioMixerController mixer
		{
			get;
			set;
		}

		public string infoText
		{
			get;
			set;
		}

		public float labelWidth
		{
			get;
			set;
		}

		private bool lastSuspendedState
		{
			get;
			set;
		}

		public AudioMixerItem(int id, int depth, TreeViewItem parent, string displayName, AudioMixerController mixer, string infoText) : base(id, depth, parent, displayName)
		{
			this.mixer = mixer;
			this.infoText = infoText;
			this.UpdateSuspendedString(true);
		}

		public void UpdateSuspendedString(bool force)
		{
			bool isSuspended = this.mixer.isSuspended;
			if (this.lastSuspendedState != isSuspended || force)
			{
				this.lastSuspendedState = isSuspended;
				if (isSuspended)
				{
					this.AddSuspendedText();
				}
				else
				{
					this.RemoveSuspendedText();
				}
			}
		}

		private void RemoveSuspendedText()
		{
			int num = this.infoText.IndexOf(" - Inactive", StringComparison.Ordinal);
			if (num >= 0)
			{
				this.infoText = this.infoText.Remove(num, " - Inactive".Length);
			}
		}

		private void AddSuspendedText()
		{
			int num = this.infoText.IndexOf(" - Inactive", StringComparison.Ordinal);
			if (num < 0)
			{
				this.infoText += " - Inactive";
			}
		}
	}
}
