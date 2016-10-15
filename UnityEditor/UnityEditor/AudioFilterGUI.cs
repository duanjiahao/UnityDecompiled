using System;
using UnityEngine;

namespace UnityEditor
{
	internal class AudioFilterGUI
	{
		private EditorGUI.VUMeter.SmoothingData[] dataOut;

		public void DrawAudioFilterGUI(MonoBehaviour behaviour)
		{
			int customFilterChannelCount = AudioUtil.GetCustomFilterChannelCount(behaviour);
			if (customFilterChannelCount > 0)
			{
				if (this.dataOut == null)
				{
					this.dataOut = new EditorGUI.VUMeter.SmoothingData[customFilterChannelCount];
				}
				double num = (double)AudioUtil.GetCustomFilterProcessTime(behaviour) / 1000000.0;
				float num2 = (float)num / ((float)AudioSettings.outputSampleRate / 1024f / (float)customFilterChannelCount);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(13f);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				EditorGUILayout.Space();
				for (int i = 0; i < customFilterChannelCount; i++)
				{
					EditorGUILayout.VUMeterHorizontal(AudioUtil.GetCustomFilterMaxOut(behaviour, i), ref this.dataOut[i], new GUILayoutOption[]
					{
						GUILayout.MinWidth(50f),
						GUILayout.Height(5f)
					});
				}
				GUILayout.EndVertical();
				Color color = GUI.color;
				GUI.color = new Color(num2, 1f - num2, 0f, 1f);
				GUILayout.Box(string.Format("{0:00.00}ms", num), new GUILayoutOption[]
				{
					GUILayout.MinWidth(40f),
					GUILayout.Height(20f)
				});
				GUI.color = color;
				GUILayout.EndHorizontal();
				EditorGUILayout.Space();
				GUIView.current.Repaint();
			}
		}
	}
}
