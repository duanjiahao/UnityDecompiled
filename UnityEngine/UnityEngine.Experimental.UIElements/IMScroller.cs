using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMScroller : IMElement
	{
		private const float ScrollStepSize = 10f;

		private readonly IMSlider m_Slider;

		private readonly IMRepeatButton m_LeftButton;

		private readonly IMRepeatButton m_RightButton;

		private float m_PageSize;

		private float m_LeftValue;

		private float m_RightValue;

		public DateTime m_NextScrollStepTime = DateTime.Now;

		private static int s_ScrollControlId = 0;

		public float value
		{
			get;
			private set;
		}

		public IMScroller()
		{
			this.m_NextScrollStepTime = DateTime.Now;
			this.m_Slider = new IMSlider();
			this.m_LeftButton = new IMRepeatButton();
			this.m_RightButton = new IMRepeatButton();
		}

		public void SetProperties(Rect pos, float val, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
		{
			base.position = pos;
			this.m_PageSize = size;
			this.m_LeftValue = leftValue;
			this.m_RightValue = rightValue;
			this.value = val;
			Rect pos2;
			Rect position;
			Rect position2;
			this.GetRects(horiz, base.position, leftButton, rightButton, out pos2, out position, out position2);
			this.m_Slider.SetProperties(pos2, this.value, size, leftValue, rightValue, slider, thumb, horiz);
			this.m_LeftButton.position = position;
			this.m_LeftButton.style = leftButton;
			this.m_RightButton.position = position2;
			this.m_RightButton.style = rightButton;
		}

		public override void OnReuse()
		{
			base.OnReuse();
			this.m_Slider.OnReuse();
			this.m_LeftButton.OnReuse();
			this.m_RightButton.OnReuse();
		}

		public override bool OnGUI(Event evt)
		{
			bool flag = this.m_Slider.OnGUI(evt);
			this.value = this.m_Slider.value;
			bool flag2 = evt.type == EventType.MouseUp;
			flag |= this.m_LeftButton.OnGUI(evt);
			if (this.m_LeftButton.isPressed && this.OnScrollerButton(evt))
			{
				this.value -= 10f * ((this.m_LeftValue >= this.m_RightValue) ? -1f : 1f);
			}
			flag |= this.m_RightButton.OnGUI(evt);
			if (this.m_RightButton.isPressed && this.OnScrollerButton(evt))
			{
				this.value += 10f * ((this.m_LeftValue >= this.m_RightValue) ? -1f : 1f);
			}
			if (flag2 && evt.type == EventType.Used)
			{
				IMScroller.s_ScrollControlId = 0;
			}
			if (this.m_LeftValue < this.m_RightValue)
			{
				this.value = Mathf.Clamp(this.value, this.m_LeftValue, this.m_RightValue - this.m_PageSize);
			}
			else
			{
				this.value = Mathf.Clamp(this.value, this.m_RightValue, this.m_LeftValue - this.m_PageSize);
			}
			if (flag)
			{
				evt.Use();
			}
			this.m_Slider.value = this.value;
			return flag;
		}

		protected override int DoGenerateControlID()
		{
			this.m_Slider.GenerateControlID();
			this.m_LeftButton.GenerateControlID();
			this.m_RightButton.GenerateControlID();
			return GUIUtility.GetControlID("IMScroller".GetHashCode(), base.focusType, base.position);
		}

		private void GetRects(bool horiz, Rect pos, GUIStyle leftButton, GUIStyle rightButton, out Rect sliderRect, out Rect minRect, out Rect maxRect)
		{
			if (horiz)
			{
				sliderRect = new Rect(pos.x + leftButton.fixedWidth, pos.y, pos.width - leftButton.fixedWidth - rightButton.fixedWidth, pos.height);
				minRect = new Rect(pos.x, pos.y, leftButton.fixedWidth, pos.height);
				maxRect = new Rect(pos.xMax - rightButton.fixedWidth, pos.y, rightButton.fixedWidth, pos.height);
			}
			else
			{
				sliderRect = new Rect(pos.x, pos.y + leftButton.fixedHeight, pos.width, pos.height - leftButton.fixedHeight - rightButton.fixedHeight);
				minRect = new Rect(pos.x, pos.y, pos.width, leftButton.fixedHeight);
				maxRect = new Rect(pos.x, pos.yMax - rightButton.fixedHeight, pos.width, rightButton.fixedHeight);
			}
		}

		private bool OnScrollerButton(Event evt)
		{
			bool flag = IMScroller.s_ScrollControlId != this.m_Slider.id;
			IMScroller.s_ScrollControlId = this.m_Slider.id;
			bool result = false;
			if (flag)
			{
				result = true;
				this.m_NextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
			}
			else if (DateTime.Now >= this.m_NextScrollStepTime)
			{
				result = true;
				this.m_NextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
			}
			if (evt.type == EventType.Repaint)
			{
				GUI.InternalRepaintEditorWindow();
			}
			return result;
		}
	}
}
