using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DuckVolumeGUI : IAudioEffectPluginGUI
	{
		public enum DragType
		{
			None,
			ThresholdAndKnee,
			Ratio,
			MakeupGain
		}

		public static string kThresholdName = "Threshold";

		public static string kRatioName = "Ratio";

		public static string kMakeupGainName = "Make-up Gain";

		public static string kAttackTimeName = "Attack Time";

		public static string kReleaseTimeName = "Release Time";

		public static string kKneeName = "Knee";

		public static GUIStyle textStyle10 = DuckVolumeGUI.BuildGUIStyleForLabel(Color.grey, 10, false, FontStyle.Normal, TextAnchor.MiddleLeft);

		private static DuckVolumeGUI.DragType dragtype = DuckVolumeGUI.DragType.None;

		public override string Name
		{
			get
			{
				return "Duck Volume";
			}
		}

		public override string Description
		{
			get
			{
				return "Volume Ducking";
			}
		}

		public override string Vendor
		{
			get
			{
				return "Unity Technologies";
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

		public static void DrawText(float x, float y, string text)
		{
			GUI.Label(new Rect(x, y - 5f, 200f, 10f), new GUIContent(text, string.Empty), DuckVolumeGUI.textStyle10);
		}

		public static void DrawLine(float x1, float y1, float x2, float y2, Color col)
		{
			Handles.color = col;
			Handles.DrawLine(new Vector3(x1, y1, 0f), new Vector3(x2, y2, 0f));
		}

		protected static Color ScaleAlpha(Color col, float blend)
		{
			return new Color(col.r, col.g, col.b, col.a * blend);
		}

		protected static void DrawVU(Rect r, float level, float blend, bool topdown)
		{
			level = 1f - level;
			Rect rect = new Rect(r.x + 1f, r.y + 1f + ((!topdown) ? (level * r.height) : 0f), r.width - 2f, r.y - 2f + ((!topdown) ? (r.height - level * r.height) : (level * r.height)));
			AudioMixerDrawUtils.DrawRect(r, new Color(0.1f, 0.1f, 0.1f));
			AudioMixerDrawUtils.DrawRect(rect, new Color(0.6f, 0.2f, 0.2f));
		}

		private static float EvaluateDuckingVolume(float x, float ratio, float threshold, float makeupGain, float knee, float dbRange, float dbMin)
		{
			float num = 1f / ratio;
			float num2 = (knee <= 0f) ? 0f : ((num - 1f) / (4f * knee));
			float num3 = threshold - knee;
			float num4 = x * dbRange + dbMin;
			float num5 = num4;
			float num6 = num4 - threshold;
			if (num6 > -knee && num6 < knee)
			{
				num6 += knee;
				num5 = num6 * (num2 * num6 + 1f) + num3;
			}
			else if (num6 > 0f)
			{
				num5 = threshold + num * num6;
			}
			return 2f * (num5 + makeupGain - dbMin) / dbRange - 1f;
		}

		private static bool CurveDisplay(IAudioEffectPlugin plugin, Rect r0, ref float threshold, ref float ratio, ref float makeupGain, ref float attackTime, ref float releaseTime, ref float knee, float sidechainLevel, float outputLevel, float blend)
		{
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Rect r = AudioCurveRendering.BeginCurveFrame(r0);
			float num = 10f;
			float min;
			float max;
			float num2;
			plugin.GetFloatParameterInfo(DuckVolumeGUI.kThresholdName, out min, out max, out num2);
			float min2;
			float max2;
			float num3;
			plugin.GetFloatParameterInfo(DuckVolumeGUI.kRatioName, out min2, out max2, out num3);
			float min3;
			float max3;
			float num4;
			plugin.GetFloatParameterInfo(DuckVolumeGUI.kMakeupGainName, out min3, out max3, out num4);
			float min4;
			float max4;
			float num5;
			plugin.GetFloatParameterInfo(DuckVolumeGUI.kKneeName, out min4, out max4, out num5);
			float dbRange = 100f;
			float dbMin = -80f;
			float num6 = r.width * (threshold - dbMin) / dbRange;
			bool result = false;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (r.Contains(Event.current.mousePosition) && current.button == 0)
				{
					DuckVolumeGUI.dragtype = DuckVolumeGUI.DragType.None;
					GUIUtility.hotControl = controlID;
					EditorGUIUtility.SetWantsMouseJumping(1);
					current.Use();
					if (Mathf.Abs(r.x + num6 - current.mousePosition.x) >= 10f)
					{
						DuckVolumeGUI.dragtype = ((current.mousePosition.x >= r.x + num6) ? DuckVolumeGUI.DragType.Ratio : DuckVolumeGUI.DragType.MakeupGain);
					}
					else
					{
						DuckVolumeGUI.dragtype = DuckVolumeGUI.DragType.ThresholdAndKnee;
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID && current.button == 0)
				{
					DuckVolumeGUI.dragtype = DuckVolumeGUI.DragType.None;
					GUIUtility.hotControl = 0;
					EditorGUIUtility.SetWantsMouseJumping(0);
					current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					float num7 = (!current.alt) ? 1f : 0.25f;
					if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.ThresholdAndKnee)
					{
						bool flag = Mathf.Abs(current.delta.x) < Mathf.Abs(current.delta.y);
						if (flag)
						{
							knee = Mathf.Clamp(knee + current.delta.y * 0.5f * num7, min4, max4);
						}
						else
						{
							threshold = Mathf.Clamp(threshold + current.delta.x * 0.1f * num7, min, max);
						}
					}
					else if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.Ratio)
					{
						ratio = Mathf.Clamp(ratio + current.delta.y * ((ratio <= 1f) ? 0.003f : 0.05f) * num7, min2, max2);
					}
					else if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.MakeupGain)
					{
						makeupGain = Mathf.Clamp(makeupGain - current.delta.y * 0.5f * num7, min3, max3);
					}
					else
					{
						Debug.LogError("Drag: Unhandled enum");
					}
					result = true;
					current.Use();
				}
				break;
			}
			if (current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial();
				float num8 = r.height * (1f - (threshold - dbMin + makeupGain) / dbRange);
				Color col2 = new Color(0.7f, 0.7f, 0.7f);
				Color black = Color.black;
				float duckGradient = 1f / ratio;
				float duckThreshold = threshold;
				float duckSidechainLevel = sidechainLevel;
				float duckMakeupGain = makeupGain;
				float duckKnee = knee;
				float duckKneeC1 = (knee <= 0f) ? 0f : ((duckGradient - 1f) / (4f * knee));
				float duckKneeC2 = duckThreshold - knee;
				AudioCurveRendering.DrawFilledCurve(r, delegate(float x, out Color col)
				{
					float num16 = x * dbRange + dbMin;
					float num17 = num16;
					float num18 = num16 - duckThreshold;
					col = DuckVolumeGUI.ScaleAlpha((duckSidechainLevel <= num16) ? Color.grey : AudioCurveRendering.kAudioOrange, blend);
					if (num18 > -duckKnee && num18 < duckKnee)
					{
						num18 += duckKnee;
						num17 = num18 * (duckKneeC1 * num18 + 1f) + duckKneeC2;
						if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.ThresholdAndKnee)
						{
							col = new Color(col.r * 1.2f, col.g * 1.2f, col.b * 1.2f);
						}
					}
					else if (num18 > 0f)
					{
						num17 = duckThreshold + duckGradient * num18;
					}
					return 2f * (num17 + duckMakeupGain - dbMin) / dbRange - 1f;
				});
				if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.MakeupGain)
				{
					AudioCurveRendering.DrawCurve(r, delegate(float x)
					{
						float num16 = x * dbRange + dbMin;
						float num17 = num16;
						float num18 = num16 - duckThreshold;
						if (num18 > -duckKnee && num18 < duckKnee)
						{
							num18 += duckKnee;
							num17 = num18 * (duckKneeC1 * num18 + 1f) + duckKneeC2;
						}
						else if (num18 > 0f)
						{
							num17 = duckThreshold + duckGradient * num18;
						}
						return 2f * (num17 + duckMakeupGain - dbMin) / dbRange - 1f;
					}, Color.white);
				}
				DuckVolumeGUI.textStyle10.normal.textColor = DuckVolumeGUI.ScaleAlpha(col2, blend);
				EditorGUI.DrawRect(new Rect(r.x + num6, r.y, 1f, r.height), DuckVolumeGUI.textStyle10.normal.textColor);
				DuckVolumeGUI.DrawText(r.x + num6 + 4f, r.y + 6f, string.Format("Threshold: {0:F1} dB", threshold));
				DuckVolumeGUI.textStyle10.normal.textColor = DuckVolumeGUI.ScaleAlpha(black, blend);
				DuckVolumeGUI.DrawText(r.x + 4f, r.y + r.height - 10f, (sidechainLevel >= -80f) ? string.Format("Input: {0:F1} dB", sidechainLevel) : "Input: None");
				if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.Ratio)
				{
					float num9 = r.height / r.width;
					Handles.DrawAAPolyLine(2f, new Color[]
					{
						Color.black,
						Color.black
					}, new Vector3[]
					{
						new Vector3(r.x + num6 + r.width, r.y + num8 - num9 * r.width, 0f),
						new Vector3(r.x + num6 - r.width, r.y + num8 + num9 * r.width, 0f)
					});
					Handles.DrawAAPolyLine(3f, new Color[]
					{
						Color.white,
						Color.white
					}, new Vector3[]
					{
						new Vector3(r.x + num6 + r.width, r.y + num8 - num9 * duckGradient * r.width, 0f),
						new Vector3(r.x + num6 - r.width, r.y + num8 + num9 * duckGradient * r.width, 0f)
					});
				}
				else if (DuckVolumeGUI.dragtype == DuckVolumeGUI.DragType.ThresholdAndKnee)
				{
					float num10 = (threshold - knee - dbMin) / dbRange;
					float num11 = (threshold + knee - dbMin) / dbRange;
					float num12 = DuckVolumeGUI.EvaluateDuckingVolume(num10, ratio, threshold, makeupGain, knee, dbRange, dbMin);
					float num13 = DuckVolumeGUI.EvaluateDuckingVolume(num11, ratio, threshold, makeupGain, knee, dbRange, dbMin);
					float num14 = r.yMax - (num12 + 1f) * 0.5f * r.height;
					float num15 = r.yMax - (num13 + 1f) * 0.5f * r.height;
					EditorGUI.DrawRect(new Rect(r.x + num10 * r.width, num14, 1f, r.height - num14), new Color(0f, 0f, 0f, 0.5f));
					EditorGUI.DrawRect(new Rect(r.x + num11 * r.width - 1f, num15, 1f, r.height - num15), new Color(0f, 0f, 0f, 0.5f));
					EditorGUI.DrawRect(new Rect(r.x + num6 - 1f, r.y, 3f, r.height), Color.white);
				}
				outputLevel = (Mathf.Clamp(outputLevel - makeupGain, dbMin, dbMin + dbRange) - dbMin) / dbRange;
				if (EditorApplication.isPlaying)
				{
					Rect r2 = new Rect(r.x + r.width - num + 2f, r.y + 2f, num - 4f, r.height - 4f);
					DuckVolumeGUI.DrawVU(r2, outputLevel, blend, true);
				}
			}
			AudioCurveRendering.EndCurveFrame();
			return result;
		}

		public override bool OnGUI(IAudioEffectPlugin plugin)
		{
			float blend = (!plugin.IsPluginEditableAndEnabled()) ? 0.5f : 1f;
			float value;
			plugin.GetFloatParameter(DuckVolumeGUI.kThresholdName, out value);
			float value2;
			plugin.GetFloatParameter(DuckVolumeGUI.kRatioName, out value2);
			float value3;
			plugin.GetFloatParameter(DuckVolumeGUI.kMakeupGainName, out value3);
			float value4;
			plugin.GetFloatParameter(DuckVolumeGUI.kAttackTimeName, out value4);
			float value5;
			plugin.GetFloatParameter(DuckVolumeGUI.kReleaseTimeName, out value5);
			float value6;
			plugin.GetFloatParameter(DuckVolumeGUI.kKneeName, out value6);
			float[] array;
			plugin.GetFloatBuffer("Metering", out array, 2);
			GUILayout.Space(5f);
			Rect rect = GUILayoutUtility.GetRect(200f, 160f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			if (DuckVolumeGUI.CurveDisplay(plugin, rect, ref value, ref value2, ref value3, ref value4, ref value5, ref value6, array[0], array[1], blend))
			{
				plugin.SetFloatParameter(DuckVolumeGUI.kThresholdName, value);
				plugin.SetFloatParameter(DuckVolumeGUI.kRatioName, value2);
				plugin.SetFloatParameter(DuckVolumeGUI.kMakeupGainName, value3);
				plugin.SetFloatParameter(DuckVolumeGUI.kAttackTimeName, value4);
				plugin.SetFloatParameter(DuckVolumeGUI.kReleaseTimeName, value5);
				plugin.SetFloatParameter(DuckVolumeGUI.kKneeName, value6);
			}
			return true;
		}
	}
}
