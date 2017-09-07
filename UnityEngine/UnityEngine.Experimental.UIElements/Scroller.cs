using System;
using System.Threading;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements
{
	public class Scroller : VisualContainer
	{
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

		public Slider slider
		{
			get;
			private set;
		}

		public ScrollerButton lowButton
		{
			get;
			private set;
		}

		public ScrollerButton highButton
		{
			get;
			private set;
		}

		public float value
		{
			get
			{
				return this.slider.value;
			}
			set
			{
				this.slider.value = value;
				if (this.valueChanged != null)
				{
					this.valueChanged(this.slider.value);
				}
				base.Dirty(ChangeType.Repaint);
			}
		}

		public float lowValue
		{
			get
			{
				return this.slider.lowValue;
			}
		}

		public float highValue
		{
			get
			{
				return this.slider.highValue;
			}
		}

		public Slider.Direction direction
		{
			get
			{
				return (base.flexDirection != FlexDirection.Row) ? Slider.Direction.Vertical : Slider.Direction.Horizontal;
			}
			set
			{
				if (value == Slider.Direction.Horizontal)
				{
					base.flexDirection = FlexDirection.Row;
					base.AddToClassList("horizontal");
				}
				else
				{
					base.flexDirection = FlexDirection.Column;
					base.AddToClassList("vertical");
				}
			}
		}

		public override bool enabled
		{
			get
			{
				return base.enabled;
			}
			set
			{
				base.enabled = value;
				this.PropagateEnabled(this, value);
			}
		}

		public Scroller(float lowValue, float highValue, Action<float> valueChanged, Slider.Direction direction = Slider.Direction.Vertical)
		{
			base.phaseInterest = EventPhase.BubbleUp;
			this.direction = direction;
			this.valueChanged = valueChanged;
			this.slider = new Slider(lowValue, highValue, new Action<float>(this.OnSliderValueChange), direction, 10f)
			{
				name = "Slider"
			};
			base.AddChild(this.slider);
			this.lowButton = new ScrollerButton(new Action(this.ScrollPageUp), 250L, 30L)
			{
				name = "LowButton"
			};
			base.AddChild(this.lowButton);
			this.highButton = new ScrollerButton(new Action(this.ScrollPageDown), 250L, 30L)
			{
				name = "HighButton"
			};
			base.AddChild(this.highButton);
		}

		public void PropagateEnabled(VisualContainer c, bool enabled)
		{
			if (c != null)
			{
				foreach (VisualElement current in c)
				{
					current.enabled = enabled;
					this.PropagateEnabled(current as VisualContainer, enabled);
				}
			}
		}

		public void Adjust(float factor)
		{
			this.enabled = (factor < 1f);
			this.slider.AdjustDragElement(factor);
		}

		private void OnSliderValueChange(float newValue)
		{
			this.value = newValue;
		}

		public void ScrollPageUp()
		{
			this.value -= this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? -1f : 1f);
		}

		public void ScrollPageDown()
		{
			this.value += this.slider.pageSize * ((this.slider.lowValue >= this.slider.highValue) ? -1f : 1f);
		}
	}
}
