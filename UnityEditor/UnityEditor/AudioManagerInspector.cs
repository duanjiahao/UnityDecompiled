using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AudioManager))]
	internal class AudioManagerInspector : Editor
	{
		private class Styles
		{
			public static GUIContent Volume = EditorGUIUtility.TextContent("Global Volume|Initial volume multiplier (AudioListener.volume)");

			public static GUIContent RolloffScale = EditorGUIUtility.TextContent("Volume Rolloff Scale|Global volume rolloff multiplier (applies only to logarithmic volume curves).");

			public static GUIContent DopplerFactor = EditorGUIUtility.TextContent("Doppler Factor|Global Doppler speed multiplier for sounds in motion.");

			public static GUIContent DefaultSpeakerMode = EditorGUIUtility.TextContent("Default Speaker Mode|Speaker mode at start of the game. This may be changed at runtime using the AudioSettings.Reset function.");

			public static GUIContent SampleRate = EditorGUIUtility.TextContent("System Sample Rate|Sample rate at which the output device of the audio system runs. Individual sounds may run at different sample rates and will be slowed down/sped up accordingly to match the output rate.");

			public static GUIContent DSPBufferSize = EditorGUIUtility.TextContent("DSP Buffer Size|Length of mixing buffer. This determines the output latency of the game.");

			public static GUIContent VirtualVoiceCount = EditorGUIUtility.TextContent("Max Virtual Voices|Maximum number of sounds managed by the system. Even though at most RealVoiceCount of the loudest sounds will be physically playing, the remaining sounds will still be updating their play position.");

			public static GUIContent RealVoiceCount = EditorGUIUtility.TextContent("Max Real Voices|Maximum number of actual simultanously playing sounds.");

			public static GUIContent SpatializerPlugin = EditorGUIUtility.TextContent("Spatializer Plugin|Native audio plugin performing spatialized filtering of 3D sources.");

			public static GUIContent DisableAudio = EditorGUIUtility.TextContent("Disable Unity Audio|Prevent allocating the output device in the runtime. Use this if you want to use other sound systems than the built-in one.");

			public static GUIContent VirtualizeEffects = EditorGUIUtility.TextContent("Virtualize Effects|When enabled dynamically turn off effects and spatializers on AudioSources that are culled in order to save CPU.");
		}

		private SerializedProperty m_Volume;

		private SerializedProperty m_RolloffScale;

		private SerializedProperty m_DopplerFactor;

		private SerializedProperty m_DefaultSpeakerMode;

		private SerializedProperty m_SampleRate;

		private SerializedProperty m_DSPBufferSize;

		private SerializedProperty m_VirtualVoiceCount;

		private SerializedProperty m_RealVoiceCount;

		private SerializedProperty m_SpatializerPlugin;

		private SerializedProperty m_DisableAudio;

		private SerializedProperty m_VirtualizeEffects;

		private void OnEnable()
		{
			this.m_Volume = base.serializedObject.FindProperty("m_Volume");
			this.m_RolloffScale = base.serializedObject.FindProperty("Rolloff Scale");
			this.m_DopplerFactor = base.serializedObject.FindProperty("Doppler Factor");
			this.m_DefaultSpeakerMode = base.serializedObject.FindProperty("Default Speaker Mode");
			this.m_SampleRate = base.serializedObject.FindProperty("m_SampleRate");
			this.m_DSPBufferSize = base.serializedObject.FindProperty("m_DSPBufferSize");
			this.m_VirtualVoiceCount = base.serializedObject.FindProperty("m_VirtualVoiceCount");
			this.m_RealVoiceCount = base.serializedObject.FindProperty("m_RealVoiceCount");
			this.m_SpatializerPlugin = base.serializedObject.FindProperty("m_SpatializerPlugin");
			this.m_DisableAudio = base.serializedObject.FindProperty("m_DisableAudio");
			this.m_VirtualizeEffects = base.serializedObject.FindProperty("m_VirtualizeEffects");
		}

		private int FindPluginStringIndex(string[] strs, string element)
		{
			for (int i = 1; i < strs.Length; i++)
			{
				if (element == strs[i])
				{
					return i;
				}
			}
			return 0;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Volume, AudioManagerInspector.Styles.Volume, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_RolloffScale, AudioManagerInspector.Styles.RolloffScale, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_DopplerFactor, AudioManagerInspector.Styles.DopplerFactor, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_DefaultSpeakerMode, AudioManagerInspector.Styles.DefaultSpeakerMode, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SampleRate, AudioManagerInspector.Styles.SampleRate, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_DSPBufferSize, AudioManagerInspector.Styles.DSPBufferSize, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_VirtualVoiceCount, AudioManagerInspector.Styles.VirtualVoiceCount, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_RealVoiceCount, AudioManagerInspector.Styles.RealVoiceCount, new GUILayoutOption[0]);
			List<string> list = new List<string>(AudioUtil.GetSpatializerPluginNames());
			list.Insert(0, "None");
			string[] array = list.ToArray();
			List<GUIContent> list2 = new List<GUIContent>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				list2.Add(new GUIContent(text));
			}
			EditorGUI.BeginChangeCheck();
			int num = this.FindPluginStringIndex(array, this.m_SpatializerPlugin.stringValue);
			num = EditorGUILayout.Popup(AudioManagerInspector.Styles.SpatializerPlugin, num, list2.ToArray(), new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				if (num == 0)
				{
					this.m_SpatializerPlugin.stringValue = string.Empty;
				}
				else
				{
					this.m_SpatializerPlugin.stringValue = array[num];
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_DisableAudio, AudioManagerInspector.Styles.DisableAudio, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_VirtualizeEffects, AudioManagerInspector.Styles.VirtualizeEffects, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
