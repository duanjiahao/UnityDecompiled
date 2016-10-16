using System;
using UnityEditor.Audio;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioMixerSnapshotController))]
	internal class AudioMixerSnapshotControllerInspector : Editor
	{
		public override void OnInspectorGUI()
		{
		}
	}
}
