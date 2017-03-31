using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class EventManipulationHandler
	{
		private class EventModificationContextMenuObject
		{
			public AnimationClipInfoProperties m_Info;

			public float m_Time;

			public int m_Index;

			public bool[] m_Selected;

			public EventModificationContextMenuObject(AnimationClipInfoProperties info, float time, int index, bool[] selected)
			{
				this.m_Info = info;
				this.m_Time = time;
				this.m_Index = index;
				this.m_Selected = selected;
			}
		}

		private Rect[] m_EventRects = new Rect[0];

		private static AnimationEvent[] m_EventsAtMouseDown;

		private static float[] m_EventTimes;

		private int m_HoverEvent = -1;

		private string m_InstantTooltipText = null;

		private Vector2 m_InstantTooltipPoint = Vector2.zero;

		private bool[] m_EventsSelected;

		private AnimationWindowEvent[] m_Events;

		private TimeArea m_Timeline;

		public EventManipulationHandler(TimeArea timeArea)
		{
			this.m_Timeline = timeArea;
		}

		public void SelectEvent(AnimationEvent[] events, int index, AnimationClipInfoProperties clipInfo)
		{
			this.m_EventsSelected = new bool[events.Length];
			this.m_EventsSelected[index] = true;
			this.EditEvents(clipInfo, this.m_EventsSelected);
		}

		public bool HandleEventManipulation(Rect rect, ref AnimationEvent[] events, AnimationClipInfoProperties clipInfo)
		{
			Texture image = EditorGUIUtility.IconContent("Animation.EventMarker").image;
			bool result = false;
			Rect[] array = new Rect[events.Length];
			Rect[] array2 = new Rect[events.Length];
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < events.Length; i++)
			{
				AnimationEvent animationEvent = events[i];
				if (num2 == 0)
				{
					num = 1;
					while (i + num < events.Length && events[i + num].time == animationEvent.time)
					{
						num++;
					}
					num2 = num;
				}
				num2--;
				float num3 = Mathf.Floor(this.m_Timeline.TimeToPixel(animationEvent.time, rect));
				int num4 = 0;
				if (num > 1)
				{
					float num5 = (float)Mathf.Min((num - 1) * (image.width - 1), (int)(1f / this.m_Timeline.PixelDeltaToTime(rect) - (float)(image.width * 2)));
					num4 = Mathf.FloorToInt(Mathf.Max(0f, num5 - (float)((image.width - 1) * num2)));
				}
				Rect rect2 = new Rect(num3 + (float)num4 - (float)(image.width / 2), (rect.height - 10f) * (float)(num2 - num + 1) / (float)Mathf.Max(1, num - 1), (float)image.width, (float)image.height);
				array[i] = rect2;
				array2[i] = rect2;
			}
			this.m_EventRects = new Rect[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				this.m_EventRects[j] = new Rect(array[j].x + rect.x, array[j].y + rect.y, array[j].width, array[j].height);
			}
			if (this.m_EventsSelected == null || this.m_EventsSelected.Length != events.Length || this.m_EventsSelected.Length == 0)
			{
				this.m_EventsSelected = new bool[events.Length];
				this.m_Events = null;
			}
			Vector2 zero = Vector2.zero;
			int num6;
			float num7;
			float num8;
			HighLevelEvent highLevelEvent = EditorGUIExt.MultiSelection(rect, array2, new GUIContent(image), array, ref this.m_EventsSelected, null, out num6, out zero, out num7, out num8, GUIStyle.none);
			if (highLevelEvent != HighLevelEvent.None)
			{
				switch (highLevelEvent)
				{
				case HighLevelEvent.ContextClick:
				{
					int num9 = this.m_EventsSelected.Count((bool selected) => selected);
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new EventManipulationHandler.EventModificationContextMenuObject(clipInfo, events[num6].time, num6, this.m_EventsSelected));
					genericMenu.AddItem(new GUIContent((num9 <= 1) ? "Delete Animation Event" : "Delete Animation Events"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), new EventManipulationHandler.EventModificationContextMenuObject(clipInfo, events[num6].time, num6, this.m_EventsSelected));
					genericMenu.ShowAsContext();
					this.m_InstantTooltipText = null;
					break;
				}
				case HighLevelEvent.BeginDrag:
					EventManipulationHandler.m_EventsAtMouseDown = events;
					EventManipulationHandler.m_EventTimes = new float[events.Length];
					for (int k = 0; k < events.Length; k++)
					{
						EventManipulationHandler.m_EventTimes[k] = events[k].time;
					}
					break;
				case HighLevelEvent.Drag:
				{
					for (int l = events.Length - 1; l >= 0; l--)
					{
						if (this.m_EventsSelected[l])
						{
							AnimationEvent animationEvent2 = EventManipulationHandler.m_EventsAtMouseDown[l];
							animationEvent2.time = Mathf.Clamp01(EventManipulationHandler.m_EventTimes[l] + zero.x / rect.width);
						}
					}
					int[] array3 = new int[this.m_EventsSelected.Length];
					for (int m = 0; m < array3.Length; m++)
					{
						array3[m] = m;
					}
					Array.Sort(EventManipulationHandler.m_EventsAtMouseDown, array3, new AnimationEventTimeLine.EventComparer());
					bool[] array4 = (bool[])this.m_EventsSelected.Clone();
					float[] array5 = (float[])EventManipulationHandler.m_EventTimes.Clone();
					for (int n = 0; n < array3.Length; n++)
					{
						this.m_EventsSelected[n] = array4[array3[n]];
						EventManipulationHandler.m_EventTimes[n] = array5[array3[n]];
					}
					events = EventManipulationHandler.m_EventsAtMouseDown;
					result = true;
					break;
				}
				case HighLevelEvent.Delete:
					result = this.DeleteEvents(ref events, this.m_EventsSelected);
					break;
				case HighLevelEvent.SelectionChanged:
					this.EditEvents(clipInfo, this.m_EventsSelected);
					break;
				}
			}
			if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
				int num10 = this.m_EventsSelected.Count((bool selected) => selected);
				float time = Mathf.Max(this.m_Timeline.PixelToTime(Event.current.mousePosition.x, rect), 0f);
				GenericMenu genericMenu2 = new GenericMenu();
				genericMenu2.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new EventManipulationHandler.EventModificationContextMenuObject(clipInfo, time, -1, this.m_EventsSelected));
				if (num10 > 0)
				{
					genericMenu2.AddItem(new GUIContent((num10 <= 1) ? "Delete Animation Event" : "Delete Animation Events"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), new EventManipulationHandler.EventModificationContextMenuObject(clipInfo, time, -1, this.m_EventsSelected));
				}
				genericMenu2.ShowAsContext();
				this.m_InstantTooltipText = null;
			}
			this.CheckRectsOnMouseMove(rect, events, array);
			return result;
		}

		public void EventLineContextMenuAdd(object obj)
		{
			EventManipulationHandler.EventModificationContextMenuObject eventModificationContextMenuObject = (EventManipulationHandler.EventModificationContextMenuObject)obj;
			eventModificationContextMenuObject.m_Info.AddEvent(eventModificationContextMenuObject.m_Time);
			this.SelectEvent(eventModificationContextMenuObject.m_Info.GetEvents(), eventModificationContextMenuObject.m_Info.GetEventCount() - 1, eventModificationContextMenuObject.m_Info);
		}

		public void EventLineContextMenuDelete(object obj)
		{
			EventManipulationHandler.EventModificationContextMenuObject eventModificationContextMenuObject = (EventManipulationHandler.EventModificationContextMenuObject)obj;
			if (Array.Exists<bool>(eventModificationContextMenuObject.m_Selected, (bool selected) => selected))
			{
				for (int i = eventModificationContextMenuObject.m_Selected.Length - 1; i >= 0; i--)
				{
					if (eventModificationContextMenuObject.m_Selected[i])
					{
						eventModificationContextMenuObject.m_Info.RemoveEvent(i);
					}
				}
			}
			else if (eventModificationContextMenuObject.m_Index >= 0)
			{
				eventModificationContextMenuObject.m_Info.RemoveEvent(eventModificationContextMenuObject.m_Index);
			}
		}

		private void CheckRectsOnMouseMove(Rect eventLineRect, AnimationEvent[] events, Rect[] hitRects)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = false;
			this.m_InstantTooltipText = "";
			if (events.Length == hitRects.Length)
			{
				for (int i = hitRects.Length - 1; i >= 0; i--)
				{
					if (hitRects[i].Contains(mousePosition))
					{
						flag = true;
						if (this.m_HoverEvent != i)
						{
							this.m_HoverEvent = i;
							this.m_InstantTooltipText = events[this.m_HoverEvent].functionName;
							this.m_InstantTooltipPoint = new Vector2(mousePosition.x, mousePosition.y);
						}
					}
				}
			}
			if (!flag)
			{
				this.m_HoverEvent = -1;
			}
		}

		public void Draw(Rect window)
		{
			EditorGUI.indentLevel++;
			if (this.m_Events != null && this.m_Events.Length > 0)
			{
				AnimationWindowEventInspector.OnEditAnimationEvents(this.m_Events);
			}
			else
			{
				AnimationWindowEventInspector.OnDisabledAnimationEvent();
			}
			EditorGUI.indentLevel--;
			if (this.m_InstantTooltipText != null && this.m_InstantTooltipText != "")
			{
				GUIStyle gUIStyle = "AnimationEventTooltip";
				Vector2 vector = gUIStyle.CalcSize(new GUIContent(this.m_InstantTooltipText));
				Rect position = new Rect(window.x + this.m_InstantTooltipPoint.x, window.y + this.m_InstantTooltipPoint.y, vector.x, vector.y);
				if (position.xMax > window.width)
				{
					position.x = window.width - position.width;
				}
				GUI.Label(position, this.m_InstantTooltipText, gUIStyle);
			}
		}

		public bool DeleteEvents(ref AnimationEvent[] eventList, bool[] deleteIndices)
		{
			bool flag = false;
			for (int i = eventList.Length - 1; i >= 0; i--)
			{
				if (deleteIndices[i])
				{
					ArrayUtility.RemoveAt<AnimationEvent>(ref eventList, i);
					flag = true;
				}
			}
			if (flag)
			{
				this.m_EventsSelected = new bool[eventList.Length];
				this.m_Events = null;
			}
			return flag;
		}

		public void EditEvents(AnimationClipInfoProperties clipInfo, bool[] selectedIndices)
		{
			List<AnimationWindowEvent> list = new List<AnimationWindowEvent>();
			for (int i = 0; i < selectedIndices.Length; i++)
			{
				if (selectedIndices[i])
				{
					list.Add(AnimationWindowEvent.Edit(clipInfo, i));
				}
			}
			this.m_Events = list.ToArray();
		}

		public void UpdateEvents(AnimationClipInfoProperties clipInfo)
		{
			if (this.m_Events != null)
			{
				AnimationWindowEvent[] events = this.m_Events;
				for (int i = 0; i < events.Length; i++)
				{
					AnimationWindowEvent animationWindowEvent = events[i];
					animationWindowEvent.clipInfo = clipInfo;
				}
			}
		}
	}
}
