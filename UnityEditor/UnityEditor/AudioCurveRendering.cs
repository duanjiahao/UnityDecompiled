using System;
using UnityEngine;
namespace UnityEditor
{
	public class AudioCurveRendering
	{
		public delegate float AudioCurveEvaluator(float x);
		public delegate float AudioCurveAndColorEvaluator(float x, out Color col);
		public static readonly Color kAudioOrange = new Color(1f, 0.65882355f, 0.02745098f);
		private static Vector3[] s_PointCache;
		public static Rect BeginCurveFrame(Rect r)
		{
			AudioCurveRendering.DrawCurveBackground(r);
			r = AudioCurveRendering.DrawCurveFrame(r);
			GUI.BeginGroup(r);
			return new Rect(0f, 0f, r.width, r.height);
		}
		public static void EndCurveFrame()
		{
			GUI.EndGroup();
		}
		public static Rect DrawCurveFrame(Rect r)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return r;
			}
			EditorStyles.colorPickerBox.Draw(r, false, false, false, false);
			r.x += 1f;
			r.y += 1f;
			r.width -= 2f;
			r.height -= 2f;
			return r;
		}
		public static void DrawCurveBackground(Rect r)
		{
			EditorGUI.DrawRect(r, new Color(0.3f, 0.3f, 0.3f));
		}
		public static void DrawFilledCurve(Rect r, AudioCurveRendering.AudioCurveEvaluator eval, Color curveColor)
		{
			AudioCurveRendering.DrawFilledCurve(r, delegate(float x, out Color color)
			{
				color = curveColor;
				return eval(x);
			});
		}
		public static void DrawFilledCurve(Rect r, AudioCurveRendering.AudioCurveAndColorEvaluator eval)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			float num = 1f / (r.width - 1f);
			float num2 = r.height * 0.5f;
			Color c;
			float num3 = r.y + Mathf.Clamp(num2 - num2 * eval(0.0001f, out c), 0f, r.height);
			int num4 = 0;
			while ((float)num4 < r.width)
			{
				float num5 = (float)((int)r.x + num4);
				float num6 = r.y + Mathf.Clamp(num2 - num2 * eval((float)num4 * num, out c), 0f, r.height);
				GL.Color(c);
				AudioMixerDrawUtils.Vertex(num5, (float)((int)Mathf.Ceil(num6)));
				AudioMixerDrawUtils.Vertex(num5, r.y + r.height);
				if (Mathf.Abs(num3 - num6) <= 1f)
				{
					GL.Color(new Color(c.r, c.g, c.b, c.a * (Mathf.Ceil(num6) - num6)));
					AudioMixerDrawUtils.Vertex(num5, Mathf.Ceil(num6) - 1f);
					AudioMixerDrawUtils.Vertex(num5, Mathf.Ceil(num6));
				}
				else
				{
					if (num3 < num6)
					{
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num5, Mathf.Ceil(num3));
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num5, Mathf.Ceil(num6));
					}
					else
					{
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num5 - 1f, Mathf.Ceil(num6));
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num5 - 1f, Mathf.Ceil(num3));
					}
				}
				num3 = num6;
				num4++;
			}
			GL.End();
		}
		public static void DrawSymmetricFilledCurve(Rect r, AudioCurveRendering.AudioCurveAndColorEvaluator eval)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			float num = 1f / (r.width - 1f);
			float num2 = r.height * 0.5f;
			float y = r.y;
			float num3 = r.y + r.height;
			Color c;
			float num4 = Mathf.Clamp(num2 - num2 * eval(0.0001f, out c), 0f, num2);
			int num5 = 0;
			while ((float)num5 < r.width)
			{
				float num6 = (float)((int)r.x + num5);
				float num7 = Mathf.Clamp(num2 - num2 * eval((float)num5 * num, out c), 0f, num2);
				GL.Color(c);
				AudioMixerDrawUtils.Vertex(num6, y + (float)((int)Mathf.Ceil(num7)));
				AudioMixerDrawUtils.Vertex(num6, num3 - (float)((int)Mathf.Ceil(num7)));
				if (Mathf.Abs(num4 - num7) <= 1f)
				{
					GL.Color(new Color(c.r, c.g, c.b, c.a * (Mathf.Ceil(num7) - num7)));
					AudioMixerDrawUtils.Vertex(num6, y + Mathf.Ceil(num7) - 1f);
					AudioMixerDrawUtils.Vertex(num6, y + Mathf.Ceil(num7));
					AudioMixerDrawUtils.Vertex(num6, num3 - Mathf.Ceil(num7) + 1f);
					AudioMixerDrawUtils.Vertex(num6, num3 - Mathf.Ceil(num7));
				}
				else
				{
					if (num4 < num7)
					{
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num6, y + Mathf.Ceil(num4));
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num6, y + Mathf.Ceil(num7));
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num6, num3 - Mathf.Ceil(num4) - 1f);
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num6, num3 - Mathf.Ceil(num7) - 1f);
					}
					else
					{
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num6 - 1f, y + Mathf.Ceil(num7));
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num6 - 1f, y + Mathf.Ceil(num4));
						GL.Color(new Color(c.r, c.g, c.b, 0f));
						AudioMixerDrawUtils.Vertex(num6 - 1f, num3 - Mathf.Ceil(num7) - 1f);
						GL.Color(c);
						AudioMixerDrawUtils.Vertex(num6 - 1f, num3 - Mathf.Ceil(num4) - 1f);
					}
				}
				num4 = num7;
				num5++;
			}
			GL.End();
		}
		public static void DrawCurve(Rect r, AudioCurveRendering.AudioCurveEvaluator eval, Color curveColor)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			int num = (int)r.width;
			float num2 = r.height * 0.5f;
			float num3 = 1f / (float)(num - 1);
			Vector3[] pointCache = AudioCurveRendering.GetPointCache(num);
			for (int i = 0; i < num; i++)
			{
				pointCache[i].x = (float)i + r.x;
				pointCache[i].y = num2 - num2 * eval((float)i * num3) + r.y;
				pointCache[i].z = 0f;
			}
			GUI.BeginClip(r);
			Handles.color = curveColor;
			Handles.DrawAAPolyLine(3f, num, pointCache);
			GUI.EndClip();
		}
		private static Vector3[] GetPointCache(int numPoints)
		{
			if (AudioCurveRendering.s_PointCache == null || AudioCurveRendering.s_PointCache.Length != numPoints)
			{
				AudioCurveRendering.s_PointCache = new Vector3[numPoints];
			}
			return AudioCurveRendering.s_PointCache;
		}
		public static void DrawGradientRect(Rect r, Color c1, Color c2, float blend, bool horizontal)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.Begin(7);
			if (horizontal)
			{
				GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
				GL.Vertex3(r.x, r.y, 0f);
				GL.Vertex3(r.x + r.width, r.y, 0f);
				GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
				GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
				GL.Vertex3(r.x, r.y + r.height, 0f);
			}
			else
			{
				GL.Color(new Color(c1.r, c1.g, c1.b, c1.a * blend));
				GL.Vertex3(r.x, r.y + r.height, 0f);
				GL.Vertex3(r.x, r.y, 0f);
				GL.Color(new Color(c2.r, c2.g, c2.b, c2.a * blend));
				GL.Vertex3(r.x + r.width, r.y, 0f);
				GL.Vertex3(r.x + r.width, r.y + r.height, 0f);
			}
			GL.End();
		}
	}
}
