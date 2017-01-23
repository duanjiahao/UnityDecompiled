using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerDSPView
	{
		private class AudioProfilerDSPNode
		{
			public AudioProfilerDSPView.AudioProfilerDSPNode firstTarget;

			public AudioProfilerDSPInfo info;

			public int x;

			public int y;

			public int level;

			public int maxY;

			public int targetPort;

			public bool audible;

			public AudioProfilerDSPNode(AudioProfilerDSPView.AudioProfilerDSPNode firstTarget, AudioProfilerDSPInfo info, int x, int y, int level)
			{
				this.firstTarget = firstTarget;
				this.info = info;
				this.x = x;
				this.y = y;
				this.level = level;
				this.maxY = y;
				this.audible = ((info.flags & 1) != 0 && (info.flags & 2) == 0);
				if (firstTarget != null)
				{
					this.audible &= firstTarget.audible;
				}
			}
		}

		private class AudioProfilerDSPWire
		{
			public AudioProfilerDSPView.AudioProfilerDSPNode source;

			public AudioProfilerDSPView.AudioProfilerDSPNode target;

			public AudioProfilerDSPInfo info;

			public int targetPort;

			public AudioProfilerDSPWire(AudioProfilerDSPView.AudioProfilerDSPNode source, AudioProfilerDSPView.AudioProfilerDSPNode target, AudioProfilerDSPInfo info)
			{
				this.source = source;
				this.target = target;
				this.info = info;
				this.targetPort = target.targetPort;
			}
		}

		private const int AUDIOPROFILER_DSPFLAGS_ACTIVE = 1;

		private const int AUDIOPROFILER_DSPFLAGS_BYPASS = 2;

		private void DrawRectClipped(Rect r, Color col, string name, Rect c, float zoomFactor)
		{
			Rect rect = new Rect(r.x * zoomFactor, r.y * zoomFactor, r.width * zoomFactor, r.height * zoomFactor);
			float x = rect.x;
			float a = rect.x + rect.width;
			float y = rect.y;
			float a2 = rect.y + rect.height;
			float x2 = c.x;
			float b = c.x + c.width;
			float y2 = c.y;
			float b2 = c.y + c.height;
			float num = Mathf.Max(x, x2);
			float num2 = Mathf.Max(y, y2);
			float num3 = Mathf.Min(a, b);
			float num4 = Mathf.Min(a2, b2);
			if (num < num3 && num2 < num4)
			{
				if (name == null)
				{
					EditorGUI.DrawRect(rect, col);
				}
				else
				{
					GUI.color = col;
					GUI.Button(rect, name);
				}
			}
		}

		private static int GetOutCode(Vector3 p, Rect c)
		{
			int num = 0;
			if (p.x < c.x)
			{
				num |= 1;
			}
			if (p.x > c.x + c.width)
			{
				num |= 2;
			}
			if (p.y < c.y)
			{
				num |= 4;
			}
			if (p.y > c.y + c.height)
			{
				num |= 8;
			}
			return num;
		}

		public void OnGUI(Rect clippingRect, ProfilerProperty property, bool showInactiveDSPChains, bool highlightAudibleDSPChains, ref float zoomFactor, ref Vector2 scrollPos)
		{
			if (Event.current.type == EventType.ScrollWheel && clippingRect.Contains(Event.current.mousePosition) && Event.current.shift)
			{
				float num = 1.05f;
				float num2 = zoomFactor;
				zoomFactor *= ((Event.current.delta.y <= 0f) ? (1f / num) : num);
				scrollPos += (Event.current.mousePosition - scrollPos) * (zoomFactor - num2);
				Event.current.Use();
			}
			else if (Event.current.type == EventType.Repaint)
			{
				int num3 = 64;
				int num4 = 16;
				int num5 = 140;
				int num6 = 30;
				int num7 = num3 + 10;
				int num8 = 5;
				Dictionary<int, AudioProfilerDSPView.AudioProfilerDSPNode> dictionary = new Dictionary<int, AudioProfilerDSPView.AudioProfilerDSPNode>();
				List<AudioProfilerDSPView.AudioProfilerDSPWire> list = new List<AudioProfilerDSPView.AudioProfilerDSPWire>();
				AudioProfilerDSPInfo[] audioProfilerDSPInfo = property.GetAudioProfilerDSPInfo();
				if (audioProfilerDSPInfo != null)
				{
					bool flag = true;
					AudioProfilerDSPInfo[] array = audioProfilerDSPInfo;
					for (int i = 0; i < array.Length; i++)
					{
						AudioProfilerDSPInfo info = array[i];
						if (showInactiveDSPChains || (info.flags & 1) != 0)
						{
							if (!dictionary.ContainsKey(info.id))
							{
								AudioProfilerDSPView.AudioProfilerDSPNode audioProfilerDSPNode = (!dictionary.ContainsKey(info.target)) ? null : dictionary[info.target];
								if (audioProfilerDSPNode != null)
								{
									dictionary[info.id] = new AudioProfilerDSPView.AudioProfilerDSPNode(audioProfilerDSPNode, info, audioProfilerDSPNode.x + num5 + num7, audioProfilerDSPNode.maxY, audioProfilerDSPNode.level + 1);
									audioProfilerDSPNode.maxY += num6 + num8;
									for (AudioProfilerDSPView.AudioProfilerDSPNode audioProfilerDSPNode2 = audioProfilerDSPNode; audioProfilerDSPNode2 != null; audioProfilerDSPNode2 = audioProfilerDSPNode2.firstTarget)
									{
										audioProfilerDSPNode2.maxY = Mathf.Max(audioProfilerDSPNode2.maxY, audioProfilerDSPNode.maxY);
									}
								}
								else if (flag)
								{
									flag = false;
									dictionary[info.id] = new AudioProfilerDSPView.AudioProfilerDSPNode(audioProfilerDSPNode, info, 10 + num5 / 2, 10 + num6 / 2, 1);
								}
								if (audioProfilerDSPNode != null)
								{
									list.Add(new AudioProfilerDSPView.AudioProfilerDSPWire(dictionary[info.id], audioProfilerDSPNode, info));
								}
							}
							else
							{
								list.Add(new AudioProfilerDSPView.AudioProfilerDSPWire(dictionary[info.id], dictionary[info.target], info));
							}
						}
					}
					foreach (KeyValuePair<int, AudioProfilerDSPView.AudioProfilerDSPNode> current in dictionary)
					{
						AudioProfilerDSPView.AudioProfilerDSPNode value = current.Value;
						value.y += ((value.maxY != value.y) ? (value.maxY - value.y) : (num6 + num8)) / 2;
					}
					foreach (AudioProfilerDSPView.AudioProfilerDSPWire current2 in list)
					{
						float num9 = 4f;
						AudioProfilerDSPView.AudioProfilerDSPNode source = current2.source;
						AudioProfilerDSPView.AudioProfilerDSPNode target = current2.target;
						AudioProfilerDSPInfo info2 = current2.info;
						Vector3 vector = new Vector3(((float)source.x - (float)num5 * 0.5f) * zoomFactor, (float)source.y * zoomFactor, 0f);
						Vector3 vector2 = new Vector3(((float)target.x + (float)num5 * 0.5f) * zoomFactor, ((float)target.y + (float)current2.targetPort * num9) * zoomFactor, 0f);
						int outCode = AudioProfilerDSPView.GetOutCode(vector, clippingRect);
						int outCode2 = AudioProfilerDSPView.GetOutCode(vector2, clippingRect);
						if ((outCode & outCode2) == 0)
						{
							float width = 3f;
							Handles.color = new Color(info2.weight, 0f, 0f, (highlightAudibleDSPChains && !source.audible) ? 0.4f : 1f);
							Handles.DrawAAPolyLine(width, 2, new Vector3[]
							{
								vector,
								vector2
							});
						}
					}
					foreach (AudioProfilerDSPView.AudioProfilerDSPWire current3 in list)
					{
						AudioProfilerDSPView.AudioProfilerDSPNode source2 = current3.source;
						AudioProfilerDSPView.AudioProfilerDSPNode target2 = current3.target;
						AudioProfilerDSPInfo info3 = current3.info;
						if (info3.weight != 1f)
						{
							int num10 = source2.x - (num7 + num5) / 2;
							int num11 = (target2 == null) ? target2.y : ((int)((float)target2.y + ((float)(num10 - target2.x) - (float)num5 * 0.5f) * (float)(source2.y - target2.y) / (float)(source2.x - target2.x - num5)));
							this.DrawRectClipped(new Rect((float)(num10 - num3 / 2), (float)(num11 - num4 / 2), (float)num3, (float)num4), new Color(1f, 0.3f, 0.2f, (highlightAudibleDSPChains && !source2.audible) ? 0.4f : 1f), string.Format("{0:0.00}%", 100f * info3.weight), clippingRect, zoomFactor);
						}
					}
					foreach (KeyValuePair<int, AudioProfilerDSPView.AudioProfilerDSPNode> current4 in dictionary)
					{
						AudioProfilerDSPView.AudioProfilerDSPNode value2 = current4.Value;
						AudioProfilerDSPInfo info4 = value2.info;
						if (dictionary.ContainsKey(info4.target) && value2.firstTarget == dictionary[info4.target])
						{
							string text = property.GetAudioProfilerNameByOffset(info4.nameOffset);
							float num12 = 0.01f * info4.cpuLoad;
							float num13 = 0.1f;
							bool flag2 = (info4.flags & 1) != 0;
							bool flag3 = (info4.flags & 2) != 0;
							Color col = new Color((flag2 && !flag3) ? Mathf.Clamp(2f * num13 * num12, 0f, 1f) : 0.5f, (flag2 && !flag3) ? Mathf.Clamp(2f - 2f * num13 * num12, 0f, 1f) : 0.5f, (!flag3) ? ((!flag2) ? 0.5f : 0f) : 1f, (highlightAudibleDSPChains && !value2.audible) ? 0.4f : 1f);
							text = text.Replace("ChannelGroup", "Group");
							text = text.Replace("FMOD Channel", "Channel");
							text = text.Replace("FMOD WaveTable Unit", "Wavetable");
							text = text.Replace("FMOD Resampler Unit", "Resampler");
							text = text.Replace("FMOD Channel DSPHead Unit", "Channel DSP");
							text = text.Replace("FMOD Channel DSPHead Unit", "Channel DSP");
							text += string.Format(" ({0:0.00}%)", num12);
							this.DrawRectClipped(new Rect((float)value2.x - (float)num5 * 0.5f, (float)value2.y - (float)num6 * 0.5f, (float)num5, (float)num6), col, text, clippingRect, zoomFactor);
							if (value2.audible)
							{
								if (info4.numLevels >= 1)
								{
									float num14 = (float)(num6 - 6) * Mathf.Clamp(info4.level1, 0f, 1f);
									this.DrawRectClipped(new Rect((float)value2.x - (float)num5 * 0.5f + 3f, (float)value2.y - (float)num6 * 0.5f + 3f, 4f, (float)(num6 - 6)), Color.black, null, clippingRect, zoomFactor);
									this.DrawRectClipped(new Rect((float)value2.x - (float)num5 * 0.5f + 3f, (float)value2.y - (float)num6 * 0.5f - 3f + (float)num6 - num14, 4f, num14), Color.red, null, clippingRect, zoomFactor);
								}
								if (info4.numLevels >= 2)
								{
									float num15 = (float)(num6 - 6) * Mathf.Clamp(info4.level2, 0f, 1f);
									this.DrawRectClipped(new Rect((float)value2.x - (float)num5 * 0.5f + 8f, (float)value2.y - (float)num6 * 0.5f + 3f, 4f, (float)(num6 - 6)), Color.black, null, clippingRect, zoomFactor);
									this.DrawRectClipped(new Rect((float)value2.x - (float)num5 * 0.5f + 8f, (float)value2.y - (float)num6 * 0.5f - 3f + (float)num6 - num15, 4f, num15), Color.red, null, clippingRect, zoomFactor);
								}
							}
						}
					}
				}
			}
		}
	}
}
