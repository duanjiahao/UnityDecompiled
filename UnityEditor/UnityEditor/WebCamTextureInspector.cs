using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(WebCamTexture))]
	internal class WebCamTextureInspector : Editor
	{
		private static GUIContent[] s_PlayIcons = new GUIContent[2];

		private Vector2 m_Pos;

		public override void OnInspectorGUI()
		{
			WebCamTexture webCamTexture = base.target as WebCamTexture;
			EditorGUILayout.LabelField("Requested FPS", webCamTexture.requestedFPS.ToString(), new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Requested Width", webCamTexture.requestedWidth.ToString(), new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Requested Height", webCamTexture.requestedHeight.ToString(), new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Device Name", webCamTexture.deviceName, new GUILayoutOption[0]);
		}

		private static void Init()
		{
			WebCamTextureInspector.s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff");
			WebCamTextureInspector.s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn");
		}

		public override bool HasPreviewGUI()
		{
			return base.target != null;
		}

		public override void OnPreviewSettings()
		{
			WebCamTextureInspector.Init();
			GUI.enabled = !Application.isPlaying;
			WebCamTexture webCamTexture = base.target as WebCamTexture;
			bool flag = PreviewGUI.CycleButton((!webCamTexture.isPlaying) ? 0 : 1, WebCamTextureInspector.s_PlayIcons) != 0;
			if (flag != webCamTexture.isPlaying)
			{
				if (flag)
				{
					webCamTexture.Stop();
					webCamTexture.Play();
				}
				else
				{
					webCamTexture.Pause();
				}
			}
			GUI.enabled = true;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(r, false, false, false, false);
			}
			WebCamTexture webCamTexture = base.target as WebCamTexture;
			float num = Mathf.Min(Mathf.Min(r.width / (float)webCamTexture.width, r.height / (float)webCamTexture.height), 1f);
			Rect rect = new Rect(r.x, r.y, (float)webCamTexture.width * num, (float)webCamTexture.height * num);
			PreviewGUI.BeginScrollView(r, this.m_Pos, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
			GUI.DrawTexture(rect, webCamTexture, ScaleMode.StretchToFill, false);
			this.m_Pos = PreviewGUI.EndScrollView();
			if (webCamTexture.isPlaying)
			{
				GUIView.current.Repaint();
			}
			if (Application.isPlaying)
			{
				if (webCamTexture.isPlaying)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't pause preview when in play mode");
				}
				else
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't start preview when in play mode");
				}
			}
		}

		public void OnDisable()
		{
			WebCamTexture webCamTexture = base.target as WebCamTexture;
			if (!Application.isPlaying && webCamTexture != null)
			{
				webCamTexture.Stop();
			}
		}

		public override string GetInfoString()
		{
			Texture texture = base.target as Texture;
			string str = texture.width.ToString() + "x" + texture.height.ToString();
			TextureFormat textureFormat = TextureUtil.GetTextureFormat(texture);
			return str + "  " + TextureUtil.GetTextureFormatString(textureFormat);
		}
	}
}
