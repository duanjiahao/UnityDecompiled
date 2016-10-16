using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class ProfilerTimelineGUI
	{
		internal class ThreadInfo
		{
			public float height;

			public float desiredWeight;

			public float weight;

			public int threadIndex;

			public string name;
		}

		internal class GroupInfo
		{
			public bool expanded;

			public string name;

			public float height;

			public List<ProfilerTimelineGUI.ThreadInfo> threads;
		}

		internal class Styles
		{
			public GUIStyle background = "OL Box";

			public GUIStyle tooltip = "AnimationEventTooltip";

			public GUIStyle tooltipArrow = "AnimationEventTooltipArrow";

			public GUIStyle bar = "ProfilerTimelineBar";

			public GUIStyle leftPane = "ProfilerTimelineLeftPane";

			public GUIStyle rightPane = "ProfilerRightPane";

			public GUIStyle foldout = "ProfilerTimelineFoldout";

			public GUIStyle profilerGraphBackground = new GUIStyle("ProfilerScrollviewBackground");

			internal Styles()
			{
				GUIStyleState arg_C1_0 = this.bar.normal;
				Texture2D texture2D = EditorGUIUtility.whiteTexture;
				this.bar.active.background = texture2D;
				texture2D = texture2D;
				this.bar.hover.background = texture2D;
				arg_C1_0.background = texture2D;
				GUIStyleState arg_FC_0 = this.bar.normal;
				Color color = Color.black;
				this.bar.active.textColor = color;
				color = color;
				this.bar.hover.textColor = color;
				arg_FC_0.textColor = color;
				this.profilerGraphBackground.overflow.left = -169;
				this.leftPane.padding.left = 15;
			}
		}

		private const float kSmallWidth = 7f;

		private const float kTextFadeStartWidth = 50f;

		private const float kTextFadeOutWidth = 20f;

		private const float kTextLongWidth = 200f;

		private const float kLineHeight = 16f;

		private const float kGroupHeight = 20f;

		private float animationTime = 1f;

		private double lastScrollUpdate;

		private List<ProfilerTimelineGUI.GroupInfo> groups;

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
			this.groups = new List<ProfilerTimelineGUI.GroupInfo>();
		}

		private void CalculateBars(Rect r, int frameIndex, float time)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			int groupCount = profilerFrameDataIterator.GetGroupCount(frameIndex);
			float num = 0f;
			profilerFrameDataIterator.SetRoot(frameIndex, 0);
			int threadCount = profilerFrameDataIterator.GetThreadCount(frameIndex);
			int i;
			for (i = 0; i < threadCount; i++)
			{
				profilerFrameDataIterator.SetRoot(frameIndex, i);
				string groupname = profilerFrameDataIterator.GetGroupName();
				ProfilerTimelineGUI.GroupInfo groupInfo = this.groups.Find((ProfilerTimelineGUI.GroupInfo g) => g.name == groupname);
				if (groupInfo == null)
				{
					groupInfo = new ProfilerTimelineGUI.GroupInfo();
					groupInfo.name = groupname;
					groupInfo.height = 20f;
					groupInfo.expanded = false;
					groupInfo.threads = new List<ProfilerTimelineGUI.ThreadInfo>();
					this.groups.Add(groupInfo);
					if (groupname == string.Empty || groupname == "Unity Job System")
					{
						groupInfo.expanded = true;
					}
				}
				List<ProfilerTimelineGUI.ThreadInfo> threads = groupInfo.threads;
				ProfilerTimelineGUI.ThreadInfo threadInfo = threads.Find((ProfilerTimelineGUI.ThreadInfo t) => t.threadIndex == i);
				if (threadInfo == null)
				{
					threadInfo = new ProfilerTimelineGUI.ThreadInfo();
					threadInfo.name = profilerFrameDataIterator.GetThreadName();
					threadInfo.height = 0f;
					threadInfo.weight = (threadInfo.desiredWeight = (float)((!groupInfo.expanded) ? 0 : 1));
					threadInfo.threadIndex = i;
					groupInfo.threads.Add(threadInfo);
				}
				if (threadInfo.weight != threadInfo.desiredWeight)
				{
					threadInfo.weight = threadInfo.desiredWeight * time + (1f - threadInfo.desiredWeight) * (1f - time);
				}
				num += threadInfo.weight;
			}
			float num2 = 20f * (float)groupCount;
			float num3 = r.height - num2;
			float num4 = num3 / (num + 2f);
			foreach (ProfilerTimelineGUI.GroupInfo current in this.groups)
			{
				foreach (ProfilerTimelineGUI.ThreadInfo current2 in current.threads)
				{
					current2.height = num4 * current2.weight;
				}
			}
			this.groups[0].expanded = true;
			this.groups[0].height = 0f;
			this.groups[0].threads[0].height = 3f * num4;
		}

		private void UpdateAnimatedFoldout()
		{
			double num = EditorApplication.timeSinceStartup - this.lastScrollUpdate;
			this.animationTime = Math.Min(1f, this.animationTime + (float)num);
			this.m_Window.Repaint();
			if (this.animationTime == 1f)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
			}
		}

		private bool DrawBar(Rect r, float y, float height, string name, bool group, bool expanded, bool indent)
		{
			Rect position = new Rect(r.x - 170f, y, 170f, height);
			Rect position2 = new Rect(r.x, y, r.width, height);
			if (Event.current.type == EventType.Repaint)
			{
				ProfilerTimelineGUI.styles.rightPane.Draw(position2, false, false, false, false);
				bool flag = height < 10f;
				bool flag2 = height < 25f;
				GUIContent content = (!group && !flag) ? GUIContent.Temp(name) : GUIContent.none;
				if (flag2)
				{
					ProfilerTimelineGUI.styles.leftPane.padding.top -= (int)(25f - height) / 2;
				}
				if (indent)
				{
					ProfilerTimelineGUI.styles.leftPane.padding.left += 10;
				}
				ProfilerTimelineGUI.styles.leftPane.Draw(position, content, false, false, false, false);
				if (indent)
				{
					ProfilerTimelineGUI.styles.leftPane.padding.left -= 10;
				}
				if (flag2)
				{
					ProfilerTimelineGUI.styles.leftPane.padding.top += (int)(25f - height) / 2;
				}
			}
			if (group)
			{
				position.width -= 1f;
				position.xMin += 1f;
				return GUI.Toggle(position, expanded, GUIContent.Temp(name), ProfilerTimelineGUI.styles.foldout);
			}
			return false;
		}

		private void DrawBars(Rect r, int frameIndex)
		{
			float num = r.y;
			foreach (ProfilerTimelineGUI.GroupInfo current in this.groups)
			{
				bool flag = current.name == string.Empty;
				if (!flag)
				{
					float height = current.height;
					bool expanded = current.expanded;
					current.expanded = this.DrawBar(r, num, height, current.name, true, expanded, false);
					if (current.expanded != expanded)
					{
						this.animationTime = 0f;
						this.lastScrollUpdate = EditorApplication.timeSinceStartup;
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedFoldout));
						foreach (ProfilerTimelineGUI.ThreadInfo current2 in current.threads)
						{
							current2.desiredWeight = ((!current.expanded) ? 0f : 1f);
						}
					}
					num += height;
				}
				foreach (ProfilerTimelineGUI.ThreadInfo current3 in current.threads)
				{
					float height2 = current3.height;
					if (height2 != 0f)
					{
						this.DrawBar(r, num, height2, current3.name, false, true, !flag);
					}
					num += height2;
				}
			}
		}

		private void DrawGrid(Rect r, int threadCount, float frameTime)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
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
				Chart.DoLabel(num3 + 2f, r.yMax - 12f, string.Format("{0:f1}ms", num2), 0f);
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
			num3 = this.m_TimeArea.TimeToPixel(frameTime, r);
			Chart.DoLabel(num3 + 2f, r.yMax - 12f, string.Format("{0:f1}ms ({1:f0}FPS)", frameTime, 1000f / frameTime), 0f);
		}

		private void DrawSmallGroup(float x1, float x2, float y, float height, int size)
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
			GUI.Label(new Rect(x1, y, x2 - x1, height), content, ProfilerTimelineGUI.styles.bar);
		}

		private static float TimeToPixelCached(float time, float rectWidthDivShownWidth, float shownX, float rectX)
		{
			return (time - shownX) * rectWidthDivShownWidth + rectX;
		}

		private void DrawProfilingData(ProfilerFrameDataIterator iter, Rect r, int threadIdx, float timeOffset, bool ghost, bool includeSubSamples)
		{
			float num = (!ghost) ? 7f : 21f;
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
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
			float num6 = (!includeSubSamples) ? r.height : 16f;
			float num7 = (float)((!includeSubSamples) ? 0 : 1);
			float height = num6 - 2f * num7;
			r.height -= num7;
			GUI.BeginGroup(r);
			float num8 = 0f;
			r.y = num8;
			r.x = num8;
			bool flag2 = Event.current.clickCount == 1 && Event.current.type == EventType.MouseDown;
			bool flag3 = Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown;
			Rect shownArea = this.m_TimeArea.shownArea;
			float rectWidthDivShownWidth = r.width / shownArea.width;
			float x2 = r.x;
			float x3 = shownArea.x;
			bool enterChildren = true;
			while (iter.Next(enterChildren))
			{
				enterChildren = includeSubSamples;
				float num9 = iter.startTimeMS + timeOffset;
				float durationMS = iter.durationMS;
				float num10 = Mathf.Max(durationMS, 0.0003f);
				float num11 = ProfilerTimelineGUI.TimeToPixelCached(num9, rectWidthDivShownWidth, x3, x2);
				float num12 = ProfilerTimelineGUI.TimeToPixelCached(num9 + num10, rectWidthDivShownWidth, x3, x2) - 1f;
				float num13 = num12 - num11;
				if (num11 > r.x + r.width || num12 < r.x)
				{
					enterChildren = false;
				}
				else
				{
					float num14 = (float)(iter.depth - 1);
					float num15 = r.y + num14 * num6;
					if (flag)
					{
						bool flag4 = false;
						if (num13 >= num)
						{
							flag4 = true;
						}
						if (num3 != num15)
						{
							flag4 = true;
						}
						if (num11 - num2 > 6f)
						{
							flag4 = true;
						}
						if (flag4)
						{
							this.DrawSmallGroup(x, num2, num3, height, num4);
							flag = false;
						}
					}
					if (num13 < num)
					{
						enterChildren = false;
						if (!flag)
						{
							flag = true;
							num3 = num15;
							x = num11;
							num4 = 0;
						}
						num2 = num12;
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
							this.m_SelectedTime = num9;
							this.m_SelectedDur = durationMS;
							num5 = num15 + num6;
						}
						if (num13 < 20f || !includeSubSamples)
						{
							text2 = string.Empty;
						}
						else
						{
							if (num13 < 50f && !flag5)
							{
								white.a *= (num13 - 20f) / 30f;
							}
							if (num13 > 200f)
							{
								text2 += string.Format(" ({0:f2}ms)", durationMS);
							}
						}
						GUI.color = color2;
						GUI.contentColor = white;
						Rect position = new Rect(num11, num15, num13, height);
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
								else if (flag3)
								{
									Selection.objects = new List<UnityEngine.Object>
									{
										@object
									}.ToArray();
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
				this.DrawSmallGroup(x, num2, num3, height, num4);
			}
			GUI.color = color;
			GUI.contentColor = contentColor;
			if (text != null && threadIdx == this.m_SelectedThread && includeSubSamples)
			{
				string text3 = string.Format(((double)this.m_SelectedDur < 1.0) ? "{0}\n{1:f3}ms" : "{0}\n{1:f2}ms", text, this.m_SelectedDur);
				GUIContent content = new GUIContent(text3);
				GUIStyle tooltip = ProfilerTimelineGUI.styles.tooltip;
				Vector2 vector = tooltip.CalcSize(content);
				float num16 = this.m_TimeArea.TimeToPixel(this.m_SelectedTime + this.m_SelectedDur * 0.5f, r);
				if (num16 < r.x)
				{
					num16 = r.x + 20f;
				}
				if (num16 > r.xMax)
				{
					num16 = r.xMax - 20f;
				}
				Rect position2;
				if (num5 + 6f + vector.y < r.yMax)
				{
					position2 = new Rect(num16 - 32f, num5, 50f, 7f);
					GUI.Label(position2, GUIContent.none, ProfilerTimelineGUI.styles.tooltipArrow);
				}
				position2 = new Rect(num16, num5 + 6f, vector.x, vector.y);
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
			profilerFrameDataIterator.SetRoot(frameIndex, 0);
			if (!ghost)
			{
				threadCount = threadCount2;
				this.DrawGrid(fullRect, threadCount, profilerFrameDataIterator.frameTimeMS);
				this.HandleFrameSelected(profilerFrameDataIterator.frameTimeMS);
			}
			float num = fullRect.y;
			foreach (ProfilerTimelineGUI.GroupInfo current in this.groups)
			{
				Rect r = fullRect;
				bool expanded = current.expanded;
				if (expanded)
				{
					num += current.height;
				}
				float num2 = num;
				int count = current.threads.Count;
				foreach (ProfilerTimelineGUI.ThreadInfo current2 in current.threads)
				{
					profilerFrameDataIterator.SetRoot(frameIndex, current2.threadIndex);
					r.y = num;
					r.height = ((!expanded) ? Math.Max(current.height / (float)count - 1f, 2f) : current2.height);
					this.DrawProfilingData(profilerFrameDataIterator, r, current2.threadIndex, offset, ghost, expanded);
					num += r.height;
				}
				if (!expanded)
				{
					num = num2 + current.height;
				}
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
			this.CalculateBars(drawRect, frameIndex, this.animationTime);
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
