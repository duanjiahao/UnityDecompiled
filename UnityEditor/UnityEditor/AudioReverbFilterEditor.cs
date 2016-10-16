using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioReverbFilter))]
	internal class AudioReverbFilterEditor : Editor
	{
		private SerializedProperty m_ReverbPreset;

		private SerializedProperty m_DryLevel;

		private SerializedProperty m_Room;

		private SerializedProperty m_RoomHF;

		private SerializedProperty m_RoomLF;

		private SerializedProperty m_DecayTime;

		private SerializedProperty m_DecayHFRatio;

		private SerializedProperty m_ReflectionsLevel;

		private SerializedProperty m_ReflectionsDelay;

		private SerializedProperty m_ReverbLevel;

		private SerializedProperty m_ReverbDelay;

		private SerializedProperty m_HFReference;

		private SerializedProperty m_LFReference;

		private SerializedProperty m_Diffusion;

		private SerializedProperty m_Density;

		private void OnEnable()
		{
			this.m_ReverbPreset = base.serializedObject.FindProperty("m_ReverbPreset");
			this.m_DryLevel = base.serializedObject.FindProperty("m_DryLevel");
			this.m_Room = base.serializedObject.FindProperty("m_Room");
			this.m_RoomHF = base.serializedObject.FindProperty("m_RoomHF");
			this.m_RoomLF = base.serializedObject.FindProperty("m_RoomLF");
			this.m_DecayTime = base.serializedObject.FindProperty("m_DecayTime");
			this.m_DecayHFRatio = base.serializedObject.FindProperty("m_DecayHFRatio");
			this.m_ReflectionsLevel = base.serializedObject.FindProperty("m_ReflectionsLevel");
			this.m_ReflectionsDelay = base.serializedObject.FindProperty("m_ReflectionsDelay");
			this.m_ReverbLevel = base.serializedObject.FindProperty("m_ReverbLevel");
			this.m_ReverbDelay = base.serializedObject.FindProperty("m_ReverbDelay");
			this.m_HFReference = base.serializedObject.FindProperty("m_HFReference");
			this.m_LFReference = base.serializedObject.FindProperty("m_LFReference");
			this.m_Diffusion = base.serializedObject.FindProperty("m_Diffusion");
			this.m_Density = base.serializedObject.FindProperty("m_Density");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_ReverbPreset, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				base.serializedObject.SetIsDifferentCacheDirty();
			}
			using (new EditorGUI.DisabledScope(this.m_ReverbPreset.enumValueIndex != 27 || this.m_ReverbPreset.hasMultipleDifferentValues))
			{
				EditorGUILayout.Slider(this.m_DryLevel, -10000f, 0f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Room, -10000f, 0f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_RoomHF, -10000f, 0f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_RoomLF, -10000f, 0f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_DecayTime, 0.1f, 20f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_DecayHFRatio, 0.1f, 2f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReflectionsLevel, -10000f, 1000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReflectionsDelay, 0f, 0.3f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReverbLevel, -10000f, 2000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ReverbDelay, 0f, 0.1f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_HFReference, 1000f, 20000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_LFReference, 20f, 1000f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Diffusion, 0f, 100f, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_Density, 0f, 100f, new GUILayoutOption[0]);
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
