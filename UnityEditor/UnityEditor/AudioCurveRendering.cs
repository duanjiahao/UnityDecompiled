using System;
using UnityEngine;

namespace UnityEditor
{
	public class AudioCurveRendering
	{
		public delegate float AudioCurveEvaluator(float x);

		public delegate float AudioCurveAndColorEvaluator(float x, out Color col);

		public delegate void AudioMinMaxCurveAndColorEvaluator(float x, out Color col, out float minValue, out float maxValue);

		private static float pixelEpsilon = 0.005f;

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
			float num = EditorGUIUtility.pixelsPerPoint;
			float num2 = 1f / num;
			float num3 = 0.5f * num2;
			float num4 = Mathf.Ceil(r.width) * num;
			float num5 = Mathf.Floor(r.x) + AudioCurveRendering.pixelEpsilon;
			float num6 = 1f / (num4 - 1f);
			float num7 = r.height * 0.5f;
			float num8 = r.y + 0.5f * r.height;
			float y = r.y + r.height;
			Color c;
			float b = Mathf.Clamp(num7 * eval(0f, out c), -num7, num7);
			int num9 = 0;
			while ((float)num9 < num4)
			{
				float x = num5 + (float)num9 * num2;
				float num10 = Mathf.Clamp(num7 * eval((float)num9 * num6, out c), -num7, num7);
				float num11 = Mathf.Min(num10, b) - num3;
				float num12 = Mathf.Max(num10, b) + num3;
				GL.Color(new Color(c.r, c.g, c.b, 0f));
				AudioMixerDrawUtils.Vertex(x, num8 - num12);
				GL.Color(c);
				AudioMixerDrawUtils.Vertex(x, num8 - num11);
				AudioMixerDrawUtils.Vertex(x, num8 - num11);
				AudioMixerDrawUtils.Vertex(x, y);
				b = num10;
				num9++;
			}
			GL.End();
		}

		private static void Sort2(ref float minValue, ref float maxValue)
		{
			if (minValue > maxValue)
			{
				float num = minValue;
				minValue = maxValue;
				maxValue = num;
			}
		}

		public static void DrawMinMaxFilledCurve(Rect r, AudioCurveRendering.AudioMinMaxCurveAndColorEvaluator eval)
		{
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			float num = EditorGUIUtility.pixelsPerPoint;
			float num2 = 1f / num;
			float num3 = 0.5f * num2;
			float num4 = Mathf.Ceil(r.width) * num;
			float num5 = Mathf.Floor(r.x) + AudioCurveRendering.pixelEpsilon;
			float num6 = 1f / (num4 - 1f);
			float num7 = r.height * 0.5f;
			float num8 = r.y + 0.5f * r.height;
			Color c;
			float value;
			float value2;
			eval(0.0001f, out c, out value, out value2);
			AudioCurveRendering.Sort2(ref value, ref value2);
			float b = num8 - num7 * Mathf.Clamp(value2, -1f, 1f);
			float b2 = num8 - num7 * Mathf.Clamp(value, -1f, 1f);
			float y = r.y;
			float max = r.y + r.height;
			int num9 = 0;
			while ((float)num9 < num4)
			{
				float x = num5 + (float)num9 * num2;
				eval((float)num9 * num6, out c, out value, out value2);
				AudioCurveRendering.Sort2(ref value, ref value2);
				Color c2 = new Color(c.r, c.g, c.b, 0f);
				float num10 = num8 - num7 * Mathf.Clamp(value2, -1f, 1f);
				float num11 = num8 - num7 * Mathf.Clamp(value, -1f, 1f);
				float y2 = Mathf.Clamp(Mathf.Min(num10, b) - num3, y, max);
				float y3 = Mathf.Clamp(Mathf.Max(num10, b) + num3, y, max);
				float y4 = Mathf.Clamp(Mathf.Min(num11, b2) - num3, y, max);
				float y5 = Mathf.Clamp(Mathf.Max(num11, b2) + num3, y, max);
				AudioCurveRendering.Sort2(ref y2, ref y4);
				AudioCurveRendering.Sort2(ref y3, ref y5);
				AudioCurveRendering.Sort2(ref y2, ref y3);
				AudioCurveRendering.Sort2(ref y4, ref y5);
				AudioCurveRendering.Sort2(ref y3, ref y4);
				GL.Color(c2);
				AudioMixerDrawUtils.Vertex(x, y2);
				GL.Color(c);
				AudioMixerDrawUtils.Vertex(x, y3);
				AudioMixerDrawUtils.Vertex(x, y3);
				AudioMixerDrawUtils.Vertex(x, y4);
				AudioMixerDrawUtils.Vertex(x, y4);
				GL.Color(c2);
				AudioMixerDrawUtils.Vertex(x, y5);
				b2 = num11;
				b = num10;
				num9++;
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
			float num = EditorGUIUtility.pixelsPerPoint;
			float num2 = 1f / num;
			float num3 = 0.5f * num2;
			float num4 = Mathf.Ceil(r.width) * num;
			float num5 = Mathf.Floor(r.x) + AudioCurveRendering.pixelEpsilon;
			float num6 = 1f / (num4 - 1f);
			float num7 = r.height * 0.5f;
			float num8 = r.y + 0.5f * r.height;
			Color c;
			float b = Mathf.Clamp(num7 * eval(0.0001f, out c), 0f, num7);
			int num9 = 0;
			while ((float)num9 < num4)
			{
				float x = num5 + (float)num9 * num2;
				float num10 = Mathf.Clamp(num7 * eval((float)num9 * num6, out c), 0f, num7);
				float num11 = Mathf.Max(Mathf.Min(num10, b) - num3, 0f);
				float num12 = Mathf.Min(Mathf.Max(num10, b) + num3, num7);
				Color c2 = new Color(c.r, c.g, c.b, 0f);
				GL.Color(c2);
				AudioMixerDrawUtils.Vertex(x, num8 - num12);
				GL.Color(c);
				AudioMixerDrawUtils.Vertex(x, num8 - num11);
				AudioMixerDrawUtils.Vertex(x, num8 - num11);
				AudioMixerDrawUtils.Vertex(x, num8 + num11);
				AudioMixerDrawUtils.Vertex(x, num8 + num11);
				GL.Color(c2);
				AudioMixerDrawUtils.Vertex(x, num8 + num12);
				b = num10;
				num9++;
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
			int num = (int)Mathf.Ceil(r.width);
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
