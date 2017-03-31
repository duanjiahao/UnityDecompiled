using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private class EntryInfo
		{
			public int frameId = -1;

			public int threadId = -1;

			public int nativeIndex = -1;

			public float relativeYPos = 0f;

			public float time = 0f;

			public float duration = 0f;

			public string name = string.Empty;

			public bool IsValid()
			{
				return this.name.Length > 0;
			}

			public bool Equals(int frameId, int threadId, int nativeIndex)
			{
				return frameId == this.frameId && threadId == this.threadId && nativeIndex == this.nativeIndex;
			}

			public virtual void Reset()
			{
				this.frameId = -1;
				this.threadId = -1;
				this.nativeIndex = -1;
				this.relativeYPos = 0f;
				this.time = 0f;
				this.duration = 0f;
				this.name = string.Empty;
			}
		}

		private class SelectedEntryInfo : ProfilerTimelineGUI.EntryInfo
		{
			public int instanceId = -1;

			public string metaData = string.Empty;

			public float totalDuration = -1f;

			public int instanceCount = -1;

			public string allocationInfo = string.Empty;

			public override void Reset()
			{
				base.Reset();
				this.instanceId = -1;
				this.metaData = string.Empty;
				this.totalDuration = -1f;
				this.instanceCount = -1;
				this.allocationInfo = string.Empty;
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

		private ProfilerTimelineGUI.SelectedEntryInfo m_SelectedEntry = new ProfilerTimelineGUI.SelectedEntryInfo();

		private float m_SelectedThreadY = 0f;

		private string m_LocalizedString_Total;

		private string m_LocalizedString_Instances;

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
			this.groups = new List<ProfilerTimelineGUI.GroupInfo>(new ProfilerTimelineGUI.GroupInfo[]
			{
				new ProfilerTimelineGUI.GroupInfo
				{
					name = "",
					height = 20f,
					expanded = true,
					threads = new List<ProfilerTimelineGUI.ThreadInfo>()
				},
				new ProfilerTimelineGUI.GroupInfo
				{
					name = "Unity Job System",
					height = 20f,
					expanded = true,
					threads = new List<ProfilerTimelineGUI.ThreadInfo>()
				},
				new ProfilerTimelineGUI.GroupInfo
				{
					name = "Loading",
					height = 20f,
					expanded = false,
					threads = new List<ProfilerTimelineGUI.ThreadInfo>()
				}
			});
			this.m_LocalizedString_Total = LocalizationDatabase.GetLocalizedString("Total");
			this.m_LocalizedString_Instances = LocalizationDatabase.GetLocalizedString("Instances");
		}

		private void CalculateBars(Rect r, int frameIndex, float time)
		{
			ProfilerFrameDataIterator profilerFrameDataIterator = new ProfilerFrameDataIterator();
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
			int num2 = this.groups.Count((ProfilerTimelineGUI.GroupInfo group) => group.threads.Count > 1);
			float num3 = 20f * (float)num2;
			float num4 = r.height - num3;
			float num5 = num4 / (num + 1f);
			foreach (ProfilerTimelineGUI.GroupInfo current in this.groups)
			{
				foreach (ProfilerTimelineGUI.ThreadInfo current2 in current.threads)
				{
					current2.height = num5 * current2.weight;
				}
			}
			this.groups[0].expanded = true;
			this.groups[0].height = 0f;
			this.groups[0].threads[0].height = 2f * num5;
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
				bool flag = height < 25f;
				GUIContent content = GUIContent.Temp(name);
				if (flag)
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
				if (flag)
				{
					ProfilerTimelineGUI.styles.leftPane.padding.top += (int)(25f - height) / 2;
				}
			}
			bool result;
			if (group)
			{
				position.width -= 1f;
				position.xMin += 1f;
				result = GUI.Toggle(position, expanded, GUIContent.none, ProfilerTimelineGUI.styles.foldout);
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
				if (frameTime > 1000f)
				{
					num = 100f;
				}
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
						if (this.m_SelectedEntry.instanceId >= 0)
						{
							flag5 &= (instanceId == this.m_SelectedEntry.instanceId);
						}
						flag5 &= (threadIndex == this.m_SelectedEntry.threadId);
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
							this.m_SelectedEntry.Reset();
							this.m_SelectedEntry.frameId = frameIndex;
							this.m_SelectedEntry.threadId = threadIndex;
							this.m_SelectedEntry.instanceId = instanceId;
							this.m_SelectedEntry.name = iter.name;
							if (iter.extraTooltipInfo != null)
							{
								this.m_SelectedEntry.metaData = iter.extraTooltipInfo;
							}
							this.m_SelectedEntry.time = num8;
							this.m_SelectedEntry.duration = durationMS;
							this.m_SelectedEntry.relativeYPos = num14 + num5;
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
				Event.current.Use();
			}
			GUI.EndGroup();
		}

		private void DoNativeProfilerTimeline(Rect r, int frameIndex, int threadIndex, float timeOffset, bool ghost)
		{
			Rect rect = r;
			float num = Math.Min(rect.height * 0.25f, 1f);
			float num2 = num + 1f;
			rect.y += num;
			rect.height -= num2;
			GUI.BeginGroup(rect);
			Rect threadRect = rect;
			threadRect.x = 0f;
			threadRect.y = 0f;
			if (Event.current.type == EventType.Repaint)
			{
				this.DrawNativeProfilerTimeline(threadRect, frameIndex, threadIndex, timeOffset, ghost);
			}
			else if (Event.current.type == EventType.MouseDown && !ghost)
			{
				this.HandleNativeProfilerTimelineInput(threadRect, frameIndex, threadIndex, timeOffset, num);
			}
			GUI.EndGroup();
		}

		private void DrawNativeProfilerTimeline(Rect threadRect, int frameIndex, int threadIndex, float timeOffset, bool ghost)
		{
			bool flag = this.m_SelectedEntry.threadId == threadIndex && this.m_SelectedEntry.frameId == frameIndex;
			NativeProfilerTimeline_DrawArgs nativeProfilerTimeline_DrawArgs = default(NativeProfilerTimeline_DrawArgs);
			nativeProfilerTimeline_DrawArgs.Reset();
			nativeProfilerTimeline_DrawArgs.frameIndex = frameIndex;
			nativeProfilerTimeline_DrawArgs.threadIndex = threadIndex;
			nativeProfilerTimeline_DrawArgs.timeOffset = timeOffset;
			nativeProfilerTimeline_DrawArgs.threadRect = threadRect;
			nativeProfilerTimeline_DrawArgs.shownAreaRect = this.m_TimeArea.shownArea;
			nativeProfilerTimeline_DrawArgs.selectedEntryIndex = ((!flag) ? -1 : this.m_SelectedEntry.nativeIndex);
			nativeProfilerTimeline_DrawArgs.mousedOverEntryIndex = -1;
			NativeProfilerTimeline.Draw(ref nativeProfilerTimeline_DrawArgs);
		}

		private void HandleNativeProfilerTimelineInput(Rect threadRect, int frameIndex, int threadIndex, float timeOffset, float topMargin)
		{
			if (threadRect.Contains(Event.current.mousePosition))
			{
				bool flag = Event.current.clickCount == 1 && Event.current.type == EventType.MouseDown;
				bool flag2 = Event.current.clickCount == 2 && Event.current.type == EventType.MouseDown;
				bool flag3 = (flag || flag2) && Event.current.button == 0;
				if (flag3)
				{
					NativeProfilerTimeline_GetEntryAtPositionArgs nativeProfilerTimeline_GetEntryAtPositionArgs = default(NativeProfilerTimeline_GetEntryAtPositionArgs);
					nativeProfilerTimeline_GetEntryAtPositionArgs.Reset();
					nativeProfilerTimeline_GetEntryAtPositionArgs.frameIndex = frameIndex;
					nativeProfilerTimeline_GetEntryAtPositionArgs.threadIndex = threadIndex;
					nativeProfilerTimeline_GetEntryAtPositionArgs.timeOffset = timeOffset;
					nativeProfilerTimeline_GetEntryAtPositionArgs.threadRect = threadRect;
					nativeProfilerTimeline_GetEntryAtPositionArgs.shownAreaRect = this.m_TimeArea.shownArea;
					nativeProfilerTimeline_GetEntryAtPositionArgs.position = Event.current.mousePosition;
					NativeProfilerTimeline.GetEntryAtPosition(ref nativeProfilerTimeline_GetEntryAtPositionArgs);
					int out_EntryIndex = nativeProfilerTimeline_GetEntryAtPositionArgs.out_EntryIndex;
					if (out_EntryIndex != -1)
					{
						bool flag4 = !this.m_SelectedEntry.Equals(frameIndex, threadIndex, out_EntryIndex);
						if (flag4)
						{
							NativeProfilerTimeline_GetEntryTimingInfoArgs nativeProfilerTimeline_GetEntryTimingInfoArgs = default(NativeProfilerTimeline_GetEntryTimingInfoArgs);
							nativeProfilerTimeline_GetEntryTimingInfoArgs.Reset();
							nativeProfilerTimeline_GetEntryTimingInfoArgs.frameIndex = frameIndex;
							nativeProfilerTimeline_GetEntryTimingInfoArgs.threadIndex = threadIndex;
							nativeProfilerTimeline_GetEntryTimingInfoArgs.entryIndex = out_EntryIndex;
							nativeProfilerTimeline_GetEntryTimingInfoArgs.calculateFrameData = true;
							NativeProfilerTimeline.GetEntryTimingInfo(ref nativeProfilerTimeline_GetEntryTimingInfoArgs);
							NativeProfilerTimeline_GetEntryInstanceInfoArgs nativeProfilerTimeline_GetEntryInstanceInfoArgs = default(NativeProfilerTimeline_GetEntryInstanceInfoArgs);
							nativeProfilerTimeline_GetEntryInstanceInfoArgs.Reset();
							nativeProfilerTimeline_GetEntryInstanceInfoArgs.frameIndex = frameIndex;
							nativeProfilerTimeline_GetEntryInstanceInfoArgs.threadIndex = threadIndex;
							nativeProfilerTimeline_GetEntryInstanceInfoArgs.entryIndex = out_EntryIndex;
							NativeProfilerTimeline.GetEntryInstanceInfo(ref nativeProfilerTimeline_GetEntryInstanceInfoArgs);
							this.m_Window.SetSelectedPropertyPath(nativeProfilerTimeline_GetEntryInstanceInfoArgs.out_Path);
							this.m_SelectedEntry.Reset();
							this.m_SelectedEntry.frameId = frameIndex;
							this.m_SelectedEntry.threadId = threadIndex;
							this.m_SelectedEntry.nativeIndex = out_EntryIndex;
							this.m_SelectedEntry.instanceId = nativeProfilerTimeline_GetEntryInstanceInfoArgs.out_Id;
							this.m_SelectedEntry.time = nativeProfilerTimeline_GetEntryTimingInfoArgs.out_LocalStartTime;
							this.m_SelectedEntry.duration = nativeProfilerTimeline_GetEntryTimingInfoArgs.out_Duration;
							this.m_SelectedEntry.totalDuration = nativeProfilerTimeline_GetEntryTimingInfoArgs.out_TotalDurationForFrame;
							this.m_SelectedEntry.instanceCount = nativeProfilerTimeline_GetEntryTimingInfoArgs.out_InstanceCountForFrame;
							this.m_SelectedEntry.relativeYPos = nativeProfilerTimeline_GetEntryAtPositionArgs.out_EntryYMaxPos + topMargin;
							this.m_SelectedEntry.name = nativeProfilerTimeline_GetEntryAtPositionArgs.out_EntryName;
							this.m_SelectedEntry.allocationInfo = nativeProfilerTimeline_GetEntryInstanceInfoArgs.out_AllocationInfo;
							this.m_SelectedEntry.metaData = nativeProfilerTimeline_GetEntryInstanceInfoArgs.out_MetaData;
						}
						Event.current.Use();
						this.UpdateSelectedObject(flag, flag2);
					}
					else if (flag3)
					{
						this.ClearSelection();
						Event.current.Use();
					}
				}
			}
		}

		private void UpdateSelectedObject(bool singleClick, bool doubleClick)
		{
			UnityEngine.Object @object = EditorUtility.InstanceIDToObject(this.m_SelectedEntry.instanceId);
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
			this.m_SelectedEntry.Reset();
		}

		private void PerformFrameSelected(float frameMS)
		{
			float num = this.m_SelectedEntry.time;
			float num2 = this.m_SelectedEntry.duration;
			if (this.m_SelectedEntry.instanceId < 0 || num2 <= 0f)
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
							this.DoNativeProfilerTimeline(r, frameIndex, current2.threadIndex, offset, ghost);
						}
						else
						{
							this.DrawProfilingData(profilerFrameDataIterator, r, frameIndex, current2.threadIndex, offset, ghost, expanded);
						}
						bool flag = this.m_SelectedEntry.IsValid() && this.m_SelectedEntry.frameId == frameIndex && this.m_SelectedEntry.threadId == current2.threadIndex;
						if (flag)
						{
							this.m_SelectedThreadY = num;
						}
						num += r.height;
					}
					if (!expanded)
					{
						num = num2 + current.height;
					}
				}
			}
		}

		private void DoSelectionTooltip(int frameIndex, Rect fullRect, bool detailView)
		{
			if (this.m_SelectedEntry.IsValid() && this.m_SelectedEntry.frameId == frameIndex)
			{
				string arg = string.Format(((double)this.m_SelectedEntry.duration < 1.0) ? "{0:f3}ms" : "{0:f2}ms", this.m_SelectedEntry.duration);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("{0}\n{1}", this.m_SelectedEntry.name, arg));
				if (this.m_SelectedEntry.instanceCount > 1)
				{
					string text = string.Format(((double)this.m_SelectedEntry.totalDuration < 1.0) ? "{0:f3}ms" : "{0:f2}ms", this.m_SelectedEntry.totalDuration);
					stringBuilder.Append(string.Format("\n{0}: {1} ({2} {3})", new object[]
					{
						this.m_LocalizedString_Total,
						text,
						this.m_SelectedEntry.instanceCount,
						this.m_LocalizedString_Instances
					}));
				}
				if (this.m_SelectedEntry.metaData.Length > 0)
				{
					stringBuilder.Append(string.Format("\n{0}", this.m_SelectedEntry.metaData));
				}
				if (this.m_SelectedEntry.allocationInfo.Length > 0)
				{
					stringBuilder.Append(string.Format("\n{0}", this.m_SelectedEntry.allocationInfo));
				}
				float num = fullRect.y + this.m_SelectedThreadY + this.m_SelectedEntry.relativeYPos;
				GUIContent content = new GUIContent(stringBuilder.ToString());
				GUIStyle tooltip = ProfilerTimelineGUI.styles.tooltip;
				Vector2 vector = tooltip.CalcSize(content);
				float num2 = this.m_TimeArea.TimeToPixel(this.m_SelectedEntry.time + this.m_SelectedEntry.duration * 0.5f, fullRect);
				Rect position = new Rect(num2 - 32f, num, 64f, 6f);
				Rect position2 = new Rect(num2, num + 6f, vector.x, vector.y);
				if (position2.xMax > fullRect.xMax + 16f)
				{
					position2.x = fullRect.xMax - position2.width + 16f;
				}
				if (position.xMax > fullRect.xMax + 20f)
				{
					position.x = fullRect.xMax - position.width + 20f;
				}
				if (position2.xMin < fullRect.xMin + 30f)
				{
					position2.x = fullRect.xMin + 30f;
				}
				if (position.xMin < fullRect.xMin - 20f)
				{
					position.x = fullRect.xMin - 20f;
				}
				float num3 = 16f + position2.height + 2f * position.height;
				bool flag = num + vector.y + 6f > fullRect.yMax && position2.y - num3 > 0f;
				if (flag)
				{
					position2.y -= num3;
					position.y -= 16f + 2f * position.height;
				}
				GUI.BeginClip(position);
				Matrix4x4 matrix = GUI.matrix;
				if (flag)
				{
					GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(position.width * 0.5f, position.height));
				}
				GUI.Label(new Rect(0f, 0f, position.width, position.height), GUIContent.none, ProfilerTimelineGUI.styles.tooltipArrow);
				GUI.matrix = matrix;
				GUI.EndClip();
				GUI.Label(position2, content, tooltip);
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
			if (flag)
			{
				NativeProfilerTimeline_InitializeArgs nativeProfilerTimeline_InitializeArgs = default(NativeProfilerTimeline_InitializeArgs);
				nativeProfilerTimeline_InitializeArgs.Reset();
				nativeProfilerTimeline_InitializeArgs.profilerColors = ProfilerColors.colors;
				nativeProfilerTimeline_InitializeArgs.nativeAllocationColor = ProfilerColors.nativeAllocation;
				nativeProfilerTimeline_InitializeArgs.ghostAlpha = 0.3f;
				nativeProfilerTimeline_InitializeArgs.nonSelectedAlpha = 0.75f;
				nativeProfilerTimeline_InitializeArgs.guiStyle = ProfilerTimelineGUI.styles.bar.m_Ptr;
				nativeProfilerTimeline_InitializeArgs.lineHeight = 16f;
				nativeProfilerTimeline_InitializeArgs.textFadeOutWidth = 20f;
				nativeProfilerTimeline_InitializeArgs.textFadeStartWidth = 50f;
				NativeProfilerTimeline.Initialize(ref nativeProfilerTimeline_InitializeArgs);
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
			this.DoSelectionTooltip(frameIndex, this.m_TimeArea.drawRect, detailView);
		}
	}
}
