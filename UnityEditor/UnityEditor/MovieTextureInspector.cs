using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MovieTexture))]
	internal class MovieTextureInspector : TextureInspector
	{
		private static GUIContent[] s_PlayIcons = new GUIContent[2];

		private static void Init()
		{
			MovieTextureInspector.s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff");
			MovieTextureInspector.s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn");
		}

		protected override void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
		}

		public override void OnPreviewSettings()
		{
			MovieTextureInspector.Init();
			using (new EditorGUI.DisabledScope(Application.isPlaying || base.targets.Length > 1))
			{
				MovieTexture movieTexture = this.target as MovieTexture;
				AudioClip audioClip = movieTexture.audioClip;
				bool flag = PreviewGUI.CycleButton((!movieTexture.isPlaying) ? 0 : 1, MovieTextureInspector.s_PlayIcons) != 0;
				if (flag != movieTexture.isPlaying)
				{
					if (flag)
					{
						movieTexture.Stop();
						movieTexture.Play();
						if (audioClip != null)
						{
							AudioUtil.PlayClip(audioClip);
						}
					}
					else
					{
						movieTexture.Pause();
						if (audioClip != null)
						{
							AudioUtil.PauseClip(audioClip);
						}
					}
				}
			}
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
			MovieTexture movieTexture = this.target as MovieTexture;
			float num = Mathf.Min(Mathf.Min(r.width / (float)movieTexture.width, r.height / (float)movieTexture.height), 1f);
			Rect rect = new Rect(r.x, r.y, (float)movieTexture.width * num, (float)movieTexture.height * num);
			PreviewGUI.BeginScrollView(r, this.m_Pos, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
			EditorGUI.DrawPreviewTexture(rect, movieTexture, null, ScaleMode.StretchToFill);
			this.m_Pos = PreviewGUI.EndScrollView();
			if (movieTexture.isPlaying)
			{
				GUIView.current.Repaint();
			}
			if (Application.isPlaying)
			{
				if (movieTexture.isPlaying)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't pause preview when in play mode");
				}
				else
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't start preview when in play mode");
				}
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			MovieTexture movieTexture = this.target as MovieTexture;
			if (!Application.isPlaying && movieTexture != null)
			{
				AudioClip audioClip = movieTexture.audioClip;
				movieTexture.Stop();
				if (audioClip != null)
				{
					AudioUtil.StopClip(audioClip);
				}
			}
		}

		public override string GetInfoString()
		{
			string text = base.GetInfoString();
			MovieTexture movieTexture = this.target as MovieTexture;
			if (!movieTexture.isReadyToPlay)
			{
				text += "/nNot ready to play yet.";
			}
			return text;
		}
	}
}
