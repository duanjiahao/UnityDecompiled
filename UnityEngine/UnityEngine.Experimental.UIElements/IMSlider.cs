using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMSlider : IMElement
	{
		private float m_PageSize;

		private float m_Start;

		private float m_End;

		private GUIStyle m_SliderStyle = GUIStyle.none;

		private GUIStyle m_ThumbStyle = GUIStyle.none;

		private bool m_Horiz;

		public float value
		{
			get;
			set;
		}

		public DateTime nextScrollStepTime
		{
			get;
			set;
		}

		private int scrollTroughSide
		{
			get;
			set;
		}

		public IMSlider()
		{
			this.nextScrollStepTime = DateTime.Now;
			this.scrollTroughSide = 0;
		}

		public void SetProperties(Rect pos, float val, float pageSize, float start, float end, GUIStyle sliderStyle, GUIStyle thumbStyle, bool horiz)
		{
			base.position = pos;
			this.value = val;
			this.m_PageSize = pageSize;
			this.m_Start = start;
			this.m_End = end;
			this.m_SliderStyle = sliderStyle;
			this.m_ThumbStyle = thumbStyle;
			this.m_Horiz = horiz;
		}

		public override bool OnGUI(Event evt)
		{
			return this.m_SliderStyle != null && this.m_ThumbStyle != null && base.OnGUI(evt);
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMSlider".GetHashCode(), base.focusType, base.position);
		}

		protected override bool DoMouseDown(MouseEventArgs args)
		{
			bool result;
			if (!base.position.Contains(args.mousePosition) || this.IsEmptySlider())
			{
				result = false;
			}
			else
			{
				this.scrollTroughSide = 0;
				GUIUtility.hotControl = base.id;
				if (this.ThumbSelectionRect().Contains(args.mousePosition))
				{
					this.StartDraggingWithValue(this.Clampedvalue(), args.mousePosition);
					result = true;
				}
				else
				{
					GUI.changed = true;
					if (this.SupportsPageMovements())
					{
						this.SliderState().isDragging = false;
						this.nextScrollStepTime = SystemClock.now.AddMilliseconds(250.0);
						this.scrollTroughSide = this.CurrentScrollTroughSide(args.mousePosition);
						this.value = this.PageMovementValue(args.mousePosition);
						result = true;
					}
					else
					{
						float num = this.ValueForCurrentMousePosition(args.mousePosition);
						this.StartDraggingWithValue(num, args.mousePosition);
						this.value = this.Clamp(num);
						result = true;
					}
				}
			}
			return result;
		}

		protected override bool DoMouseDrag(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl != base.id)
			{
				result = false;
			}
			else
			{
				SliderState sliderState = this.SliderState();
				if (!sliderState.isDragging)
				{
					result = false;
				}
				else
				{
					GUI.changed = true;
					float num = this.MousePosition(args.mousePosition) - sliderState.dragStartPos;
					float val = sliderState.dragStartValue + num / this.ValuesPerPixel();
					this.value = this.Clamp(val);
					result = true;
				}
			}
			return result;
		}

		protected override bool DoMouseUp(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				GUIUtility.hotControl = 0;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal override void DoRepaint(IStylePainter args)
		{
			this.m_SliderStyle.Draw(base.position, GUIContent.none, base.id);
			if (!this.IsEmptySlider())
			{
				this.m_ThumbStyle.Draw(this.ThumbRect(), GUIContent.none, base.id);
			}
			if (GUIUtility.hotControl == base.id && base.position.Contains(args.mousePosition) && !this.IsEmptySlider())
			{
				if (this.ThumbRect().Contains(args.mousePosition))
				{
					if (this.scrollTroughSide != 0)
					{
						GUIUtility.hotControl = 0;
					}
				}
				else
				{
					GUI.InternalRepaintEditorWindow();
					if (!(SystemClock.now < this.nextScrollStepTime))
					{
						if (this.CurrentScrollTroughSide(args.mousePosition) == this.scrollTroughSide)
						{
							this.nextScrollStepTime = SystemClock.now.AddMilliseconds(30.0);
							if (this.SupportsPageMovements())
							{
								this.SliderState().isDragging = false;
								GUI.changed = true;
								this.value = this.PageMovementValue(args.mousePosition);
							}
							else
							{
								this.value = this.Clampedvalue();
							}
						}
					}
				}
			}
		}

		private int CurrentScrollTroughSide(Vector2 mousePosition)
		{
			float num = (!this.m_Horiz) ? mousePosition.y : mousePosition.x;
			float num2 = (!this.m_Horiz) ? this.ThumbRect().y : this.ThumbRect().x;
			return (num <= num2) ? -1 : 1;
		}

		private bool IsEmptySlider()
		{
			return this.m_Start == this.m_End;
		}

		private bool SupportsPageMovements()
		{
			return this.m_PageSize != 0f && GUI.usePageScrollbars;
		}

		private float PageMovementValue(Vector2 currentMousePos)
		{
			float num = this.value;
			int num2 = (this.m_Start <= this.m_End) ? 1 : -1;
			if (this.MousePosition(currentMousePos) > this.PageUpMovementBound())
			{
				num += this.m_PageSize * (float)num2 * 0.9f;
			}
			else
			{
				num -= this.m_PageSize * (float)num2 * 0.9f;
			}
			return this.Clamp(num);
		}

		private float PageUpMovementBound()
		{
			float result;
			if (this.m_Horiz)
			{
				result = this.ThumbRect().xMax - base.position.x;
			}
			else
			{
				result = this.ThumbRect().yMax - base.position.y;
			}
			return result;
		}

		private float ValueForCurrentMousePosition(Vector2 currentMousePos)
		{
			float result;
			if (this.m_Horiz)
			{
				result = (this.MousePosition(currentMousePos) - this.ThumbRect().width * 0.5f) / this.ValuesPerPixel() + this.m_Start - this.m_PageSize * 0.5f;
			}
			else
			{
				result = (this.MousePosition(currentMousePos) - this.ThumbRect().height * 0.5f) / this.ValuesPerPixel() + this.m_Start - this.m_PageSize * 0.5f;
			}
			return result;
		}

		private float Clamp(float val)
		{
			return Mathf.Clamp(val, this.MinValue(), this.MaxValue());
		}

		private Rect ThumbSelectionRect()
		{
			return this.ThumbRect();
		}

		private void StartDraggingWithValue(float dragStartValue, Vector2 currentMousePos)
		{
			SliderState sliderState = this.SliderState();
			sliderState.dragStartPos = this.MousePosition(currentMousePos);
			sliderState.dragStartValue = dragStartValue;
			sliderState.isDragging = true;
		}

		private SliderState SliderState()
		{
			return (SliderState)GUIUtility.GetStateObject(typeof(SliderState), base.id);
		}

		private Rect ThumbRect()
		{
			return (!this.m_Horiz) ? this.VerticalThumbRect() : this.HorizontalThumbRect();
		}

		private Rect VerticalThumbRect()
		{
			float num = this.ValuesPerPixel();
			Rect result;
			if (this.m_Start < this.m_End)
			{
				result = new Rect(base.position.x + (float)this.m_SliderStyle.padding.left, (this.Clampedvalue() - this.m_Start) * num + base.position.y + (float)this.m_SliderStyle.padding.top, base.position.width - (float)this.m_SliderStyle.padding.horizontal, this.m_PageSize * num + this.ThumbSize());
			}
			else
			{
				result = new Rect(base.position.x + (float)this.m_SliderStyle.padding.left, (this.Clampedvalue() + this.m_PageSize - this.m_Start) * num + base.position.y + (float)this.m_SliderStyle.padding.top, base.position.width - (float)this.m_SliderStyle.padding.horizontal, this.m_PageSize * -num + this.ThumbSize());
			}
			return result;
		}

		private Rect HorizontalThumbRect()
		{
			float num = this.ValuesPerPixel();
			Rect result;
			if (this.m_Start < this.m_End)
			{
				result = new Rect((this.Clampedvalue() - this.m_Start) * num + base.position.x + (float)this.m_SliderStyle.padding.left, base.position.y + (float)this.m_SliderStyle.padding.top, this.m_PageSize * num + this.ThumbSize(), base.position.height - (float)this.m_SliderStyle.padding.vertical);
			}
			else
			{
				result = new Rect((this.Clampedvalue() + this.m_PageSize - this.m_Start) * num + base.position.x + (float)this.m_SliderStyle.padding.left, base.position.y, this.m_PageSize * -num + this.ThumbSize(), base.position.height);
			}
			return result;
		}

		private float Clampedvalue()
		{
			return this.Clamp(this.value);
		}

		private float MousePosition(Vector2 currentMousePos)
		{
			float result;
			if (this.m_Horiz)
			{
				result = currentMousePos.x - base.position.x;
			}
			else
			{
				result = currentMousePos.y - base.position.y;
			}
			return result;
		}

		private float ValuesPerPixel()
		{
			float result;
			if (this.m_Horiz)
			{
				result = (base.position.width - (float)this.m_SliderStyle.padding.horizontal - this.ThumbSize()) / (this.m_End - this.m_Start);
			}
			else
			{
				result = (base.position.height - (float)this.m_SliderStyle.padding.vertical - this.ThumbSize()) / (this.m_End - this.m_Start);
			}
			return result;
		}

		private float ThumbSize()
		{
			float result;
			if (this.m_Horiz)
			{
				result = ((this.m_ThumbStyle.fixedWidth == 0f) ? ((float)this.m_ThumbStyle.padding.horizontal) : this.m_ThumbStyle.fixedWidth);
			}
			else
			{
				result = ((this.m_ThumbStyle.fixedHeight == 0f) ? ((float)this.m_ThumbStyle.padding.vertical) : this.m_ThumbStyle.fixedHeight);
			}
			return result;
		}

		private float MaxValue()
		{
			return Mathf.Max(this.m_Start, this.m_End) - this.m_PageSize;
		}

		private float MinValue()
		{
			return Mathf.Min(this.m_Start, this.m_End);
		}
	}
}
