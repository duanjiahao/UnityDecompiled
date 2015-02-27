using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioClip))]
	internal class AudioClipInspector : Editor
	{
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
			AudioUtil.ClearWaveForm(this.target as AudioClip);
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
		public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
		{
			AudioImporter audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;
			if (!audioImporter)
			{
				return null;
			}
			AudioClip audioClip = this.target as AudioClip;
			Texture2D[] array = new Texture2D[audioClip.channels];
			for (int i = 0; i < audioClip.channels; i++)
			{
				array[i] = AudioUtil.GetWaveForm(audioClip, audioImporter, i, (float)width, (float)(height / audioClip.channels));
			}
			return AudioClipInspector.CombineWaveForms(array);
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
			EditorGUI.BeginDisabledGroup(AudioUtil.IsMovieAudio(audioClip));
			bool flag = base.targets.Length > 1;
			EditorGUI.BeginDisabledGroup(flag);
			AudioClipInspector.m_bAutoPlay = (!flag && AudioClipInspector.m_bAutoPlay);
			AudioClipInspector.m_bAutoPlay = (PreviewGUI.CycleButton((!AudioClipInspector.m_bAutoPlay) ? 0 : 1, AudioClipInspector.s_AutoPlayIcons) != 0);
			EditorGUI.EndDisabledGroup();
			bool bLoop = AudioClipInspector.m_bLoop;
			AudioClipInspector.m_bLoop = (PreviewGUI.CycleButton((!AudioClipInspector.m_bLoop) ? 0 : 1, AudioClipInspector.s_LoopIcons) != 0);
			if (bLoop != AudioClipInspector.m_bLoop && this.playing)
			{
				AudioUtil.LoopClip(audioClip, AudioClipInspector.m_bLoop);
			}
			EditorGUI.BeginDisabledGroup(flag && !this.playing);
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
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
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
			if (!AudioUtil.HasPreview(audioClip) && (AudioUtil.IsMOD(audioClip) || AudioUtil.IsMovieAudio(audioClip)))
			{
				float num3 = (r.height <= 150f) ? (r.y + r.height / 2f - 25f) : (r.y + r.height / 2f - 10f);
				if (r.width > 64f)
				{
					if (AudioUtil.IsMOD(audioClip))
					{
						EditorGUI.DropShadowLabel(new Rect(r.x, num3, r.width, 20f), string.Format("Module file with " + AudioUtil.GetMODChannelCount(audioClip) + " channels.", new object[0]));
					}
					else
					{
						if (AudioUtil.IsMovieAudio(audioClip))
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
				Texture2D texture2D;
				if (r.width < 100f)
				{
					texture2D = AssetPreview.GetAssetPreview(audioClip);
				}
				else
				{
					texture2D = AudioUtil.GetWaveFormFast(audioClip, 1, 0, audioClip.samples, r.width, r.height);
				}
				if (texture2D == null)
				{
					GUI.DrawTexture(new Rect
					{
						x = (AudioClipInspector.m_wantedRect.width - (float)AudioClipInspector.s_DefaultIcon.width) / 2f + AudioClipInspector.m_wantedRect.x,
						y = (AudioClipInspector.m_wantedRect.height - (float)AudioClipInspector.s_DefaultIcon.height) / 2f + AudioClipInspector.m_wantedRect.y,
						width = (float)AudioClipInspector.s_DefaultIcon.width,
						height = (float)AudioClipInspector.s_DefaultIcon.height
					}, AudioClipInspector.s_DefaultIcon);
					base.Repaint();
				}
				else
				{
					GUI.DrawTexture(new Rect(AudioClipInspector.m_wantedRect.x, AudioClipInspector.m_wantedRect.y, AudioClipInspector.m_wantedRect.width, AudioClipInspector.m_wantedRect.height), texture2D);
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
		private static Texture2D CombineWaveForms(Texture2D[] waveForms)
		{
			if (waveForms.Length == 1)
			{
				return waveForms[0];
			}
			int width = waveForms[0].width;
			int num = 0;
			for (int i = 0; i < waveForms.Length; i++)
			{
				Texture2D texture2D = waveForms[i];
				num += texture2D.height;
			}
			Texture2D texture2D2 = new Texture2D(width, num, TextureFormat.ARGB32, false);
			int num2 = 0;
			for (int j = 0; j < waveForms.Length; j++)
			{
				Texture2D texture2D3 = waveForms[j];
				num2 += texture2D3.height;
				texture2D2.SetPixels(0, num - num2, width, texture2D3.height, texture2D3.GetPixels());
				UnityEngine.Object.DestroyImmediate(texture2D3);
			}
			texture2D2.Apply();
			return texture2D2;
		}
		public override string GetInfoString()
		{
			AudioClip clip = this.target as AudioClip;
			int channelCount = AudioUtil.GetChannelCount(clip);
			string text = (channelCount != 1) ? ((channelCount != 2) ? ((channelCount - 1).ToString() + ".1") : "Stereo") : "Mono";
			string text2 = string.Empty;
			if (AudioUtil.GetClipType(clip) != AudioType.MPEG)
			{
				text2 = string.Concat(new object[]
				{
					AudioUtil.GetBitsPerSample(clip),
					" bit, ",
					AudioUtil.GetFrequency(clip),
					" Hz, ",
					text,
					", "
				});
			}
			else
			{
				text2 = string.Concat(new object[]
				{
					AudioUtil.GetFrequency(clip),
					" Hz, ",
					text,
					", "
				});
			}
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int)AudioUtil.GetDuration(clip));
			if ((uint)AudioUtil.GetDuration(clip) == 4294967295u)
			{
				text2 += "Unlimited";
			}
			else
			{
				text2 += string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
			}
			text2 += ", ";
			text2 += EditorUtility.FormatBytes(AudioUtil.GetSoundSize(clip));
			string text3 = text2;
			return string.Concat(new object[]
			{
				text3,
				" (",
				AudioUtil.GetClipType(clip),
				")"
			});
		}
	}
}
