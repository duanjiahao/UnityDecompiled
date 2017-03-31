using System;
using UnityEngine;
using UnityEngine.Video;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(VideoClip))]
	internal class VideoClipInspector : Editor
	{
		private static readonly GUID kEmptyGUID;

		private VideoClip m_PlayingClip;

		private GUID m_PreviewID;

		private Vector2 m_Position = Vector2.zero;

		public override void OnInspectorGUI()
		{
		}

		private static void Init()
		{
		}

		public void OnDisable()
		{
		}

		public void OnEnable()
		{
		}

		public void OnDestroy()
		{
			this.StopPreview();
		}

		public override bool HasPreviewGUI()
		{
			return base.targets != null;
		}

		private void PlayPreview()
		{
			this.m_PreviewID = VideoUtil.StartPreview(this.m_PlayingClip);
			VideoUtil.PlayPreview(this.m_PreviewID, true);
		}

		private void StopPreview()
		{
			if (!this.m_PreviewID.Empty())
			{
				VideoUtil.StopPreview(this.m_PreviewID);
			}
			this.m_PlayingClip = null;
			this.m_PreviewID = VideoClipInspector.kEmptyGUID;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			VideoClip videoClip = base.target as VideoClip;
			Event current = Event.current;
			if (current.type != EventType.Repaint && current.type != EventType.Layout && current.type != EventType.Used)
			{
				EventType type = current.type;
				if (type == EventType.MouseDown)
				{
					if (r.Contains(current.mousePosition))
					{
						if (this.m_PlayingClip != null)
						{
							if (this.m_PreviewID.Empty() || !VideoUtil.IsPreviewPlaying(this.m_PreviewID))
							{
								this.PlayPreview();
							}
							else
							{
								this.StopPreview();
							}
						}
						current.Use();
					}
				}
			}
			else
			{
				bool flag = true;
				bool flag2 = videoClip != this.m_PlayingClip || (!this.m_PreviewID.Empty() && VideoUtil.IsPreviewPlaying(this.m_PreviewID));
				if (videoClip != this.m_PlayingClip)
				{
					this.StopPreview();
					this.m_PlayingClip = videoClip;
				}
				Texture texture = null;
				if (!this.m_PreviewID.Empty())
				{
					texture = VideoUtil.GetPreviewTexture(this.m_PreviewID);
				}
				if (texture == null || texture.width == 0 || texture.height == 0)
				{
					texture = this.GetAssetPreviewTexture();
					flag = false;
				}
				if (!(texture == null) && texture.width != 0 && texture.height != 0)
				{
					if (Event.current.type == EventType.Repaint)
					{
						background.Draw(r, false, false, false, false);
					}
					float num = 1f;
					float num2 = 1f;
					float num3 = Mathf.Min(new float[]
					{
						num * r.width / (float)texture.width,
						num2 * r.height / (float)texture.height,
						num,
						num2
					});
					Rect rect = (!flag) ? r : new Rect(r.x, r.y, (float)texture.width * num3, (float)texture.height * num3);
					PreviewGUI.BeginScrollView(r, this.m_Position, rect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
					if (flag)
					{
						EditorGUI.DrawTextureTransparent(rect, texture, ScaleMode.StretchToFill);
					}
					else
					{
						GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
					}
					this.m_Position = PreviewGUI.EndScrollView();
					if (flag2)
					{
						GUIView.current.Repaint();
					}
				}
			}
		}

		private Texture GetAssetPreviewTexture()
		{
			bool flag = AssetPreview.IsLoadingAssetPreview(base.target.GetInstanceID());
			Texture texture = AssetPreview.GetAssetPreview(base.target);
			if (!texture)
			{
				if (flag)
				{
					GUIView.current.Repaint();
				}
				texture = AssetPreview.GetMiniThumbnail(base.target);
			}
			return texture;
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			GUI.DrawTexture(iconRect, this.GetAssetPreviewTexture(), ScaleMode.StretchToFill);
		}

		public override string GetInfoString()
		{
			VideoClip videoClip = base.target as VideoClip;
			ulong frameCount = videoClip.frameCount;
			double frameRate = videoClip.frameRate;
			string arg_5E_0;
			if (frameRate > 0.0)
			{
				arg_5E_0 = TimeSpan.FromSeconds(frameCount / frameRate).ToString();
			}
			else
			{
				TimeSpan timeSpan = new TimeSpan(0L);
				arg_5E_0 = timeSpan.ToString();
			}
			string text = arg_5E_0;
			if (text.IndexOf('.') != -1)
			{
				text = text.Substring(0, text.Length - 4);
			}
			string text2 = text;
			text2 = text2 + ", " + frameCount.ToString() + " frames";
			text2 = text2 + ", " + frameRate.ToString("F2") + " FPS";
			string text3 = text2;
			return string.Concat(new string[]
			{
				text3,
				", ",
				videoClip.width.ToString(),
				"x",
				videoClip.height.ToString()
			});
		}
	}
}
