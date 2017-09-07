using System;
using System.Threading;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public class Slider : VisualContainer
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

		private Rect m_DragElementStartPos;

		private float m_Value;

		private Slider.Direction m_Direction;

		public event Action<float> valueChanged
		{
			add
			{
				Action<float> action = this.valueChanged;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<float>>(ref this.valueChanged, (Action<float>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<float> action = this.valueChanged;
				Action<float> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<float>>(ref this.valueChanged, (Action<float>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public VisualElement dragElement
		{
			get;
			private set;
		}

		public float lowValue
		{
			get;
			set;
		}

		public float highValue
		{
			get;
			set;
		}

		public float range
		{
			get
			{
				return this.highValue - this.lowValue;
			}
		}

		public float pageSize
		{
			get;
			set;
		}

		internal ClampedDragger clampedDragger
		{
			get;
			private set;
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				float num = Mathf.Clamp(value, this.lowValue, this.highValue);
				if (!Mathf.Approximately(this.m_Value, num))
				{
					this.m_Value = num;
					this.UpdateDragElementPosition();
					if (this.valueChanged != null)
					{
						this.valueChanged(this.m_Value);
					}
					base.Dirty(ChangeType.Repaint);
				}
			}
		}

		public Slider.Direction direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				this.m_Direction = value;
				if (this.m_Direction == Slider.Direction.Horizontal)
				{
					base.RemoveFromClassList("vertical");
					base.AddToClassList("horizontal");
				}
				else
				{
					base.RemoveFromClassList("horizontal");
					base.AddToClassList("vertical");
				}
			}
		}

		public Slider(float start, float end, Action<float> valueChanged, Slider.Direction direction = Slider.Direction.Horizontal, float pageSize = 10f)
		{
			this.valueChanged = valueChanged;
			this.direction = direction;
			this.pageSize = pageSize;
			this.lowValue = start;
			this.highValue = end;
			base.AddChild(new VisualElement
			{
				name = "TrackElement"
			});
			this.m_Value = this.lowValue;
			this.dragElement = new VisualElement
			{
				name = "DragElement"
			};
			base.AddChild(this.dragElement);
			this.clampedDragger = new ClampedDragger(this, new Action(this.SetSliderValueFromClick), new Action(this.SetSliderValueFromDrag));
			base.AddManipulator(this.clampedDragger);
		}

		private void SetSliderValueFromDrag()
		{
			if (this.clampedDragger.dragDirection == ClampedDragger.DragDirection.Free)
			{
				Vector2 delta = this.clampedDragger.delta;
				if (this.direction == Slider.Direction.Horizontal)
				{
					this.ComputeValueAndDirectionFromDrag(base.position.width, this.dragElement.position.width, this.m_DragElementStartPos.x + delta.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromDrag(base.position.height, this.dragElement.position.height, this.m_DragElementStartPos.y + delta.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				this.value = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num * this.range + this.lowValue;
			}
		}

		private void SetSliderValueFromClick()
		{
			if (this.clampedDragger.dragDirection != ClampedDragger.DragDirection.Free)
			{
				if (this.clampedDragger.dragDirection == ClampedDragger.DragDirection.None)
				{
					if (this.pageSize == 0f)
					{
						float x = (this.direction != Slider.Direction.Horizontal) ? this.dragElement.position.x : (this.clampedDragger.startMousePosition.x - this.dragElement.position.width / 2f);
						float y = (this.direction != Slider.Direction.Horizontal) ? (this.clampedDragger.startMousePosition.y - this.dragElement.position.height / 2f) : this.dragElement.position.y;
						Rect rect = new Rect(x, y, this.dragElement.position.width, this.dragElement.position.height);
						this.dragElement.position = rect;
						this.m_DragElementStartPos = rect;
						this.clampedDragger.dragDirection = ClampedDragger.DragDirection.Free;
						if (this.direction == Slider.Direction.Horizontal)
						{
							this.ComputeValueAndDirectionFromDrag(base.position.width, this.dragElement.position.width, this.m_DragElementStartPos.x);
						}
						else
						{
							this.ComputeValueAndDirectionFromDrag(base.position.height, this.dragElement.position.height, this.m_DragElementStartPos.y);
						}
						return;
					}
					this.m_DragElementStartPos = this.dragElement.position;
				}
				if (this.direction == Slider.Direction.Horizontal)
				{
					this.ComputeValueAndDirectionFromClick(base.position.width, this.dragElement.position.width, this.dragElement.position.x, this.clampedDragger.lastMousePosition.x);
				}
				else
				{
					this.ComputeValueAndDirectionFromClick(base.position.height, this.dragElement.position.height, this.dragElement.position.y, this.clampedDragger.lastMousePosition.y);
				}
			}
		}

		private void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
		{
			float num = sliderLength - dragElementLength;
			if (Mathf.Abs(num) >= Mathf.Epsilon)
			{
				if (dragElementLastPos < dragElementPos && this.clampedDragger.dragDirection != ClampedDragger.DragDirection.LowToHigh)
				{
					this.clampedDragger.dragDirection = ClampedDragger.DragDirection.HighToLow;
					this.value = Mathf.Max(0f, Mathf.Min(dragElementPos - this.pageSize, num)) / num * this.range + this.lowValue;
				}
				else if (dragElementLastPos > dragElementPos + dragElementLength && this.clampedDragger.dragDirection != ClampedDragger.DragDirection.HighToLow)
				{
					this.clampedDragger.dragDirection = ClampedDragger.DragDirection.LowToHigh;
					this.value = Mathf.Max(0f, Mathf.Min(dragElementPos + this.pageSize, num)) / num * this.range + this.lowValue;
				}
			}
		}

		public void AdjustDragElement(float factor)
		{
			Rect position;
			if (factor >= 1f)
			{
				if (this.direction == Slider.Direction.Horizontal)
				{
					position = new Rect(this.dragElement.position.x, this.dragElement.position.y, 0f, this.dragElement.height);
				}
				else
				{
					position = new Rect(this.dragElement.position.x, this.dragElement.position.y, this.dragElement.width, 0f);
				}
			}
			else
			{
				VisualElementStyles styles = this.dragElement.styles;
				position = this.dragElement.position;
				if (this.direction == Slider.Direction.Horizontal)
				{
					float specifiedValueOrDefault = styles.minWidth.GetSpecifiedValueOrDefault(0f);
					position.width = Mathf.Max(base.position.width * factor, specifiedValueOrDefault);
				}
				else
				{
					float specifiedValueOrDefault2 = styles.minHeight.GetSpecifiedValueOrDefault(0f);
					position.height = Mathf.Max(base.position.height * factor, specifiedValueOrDefault2);
				}
			}
			this.dragElement.position = position;
		}

		private void UpdateDragElementPosition()
		{
			if (base.panel != null)
			{
				float num = this.m_Value - this.lowValue;
				float width = this.dragElement.position.width;
				float height = this.dragElement.position.height;
				if (this.direction == Slider.Direction.Horizontal)
				{
					float num2 = base.position.width - width;
					this.dragElement.position = new Rect(num / this.range * num2, this.dragElement.position.y, width, height);
				}
				else
				{
					float num3 = base.position.height - height;
					this.dragElement.position = new Rect(this.dragElement.position.x, num / this.range * num3, width, height);
				}
			}
		}

		protected internal override void OnPostLayout(bool hasNewLayout)
		{
			if (hasNewLayout)
			{
				this.UpdateDragElementPosition();
			}
		}
	}
}
