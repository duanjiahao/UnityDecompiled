using System;
using UnityEngine;

namespace UnityEditor
{
	internal class EventManipulationHandler
	{
		private class EventModificationContextMenuObjet
		{
			public AnimationClipInfoProperties m_Info;

			public float m_Time;

			public int m_Index;

			public EventModificationContextMenuObjet(AnimationClipInfoProperties info, float time, int index)
			{
				this.m_Info = info;
				this.m_Time = time;
				this.m_Index = index;
			}
		}

		private Rect[] m_EventRects = new Rect[0];

		private static AnimationEvent[] m_EventsAtMouseDown;

		private static float[] m_EventTimes;

		private int m_HoverEvent = -1;

		private string m_InstantTooltipText = null;

		private Vector2 m_InstantTooltipPoint = Vector2.zero;

		private bool[] m_EventsSelected;

		private AnimationWindowEvent m_Event = null;

		private TimeArea m_Timeline;

		public EventManipulationHandler(TimeArea timeArea)
		{
			this.m_Timeline = timeArea;
		}

		public void SelectEvent(AnimationEvent[] events, int index, AnimationClipInfoProperties clipInfo)
		{
			this.m_EventsSelected = new bool[events.Length];
			this.m_EventsSelected[index] = true;
			this.m_Event = AnimationWindowEvent.Edit(clipInfo, index);
		}

		public void UpdateEvent(AnimationClipInfoProperties info)
		{
			if (this.m_Event != null)
			{
				this.m_Event.clipInfo = info;
			}
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
				this.m_Event = null;
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
					GenericMenu genericMenu = new GenericMenu();
					genericMenu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new EventManipulationHandler.EventModificationContextMenuObjet(clipInfo, events[num6].time, num6));
					genericMenu.AddItem(new GUIContent("Delete Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), new EventManipulationHandler.EventModificationContextMenuObjet(clipInfo, events[num6].time, num6));
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
					this.m_Event = ((num6 < 0) ? null : AnimationWindowEvent.Edit(clipInfo, num6));
					break;
				}
			}
			this.CheckRectsOnMouseMove(rect, events, array);
			return result;
		}

		public void EventLineContextMenuAdd(object obj)
		{
			EventManipulationHandler.EventModificationContextMenuObjet eventModificationContextMenuObjet = (EventManipulationHandler.EventModificationContextMenuObjet)obj;
			eventModificationContextMenuObjet.m_Info.AddEvent(eventModificationContextMenuObjet.m_Time);
			this.SelectEvent(eventModificationContextMenuObjet.m_Info.GetEvents(), eventModificationContextMenuObjet.m_Info.GetEventCount() - 1, eventModificationContextMenuObjet.m_Info);
		}

		public void EventLineContextMenuDelete(object obj)
		{
			EventManipulationHandler.EventModificationContextMenuObjet eventModificationContextMenuObjet = (EventManipulationHandler.EventModificationContextMenuObjet)obj;
			eventModificationContextMenuObjet.m_Info.RemoveEvent(eventModificationContextMenuObjet.m_Index);
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
			if (this.m_Event != null)
			{
				AnimationWindowEventInspector.OnEditAnimationEvent(this.m_Event);
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

		private bool DeleteEvents(ref AnimationEvent[] eventList, bool[] deleteIndices)
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
				this.m_Event = null;
			}
			return flag;
		}
	}
}
