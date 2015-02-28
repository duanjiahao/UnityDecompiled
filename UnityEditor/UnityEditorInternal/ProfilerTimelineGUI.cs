using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	[Serializable]
	internal class ProfilerTimelineGUI
	{
		internal class Styles
		{
			public GUIStyle background = "OL Box";
			public GUIStyle tooltip = "AnimationEventTooltip";
			public GUIStyle tooltipArrow = "AnimationEventTooltipArrow";
			public GUIStyle bar;
			public GUIStyle profilerGraphBackground = new GUIStyle("ProfilerScrollviewBackground");
			public GUIStyle leftPane = new GUIStyle("ProfilerLeftPane");
			public GUIStyle rightPane = "ProfilerRightPane";
			internal Styles()
			{
				this.bar = new GUIStyle(EditorStyles.miniButton);
				this.bar.margin = new RectOffset(0, 0, 0, 0);
				this.bar.padding = new RectOffset(0, 0, 0, 0);
				this.bar.border = new RectOffset(1, 1, 1, 1);
				GUIStyleState arg_F2_0 = this.bar.normal;
				Texture2D texture2D = EditorGUIUtility.whiteTexture;
				this.bar.active.background = texture2D;
				texture2D = texture2D;
				this.bar.hover.background = texture2D;
				arg_F2_0.background = texture2D;
				GUIStyleState arg_12D_0 = this.bar.normal;
				Color color = Color.black;
				this.bar.active.textColor = color;
				color = color;
				this.bar.hover.textColor = color;
				arg_12D_0.textColor = color;
				this.profilerGraphBackground.overflow.left = -169;
				this.leftPane.padding.left = this.leftPane.padding.left - 40;
				this.leftPane.padding.top = 4;
			}
		}
		private const float kSmallWidth = 7f;
		private const float kTextFadeStartWidth = 50f;
		private const float kTextFadeOutWidth = 20f;
		private const float kTextLongWidth = 200f;
		private const float kLineHeight = 16f;
		private static ProfilerTimelineGUI.Styles ms_Styles;
		[NonSerialized]
		private ZoomableArea m_TimeArea;
		private IProfilerWindowController m_Window;
		private int m_SelectedThread;
		private int m_SelectedID = -1;
		private float m_SelectedTime;
		private float m_SelectedDur;
		private static ProfilerTimelineGUI.Styles styles
		{
			get
			{
				ProfilerTimelineGUI.Styles arg_17_0;
				if ((arg_17_0 = ProfilerTimelineGUI.ms_Styles) == null)
				{
					arg_17_0 = (ProfilerTimelineGUI.ms_Styles = new ProfilerTimelineGUI.Styles());
				}
				return arg_17_0;
			}
		}
		public ProfilerTimelineGUI(IProfilerWindowController window)
		{
			this.m_Window = window;
		}
		private float GetThreadY(Rect r, int thread, int threadCount)
		{
			if (thread > 0)
			{
				thread += 2;
			}
			return r.y + r.height / (float)(threadCount + 2) * (float)thread;
		}
		private void DrawBars(Rect r, int frameIndex)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			profilerFrameDataIterator.SetRoot(frameIndex, 0);
			int threadCount = profilerFrameDataIterator.GetThreadCount(frameIndex);
			if (Event.current.type == EventType.Repaint)
			{
				for (int i = 0; i < threadCount; i++)
				{
					profilerFrameDataIterator.SetRoot(frameIndex, i);
					float threadY = this.GetThreadY(r, i, threadCount);
					float height = this.GetThreadY(r, i + 1, threadCount) - threadY;
					Rect position = new Rect(r.x - 170f, threadY, 170f, height);
					Rect position2 = new Rect(r.x, threadY, r.width, height);
					ProfilerTimelineGUI.styles.rightPane.Draw(position2, false, false, false, false);
					ProfilerTimelineGUI.styles.leftPane.Draw(position, GUIContent.Temp(profilerFrameDataIterator.GetThreadName()), false, false, false, false);
				}
			}
		}
		private void DrawGrid(Rect r, int threadCount, float frameTime)
		{
			float num = 16.66667f;
			HandleUtility.ApplyWireMaterial();
			GL.Begin(1);
			GL.Color(new Color(1f, 1f, 1f, 0.2f));
			float num3;
			for (float num2 = num; num2 <= frameTime; num2 += num)
			{
				num3 = this.m_TimeArea.TimeToPixel(num2, r);
				GL.Vertex3(num3, r.y, 0f);
				GL.Vertex3(num3, r.y + r.height, 0f);
			}
			GL.Color(new Color(1f, 1f, 1f, 0.8f));
			num3 = this.m_TimeArea.TimeToPixel(0f, r);
			GL.Vertex3(num3, r.y, 0f);
			GL.Vertex3(num3, r.y + r.height, 0f);
			num3 = this.m_TimeArea.TimeToPixel(frameTime, r);
			GL.Vertex3(num3, r.y, 0f);
			GL.Vertex3(num3, r.y + r.height, 0f);
			GL.End();
			GUI.color = new Color(1f, 1f, 1f, 0.4f);
			for (float num2 = 0f; num2 <= frameTime; num2 += num)
			{
				num3 = this.m_TimeArea.TimeToPixel(num2, r);
				GUI.Label(new Rect(num3, r.yMax - 16f, 200f, 16f), string.Format("{0:f1}ms ({1:f0}FPS)", num2, 1000f / num2));
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
			num3 = this.m_TimeArea.TimeToPixel(frameTime, r);
			GUI.Label(new Rect(num3, r.yMax - 16f, 200f, 16f), string.Format("{0:f1}ms ({1:f0}FPS)", frameTime, 1000f / frameTime));
		}
		private void DrawSmallGroup(float x1, float x2, float y, int size)
		{
			if (x2 - x1 < 1f)
			{
				return;
			}
			GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
			GUI.contentColor = Color.white;
			GUIContent content = GUIContent.none;
			if (x2 - x1 > 20f)
			{
				content = new GUIContent(size + " items");
			}
			GUI.Label(new Rect(x1, y, x2 - x1, 14f), content, ProfilerTimelineGUI.styles.bar);
		}
		private static float TimeToPixelCached(float time, float rectWidthDivShownWidth, float shownX, float rectX)
		{
			return (time - shownX) * rectWidthDivShownWidth + rectX;
		}
		private void DrawProfilingData(ProfilerFrameDataIterator iter, Rect r, int threadIdx, float timeOffset, bool ghost)
		{
			float num = (!ghost) ? 7f : 21f;
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			bool enterChildren = true;
			Color color = GUI.color;
			Color contentColor = GUI.contentColor;
			Color[] colors = ProfilerColors.colors;
			bool flag = false;
			float x = -1f;
			float num2 = -1f;
			float num3 = -1f;
			int num4 = 0;
			float num5 = -1f;
			string text = null;
			r.height -= 1f;
			GUI.BeginGroup(r);
			float num6 = 0f;
			r.y = num6;
			r.x = num6;
			bool flag2 = Event.current.clickCount == 1 && Event.current.type == EventType.MouseDown;
			bool flag3 = Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown;
			Rect shownArea = this.m_TimeArea.shownArea;
			float rectWidthDivShownWidth = r.width / shownArea.width;
			float x2 = r.x;
			float x3 = shownArea.x;
			while (iter.Next(enterChildren))
			{
				enterChildren = true;
				float num7 = iter.startTimeMS + timeOffset;
				float durationMS = iter.durationMS;
				float num8 = Mathf.Max(durationMS, 0.0003f);
				float num9 = ProfilerTimelineGUI.TimeToPixelCached(num7, rectWidthDivShownWidth, x3, x2);
				float num10 = ProfilerTimelineGUI.TimeToPixelCached(num7 + num8, rectWidthDivShownWidth, x3, x2) - 1f;
				float num11 = num10 - num9;
				if (num9 > r.x + r.width || num10 < r.x)
				{
					enterChildren = false;
				}
				else
				{
					float num12 = (float)(iter.depth - 1);
					float num13 = r.y + num12 * 16f;
					if (flag)
					{
						bool flag4 = false;
						if (num11 >= num)
						{
							flag4 = true;
						}
						if (num3 != num13)
						{
							flag4 = true;
						}
						if (num9 - num2 > 6f)
						{
							flag4 = true;
						}
						if (flag4)
						{
							this.DrawSmallGroup(x, num2, num3, num4);
							flag = false;
						}
					}
					if (num11 < num)
					{
						enterChildren = false;
						if (!flag)
						{
							flag = true;
							num3 = num13;
							x = num9;
							num4 = 0;
						}
						num2 = num10;
						num4++;
					}
					else
					{
						int id = iter.id;
						string path = iter.path;
						bool flag5 = path == selectedPropertyPath && !ghost;
						if (this.m_SelectedID >= 0)
						{
							flag5 &= (id == this.m_SelectedID);
						}
						flag5 &= (threadIdx == this.m_SelectedThread);
						Color white = Color.white;
						Color color2 = colors[iter.group % colors.Length];
						color2.a = ((!flag5) ? 0.75f : 1f);
						if (ghost)
						{
							color2.a = 0.4f;
							white.a = 0.5f;
						}
						string text2 = iter.name;
						if (flag5)
						{
							text = text2;
							this.m_SelectedTime = num7;
							this.m_SelectedDur = durationMS;
							num5 = num13 + 16f;
						}
						if (num11 < 20f)
						{
							text2 = string.Empty;
						}
						else
						{
							if (num11 < 50f && !flag5)
							{
								white.a *= (num11 - 20f) / 30f;
							}
							if (num11 > 200f)
							{
								text2 += string.Format(" ({0:f2}ms)", durationMS);
							}
						}
						GUI.color = color2;
						GUI.contentColor = white;
						Rect position = new Rect(num9, num13, num11, 14f);
						GUI.Label(position, text2, ProfilerTimelineGUI.styles.bar);
						if ((flag2 || flag3) && position.Contains(Event.current.mousePosition))
						{
							this.m_Window.SetSelectedPropertyPath(path);
							this.m_SelectedThread = threadIdx;
							this.m_SelectedID = id;
							UnityEngine.Object @object = EditorUtility.InstanceIDToObject(iter.instanceId);
							if (@object is Component)
							{
								@object = ((Component)@object).gameObject;
							}
							if (@object != null)
							{
								if (flag2)
								{
									EditorGUIUtility.PingObject(@object.GetInstanceID());
								}
								else
								{
									if (flag3)
									{
										Selection.objects = new List<UnityEngine.Object>
										{
											@object
										}.ToArray();
									}
								}
							}
							Event.current.Use();
						}
						flag = false;
					}
				}
			}
			if (flag)
			{
				this.DrawSmallGroup(x, num2, num3, num4);
			}
			GUI.color = color;
			GUI.contentColor = contentColor;
			if (text != null && threadIdx == this.m_SelectedThread)
			{
				string text3 = string.Format(((double)this.m_SelectedDur < 1.0) ? "{0}\n{1:f3}ms" : "{0}\n{1:f2}ms", text, this.m_SelectedDur);
				GUIContent content = new GUIContent(text3);
				GUIStyle tooltip = ProfilerTimelineGUI.styles.tooltip;
				Vector2 vector = tooltip.CalcSize(content);
				float num14 = this.m_TimeArea.TimeToPixel(this.m_SelectedTime + this.m_SelectedDur * 0.5f, r);
				if (num14 < r.x)
				{
					num14 = r.x + 20f;
				}
				if (num14 > r.xMax)
				{
					num14 = r.xMax - 20f;
				}
				Rect position2;
				if (num5 + 6f + vector.y < r.yMax)
				{
					position2 = new Rect(num14 - 32f, num5, 50f, 7f);
					GUI.Label(position2, GUIContent.none, ProfilerTimelineGUI.styles.tooltipArrow);
				}
				position2 = new Rect(num14, num5 + 6f, vector.x, vector.y);
				if (position2.xMax > r.xMax + 20f)
				{
					position2.x = r.xMax - position2.width + 20f;
				}
				if (position2.yMax > r.yMax)
				{
					position2.y = r.yMax - position2.height;
				}
				if (position2.y < r.y)
				{
					position2.y = r.y;
				}
				GUI.Label(position2, content, tooltip);
			}
			if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
			{
				this.m_Window.ClearSelectedPropertyPath();
				this.m_SelectedID = -1;
				this.m_SelectedThread = threadIdx;
				Event.current.Use();
			}
			GUI.EndGroup();
		}
		private void PerformFrameSelected(float frameMS)
		{
			float num = this.m_SelectedTime;
			float num2 = this.m_SelectedDur;
			if (this.m_SelectedID < 0 || num2 <= 0f)
			{
				num = 0f;
				num2 = frameMS;
			}
			this.m_TimeArea.SetShownHRangeInsideMargins(num - num2 * 0.2f, num + num2 * 1.2f);
		}
		private void HandleFrameSelected(float frameMS)
		{
			Event current = Event.current;
			if ((current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand) && current.commandName == "FrameSelected")
			{
				bool flag = current.type == EventType.ExecuteCommand;
				if (flag)
				{
					this.PerformFrameSelected(frameMS);
				}
				current.Use();
			}
		}
		private void DoProfilerFrame(int frameIndex, Rect fullRect, bool ghost, ref int threadCount, float offset)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			int threadCount2 = profilerFrameDataIterator.GetThreadCount(frameIndex);
			if (ghost && threadCount2 != threadCount)
			{
				return;
			}
			if (!ghost)
			{
				threadCount = threadCount2;
			}
			for (int i = 0; i < threadCount; i++)
			{
				Rect r = fullRect;
				r.y = this.GetThreadY(fullRect, i, threadCount);
				r.height = this.GetThreadY(fullRect, i + 1, threadCount) - r.y;
				profilerFrameDataIterator.SetRoot(frameIndex, i);
				if (i == 0 && !ghost)
				{
					this.DrawGrid(fullRect, threadCount, profilerFrameDataIterator.frameTimeMS);
					this.HandleFrameSelected(profilerFrameDataIterator.frameTimeMS);
				}
				this.DrawProfilingData(profilerFrameDataIterator, r, i, offset, ghost);
			}
		}
		public void DoGUI(int frameIndex, float width, float ypos, float height)
		{
			Rect drawRect = new Rect(0f, ypos - 1f, width, height + 1f);
			float num = 169f;
			if (Event.current.type == EventType.Repaint)
			{
				ProfilerTimelineGUI.styles.profilerGraphBackground.Draw(drawRect, false, false, false, false);
				EditorStyles.toolbar.Draw(new Rect(0f, ypos + height - 15f, num, 15f), false, false, false, false);
			}
			bool flag = false;
			if (this.m_TimeArea == null)
			{
				flag = true;
				this.m_TimeArea = new ZoomableArea();
				this.m_TimeArea.hRangeLocked = false;
				this.m_TimeArea.vRangeLocked = true;
				this.m_TimeArea.hSlider = true;
				this.m_TimeArea.vSlider = false;
				this.m_TimeArea.scaleWithWindow = true;
				this.m_TimeArea.rect = new Rect(drawRect.x + num - 1f, drawRect.y, drawRect.width - num, drawRect.height);
				this.m_TimeArea.margin = 10f;
				this.m_TimeArea.OnEnable();
			}
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			profilerFrameDataIterator.SetRoot(frameIndex, 0);
			this.m_TimeArea.hBaseRangeMin = 0f;
			this.m_TimeArea.hBaseRangeMax = profilerFrameDataIterator.frameTimeMS;
			if (flag)
			{
				this.PerformFrameSelected(profilerFrameDataIterator.frameTimeMS);
			}
			this.m_TimeArea.rect = new Rect(drawRect.x + num, drawRect.y, drawRect.width - num, drawRect.height);
			this.m_TimeArea.BeginViewGUI();
			this.m_TimeArea.EndViewGUI();
			drawRect = this.m_TimeArea.drawRect;
			this.DrawBars(drawRect, frameIndex);
			GUI.BeginClip(this.m_TimeArea.drawRect);
			drawRect.x = 0f;
			drawRect.y = 0f;
			int num2 = 0;
			this.DoProfilerFrame(frameIndex, drawRect, false, ref num2, 0f);
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(frameIndex);
			if (previousFrameIndex != -1)
			{
				ProfilerFrameDataIterator profilerFrameDataIterator2 = new ProfilerFrameDataIterator();
				profilerFrameDataIterator2.SetRoot(previousFrameIndex, 0);
				this.DoProfilerFrame(previousFrameIndex, drawRect, true, ref num2, -profilerFrameDataIterator2.frameTimeMS);
			}
			int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(frameIndex);
			if (nextFrameIndex != -1)
			{
				ProfilerFrameDataIterator profilerFrameDataIterator3 = new ProfilerFrameDataIterator();
				profilerFrameDataIterator3.SetRoot(frameIndex, 0);
				this.DoProfilerFrame(nextFrameIndex, drawRect, true, ref num2, profilerFrameDataIterator3.frameTimeMS);
			}
			GUI.enabled = enabled;
			GUI.EndClip();
		}
	}
}
