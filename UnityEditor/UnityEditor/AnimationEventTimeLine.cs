using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class AnimationEventTimeLine
	{
		public class EventComparer : IComparer
		{
			int IComparer.Compare(object objX, object objY)
			{
				AnimationEvent animationEvent = (AnimationEvent)objX;
				AnimationEvent animationEvent2 = (AnimationEvent)objY;
				float time = animationEvent.time;
				float time2 = animationEvent2.time;
				int result;
				if (time != time2)
				{
					result = (int)Mathf.Sign(time - time2);
				}
				else
				{
					int hashCode = animationEvent.GetHashCode();
					int hashCode2 = animationEvent2.GetHashCode();
					result = hashCode - hashCode2;
				}
				return result;
			}
		}

		private class EventLineContextMenuObject
		{
			public GameObject m_Animated;

			public AnimationClip m_Clip;

			public float m_Time;

			public int m_Index;

			public EventLineContextMenuObject(GameObject animated, AnimationClip clip, float time, int index)
			{
				this.m_Animated = animated;
				this.m_Clip = clip;
				this.m_Time = time;
				this.m_Index = index;
			}
		}

		[NonSerialized]
		private AnimationEvent[] m_EventsAtMouseDown;

		[NonSerialized]
		private float[] m_EventTimes;

		private bool[] m_EventsSelected;

		private bool m_DirtyTooltip = false;

		private int m_HoverEvent = -1;

		private string m_InstantTooltipText = null;

		private Vector2 m_InstantTooltipPoint = Vector2.zero;

		public AnimationEventTimeLine(EditorWindow owner)
		{
		}

		public void AddEvent(float time, GameObject rootGameObject, AnimationClip animationClip)
		{
			AnimationWindowEvent evt = AnimationWindowEvent.CreateAndEdit(rootGameObject, animationClip, time);
			this.Select(evt);
		}

		public void DeselectAll()
		{
			this.m_EventsSelected = null;
		}

		private void Select(AnimationWindowEvent evt)
		{
			this.m_EventsSelected = new bool[AnimationUtility.GetAnimationEvents(evt.clip).Length];
			this.m_EventsSelected[evt.eventIndex] = true;
			Selection.activeObject = evt;
		}

		public void EventLineGUI(Rect rect, AnimationWindowState state)
		{
			if (!(state.selectedItem == null))
			{
				AnimationClip animationClip = state.selectedItem.animationClip;
				GameObject rootGameObject = state.selectedItem.rootGameObject;
				GUI.BeginGroup(rect);
				Color color = GUI.color;
				Rect rect2 = new Rect(0f, 0f, rect.width, rect.height);
				float time = Mathf.Max((float)Mathf.RoundToInt(state.PixelToTime(Event.current.mousePosition.x, rect) * state.frameRate) / state.frameRate, 0f);
				if (animationClip != null)
				{
					AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(animationClip);
					Texture image = EditorGUIUtility.IconContent("Animation.EventMarker").image;
					Rect[] array = new Rect[animationEvents.Length];
					Rect[] array2 = new Rect[animationEvents.Length];
					int num = 1;
					int num2 = 0;
					for (int i = 0; i < animationEvents.Length; i++)
					{
						AnimationEvent animationEvent = animationEvents[i];
						if (num2 == 0)
						{
							num = 1;
							while (i + num < animationEvents.Length && animationEvents[i + num].time == animationEvent.time)
							{
								num++;
							}
							num2 = num;
						}
						num2--;
						float num3 = Mathf.Floor(state.FrameToPixel(animationEvent.time * animationClip.frameRate, rect));
						int num4 = 0;
						if (num > 1)
						{
							float num5 = (float)Mathf.Min((num - 1) * (image.width - 1), (int)(state.FrameDeltaToPixel(rect) - (float)(image.width * 2)));
							num4 = Mathf.FloorToInt(Mathf.Max(0f, num5 - (float)((image.width - 1) * num2)));
						}
						Rect rect3 = new Rect(num3 + (float)num4 - (float)(image.width / 2), (rect.height - 10f) * (float)(num2 - num + 1) / (float)Mathf.Max(1, num - 1), (float)image.width, (float)image.height);
						array[i] = rect3;
						array2[i] = rect3;
					}
					if (this.m_DirtyTooltip)
					{
						if (this.m_HoverEvent >= 0 && this.m_HoverEvent < array.Length)
						{
							this.m_InstantTooltipText = AnimationWindowEventInspector.FormatEvent(rootGameObject, animationEvents[this.m_HoverEvent]);
							this.m_InstantTooltipPoint = new Vector2(array[this.m_HoverEvent].xMin + (float)((int)(array[this.m_HoverEvent].width / 2f)) + rect.x - 30f, rect.yMax);
						}
						this.m_DirtyTooltip = false;
					}
					if (this.m_EventsSelected == null || this.m_EventsSelected.Length != animationEvents.Length)
					{
						this.m_EventsSelected = new bool[animationEvents.Length];
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
						case HighLevelEvent.DoubleClick:
							if (num6 != -1)
							{
								AnimationWindowEvent activeObject = AnimationWindowEvent.Edit(rootGameObject, animationClip, num6);
								Selection.activeObject = activeObject;
							}
							else
							{
								this.EventLineContextMenuAdd(new AnimationEventTimeLine.EventLineContextMenuObject(rootGameObject, animationClip, time, -1));
							}
							break;
						case HighLevelEvent.ContextClick:
						{
							GenericMenu genericMenu = new GenericMenu();
							AnimationEventTimeLine.EventLineContextMenuObject userData = new AnimationEventTimeLine.EventLineContextMenuObject(rootGameObject, animationClip, animationEvents[num6].time, num6);
							genericMenu.AddItem(new GUIContent("Edit Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuEdit), userData);
							genericMenu.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), userData);
							genericMenu.AddItem(new GUIContent("Delete Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuDelete), userData);
							genericMenu.ShowAsContext();
							this.m_InstantTooltipText = null;
							this.m_DirtyTooltip = true;
							state.Repaint();
							break;
						}
						case HighLevelEvent.BeginDrag:
							this.m_EventsAtMouseDown = animationEvents;
							this.m_EventTimes = new float[animationEvents.Length];
							for (int j = 0; j < animationEvents.Length; j++)
							{
								this.m_EventTimes[j] = animationEvents[j].time;
							}
							break;
						case HighLevelEvent.Drag:
						{
							for (int k = animationEvents.Length - 1; k >= 0; k--)
							{
								if (this.m_EventsSelected[k])
								{
									AnimationEvent animationEvent2 = this.m_EventsAtMouseDown[k];
									animationEvent2.time = this.m_EventTimes[k] + zero.x * state.PixelDeltaToTime(rect);
									animationEvent2.time = Mathf.Max(0f, animationEvent2.time);
									animationEvent2.time = (float)Mathf.RoundToInt(animationEvent2.time * animationClip.frameRate) / animationClip.frameRate;
								}
							}
							int[] array3 = new int[this.m_EventsSelected.Length];
							for (int l = 0; l < array3.Length; l++)
							{
								array3[l] = l;
							}
							Array.Sort(this.m_EventsAtMouseDown, array3, new AnimationEventTimeLine.EventComparer());
							bool[] array4 = (bool[])this.m_EventsSelected.Clone();
							float[] array5 = (float[])this.m_EventTimes.Clone();
							for (int m = 0; m < array3.Length; m++)
							{
								this.m_EventsSelected[m] = array4[array3[m]];
								this.m_EventTimes[m] = array5[array3[m]];
							}
							Undo.RegisterCompleteObjectUndo(animationClip, "Move Event");
							AnimationUtility.SetAnimationEvents(animationClip, this.m_EventsAtMouseDown);
							this.m_DirtyTooltip = true;
							break;
						}
						case HighLevelEvent.Delete:
							this.DeleteEvents(animationClip, this.m_EventsSelected);
							break;
						case HighLevelEvent.SelectionChanged:
							state.ClearKeySelections();
							if (num6 != -1)
							{
								AnimationWindowEvent activeObject2 = AnimationWindowEvent.Edit(rootGameObject, animationClip, num6);
								Selection.activeObject = activeObject2;
							}
							break;
						}
					}
					this.CheckRectsOnMouseMove(rect, animationEvents, array);
				}
				if (Event.current.type == EventType.ContextClick && rect2.Contains(Event.current.mousePosition))
				{
					Event.current.Use();
					GenericMenu genericMenu2 = new GenericMenu();
					genericMenu2.AddItem(new GUIContent("Add Animation Event"), false, new GenericMenu.MenuFunction2(this.EventLineContextMenuAdd), new AnimationEventTimeLine.EventLineContextMenuObject(rootGameObject, animationClip, time, -1));
					genericMenu2.ShowAsContext();
				}
				GUI.color = color;
				GUI.EndGroup();
			}
		}

		public void DrawInstantTooltip(Rect position)
		{
			if (this.m_InstantTooltipText != null && this.m_InstantTooltipText != "")
			{
				GUIStyle gUIStyle = "AnimationEventTooltip";
				gUIStyle.contentOffset = new Vector2(0f, 0f);
				gUIStyle.overflow = new RectOffset(10, 10, 0, 0);
				Vector2 vector = gUIStyle.CalcSize(new GUIContent(this.m_InstantTooltipText));
				Rect position2 = new Rect(this.m_InstantTooltipPoint.x - vector.x * 0.5f, this.m_InstantTooltipPoint.y + 24f, vector.x, vector.y);
				if (position2.xMax > position.width)
				{
					position2.x = position.width - position2.width;
				}
				GUI.Label(position2, this.m_InstantTooltipText, gUIStyle);
				position2 = new Rect(this.m_InstantTooltipPoint.x - 33f, this.m_InstantTooltipPoint.y, 7f, 25f);
				GUI.Label(position2, "", "AnimationEventTooltipArrow");
			}
		}

		private void DeleteEvents(AnimationClip clip, bool[] deleteIndices)
		{
			bool flag = false;
			List<AnimationEvent> list = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (deleteIndices[i])
				{
					list.RemoveAt(i);
					flag = true;
				}
			}
			if (flag)
			{
				Undo.RegisterCompleteObjectUndo(clip, "Delete Event");
				AnimationUtility.SetAnimationEvents(clip, list.ToArray());
				this.m_EventsSelected = new bool[list.Count];
				this.m_DirtyTooltip = true;
			}
		}

		public void EventLineContextMenuAdd(object obj)
		{
			AnimationEventTimeLine.EventLineContextMenuObject eventLineContextMenuObject = (AnimationEventTimeLine.EventLineContextMenuObject)obj;
			AnimationWindowEvent evt = AnimationWindowEvent.CreateAndEdit(eventLineContextMenuObject.m_Animated, eventLineContextMenuObject.m_Clip, eventLineContextMenuObject.m_Time);
			this.Select(evt);
		}

		public void EventLineContextMenuEdit(object obj)
		{
			AnimationEventTimeLine.EventLineContextMenuObject eventLineContextMenuObject = (AnimationEventTimeLine.EventLineContextMenuObject)obj;
			AnimationWindowEvent evt = AnimationWindowEvent.Edit(eventLineContextMenuObject.m_Animated, eventLineContextMenuObject.m_Clip, eventLineContextMenuObject.m_Index);
			this.Select(evt);
		}

		public void EventLineContextMenuDelete(object obj)
		{
			AnimationEventTimeLine.EventLineContextMenuObject eventLineContextMenuObject = (AnimationEventTimeLine.EventLineContextMenuObject)obj;
			AnimationClip clip = eventLineContextMenuObject.m_Clip;
			if (!(clip == null))
			{
				int index = eventLineContextMenuObject.m_Index;
				if (this.m_EventsSelected[index])
				{
					this.DeleteEvents(clip, this.m_EventsSelected);
				}
				else
				{
					bool[] array = new bool[this.m_EventsSelected.Length];
					array[index] = true;
					this.DeleteEvents(clip, array);
				}
			}
		}

		private void CheckRectsOnMouseMove(Rect eventLineRect, AnimationEvent[] events, Rect[] hitRects)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			bool flag = false;
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
							this.m_InstantTooltipPoint = new Vector2(hitRects[this.m_HoverEvent].xMin + (float)((int)(hitRects[this.m_HoverEvent].width / 2f)) + eventLineRect.x, eventLineRect.yMax);
							this.m_DirtyTooltip = true;
						}
					}
				}
			}
			if (!flag)
			{
				this.m_HoverEvent = -1;
				this.m_InstantTooltipText = "";
			}
		}
	}
}
