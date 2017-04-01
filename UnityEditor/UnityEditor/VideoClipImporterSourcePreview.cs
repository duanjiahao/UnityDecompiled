using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomPreview(typeof(VideoClipImporter))]
	internal class VideoClipImporterSourcePreview : ObjectPreview
	{
		private class Styles
		{
			public GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

			public Styles()
			{
				Color textColor = new Color(0.7f, 0.7f, 0.7f);
				this.labelStyle.padding.right += 4;
				this.labelStyle.normal.textColor = textColor;
			}
		}

		private VideoClipImporterSourcePreview.Styles m_Styles = new VideoClipImporterSourcePreview.Styles();

		private GUIContent m_Title;

		private const float kLabelWidth = 120f;

		private const float kIndentWidth = 30f;

		private const float kValueWidth = 200f;

		public override GUIContent GetPreviewTitle()
		{
			if (this.m_Title == null)
			{
				this.m_Title = new GUIContent("Source Info");
			}
			return this.m_Title;
		}

		public override bool HasPreviewGUI()
		{
			VideoClipImporter videoClipImporter = this.target as VideoClipImporter;
			return videoClipImporter != null && !videoClipImporter.useLegacyImporter;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (Event.current.type == EventType.Repaint)
			{
				VideoClipImporter videoClipImporter = (VideoClipImporter)this.target;
				RectOffset rectOffset = new RectOffset(-5, -5, -5, -5);
				r = rectOffset.Add(r);
				r.height = EditorGUIUtility.singleLineHeight;
				Rect rect = r;
				Rect rect2 = r;
				rect.width = 120f;
				rect2.xMin += 120f;
				rect2.width = 200f;
				this.ShowProperty(ref rect, ref rect2, "Original Size", EditorUtility.FormatBytes((long)videoClipImporter.sourceFileSize));
				this.ShowProperty(ref rect, ref rect2, "Imported Size", EditorUtility.FormatBytes((long)videoClipImporter.outputFileSize));
				int frameCount = videoClipImporter.frameCount;
				double frameRate = videoClipImporter.frameRate;
				string arg_FD_0;
				if (frameRate > 0.0)
				{
					arg_FD_0 = TimeSpan.FromSeconds((double)frameCount / frameRate).ToString();
				}
				else
				{
					TimeSpan timeSpan = new TimeSpan(0L);
					arg_FD_0 = timeSpan.ToString();
				}
				string text = arg_FD_0;
				if (text.IndexOf('.') != -1)
				{
					text = text.Substring(0, text.Length - 4);
				}
				this.ShowProperty(ref rect, ref rect2, "Duration", text);
				this.ShowProperty(ref rect, ref rect2, "Frames", frameCount.ToString());
				this.ShowProperty(ref rect, ref rect2, "FPS", frameRate.ToString("F2"));
				int resizeWidth = videoClipImporter.GetResizeWidth(VideoResizeMode.OriginalSize);
				int resizeHeight = videoClipImporter.GetResizeHeight(VideoResizeMode.OriginalSize);
				this.ShowProperty(ref rect, ref rect2, "Pixels", resizeWidth + "x" + resizeHeight);
				this.ShowProperty(ref rect, ref rect2, "Alpha", (!videoClipImporter.sourceHasAlpha) ? "No" : "Yes");
				ushort sourceAudioTrackCount = videoClipImporter.sourceAudioTrackCount;
				this.ShowProperty(ref rect, ref rect2, "Audio", (sourceAudioTrackCount != 0) ? ((sourceAudioTrackCount != 1) ? "" : this.GetAudioTrackDescription(videoClipImporter, 0)) : "none");
				if (sourceAudioTrackCount > 1)
				{
					rect.xMin += 30f;
					rect.width -= 30f;
					for (ushort num = 0; num < sourceAudioTrackCount; num += 1)
					{
						this.ShowProperty(ref rect, ref rect2, "Track #" + (int)(num + 1), this.GetAudioTrackDescription(videoClipImporter, num));
					}
				}
			}
		}

		private string GetAudioTrackDescription(VideoClipImporter importer, ushort audioTrackIdx)
		{
			ushort sourceAudioChannelCount = importer.GetSourceAudioChannelCount(audioTrackIdx);
			string arg = (sourceAudioChannelCount != 0) ? ((sourceAudioChannelCount != 1) ? ((sourceAudioChannelCount != 2) ? ((sourceAudioChannelCount != 4) ? (((int)(sourceAudioChannelCount - 1)).ToString() + ".1") : sourceAudioChannelCount.ToString()) : "Stereo") : "Mono") : "No channels";
			return importer.GetSourceAudioSampleRate(audioTrackIdx) + " Hz, " + arg;
		}

		private void ShowProperty(ref Rect labelRect, ref Rect valueRect, string label, string value)
		{
			GUI.Label(labelRect, label, this.m_Styles.labelStyle);
			GUI.Label(valueRect, value, this.m_Styles.labelStyle);
			labelRect.y += EditorGUIUtility.singleLineHeight;
			valueRect.y += EditorGUIUtility.singleLineHeight;
		}
	}
}
