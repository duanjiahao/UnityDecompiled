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
				GUIStyleState arg_C2_0 = this.bar.normal;
				Texture2D texture2D = EditorGUIUtility.whiteTexture;
				this.bar.active.background = texture2D;
				texture2D = texture2D;
				this.bar.hover.background = texture2D;
				arg_C2_0.background = texture2D;
				GUIStyleState arg_FD_0 = this.bar.normal;
				Color color = Color.black;
				this.bar.active.textColor = color;
				color = color;
				this.bar.hover.textColor = color;
				arg_FD_0.textColor = color;
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

		private double lastScrollUpdate = 0.0;

		private List<ProfilerTimelineGUI.GroupInfo> groups;

		private static ProfilerTimelineGUI.Styles ms_Styles;

		[NonSerialized]
		private ZoomableArea m_TimeArea;

		private IProfilerWindowController m_Window;

		private int m_SelectedFrameId = -1;

		private int m_SelectedThreadId = 0;

		private int m_SelectedInstanceId = -1;

		private float m_SelectedTime = 0f;

		private float m_SelectedDur = 0f;

		private float m_SelectedY = 0f;

		private string m_SelectedName = string.Empty;

		private Rect m_SelectedRect = Rect.zero;

		private static ProfilerTimelineGUI.Styles styles
		{
			get
			{
				ProfilerTimelineGUI.Styles arg_18_0;
				if ((arg_18_0 = ProfilerTimelineGUI.ms_Styles) == null)
				{
					arg_18_0 = (ProfilerTimelineGUI.ms_Styles = new ProfilerTimelineGUI.Styles());
				}
				return arg_18_0;
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
					if (groupname == "" || groupname == "Unity Job System")
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
			bool result;
			if (group)
			{
				position.width -= 1f;
				position.xMin += 1f;
				result = GUI.Toggle(position, expanded, GUIContent.Temp(name), ProfilerTimelineGUI.styles.foldout);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void DrawBars(Rect r, int frameIndex)
		{
			float num = r.y;
			foreach (ProfilerTimelineGUI.GroupInfo current in this.groups)
			{
				bool flag = current.name == "";
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
			if (Event.current.type == EventType.Repaint)
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
					Chart.DoLabel(num3 + 2f, r.yMax - 12f, string.Format("{0:f1}ms", num2), 0f);
				}
				GUI.color = new Color(1f, 1f, 1f, 1f);
				num3 = this.m_TimeArea.TimeToPixel(frameTime, r);
				Chart.DoLabel(num3 + 2f, r.yMax - 12f, string.Format("{0:f1}ms ({1:f0}FPS)", frameTime, 1000f / frameTime), 0f);
			}
		}

		private void DrawSmallGroup(float x1, float x2, float y, float height, int size)
		{
			if (x2 - x1 >= 1f)
			{
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
				GUI.contentColor = Color.white;
				GUIContent content = GUIContent.none;
				if (x2 - x1 > 20f)
				{
					content = new GUIContent(size + " items");
				}
				GUI.Label(new Rect(x1, y, x2 - x1, height), content, ProfilerTimelineGUI.styles.bar);
			}
		}

		private static float TimeToPixelCached(float time, float rectWidthDivShownWidth, float shownX, float rectX)
		{
			return (time - shownX) * rectWidthDivShownWidth + rectX;
		}

		private void DrawProfilingData(ProfilerFrameDataIterator iter, Rect r, int frameIndex, int threadIndex, float timeOffset, bool ghost, bool includeSubSamples)
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
			float num5 = (!includeSubSamples) ? r.height : 16f;
			float num6 = (float)((!includeSubSamples) ? 0 : 1);
			float height = num5 - 2f * num6;
			r.height -= num6;
			GUI.BeginGroup(r);
			float y = r.y;
			float num7 = 0f;
			r.y = num7;
			r.x = num7;
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
				float num8 = iter.startTimeMS + timeOffset;
				float durationMS = iter.durationMS;
				float num9 = Mathf.Max(durationMS, 0.0003f);
				float num10 = ProfilerTimelineGUI.TimeToPixelCached(num8, rectWidthDivShownWidth, x3, x2);
				float num11 = ProfilerTimelineGUI.TimeToPixelCached(num8 + num9, rectWidthDivShownWidth, x3, x2) - 1f;
				float num12 = num11 - num10;
				if (num10 > r.x + r.width || num11 < r.x)
				{
					enterChildren = false;
				}
				else
				{
					float num13 = (float)(iter.depth - 1);
					float num14 = r.y + num13 * num5;
					if (flag)
					{
						bool flag4 = false;
						if (num12 >= num)
						{
							flag4 = true;
						}
						if (num3 != num14)
						{
							flag4 = true;
						}
						if (num10 - num2 > 6f)
						{
							flag4 = true;
						}
						if (flag4)
						{
							this.DrawSmallGroup(x, num2, num3, height, num4);
							flag = false;
						}
					}
					if (num12 < num)
					{
						enterChildren = false;
						if (!flag)
						{
							flag = true;
							num3 = num14;
							x = num10;
							num4 = 0;
						}
						num2 = num11;
						num4++;
					}
					else
					{
						int instanceId = iter.instanceId;
						string path = iter.path;
						bool flag5 = path == selectedPropertyPath && !ghost;
						if (this.m_SelectedInstanceId >= 0)
						{
							flag5 &= (instanceId == this.m_SelectedInstanceId);
						}
						flag5 &= (threadIndex == this.m_SelectedThreadId);
						Color white = Color.white;
						Color color2 = colors[iter.group % colors.Length];
						color2.a = ((!flag5) ? 0.75f : 1f);
						if (ghost)
						{
							color2.a = 0.4f;
							white.a = 0.5f;
						}
						string text = iter.name;
						if (num12 < 20f || !includeSubSamples)
						{
							text = string.Empty;
						}
						else
						{
							if (num12 < 50f && !flag5)
							{
								white.a *= (num12 - 20f) / 30f;
							}
							if (num12 > 200f)
							{
								text += string.Format(" ({0:f2}ms)", durationMS);
							}
						}
						GUI.color = color2;
						GUI.contentColor = white;
						Rect position = new Rect(num10, num14, num12, height);
						GUI.Label(position, text, ProfilerTimelineGUI.styles.bar);
						if ((flag2 || flag3) && position.Contains(Event.current.mousePosition) && includeSubSamples)
						{
							this.m_Window.SetSelectedPropertyPath(path);
							this.m_SelectedFrameId = frameIndex;
							this.m_SelectedThreadId = threadIndex;
							this.m_SelectedInstanceId = instanceId;
							this.m_SelectedName = iter.name;
							this.m_SelectedTime = num8;
							this.m_SelectedDur = durationMS;
							this.m_SelectedRect = r;
							this.m_SelectedY = y + num14 + num5;
							this.UpdateSelectedObject(flag2, flag3);
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
			if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
			{
				this.ClearSelection();
			}
			GUI.EndGroup();
		}

		private void DrawProfilingDataDetailNative(Rect r, int frameIndex, int threadIndex, float timeOffset)
		{
			bool flag = Event.current.clickCount == 1 && Event.current.type == EventType.MouseDown;
			bool flag2 = Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown;
			bool flag3 = r.Contains(Event.current.mousePosition);
			GUI.BeginGroup(r);
			ProfilingDataDrawNativeInfo profilingDataDrawNativeInfo = default(ProfilingDataDrawNativeInfo);
			profilingDataDrawNativeInfo.Reset();
			profilingDataDrawNativeInfo.trySelect = ((!flag && !flag2) ? 0 : 1);
			profilingDataDrawNativeInfo.frameIndex = frameIndex;
			profilingDataDrawNativeInfo.threadIndex = threadIndex;
			profilingDataDrawNativeInfo.timeOffset = timeOffset;
			profilingDataDrawNativeInfo.threadRect = r;
			profilingDataDrawNativeInfo.shownAreaRect = this.m_TimeArea.shownArea;
			profilingDataDrawNativeInfo.mousePos = Event.current.mousePosition;
			profilingDataDrawNativeInfo.profilerColors = ProfilerColors.colors;
			profilingDataDrawNativeInfo.nativeAllocationColor = ProfilerColors.nativeAllocation;
			profilingDataDrawNativeInfo.ghostAlpha = 0.3f;
			profilingDataDrawNativeInfo.nonSelectedAlpha = 0.75f;
			profilingDataDrawNativeInfo.guiStyle = ProfilerTimelineGUI.styles.bar.m_Ptr;
			profilingDataDrawNativeInfo.lineHeight = 16f;
			profilingDataDrawNativeInfo.textFadeOutWidth = 20f;
			profilingDataDrawNativeInfo.textFadeStartWidth = 50f;
			ProfilerDraw.DrawNative(ref profilingDataDrawNativeInfo);
			if (flag || flag2)
			{
				if (profilingDataDrawNativeInfo.out_SelectedPath.Length > 0)
				{
					this.m_Window.SetSelectedPropertyPath(profilingDataDrawNativeInfo.out_SelectedPath);
					this.m_SelectedFrameId = frameIndex;
					this.m_SelectedThreadId = threadIndex;
					this.m_SelectedInstanceId = profilingDataDrawNativeInfo.out_SelectedInstanceId;
					this.m_SelectedTime = profilingDataDrawNativeInfo.out_SelectedTime;
					this.m_SelectedDur = profilingDataDrawNativeInfo.out_SelectedDur;
					this.m_SelectedY = r.y + profilingDataDrawNativeInfo.out_SelectedY;
					this.m_SelectedName = profilingDataDrawNativeInfo.out_SelectedName;
					this.m_SelectedRect = r;
					this.UpdateSelectedObject(flag, flag2);
					Event.current.Use();
				}
				else if (flag3)
				{
					this.ClearSelection();
				}
			}
			GUI.EndGroup();
		}

		private void UpdateSelectedObject(bool singleClick, bool doubleClick)
		{
			UnityEngine.Object @object = EditorUtility.InstanceIDToObject(this.m_SelectedInstanceId);
			if (@object is Component)
			{
				@object = ((Component)@object).gameObject;
			}
			if (@object != null)
			{
				if (singleClick)
				{
					EditorGUIUtility.PingObject(@object.GetInstanceID());
				}
				else if (doubleClick)
				{
					Selection.objects = new List<UnityEngine.Object>
					{
						@object
					}.ToArray();
				}
			}
		}

		private void ClearSelection()
		{
			this.m_Window.ClearSelectedPropertyPath();
			this.m_SelectedFrameId = -1;
			this.m_SelectedThreadId = 0;
			this.m_SelectedInstanceId = -1;
			this.m_SelectedTime = 0f;
			this.m_SelectedDur = 0f;
			this.m_SelectedY = 0f;
			this.m_SelectedName = string.Empty;
			this.m_SelectedRect = Rect.zero;
			Event.current.Use();
		}

		private void PerformFrameSelected(float frameMS)
		{
			float num = this.m_SelectedTime;
			float num2 = this.m_SelectedDur;
			if (this.m_SelectedInstanceId < 0 || num2 <= 0f)
			{
				num = 0f;
				num2 = frameMS;
			}
			this.m_TimeArea.SetShownHRangeInsideMargins(num - num2 * 0.2f, num + num2 * 1.2f);
		}

		private void HandleFrameSelected(float frameMS)
		{
			Event current = Event.current;
			if (current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand)
			{
				if (current.commandName == "FrameSelected")
				{
					bool flag = current.type == EventType.ExecuteCommand;
					if (flag)
					{
						this.PerformFrameSelected(frameMS);
					}
					current.Use();
				}
			}
		}

		private void DoProfilerFrame(int frameIndex, Rect fullRect, bool ghost, int threadCount, float offset, bool detailView)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
			int threadCount2 = profilerFrameDataIterator.GetThreadCount(frameIndex);
			if (!ghost || threadCount2 == threadCount)
			{
				profilerFrameDataIterator.SetRoot(frameIndex, 0);
				if (!ghost)
				{
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
						if (detailView)
						{
							this.DrawProfilingDataDetailNative(r, frameIndex, current2.threadIndex, offset);
						}
						else
						{
							this.DrawProfilingData(profilerFrameDataIterator, r, frameIndex, current2.threadIndex, offset, ghost, expanded);
						}
						num += r.height;
					}
					if (!expanded)
					{
						num = num2 + current.height;
					}
				}
				if (this.m_SelectedName.Length > 0 && this.m_SelectedFrameId == frameIndex && !ghost)
				{
					string text = string.Format(((double)this.m_SelectedDur < 1.0) ? "{0}\n{1:f3}ms" : "{0}\n{1:f2}ms", this.m_SelectedName, this.m_SelectedDur);
					GUIContent content = new GUIContent(text);
					GUIStyle tooltip = ProfilerTimelineGUI.styles.tooltip;
					Vector2 vector = tooltip.CalcSize(content);
					float num3 = this.m_TimeArea.TimeToPixel(this.m_SelectedTime + this.m_SelectedDur * 0.5f, this.m_SelectedRect);
					if (num3 > this.m_SelectedRect.xMax)
					{
						num3 = this.m_SelectedRect.xMax - 20f;
					}
					if (num3 < this.m_SelectedRect.x)
					{
						num3 = this.m_SelectedRect.x + 20f;
					}
					Rect position = new Rect(num3 - 32f, this.m_SelectedY, 50f, 7f);
					Rect position2 = new Rect(num3, this.m_SelectedY + 6f, vector.x, vector.y);
					if (position2.xMax > this.m_SelectedRect.xMax + 20f)
					{
						position2.x = this.m_SelectedRect.xMax - position2.width + 20f;
					}
					if (position2.xMin < this.m_SelectedRect.xMin + 30f)
					{
						position2.x = this.m_SelectedRect.xMin + 30f;
					}
					GUI.Label(position, GUIContent.none, ProfilerTimelineGUI.styles.tooltipArrow);
					GUI.Label(position2, content, tooltip);
				}
			}
		}

		public void DoGUI(int frameIndex, float width, float ypos, float height, bool detailView)
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
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			ProfilerFrameDataIterator profilerFrameDataIterator2 = new ProfilerFrameDataIterator();
			int threadCount = profilerFrameDataIterator2.GetThreadCount(frameIndex);
			int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(frameIndex);
			if (previousFrameIndex != -1)
			{
				profilerFrameDataIterator2.SetRoot(previousFrameIndex, 0);
				this.DoProfilerFrame(previousFrameIndex, drawRect, true, threadCount, -profilerFrameDataIterator2.frameTimeMS, detailView);
			}
			int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(frameIndex);
			if (nextFrameIndex != -1)
			{
				profilerFrameDataIterator2.SetRoot(frameIndex, 0);
				this.DoProfilerFrame(nextFrameIndex, drawRect, true, threadCount, profilerFrameDataIterator2.frameTimeMS, detailView);
			}
			GUI.enabled = enabled;
			threadCount = 0;
			this.DoProfilerFrame(frameIndex, drawRect, false, threadCount, 0f, detailView);
			GUI.EndClip();
		}
	}
}
