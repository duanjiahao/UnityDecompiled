using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ParamEqGUI : IAudioEffectPluginGUI
	{
		public static string kCenterFreqName = "Center freq";

		public static string kOctaveRangeName = "Octave range";

		public static string kFrequencyGainName = "Frequency gain";

		private const bool useLogScale = true;

		public static GUIStyle textStyle10 = ParamEqGUI.BuildGUIStyleForLabel(Color.grey, 10, false, FontStyle.Normal, TextAnchor.MiddleCenter);

		public override string Name
		{
			get
			{
				return "ParamEQ";
			}
		}

		public override string Description
		{
			get
			{
				return "Parametric equalizer";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Firelight Technologies";
			}
		}

		public static GUIStyle BuildGUIStyleForLabel(Color color, int fontSize, bool wrapText, FontStyle fontstyle, TextAnchor anchor)
		{
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.focused.background = gUIStyle.onNormal.background;
			gUIStyle.focused.textColor = color;
			gUIStyle.alignment = anchor;
			gUIStyle.fontSize = fontSize;
			gUIStyle.fontStyle = fontstyle;
			gUIStyle.wordWrap = wrapText;
			gUIStyle.clipping = TextClipping.Overflow;
			gUIStyle.normal.textColor = color;
			return gUIStyle;
		}

		private static void DrawFrequencyTickMarks(Rect r, float samplerate, bool logScale, Color col)
		{
			ParamEqGUI.textStyle10.normal.textColor = col;
			float num = r.x;
			float num2 = 60f;
			for (float num3 = 0f; num3 < 1f; num3 += 0.01f)
			{
				float num4 = (float)ParamEqGUI.MapNormalizedFrequency((double)num3, (double)samplerate, logScale, true);
				float num5 = r.x + num3 * r.width;
				if (num5 - num > num2)
				{
					EditorGUI.DrawRect(new Rect(num5, r.yMax - 5f, 1f, 5f), col);
					GUI.Label(new Rect(num5, r.yMax - 22f, 1f, 15f), (num4 >= 1000f) ? string.Format("{0:F0} kHz", num4 * 0.001f) : string.Format("{0:F0} Hz", num4), ParamEqGUI.textStyle10);
					num = num5;
				}
			}
		}

		protected static Color ScaleAlpha(Color col, float blend)
		{
			return new Color(col.r, col.g, col.b, col.a * blend);
		}

		private static double MapNormalizedFrequency(double f, double sr, bool useLogScale, bool forward)
		{
			double num = 0.5 * sr;
			double result;
			if (useLogScale)
			{
				if (forward)
				{
					result = 10.0 * Math.Pow(num / 10.0, f);
				}
				else
				{
					result = Math.Log(f / 10.0) / Math.Log(num / 10.0);
				}
			}
			else
			{
				result = ((!forward) ? (f / num) : (f * num));
			}
			return result;
		}

		private static bool ParamEqualizerCurveEditor(IAudioEffectPlugin plugin, Rect r, ref float centerFreq, ref float bandwidth, ref float gain, float blend)
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			r = AudioCurveRendering.BeginCurveFrame(r);
			float min;
			float max;
			float num;
			plugin.GetFloatParameterInfo(ParamEqGUI.kCenterFreqName, out min, out max, out num);
			float min2;
			float max2;
			float num2;
			plugin.GetFloatParameterInfo(ParamEqGUI.kOctaveRangeName, out min2, out max2, out num2);
			float min3;
			float max3;
			float num3;
			plugin.GetFloatParameterInfo(ParamEqGUI.kFrequencyGainName, out min3, out max3, out num3);
			bool result = false;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl != EventType.MouseUp)
				{
					if (typeForControl == EventType.MouseDrag)
					{
						if (GUIUtility.hotControl == controlID)
						{
							float num4 = (!Event.current.alt) ? 1f : 0.25f;
							centerFreq = Mathf.Clamp((float)ParamEqGUI.MapNormalizedFrequency(ParamEqGUI.MapNormalizedFrequency((double)centerFreq, (double)plugin.GetSampleRate(), true, false) + (double)(current.delta.x / r.width), (double)plugin.GetSampleRate(), true, true), min, max);
							if (Event.current.shift)
							{
								bandwidth = Mathf.Clamp(bandwidth - current.delta.y * 0.02f * num4, min2, max2);
							}
							else
							{
								gain = Mathf.Clamp(gain - current.delta.y * 0.01f * num4, min3, max3);
							}
							result = true;
							current.Use();
						}
					}
				}
				else if (GUIUtility.hotControl == controlID && current.button == 0)
				{
					GUIUtility.hotControl = 0;
					EditorGUIUtility.SetWantsMouseJumping(0);
					current.Use();
				}
			}
			else if (r.Contains(Event.current.mousePosition) && current.button == 0)
			{
				GUIUtility.hotControl = controlID;
				EditorGUIUtility.SetWantsMouseJumping(1);
				current.Use();
			}
			if (Event.current.type == EventType.Repaint)
			{
				float num5 = (float)ParamEqGUI.MapNormalizedFrequency((double)centerFreq, (double)plugin.GetSampleRate(), true, false);
				EditorGUI.DrawRect(new Rect(num5 * r.width + r.x, r.y, 1f, r.height), (GUIUtility.hotControl != controlID) ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f));
				HandleUtility.ApplyWireMaterial();
				double num6 = 3.1415926;
				double wm = -2.0 * num6 / (double)plugin.GetSampleRate();
				double num7 = 2.0 * num6 * (double)centerFreq / (double)plugin.GetSampleRate();
				double num8 = 1.0 / (double)bandwidth;
				double num9 = (double)gain;
				double num10 = Math.Sin(num7) / (2.0 * num8);
				double b0 = 1.0 + num10 * num9;
				double b1 = -2.0 * Math.Cos(num7);
				double b2 = 1.0 - num10 * num9;
				double a0 = 1.0 + num10 / num9;
				double a1 = -2.0 * Math.Cos(num7);
				double a2 = 1.0 - num10 / num9;
				AudioCurveRendering.DrawCurve(r, delegate(float x)
				{
					double num11 = ParamEqGUI.MapNormalizedFrequency((double)x, (double)plugin.GetSampleRate(), true, true);
					ComplexD a = ComplexD.Exp(wm * num11);
					ComplexD a2 = a * (a * b2 + b1) + b0;
					ComplexD b = a * (a * a2 + a1) + a0;
					ComplexD complexD = a2 / b;
					double num12 = Math.Log10(complexD.Mag2());
					return (float)(0.5 * num12);
				}, ParamEqGUI.ScaleAlpha(AudioCurveRendering.kAudioOrange, blend));
			}
			ParamEqGUI.DrawFrequencyTickMarks(r, (float)plugin.GetSampleRate(), true, new Color(1f, 1f, 1f, 0.3f * blend));
			AudioCurveRendering.EndCurveFrame();
			return result;
		}

		public override bool OnGUI(IAudioEffectPlugin plugin)
		{
			float blend = (!plugin.IsPluginEditableAndEnabled()) ? 0.5f : 1f;
			float value;
			plugin.GetFloatParameter(ParamEqGUI.kCenterFreqName, out value);
			float value2;
			plugin.GetFloatParameter(ParamEqGUI.kOctaveRangeName, out value2);
			float value3;
			plugin.GetFloatParameter(ParamEqGUI.kFrequencyGainName, out value3);
			GUILayout.Space(5f);
			Rect rect = GUILayoutUtility.GetRect(200f, 100f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			if (ParamEqGUI.ParamEqualizerCurveEditor(plugin, rect, ref value, ref value2, ref value3, blend))
			{
				plugin.SetFloatParameter(ParamEqGUI.kCenterFreqName, value);
				plugin.SetFloatParameter(ParamEqGUI.kOctaveRangeName, value2);
				plugin.SetFloatParameter(ParamEqGUI.kFrequencyGainName, value3);
			}
			return true;
		}
	}
}
