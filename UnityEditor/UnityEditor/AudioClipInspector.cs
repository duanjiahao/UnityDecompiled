using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioClip))]
	internal class AudioClipInspector : Editor
	{
		private PreviewRenderUtility m_PreviewUtility;

		private AudioClip m_PlayingClip;

		private Vector2 m_Position = Vector2.zero;

		private GUIView m_GUI;

		private static GUIStyle s_PreButton;

		private static Rect m_wantedRect;

		private static bool m_bAutoPlay;

		private static bool m_bLoop = false;

		private static bool m_bPlayFirst;

		private static GUIContent[] s_PlayIcons = new GUIContent[2];

		private static GUIContent[] s_AutoPlayIcons = new GUIContent[2];

		private static GUIContent[] s_LoopIcons = new GUIContent[2];

		private static Texture2D s_DefaultIcon;

		private bool playing
		{
			get
			{
				return this.m_PlayingClip != null;
			}
		}

		public override void OnInspectorGUI()
		{
		}

		private static void Init()
		{
			if (AudioClipInspector.s_PreButton != null)
			{
				return;
			}
			AudioClipInspector.s_PreButton = "preButton";
			AudioClipInspector.m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
			AudioClipInspector.m_bLoop = false;
			AudioClipInspector.s_AutoPlayIcons[0] = EditorGUIUtility.IconContent("preAudioAutoPlayOff", "Turn Auto Play on");
			AudioClipInspector.s_AutoPlayIcons[1] = EditorGUIUtility.IconContent("preAudioAutoPlayOn", "Turn Auto Play off");
			AudioClipInspector.s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff", "Play");
			AudioClipInspector.s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn", "Stop");
			AudioClipInspector.s_LoopIcons[0] = EditorGUIUtility.IconContent("preAudioLoopOff", "Loop on");
			AudioClipInspector.s_LoopIcons[1] = EditorGUIUtility.IconContent("preAudioLoopOn", "Loop off");
			AudioClipInspector.s_DefaultIcon = EditorGUIUtility.LoadIcon("Profiler.Audio");
		}

		public void OnDisable()
		{
			AudioUtil.StopAllClips();
			EditorPrefs.SetBool("AutoPlayAudio", AudioClipInspector.m_bAutoPlay);
		}

		public void OnEnable()
		{
			AudioClipInspector.m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
			if (AudioClipInspector.m_bAutoPlay)
			{
				AudioClipInspector.m_bPlayFirst = true;
			}
		}

		public void OnDestroy()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
		}

		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			AudioClip clip = this.target as AudioClip;
			AssetImporter atPath = AssetImporter.GetAtPath(assetPath);
			AudioImporter audioImporter = atPath as AudioImporter;
			if (audioImporter == null || !ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				return null;
			}
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
			}
			this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float)width, (float)height));
			this.DoRenderPreview(clip, audioImporter, new Rect(0.05f * (float)width * EditorGUIUtility.pixelsPerPoint, 0.05f * (float)width * EditorGUIUtility.pixelsPerPoint, 1.9f * (float)width * EditorGUIUtility.pixelsPerPoint, 1.9f * (float)height * EditorGUIUtility.pixelsPerPoint), 1f);
			return this.m_PreviewUtility.EndStaticPreview();
		}

		public override bool HasPreviewGUI()
		{
			return base.targets != null;
		}

		public override void OnPreviewSettings()
		{
			if (AudioClipInspector.s_DefaultIcon == null)
			{
				AudioClipInspector.Init();
			}
			AudioClip audioClip = this.target as AudioClip;
			using (new EditorGUI.DisabledScope(AudioUtil.IsMovieAudio(audioClip)))
			{
				bool flag = base.targets.Length > 1;
				using (new EditorGUI.DisabledScope(flag))
				{
					AudioClipInspector.m_bAutoPlay = (!flag && AudioClipInspector.m_bAutoPlay);
					AudioClipInspector.m_bAutoPlay = (PreviewGUI.CycleButton((!AudioClipInspector.m_bAutoPlay) ? 0 : 1, AudioClipInspector.s_AutoPlayIcons) != 0);
				}
				bool bLoop = AudioClipInspector.m_bLoop;
				AudioClipInspector.m_bLoop = (PreviewGUI.CycleButton((!AudioClipInspector.m_bLoop) ? 0 : 1, AudioClipInspector.s_LoopIcons) != 0);
				if (bLoop != AudioClipInspector.m_bLoop && this.playing)
				{
					AudioUtil.LoopClip(audioClip, AudioClipInspector.m_bLoop);
				}
				using (new EditorGUI.DisabledScope(flag && !this.playing))
				{
					bool flag2 = PreviewGUI.CycleButton((!this.playing) ? 0 : 1, AudioClipInspector.s_PlayIcons) != 0;
					if (flag2 != this.playing)
					{
						if (flag2)
						{
							AudioUtil.PlayClip(audioClip, 0, AudioClipInspector.m_bLoop);
							this.m_PlayingClip = audioClip;
						}
						else
						{
							AudioUtil.StopAllClips();
							this.m_PlayingClip = null;
						}
					}
				}
			}
		}

		private void DoRenderPreview(AudioClip clip, AudioImporter audioImporter, Rect wantedRect, float scaleFactor)
		{
			scaleFactor *= 0.95f;
			float[] minMaxData = (!(audioImporter == null)) ? AudioUtil.GetMinMaxData(audioImporter) : null;
			int numChannels = clip.channels;
			int numSamples = (minMaxData != null) ? (minMaxData.Length / (2 * numChannels)) : 0;
			float num = wantedRect.height / (float)numChannels;
			int channel;
			for (channel = 0; channel < numChannels; channel++)
			{
				Rect r = new Rect(wantedRect.x, wantedRect.y + num * (float)channel, wantedRect.width, num);
				Color curveColor = new Color(1f, 0.549019635f, 0f, 1f);
				AudioCurveRendering.DrawMinMaxFilledCurve(r, delegate(float x, out Color col, out float minValue, out float maxValue)
				{
					col = curveColor;
					if (numSamples <= 0)
					{
						minValue = 0f;
						maxValue = 0f;
					}
					else
					{
						float f = Mathf.Clamp(x * (float)(numSamples - 2), 0f, (float)(numSamples - 2));
						int num2 = (int)Mathf.Floor(f);
						int num3 = (num2 * numChannels + channel) * 2;
						int num4 = num3 + numChannels * 2;
						minValue = Mathf.Min(minMaxData[num3 + 1], minMaxData[num4 + 1]) * scaleFactor;
						maxValue = Mathf.Max(minMaxData[num3], minMaxData[num4]) * scaleFactor;
						if (minValue > maxValue)
						{
							float num5 = minValue;
							minValue = maxValue;
							maxValue = num5;
						}
					}
				});
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (AudioClipInspector.s_DefaultIcon == null)
			{
				AudioClipInspector.Init();
			}
			AudioClip audioClip = this.target as AudioClip;
			Event current = Event.current;
			if (current.type != EventType.Repaint && current.type != EventType.Layout && current.type != EventType.Used)
			{
				int num = AudioUtil.GetSampleCount(audioClip) / (int)r.width;
				switch (current.type)
				{
				case EventType.MouseDown:
				case EventType.MouseDrag:
					if (r.Contains(current.mousePosition) && !AudioUtil.IsMovieAudio(audioClip))
					{
						if (this.m_PlayingClip != audioClip)
						{
							AudioUtil.StopAllClips();
							AudioUtil.PlayClip(audioClip, 0, AudioClipInspector.m_bLoop);
							this.m_PlayingClip = audioClip;
						}
						AudioUtil.SetClipSamplePosition(audioClip, num * (int)current.mousePosition.x);
						current.Use();
					}
					break;
				}
				return;
			}
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
			int channelCount = AudioUtil.GetChannelCount(audioClip);
			AudioClipInspector.m_wantedRect = new Rect(r.x, r.y, r.width, r.height);
			float num2 = AudioClipInspector.m_wantedRect.width / audioClip.length;
			if (!AudioUtil.HasPreview(audioClip) && (AudioUtil.IsTrackerFile(audioClip) || AudioUtil.IsMovieAudio(audioClip)))
			{
				float num3 = (r.height <= 150f) ? (r.y + r.height / 2f - 25f) : (r.y + r.height / 2f - 10f);
				if (r.width > 64f)
				{
					if (AudioUtil.IsTrackerFile(audioClip))
					{
						EditorGUI.DropShadowLabel(new Rect(r.x, num3, r.width, 20f), string.Format("Module file with " + AudioUtil.GetMusicChannelCount(audioClip) + " channels.", new object[0]));
					}
					else if (AudioUtil.IsMovieAudio(audioClip))
					{
						if (r.width > 450f)
						{
							EditorGUI.DropShadowLabel(new Rect(r.x, num3, r.width, 20f), "Audio is attached to a movie. To audition the sound, play the movie.");
						}
						else
						{
							EditorGUI.DropShadowLabel(new Rect(r.x, num3, r.width, 20f), "Audio is attached to a movie.");
							EditorGUI.DropShadowLabel(new Rect(r.x, num3 + 10f, r.width, 20f), "To audition the sound, play the movie.");
						}
					}
					else
					{
						EditorGUI.DropShadowLabel(new Rect(r.x, num3, r.width, 20f), "Can not show PCM data for this file");
					}
				}
				if (this.m_PlayingClip == audioClip)
				{
					float clipPosition = AudioUtil.GetClipPosition(audioClip);
					TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)(clipPosition * 1000f));
					EditorGUI.DropShadowLabel(new Rect(AudioClipInspector.m_wantedRect.x, AudioClipInspector.m_wantedRect.y, AudioClipInspector.m_wantedRect.width, 20f), string.Format("Playing - {0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
				}
			}
			else
			{
				PreviewGUI.BeginScrollView(AudioClipInspector.m_wantedRect, this.m_Position, AudioClipInspector.m_wantedRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
				if (Event.current.type == EventType.Repaint)
				{
					this.DoRenderPreview(audioClip, AudioUtil.GetImporterFromClip(audioClip), AudioClipInspector.m_wantedRect, 1f);
				}
				for (int i = 0; i < channelCount; i++)
				{
					if (channelCount > 1 && r.width > 64f)
					{
						Rect position = new Rect(AudioClipInspector.m_wantedRect.x + 5f, AudioClipInspector.m_wantedRect.y + AudioClipInspector.m_wantedRect.height / (float)channelCount * (float)i, 30f, 20f);
						EditorGUI.DropShadowLabel(position, "ch " + (i + 1).ToString());
					}
				}
				if (this.m_PlayingClip == audioClip)
				{
					float clipPosition2 = AudioUtil.GetClipPosition(audioClip);
					TimeSpan timeSpan2 = new TimeSpan(0, 0, 0, 0, (int)(clipPosition2 * 1000f));
					GUI.DrawTexture(new Rect(AudioClipInspector.m_wantedRect.x + (float)((int)(num2 * clipPosition2)), AudioClipInspector.m_wantedRect.y, 2f, AudioClipInspector.m_wantedRect.height), EditorGUIUtility.whiteTexture);
					if (r.width > 64f)
					{
						EditorGUI.DropShadowLabel(new Rect(AudioClipInspector.m_wantedRect.x, AudioClipInspector.m_wantedRect.y, AudioClipInspector.m_wantedRect.width, 20f), string.Format("{0:00}:{1:00}.{2:000}", timeSpan2.Minutes, timeSpan2.Seconds, timeSpan2.Milliseconds));
					}
					else
					{
						EditorGUI.DropShadowLabel(new Rect(AudioClipInspector.m_wantedRect.x, AudioClipInspector.m_wantedRect.y, AudioClipInspector.m_wantedRect.width, 20f), string.Format("{0:00}:{1:00}", timeSpan2.Minutes, timeSpan2.Seconds));
					}
					if (!AudioUtil.IsClipPlaying(audioClip))
					{
						this.m_PlayingClip = null;
					}
				}
				PreviewGUI.EndScrollView();
			}
			if (AudioClipInspector.m_bPlayFirst)
			{
				AudioUtil.PlayClip(audioClip, 0, AudioClipInspector.m_bLoop);
				this.m_PlayingClip = audioClip;
				AudioClipInspector.m_bPlayFirst = false;
			}
			if (this.playing)
			{
				GUIView.current.Repaint();
			}
		}

		public override string GetInfoString()
		{
			AudioClip clip = this.target as AudioClip;
			int channelCount = AudioUtil.GetChannelCount(clip);
			string text = (channelCount != 1) ? ((channelCount != 2) ? ((channelCount - 1).ToString() + ".1") : "Stereo") : "Mono";
			AudioCompressionFormat targetPlatformSoundCompressionFormat = AudioUtil.GetTargetPlatformSoundCompressionFormat(clip);
			AudioCompressionFormat soundCompressionFormat = AudioUtil.GetSoundCompressionFormat(clip);
			string text2 = targetPlatformSoundCompressionFormat.ToString();
			if (targetPlatformSoundCompressionFormat != soundCompressionFormat)
			{
				text2 = text2 + " (" + soundCompressionFormat.ToString() + " in editor)";
			}
			string text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				", ",
				AudioUtil.GetFrequency(clip),
				" Hz, ",
				text,
				", "
			});
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)AudioUtil.GetDuration(clip));
			if ((uint)AudioUtil.GetDuration(clip) == 4294967295u)
			{
				text2 += "Unlimited";
			}
			else
			{
				text2 += string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
			}
			return text2;
		}
	}
}
