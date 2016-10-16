using System;
using UnityEditor.Audio;
using UnityEditorInternal;

namespace UnityEditor.ProjectWindowCallback
{
	internal class DoCreateAudioMixer : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			AudioMixerController audioMixerController = AudioMixerController.CreateMixerControllerAtPath(pathName);
			int instanceID;
			if (!string.IsNullOrEmpty(resourceFile) && int.TryParse(resourceFile, out instanceID))
			{
				AudioMixerGroupController audioMixerGroupController = InternalEditorUtility.GetObjectFromInstanceID(instanceID) as AudioMixerGroupController;
				if (audioMixerGroupController != null)
				{
					audioMixerController.outputAudioMixerGroup = audioMixerGroupController;
				}
			}
			ProjectWindowUtil.ShowCreatedAsset(audioMixerController);
		}
	}
}
