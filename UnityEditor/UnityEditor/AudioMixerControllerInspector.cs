using System;
using UnityEditor.Audio;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioMixerController))]
	internal class AudioMixerControllerInspector : Editor
	{
		private GUIContent m_EnableSuspendLabel = new GUIContent("Enable Suspend", "Enables/disables suspending of processing in order to save CPU when the RMS signal level falls under the defined threshold (in dB). Mixers resume processing when an AudioSource referencing them starts playing again.");
		private GUIContent m_SuspendThresholdLabel = new GUIContent("Suspend Threshold", "The level of the Master Group at which the mixer suspends processing in order to save CPU. Mixers resume processing when an AudioSource referencing them starts playing again.");
		private SerializedProperty m_EnableSuspend;
		private SerializedProperty m_SuspendThreshold;
		public void OnEnable()
		{
			this.m_SuspendThreshold = base.serializedObject.FindProperty("m_SuspendThreshold");
			this.m_EnableSuspend = base.serializedObject.FindProperty("m_EnableSuspend");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_EnableSuspend, this.m_EnableSuspendLabel, new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(!this.m_EnableSuspend.boolValue || this.m_EnableSuspend.hasMultipleDifferentValues);
			EditorGUILayout.PropertyField(this.m_SuspendThreshold, this.m_SuspendThresholdLabel, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
