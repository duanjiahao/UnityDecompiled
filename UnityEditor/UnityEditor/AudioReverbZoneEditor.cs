using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioReverbZone))]
	internal class AudioReverbZoneEditor : Editor
	{
		private SerializedProperty m_MinDistance;

		private SerializedProperty m_MaxDistance;

		private SerializedProperty m_ReverbPreset;

		private SerializedProperty m_Room;

		private SerializedProperty m_RoomHF;

		private SerializedProperty m_RoomLF;

		private SerializedProperty m_DecayTime;

		private SerializedProperty m_DecayHFRatio;

		private SerializedProperty m_Reflections;

		private SerializedProperty m_ReflectionsDelay;

		private SerializedProperty m_Reverb;

		private SerializedProperty m_ReverbDelay;

		private SerializedProperty m_HFReference;

		private SerializedProperty m_LFReference;

		private SerializedProperty m_RoomRolloffFactor;

		private SerializedProperty m_Diffusion;

		private SerializedProperty m_Density;

		private void OnEnable()
		{
			this.m_MinDistance = base.serializedObject.FindProperty("m_MinDistance");
			this.m_MaxDistance = base.serializedObject.FindProperty("m_MaxDistance");
			this.m_ReverbPreset = base.serializedObject.FindProperty("m_ReverbPreset");
			this.m_Room = base.serializedObject.FindProperty("m_Room");
			this.m_RoomHF = base.serializedObject.FindProperty("m_RoomHF");
			this.m_RoomLF = base.serializedObject.FindProperty("m_RoomLF");
			this.m_DecayTime = base.serializedObject.FindProperty("m_DecayTime");
			this.m_DecayHFRatio = base.serializedObject.FindProperty("m_DecayHFRatio");
			this.m_Reflections = base.serializedObject.FindProperty("m_Reflections");
			this.m_ReflectionsDelay = base.serializedObject.FindProperty("m_ReflectionsDelay");
			this.m_Reverb = base.serializedObject.FindProperty("m_Reverb");
			this.m_ReverbDelay = base.serializedObject.FindProperty("m_ReverbDelay");
			this.m_HFReference = base.serializedObject.FindProperty("m_HFReference");
			this.m_LFReference = base.serializedObject.FindProperty("m_LFReference");
			this.m_RoomRolloffFactor = base.serializedObject.FindProperty("m_RoomRolloffFactor");
			this.m_Diffusion = base.serializedObject.FindProperty("m_Diffusion");
			this.m_Density = base.serializedObject.FindProperty("m_Density");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_MinDistance, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MaxDistance, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_ReverbPreset, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				base.serializedObject.SetIsDifferentCacheDirty();
			}
			using (new EditorGUI.DisabledScope(this.m_ReverbPreset.enumValueIndex != 27 || this.m_ReverbPreset.hasMultipleDifferentValues))
			{
				EditorGUILayout.IntSlider(this.m_Room, -10000, 0, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(this.m_RoomHF, -10000, 0, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(this.m_RoomLF, -10000, 0, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_DecayTime, 0.1f, 20f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_DecayHFRatio, 0.1f, 2f, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(this.m_Reflections, -10000, 1000, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReflectionsDelay, 0f, 0.3f, new GUILayoutOption[0]);
				EditorGUILayout.IntSlider(this.m_Reverb, -10000, 2000, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReverbDelay, 0f, 0.1f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_HFReference, 1000f, 20000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_LFReference, 20f, 1000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_RoomRolloffFactor, 0f, 10f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Diffusion, 0f, 100f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Density, 0f, 100f, new GUILayoutOption[0]);
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		private void OnSceneGUI()
		{
			AudioReverbZone audioReverbZone = (AudioReverbZone)this.target;
			Color color = Handles.color;
			if (audioReverbZone.enabled)
			{
				Handles.color = new Color(0.5f, 0.7f, 1f, 0.5f);
			}
			else
			{
				Handles.color = new Color(0.3f, 0.4f, 0.6f, 0.5f);
			}
			Vector3 position = audioReverbZone.transform.position;
			EditorGUI.BeginChangeCheck();
			float minDistance = Handles.RadiusHandle(Quaternion.identity, position, audioReverbZone.minDistance, true);
			float maxDistance = Handles.RadiusHandle(Quaternion.identity, position, audioReverbZone.maxDistance, true);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(audioReverbZone, "Reverb Distance");
				audioReverbZone.minDistance = minDistance;
				audioReverbZone.maxDistance = maxDistance;
			}
			Handles.color = color;
		}
	}
}
