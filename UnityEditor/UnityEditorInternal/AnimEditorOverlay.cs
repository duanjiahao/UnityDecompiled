using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimEditorOverlay
	{
		public AnimationWindowState state;

		private TimeCursorManipulator m_PlayHeadCursor;

		private Rect m_Rect;

		private Rect m_ContentRect;

		public Rect rect
		{
			get
			{
				return this.m_Rect;
			}
		}

		public Rect contentRect
		{
			get
			{
				return this.m_ContentRect;
			}
		}

		public void Initialize()
		{
			if (this.m_PlayHeadCursor == null)
			{
				this.m_PlayHeadCursor = new TimeCursorManipulator(AnimationWindowStyles.playHead);
				TimeCursorManipulator expr_23 = this.m_PlayHeadCursor;
				expr_23.onStartDrag = (AnimationWindowManipulator.OnStartDragDelegate)Delegate.Combine(expr_23.onStartDrag, new AnimationWindowManipulator.OnStartDragDelegate((AnimationWindowManipulator manipulator, Event evt) => evt.mousePosition.y <= this.m_Rect.yMin + 20f && this.OnStartDragPlayHead(evt)));
				TimeCursorManipulator expr_4A = this.m_PlayHeadCursor;
				expr_4A.onDrag = (AnimationWindowManipulator.OnDragDelegate)Delegate.Combine(expr_4A.onDrag, new AnimationWindowManipulator.OnDragDelegate((AnimationWindowManipulator manipulator, Event evt) => this.OnDragPlayHead(evt)));
				TimeCursorManipulator expr_71 = this.m_PlayHeadCursor;
				expr_71.onEndDrag = (AnimationWindowManipulator.OnEndDragDelegate)Delegate.Combine(expr_71.onEndDrag, new AnimationWindowManipulator.OnEndDragDelegate((AnimationWindowManipulator manipulator, Event evt) => this.OnEndDragPlayHead(evt)));
			}
		}

		public void OnGUI(Rect rect, Rect contentRect)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.m_Rect = rect;
				this.m_ContentRect = contentRect;
				this.Initialize();
				this.m_PlayHeadCursor.OnGUI(this.m_Rect, this.m_Rect.xMin + this.TimeToPixel(this.state.currentTime));
			}
		}

		public void HandleEvents()
		{
			this.Initialize();
			this.m_PlayHeadCursor.HandleEvents();
		}

		private bool OnStartDragPlayHead(Event evt)
		{
			this.state.controlInterface.StopPlayback();
			this.state.controlInterface.StartScrubTime();
			this.state.controlInterface.ScrubTime(this.MousePositionToTime(evt));
			return true;
		}

		private bool OnDragPlayHead(Event evt)
		{
			this.state.controlInterface.ScrubTime(this.MousePositionToTime(evt));
			return true;
		}

		private bool OnEndDragPlayHead(Event evt)
		{
			this.state.controlInterface.EndScrubTime();
			return true;
		}

		public float MousePositionToTime(Event evt)
		{
			float width = this.m_ContentRect.width;
			float time = Mathf.Max(evt.mousePosition.x / width * this.state.visibleTimeSpan + this.state.minVisibleTime, 0f);
			return this.state.SnapToFrame(time, AnimationWindowState.SnapMode.SnapToFrame);
		}

		public float MousePositionToValue(Event evt)
		{
			float height = this.m_ContentRect.height;
			float num = height - evt.mousePosition.y;
			TimeArea timeArea = this.state.timeArea;
			float num2 = timeArea.m_Scale.y * -1f;
			float num3 = timeArea.shownArea.yMin * num2 * -1f;
			return (num - num3) / num2;
		}

		public float TimeToPixel(float time)
		{
			return this.state.TimeToPixel(time);
		}

		public float ValueToPixel(float value)
		{
			TimeArea timeArea = this.state.timeArea;
			float num = timeArea.m_Scale.y * -1f;
			float num2 = timeArea.shownArea.yMin * num * -1f;
			return value * num + num2;
		}
	}
}
