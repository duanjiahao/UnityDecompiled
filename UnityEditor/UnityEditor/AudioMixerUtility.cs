using System;
using System.Collections.Generic;
using UnityEditor.Audio;

namespace UnityEditor
{
	internal class AudioMixerUtility
	{
		public class VisitorFetchInstanceIDs
		{
			public List<int> instanceIDs = new List<int>();

			public void Visitor(AudioMixerGroupController group)
			{
				this.instanceIDs.Add(group.GetInstanceID());
			}
		}

		public static void RepaintAudioMixerAndInspectors()
		{
			InspectorWindow.RepaintAllInspectors();
			AudioMixerWindow.RepaintAudioMixerWindow();
		}

		public static void VisitGroupsRecursivly(AudioMixerGroupController group, Action<AudioMixerGroupController> visitorCallback)
		{
			AudioMixerGroupController[] children = group.children;
			for (int i = 0; i < children.Length; i++)
			{
				AudioMixerGroupController group2 = children[i];
				AudioMixerUtility.VisitGroupsRecursivly(group2, visitorCallback);
			}
			if (visitorCallback != null)
			{
				visitorCallback(group);
			}
		}
	}
}
