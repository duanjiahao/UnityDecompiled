using System;
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
			internal Styles()
			{
				this.bar = new GUIStyle(EditorStyles.miniButton);
				this.bar.margin = new RectOffset(0, 0, 0, 0);
				this.bar.padding = new RectOffset(0, 0, 0, 0);
				this.bar.border = new RectOffset(1, 1, 1, 1);
				GUIStyleState arg_B8_0 = this.bar.normal;
				Texture2D texture2D = EditorGUIUtility.whiteTexture;
				this.bar.active.background = texture2D;
				texture2D = texture2D;
				this.bar.hover.background = texture2D;
				arg_B8_0.background = texture2D;
				GUIStyleState arg_F3_0 = this.bar.normal;
				Color color = Color.black;
				this.bar.active.textColor = color;
				color = color;
				this.bar.hover.textColor = color;
				arg_F3_0.textColor = color;
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
		private void DrawGrid(Rect r, int threadCount, float frameTime)
		{
			HandleUtility.handleWireMaterial.SetPass(0);
			float num = 16.66667f;
			GL.Begin(1);
			GL.Color(new Color(1f, 1f, 1f, 0.2f));
			for (int i = 0; i < threadCount; i++)
			{
				float threadY = this.GetThreadY(r, i, threadCount);
				GL.Vertex3(r.x, threadY, 0f);
				GL.Vertex3(r.x + r.width, threadY, 0f);
			}
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
			if (!ghost)
			{
				GUI.Label(new Rect(r.x, r.y + r.height * 0.5f - 8f, r.width, 16f), iter.GetThreadName());
			}
			Rect shownArea = this.m_TimeArea.shownArea;
			float rectWidthDivShownWidth = r.width / shownArea.width;
			float x2 = r.x;
			float x3 = shownArea.x;
			while (iter.Next(enterChildren))
			{
				enterChildren = true;
				float num7 = iter.startTimeMS + timeOffset * 1000f;
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
						bool flag2 = false;
						if (num11 >= num)
						{
							flag2 = true;
						}
						if (num3 != num13)
						{
							flag2 = true;
						}
						if (num9 - num2 > 6f)
						{
							flag2 = true;
						}
						if (flag2)
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
						bool flag3 = path == selectedPropertyPath && !ghost;
						if (this.m_SelectedID >= 0)
						{
							flag3 &= (id == this.m_SelectedID);
						}
						flag3 &= (threadIdx == this.m_SelectedThread);
						Color white = Color.white;
						Color color2 = colors[iter.group % colors.Length];
						color2.a = ((!flag3) ? 0.75f : 1f);
						if (ghost)
						{
							color2.a = 0.4f;
							white.a = 0.5f;
						}
						string text2 = iter.name;
						if (flag3)
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
							if (num11 < 50f && !flag3)
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
						if (GUI.Button(new Rect(num9, num13, num11, 14f), text2, ProfilerTimelineGUI.styles.bar))
						{
							this.m_Window.SetSelectedPropertyPath(path);
							this.m_SelectedThread = threadIdx;
							this.m_SelectedID = id;
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
				Rect position;
				if (num5 + 6f + vector.y < r.yMax)
				{
					position = new Rect(num14 - 32f, num5, 50f, 7f);
					GUI.Label(position, GUIContent.none, ProfilerTimelineGUI.styles.tooltipArrow);
				}
				position = new Rect(num14, num5 + 6f, vector.x, vector.y);
				if (position.xMax > r.xMax + 20f)
				{
					position.x = r.xMax - position.width + 20f;
				}
				if (position.yMax > r.yMax)
				{
					position.y = r.yMax - position.height;
				}
				if (position.y < r.y)
				{
					position.y = r.y;
				}
				GUI.Label(position, content, tooltip);
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
			this.m_TimeArea.SetShownHRange(num - num2 * 0.1f, num + num2 * 1.1f);
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
		private void DoProfilerFrame(int frameIndex, Rect fullRect, bool ghost, ref int threadCount, ref double startTime)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			int threadCount2 = profilerFrameDataIterator.GetThreadCount(frameIndex);
			double frameStartS = profilerFrameDataIterator.GetFrameStartS(frameIndex);
			if (ghost && threadCount2 != threadCount)
			{
				return;
			}
			if (!ghost)
			{
				threadCount = threadCount2;
				startTime = frameStartS;
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
				this.DrawProfilingData(profilerFrameDataIterator, r, i, (float)(frameStartS - startTime), ghost);
			}
		}
		public void DoGUI(int frameIndex, float width, float ypos, float height)
		{
			Rect drawRect = new Rect(0f, ypos, width, height);
			if (this.m_TimeArea == null)
			{
				this.m_TimeArea = new ZoomableArea();
				this.m_TimeArea.hRangeLocked = false;
				this.m_TimeArea.vRangeLocked = true;
				this.m_TimeArea.hSlider = true;
				this.m_TimeArea.vSlider = false;
				this.m_TimeArea.scaleWithWindow = true;
				this.m_TimeArea.rect = drawRect;
				this.m_TimeArea.SetShownHRangeInsideMargins(-2f, 33.3f);
				this.m_TimeArea.OnEnable();
			}
			this.m_TimeArea.rect = drawRect;
			this.m_TimeArea.BeginViewGUI();
			this.m_TimeArea.EndViewGUI();
			drawRect = this.m_TimeArea.drawRect;
			int num = 0;
			double num2 = 0.0;
			this.DoProfilerFrame(frameIndex, drawRect, false, ref num, ref num2);
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(frameIndex);
			if (previousFrameIndex != -1)
			{
				this.DoProfilerFrame(previousFrameIndex, drawRect, true, ref num, ref num2);
			}
			int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(frameIndex);
			if (nextFrameIndex != -1)
			{
				this.DoProfilerFrame(nextFrameIndex, drawRect, true, ref num, ref num2);
			}
			GUI.enabled = enabled;
		}
	}
}
