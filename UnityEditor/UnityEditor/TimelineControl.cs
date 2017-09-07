using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class TimelineControl
	{
		private enum DragStates
		{
			None,
			LeftSelection,
			RightSelection,
			FullSelection,
			Destination,
			Source,
			Playhead,
			TimeArea
		}

		private class Styles
		{
			public readonly GUIStyle block = new GUIStyle("MeTransitionBlock");

			public GUIStyle leftBlock = new GUIStyle("MeTransitionBlock");

			public GUIStyle rightBlock = new GUIStyle("MeTransitionBlock");

			public GUIStyle timeBlockRight = new GUIStyle("MeTimeLabel");

			public GUIStyle timeBlockLeft = new GUIStyle("MeTimeLabel");

			public readonly GUIStyle offLeft = new GUIStyle("MeTransOffLeft");

			public readonly GUIStyle offRight = new GUIStyle("MeTransOffRight");

			public readonly GUIStyle onLeft = new GUIStyle("MeTransOnLeft");

			public readonly GUIStyle onRight = new GUIStyle("MeTransOnRight");

			public readonly GUIStyle offOn = new GUIStyle("MeTransOff2On");

			public readonly GUIStyle onOff = new GUIStyle("MeTransOn2Off");

			public readonly GUIStyle background = new GUIStyle("MeTransitionBack");

			public readonly GUIStyle header = new GUIStyle("MeTransitionHead");

			public readonly GUIStyle handLeft = new GUIStyle("MeTransitionHandleLeft");

			public readonly GUIStyle handRight = new GUIStyle("MeTransitionHandleRight");

			public readonly GUIStyle handLeftPrev = new GUIStyle("MeTransitionHandleLeftPrev");

			public readonly GUIStyle playhead = new GUIStyle("MeTransPlayhead");

			public readonly GUIStyle selectHead = new GUIStyle("MeTransitionSelectHead");

			public readonly GUIStyle select = new GUIStyle("MeTransitionSelect");

			public Styles()
			{
				this.timeBlockRight.alignment = TextAnchor.MiddleRight;
				this.timeBlockRight.normal.background = null;
				this.timeBlockLeft.normal.background = null;
			}
		}

		internal class PivotSample
		{
			public float m_Time;

			public float m_Weight;
		}

		private TimeArea m_TimeArea;

		private float m_Time = float.PositiveInfinity;

		private float m_StartTime = 0f;

		private float m_StopTime = 1f;

		private string m_SrcName = "Left";

		private string m_DstName = "Right";

		private bool m_SrcLoop = false;

		private bool m_DstLoop = false;

		private float m_SrcStartTime = 0f;

		private float m_SrcStopTime = 0.75f;

		private float m_DstStartTime = 0.25f;

		private float m_DstStopTime = 1f;

		private bool m_HasExitTime = false;

		private float m_TransitionStartTime = float.PositiveInfinity;

		private float m_TransitionStopTime = float.PositiveInfinity;

		private float m_SampleStopTime = float.PositiveInfinity;

		private float m_DstDragOffset = 0f;

		private float m_LeftThumbOffset = 0f;

		private float m_RightThumbOffset = 0f;

		private float m_TimeStartDrag = 0f;

		private float m_TimeOffset = 0f;

		private TimelineControl.DragStates m_DragState = TimelineControl.DragStates.None;

		private int id = -1;

		private Rect m_Rect = new Rect(0f, 0f, 0f, 0f);

		private Vector3[] m_SrcPivotVectors;

		private Vector3[] m_DstPivotVectors;

		private List<TimelineControl.PivotSample> m_SrcPivotList = new List<TimelineControl.PivotSample>();

		private List<TimelineControl.PivotSample> m_DstPivotList = new List<TimelineControl.PivotSample>();

		private TimelineControl.Styles styles;

		public List<TimelineControl.PivotSample> SrcPivotList
		{
			get
			{
				return this.m_SrcPivotList;
			}
			set
			{
				this.m_SrcPivotList = value;
				this.m_SrcPivotVectors = null;
			}
		}

		public List<TimelineControl.PivotSample> DstPivotList
		{
			get
			{
				return this.m_DstPivotList;
			}
			set
			{
				this.m_DstPivotList = value;
				this.m_DstPivotVectors = null;
			}
		}

		public bool srcLoop
		{
			get
			{
				return this.m_SrcLoop;
			}
			set
			{
				this.m_SrcLoop = value;
			}
		}

		public bool dstLoop
		{
			get
			{
				return this.m_DstLoop;
			}
			set
			{
				this.m_DstLoop = value;
			}
		}

		public float Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		public float StartTime
		{
			get
			{
				return this.m_StartTime;
			}
			set
			{
				this.m_StartTime = value;
			}
		}

		public float StopTime
		{
			get
			{
				return this.m_StopTime;
			}
			set
			{
				this.m_StopTime = value;
			}
		}

		public string SrcName
		{
			get
			{
				return this.m_SrcName;
			}
			set
			{
				this.m_SrcName = value;
			}
		}

		public string DstName
		{
			get
			{
				return this.m_DstName;
			}
			set
			{
				this.m_DstName = value;
			}
		}

		public float SrcStartTime
		{
			get
			{
				return this.m_SrcStartTime;
			}
			set
			{
				this.m_SrcStartTime = value;
			}
		}

		public float SrcStopTime
		{
			get
			{
				return this.m_SrcStopTime;
			}
			set
			{
				this.m_SrcStopTime = value;
			}
		}

		public float SrcDuration
		{
			get
			{
				return this.SrcStopTime - this.SrcStartTime;
			}
		}

		public float DstStartTime
		{
			get
			{
				return this.m_DstStartTime;
			}
			set
			{
				this.m_DstStartTime = value;
			}
		}

		public float DstStopTime
		{
			get
			{
				return this.m_DstStopTime;
			}
			set
			{
				this.m_DstStopTime = value;
			}
		}

		public float DstDuration
		{
			get
			{
				return this.DstStopTime - this.DstStartTime;
			}
		}

		public float TransitionStartTime
		{
			get
			{
				return this.m_TransitionStartTime;
			}
			set
			{
				this.m_TransitionStartTime = value;
			}
		}

		public float TransitionStopTime
		{
			get
			{
				return this.m_TransitionStopTime;
			}
			set
			{
				this.m_TransitionStopTime = value;
			}
		}

		public bool HasExitTime
		{
			get
			{
				return this.m_HasExitTime;
			}
			set
			{
				this.m_HasExitTime = value;
			}
		}

		public float TransitionDuration
		{
			get
			{
				return this.TransitionStopTime - this.TransitionStartTime;
			}
		}

		public float SampleStopTime
		{
			get
			{
				return this.m_SampleStopTime;
			}
			set
			{
				this.m_SampleStopTime = value;
			}
		}

		public TimelineControl()
		{
			this.Init();
		}

		public void ResetRange()
		{
			this.m_TimeArea.SetShownHRangeInsideMargins(0f, this.StopTime);
		}

		private void Init()
		{
			if (this.id == -1)
			{
				this.id = GUIUtility.GetPermanentControlID();
			}
			if (this.m_TimeArea == null)
			{
				this.m_TimeArea = new TimeArea(false);
				this.m_TimeArea.hRangeLocked = false;
				this.m_TimeArea.vRangeLocked = true;
				this.m_TimeArea.hSlider = false;
				this.m_TimeArea.vSlider = false;
				this.m_TimeArea.margin = 10f;
				this.m_TimeArea.scaleWithWindow = true;
				this.m_TimeArea.hTicks.SetTickModulosForFrameRate(30f);
			}
			if (this.styles == null)
			{
				this.styles = new TimelineControl.Styles();
			}
		}

		private List<Vector3> GetControls(List<Vector3> segmentPoints, float scale)
		{
			List<Vector3> list = new List<Vector3>();
			List<Vector3> result;
			if (segmentPoints.Count < 2)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < segmentPoints.Count; i++)
				{
					if (i == 0)
					{
						Vector3 vector = segmentPoints[i];
						Vector3 a = segmentPoints[i + 1];
						Vector3 a2 = a - vector;
						Vector3 item = vector + scale * a2;
						list.Add(vector);
						list.Add(item);
					}
					else if (i == segmentPoints.Count - 1)
					{
						Vector3 b = segmentPoints[i - 1];
						Vector3 vector2 = segmentPoints[i];
						Vector3 a3 = vector2 - b;
						Vector3 item2 = vector2 - scale * a3;
						list.Add(item2);
						list.Add(vector2);
					}
					else
					{
						Vector3 b2 = segmentPoints[i - 1];
						Vector3 vector3 = segmentPoints[i];
						Vector3 a4 = segmentPoints[i + 1];
						Vector3 normalized = (a4 - b2).normalized;
						Vector3 item3 = vector3 - scale * normalized * (vector3 - b2).magnitude;
						Vector3 item4 = vector3 + scale * normalized * (a4 - vector3).magnitude;
						list.Add(item3);
						list.Add(vector3);
						list.Add(item4);
					}
				}
				result = list;
			}
			return result;
		}

		private Vector3 CalculatePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float d = num3 * num;
			float d2 = num2 * t;
			Vector3 a = d * p0;
			a += 3f * num3 * t * p1;
			a += 3f * num * num2 * p2;
			return a + d2 * p3;
		}

		private Color[] GetPivotColors(Vector3[] vectors, float motionStart, float motionStop, Color fromColor, Color toColor, Color loopColor, float offset)
		{
			Color[] array = new Color[vectors.Length];
			float num = this.m_TimeArea.TimeToPixel(this.m_TransitionStartTime, this.m_Rect) + this.m_LeftThumbOffset;
			float num2 = this.m_TimeArea.TimeToPixel(this.m_TransitionStopTime, this.m_Rect) + this.m_RightThumbOffset;
			float num3 = num2 - num;
			for (int i = 0; i < array.Length; i++)
			{
				if (vectors[i].x >= num && vectors[i].x <= num2)
				{
					array[i] = Color.Lerp(fromColor, toColor, (vectors[i].x - num) / num3);
				}
				else if (vectors[i].x < num && vectors[i].x >= motionStart + offset)
				{
					array[i] = fromColor;
				}
				else if (vectors[i].x > num2 && vectors[i].x <= motionStop + offset)
				{
					array[i] = toColor;
				}
				else
				{
					array[i] = loopColor;
				}
			}
			return array;
		}

		private Vector3[] GetPivotVectors(TimelineControl.PivotSample[] samples, float width, Rect rect, float height, bool loop)
		{
			Vector3[] result;
			if (samples.Length == 0 || width < 0.33f)
			{
				result = new Vector3[0];
			}
			else
			{
				List<Vector3> list = new List<Vector3>();
				for (int i = 0; i < samples.Length; i++)
				{
					TimelineControl.PivotSample pivotSample = samples[i];
					Vector3 zero = Vector3.zero;
					zero.x = this.m_TimeArea.TimeToPixel(pivotSample.m_Time, rect);
					zero.y = height / 16f + pivotSample.m_Weight * 12f * height / 16f;
					list.Add(zero);
				}
				if (loop && list[list.Count - 1].x <= rect.width)
				{
					float x = list[list.Count - 1].x;
					int num = 0;
					int num2 = 1;
					List<Vector3> list2 = new List<Vector3>();
					while (x < rect.width)
					{
						if (num > list.Count - 1)
						{
							num = 0;
							num2++;
						}
						Vector3 item = list[num];
						item.x += (float)num2 * width;
						x = item.x;
						list2.Add(item);
						num++;
					}
					list.AddRange(list2);
				}
				List<Vector3> controls = this.GetControls(list, 0.5f);
				list.Clear();
				for (int j = 0; j < controls.Count - 3; j += 3)
				{
					Vector3 p = controls[j];
					Vector3 p2 = controls[j + 1];
					Vector3 p3 = controls[j + 2];
					Vector3 p4 = controls[j + 3];
					if (j == 0)
					{
						list.Add(this.CalculatePoint(0f, p, p2, p3, p4));
					}
					for (int k = 1; k <= 10; k++)
					{
						list.Add(this.CalculatePoint((float)k / 10f, p, p2, p3, p4));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		private Vector3[] OffsetPivotVectors(Vector3[] vectors, float offset)
		{
			for (int i = 0; i < vectors.Length; i++)
			{
				int expr_0F_cp_1 = i;
				vectors[expr_0F_cp_1].x = vectors[expr_0F_cp_1].x + offset;
			}
			return vectors;
		}

		private void DoPivotCurves()
		{
			Color white = Color.white;
			Color white2 = Color.white;
			Color toColor = new Color(1f, 1f, 1f, 0.1f);
			Color fromColor = new Color(1f, 1f, 1f, 0.1f);
			Color loopColor = new Color(0.75f, 0.75f, 0.75f, 0.2f);
			Color loopColor2 = new Color(0.75f, 0.75f, 0.75f, 0.2f);
			Rect rect = new Rect(0f, 18f, this.m_Rect.width, 66f);
			GUI.BeginGroup(rect);
			float num = this.m_TimeArea.TimeToPixel(this.SrcStartTime, rect);
			float num2 = this.m_TimeArea.TimeToPixel(this.SrcStopTime, rect);
			float num3 = this.m_TimeArea.TimeToPixel(this.DstStartTime, rect);
			float num4 = this.m_TimeArea.TimeToPixel(this.DstStopTime, rect);
			if (this.m_SrcPivotVectors == null)
			{
				this.m_SrcPivotVectors = this.GetPivotVectors(this.m_SrcPivotList.ToArray(), num2 - num, rect, rect.height, this.srcLoop);
			}
			if (this.m_DstPivotVectors == null)
			{
				this.m_DstPivotVectors = this.GetPivotVectors(this.m_DstPivotList.ToArray(), num4 - num3, rect, rect.height, this.dstLoop);
			}
			this.m_DstPivotVectors = this.OffsetPivotVectors(this.m_DstPivotVectors, this.m_DstDragOffset + num3 - num);
			Color[] pivotColors = this.GetPivotColors(this.m_SrcPivotVectors, num, num2, white, toColor, loopColor, 0f);
			Color[] pivotColors2 = this.GetPivotColors(this.m_DstPivotVectors, num3, num4, fromColor, white2, loopColor2, this.m_DstDragOffset);
			Handles.DrawAAPolyLine(pivotColors, this.m_SrcPivotVectors);
			Handles.DrawAAPolyLine(pivotColors2, this.m_DstPivotVectors);
			GUI.EndGroup();
		}

		private void EnforceConstraints()
		{
			Rect rect = new Rect(0f, 0f, this.m_Rect.width, 150f);
			if (this.m_DragState == TimelineControl.DragStates.LeftSelection)
			{
				float min = this.m_TimeArea.TimeToPixel(this.SrcStartTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect);
				float max = this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect);
				this.m_LeftThumbOffset = Mathf.Clamp(this.m_LeftThumbOffset, min, max);
			}
			if (this.m_DragState == TimelineControl.DragStates.RightSelection)
			{
				float num = this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect) - this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect);
				if (this.m_RightThumbOffset < num)
				{
					this.m_RightThumbOffset = num;
				}
			}
		}

		private bool WasDraggingData()
		{
			return this.m_DstDragOffset != 0f || this.m_LeftThumbOffset != 0f || this.m_RightThumbOffset != 0f;
		}

		public bool DoTimeline(Rect timeRect)
		{
			bool result = false;
			this.Init();
			this.m_Rect = timeRect;
			float num = this.m_TimeArea.PixelToTime(timeRect.xMin, timeRect);
			float num2 = this.m_TimeArea.PixelToTime(timeRect.xMax, timeRect);
			if (!Mathf.Approximately(num, this.StartTime))
			{
				this.StartTime = num;
				GUI.changed = true;
			}
			if (!Mathf.Approximately(num2, this.StopTime))
			{
				this.StopTime = num2;
				GUI.changed = true;
			}
			this.Time = Mathf.Max(this.Time, 0f);
			if (Event.current.type == EventType.Repaint)
			{
				this.m_TimeArea.rect = timeRect;
			}
			this.m_TimeArea.BeginViewGUI();
			this.m_TimeArea.EndViewGUI();
			GUI.BeginGroup(timeRect);
			Event current = Event.current;
			Rect rect = new Rect(0f, 0f, timeRect.width, timeRect.height);
			Rect position = new Rect(0f, 0f, timeRect.width, 18f);
			Rect position2 = new Rect(0f, 18f, timeRect.width, 132f);
			float num3 = this.m_TimeArea.TimeToPixel(this.SrcStartTime, rect);
			float num4 = this.m_TimeArea.TimeToPixel(this.SrcStopTime, rect);
			float num5 = this.m_TimeArea.TimeToPixel(this.DstStartTime, rect) + this.m_DstDragOffset;
			float num6 = this.m_TimeArea.TimeToPixel(this.DstStopTime, rect) + this.m_DstDragOffset;
			float num7 = this.m_TimeArea.TimeToPixel(this.TransitionStartTime, rect) + this.m_LeftThumbOffset;
			float num8 = this.m_TimeArea.TimeToPixel(this.TransitionStopTime, rect) + this.m_RightThumbOffset;
			float num9 = this.m_TimeArea.TimeToPixel(this.Time, rect);
			Rect rect2 = new Rect(num3, 85f, num4 - num3, 32f);
			Rect rect3 = new Rect(num5, 117f, num6 - num5, 32f);
			Rect position3 = new Rect(num7, 0f, num8 - num7, 18f);
			Rect position4 = new Rect(num7, 18f, num8 - num7, rect.height - 18f);
			Rect position5 = new Rect(num7 - 9f, 5f, 9f, 15f);
			Rect position6 = new Rect(num8, 5f, 9f, 15f);
			Rect position7 = new Rect(num9 - 7f, 4f, 15f, 15f);
			if (current.type == EventType.KeyDown)
			{
				if (GUIUtility.keyboardControl == this.id && this.m_DragState == TimelineControl.DragStates.Destination)
				{
					this.m_DstDragOffset = 0f;
				}
				if (this.m_DragState == TimelineControl.DragStates.LeftSelection)
				{
					this.m_LeftThumbOffset = 0f;
				}
				if (this.m_DragState == TimelineControl.DragStates.RightSelection)
				{
					this.m_RightThumbOffset = 0f;
				}
				if (this.m_DragState == TimelineControl.DragStates.Playhead)
				{
					this.m_TimeOffset = 0f;
				}
				if (this.m_DragState == TimelineControl.DragStates.FullSelection)
				{
					this.m_LeftThumbOffset = 0f;
					this.m_RightThumbOffset = 0f;
				}
			}
			if (current.type == EventType.MouseDown)
			{
				if (rect.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = this.id;
					GUIUtility.keyboardControl = this.id;
					if (position7.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.Playhead;
						this.m_TimeStartDrag = this.m_TimeArea.TimeToPixel(this.Time, rect);
					}
					else if (rect2.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.Source;
					}
					else if (rect3.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.Destination;
					}
					else if (position5.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.LeftSelection;
					}
					else if (position6.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.RightSelection;
					}
					else if (position3.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.FullSelection;
					}
					else if (position.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.TimeArea;
					}
					else if (position2.Contains(current.mousePosition))
					{
						this.m_DragState = TimelineControl.DragStates.TimeArea;
					}
					else
					{
						this.m_DragState = TimelineControl.DragStates.None;
					}
					current.Use();
				}
			}
			if (current.type == EventType.MouseDrag)
			{
				if (GUIUtility.hotControl == this.id)
				{
					switch (this.m_DragState)
					{
					case TimelineControl.DragStates.LeftSelection:
						if ((current.delta.x > 0f && current.mousePosition.x > num3) || (current.delta.x < 0f && current.mousePosition.x < num8))
						{
							this.m_LeftThumbOffset += current.delta.x;
						}
						this.EnforceConstraints();
						break;
					case TimelineControl.DragStates.RightSelection:
						if ((current.delta.x > 0f && current.mousePosition.x > num7) || current.delta.x < 0f)
						{
							this.m_RightThumbOffset += current.delta.x;
						}
						this.EnforceConstraints();
						break;
					case TimelineControl.DragStates.FullSelection:
						this.m_RightThumbOffset += current.delta.x;
						this.m_LeftThumbOffset += current.delta.x;
						this.EnforceConstraints();
						break;
					case TimelineControl.DragStates.Destination:
						this.m_DstDragOffset += current.delta.x;
						this.EnforceConstraints();
						break;
					case TimelineControl.DragStates.Source:
					case TimelineControl.DragStates.TimeArea:
					{
						TimeArea expr_4DB_cp_0 = this.m_TimeArea;
						expr_4DB_cp_0.m_Translation.x = expr_4DB_cp_0.m_Translation.x + current.delta.x;
						break;
					}
					case TimelineControl.DragStates.Playhead:
						if ((current.delta.x > 0f && current.mousePosition.x > num3) || (current.delta.x < 0f && current.mousePosition.x <= this.m_TimeArea.TimeToPixel(this.SampleStopTime, rect)))
						{
							this.m_TimeOffset += current.delta.x;
						}
						this.Time = this.m_TimeArea.PixelToTime(this.m_TimeStartDrag + this.m_TimeOffset, rect);
						break;
					}
					current.Use();
					GUI.changed = true;
				}
			}
			if (Event.current.GetTypeForControl(this.id) == EventType.MouseUp)
			{
				this.SrcStartTime = this.m_TimeArea.PixelToTime(num3, rect);
				this.SrcStopTime = this.m_TimeArea.PixelToTime(num4, rect);
				this.DstStartTime = this.m_TimeArea.PixelToTime(num5, rect);
				this.DstStopTime = this.m_TimeArea.PixelToTime(num6, rect);
				this.TransitionStartTime = this.m_TimeArea.PixelToTime(num7, rect);
				this.TransitionStopTime = this.m_TimeArea.PixelToTime(num8, rect);
				GUI.changed = true;
				this.m_DragState = TimelineControl.DragStates.None;
				result = this.WasDraggingData();
				this.m_LeftThumbOffset = 0f;
				this.m_RightThumbOffset = 0f;
				this.m_TimeOffset = 0f;
				this.m_DstDragOffset = 0f;
				GUIUtility.hotControl = 0;
				current.Use();
			}
			GUI.Box(position, GUIContent.none, this.styles.header);
			GUI.Box(position2, GUIContent.none, this.styles.background);
			this.m_TimeArea.DrawMajorTicks(position2, 30f);
			GUIContent content = EditorGUIUtility.TempContent(this.SrcName);
			int num10 = (!this.srcLoop) ? 1 : (1 + (int)((num8 - rect2.xMin) / (rect2.xMax - rect2.xMin)));
			Rect position8 = rect2;
			if (rect2.width < 10f)
			{
				position8 = new Rect(rect2.x, rect2.y, (rect2.xMax - rect2.xMin) * (float)num10, rect2.height);
				num10 = 1;
			}
			for (int i = 0; i < num10; i++)
			{
				GUI.BeginGroup(position8, GUIContent.none, this.styles.leftBlock);
				float num11 = num7 - position8.xMin;
				float num12 = num8 - num7;
				float num13 = position8.xMax - position8.xMin - (num11 + num12);
				if (num11 > 0f)
				{
					GUI.Box(new Rect(0f, 0f, num11, rect2.height), GUIContent.none, this.styles.onLeft);
				}
				if (num12 > 0f)
				{
					GUI.Box(new Rect(num11, 0f, num12, rect2.height), GUIContent.none, this.styles.onOff);
				}
				if (num13 > 0f)
				{
					GUI.Box(new Rect(num11 + num12, 0f, num13, rect2.height), GUIContent.none, this.styles.offRight);
				}
				float b = 1f;
				float x = this.styles.block.CalcSize(content).x;
				float num14 = Mathf.Max(0f, num11) - 20f;
				float num15 = num14 + 15f;
				if (num14 < x && num15 > 0f && this.m_DragState == TimelineControl.DragStates.LeftSelection)
				{
					b = 0f;
				}
				GUI.EndGroup();
				float a = this.styles.leftBlock.normal.textColor.a;
				if (!Mathf.Approximately(a, b) && Event.current.type == EventType.Repaint)
				{
					a = Mathf.Lerp(a, b, 0.1f);
					this.styles.leftBlock.normal.textColor = new Color(this.styles.leftBlock.normal.textColor.r, this.styles.leftBlock.normal.textColor.g, this.styles.leftBlock.normal.textColor.b, a);
					HandleUtility.Repaint();
				}
				GUI.Box(position8, content, this.styles.leftBlock);
				position8 = new Rect(position8.xMax, 85f, position8.xMax - position8.xMin, 32f);
			}
			GUIContent content2 = EditorGUIUtility.TempContent(this.DstName);
			int num16 = (!this.dstLoop) ? 1 : (1 + (int)((num8 - rect3.xMin) / (rect3.xMax - rect3.xMin)));
			position8 = rect3;
			if (rect3.width < 10f)
			{
				position8 = new Rect(rect3.x, rect3.y, (rect3.xMax - rect3.xMin) * (float)num16, rect3.height);
				num16 = 1;
			}
			for (int j = 0; j < num16; j++)
			{
				GUI.BeginGroup(position8, GUIContent.none, this.styles.rightBlock);
				float num17 = num7 - position8.xMin;
				float num18 = num8 - num7;
				float num19 = position8.xMax - position8.xMin - (num17 + num18);
				if (num17 > 0f)
				{
					GUI.Box(new Rect(0f, 0f, num17, rect3.height), GUIContent.none, this.styles.offLeft);
				}
				if (num18 > 0f)
				{
					GUI.Box(new Rect(num17, 0f, num18, rect3.height), GUIContent.none, this.styles.offOn);
				}
				if (num19 > 0f)
				{
					GUI.Box(new Rect(num17 + num18, 0f, num19, rect3.height), GUIContent.none, this.styles.onRight);
				}
				float b2 = 1f;
				float x2 = this.styles.block.CalcSize(content2).x;
				float num20 = Mathf.Max(0f, num17) - 20f;
				float num21 = num20 + 15f;
				if (num20 < x2 && num21 > 0f && (this.m_DragState == TimelineControl.DragStates.LeftSelection || this.m_DragState == TimelineControl.DragStates.Destination))
				{
					b2 = 0f;
				}
				GUI.EndGroup();
				float a2 = this.styles.rightBlock.normal.textColor.a;
				if (!Mathf.Approximately(a2, b2) && Event.current.type == EventType.Repaint)
				{
					a2 = Mathf.Lerp(a2, b2, 0.1f);
					this.styles.rightBlock.normal.textColor = new Color(this.styles.rightBlock.normal.textColor.r, this.styles.rightBlock.normal.textColor.g, this.styles.rightBlock.normal.textColor.b, a2);
					HandleUtility.Repaint();
				}
				GUI.Box(position8, content2, this.styles.rightBlock);
				position8 = new Rect(position8.xMax, position8.yMin, position8.xMax - position8.xMin, 32f);
			}
			GUI.Box(position4, GUIContent.none, this.styles.select);
			GUI.Box(position3, GUIContent.none, this.styles.selectHead);
			this.m_TimeArea.TimeRuler(position, 30f);
			GUI.Box(position5, GUIContent.none, (!this.m_HasExitTime) ? this.styles.handLeftPrev : this.styles.handLeft);
			GUI.Box(position6, GUIContent.none, this.styles.handRight);
			GUI.Box(position7, GUIContent.none, this.styles.playhead);
			Color color = Handles.color;
			Handles.color = Color.white;
			Handles.DrawLine(new Vector3(num9, 19f, 0f), new Vector3(num9, rect.height, 0f));
			Handles.color = color;
			bool flag = this.SrcStopTime - this.SrcStartTime < 0.0333333351f;
			bool flag2 = this.DstStopTime - this.DstStartTime < 0.0333333351f;
			if (this.m_DragState == TimelineControl.DragStates.Destination && !flag2)
			{
				Rect position9 = new Rect(num7 - 50f, rect3.y, 45f, rect3.height);
				string t = string.Format("{0:0%}", (num7 - num5) / (num6 - num5));
				GUI.Box(position9, EditorGUIUtility.TempContent(t), this.styles.timeBlockRight);
			}
			if (this.m_DragState == TimelineControl.DragStates.LeftSelection)
			{
				if (!flag)
				{
					Rect position10 = new Rect(num7 - 50f, rect2.y, 45f, rect2.height);
					string t2 = string.Format("{0:0%}", (num7 - num3) / (num4 - num3));
					GUI.Box(position10, EditorGUIUtility.TempContent(t2), this.styles.timeBlockRight);
				}
				if (!flag2)
				{
					Rect position11 = new Rect(num7 - 50f, rect3.y, 45f, rect3.height);
					string t3 = string.Format("{0:0%}", (num7 - num5) / (num6 - num5));
					GUI.Box(position11, EditorGUIUtility.TempContent(t3), this.styles.timeBlockRight);
				}
			}
			if (this.m_DragState == TimelineControl.DragStates.RightSelection)
			{
				if (!flag)
				{
					Rect position12 = new Rect(num8 + 5f, rect2.y, 45f, rect2.height);
					string t4 = string.Format("{0:0%}", (num8 - num3) / (num4 - num3));
					GUI.Box(position12, EditorGUIUtility.TempContent(t4), this.styles.timeBlockLeft);
				}
				if (!flag2)
				{
					Rect position13 = new Rect(num8 + 5f, rect3.y, 45f, rect3.height);
					string t5 = string.Format("{0:0%}", (num8 - num5) / (num6 - num5));
					GUI.Box(position13, EditorGUIUtility.TempContent(t5), this.styles.timeBlockLeft);
				}
			}
			this.DoPivotCurves();
			GUI.EndGroup();
			return result;
		}
	}
}
