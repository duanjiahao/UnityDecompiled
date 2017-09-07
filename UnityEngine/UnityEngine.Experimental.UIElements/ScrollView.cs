using System;

namespace UnityEngine.Experimental.UIElements
{
	public class ScrollView : VisualContainer
	{
		public static readonly Vector2 kDefaultScrollerValues = new Vector2(0f, 100f);

		private Vector2 m_ScrollOffset;

		public Vector2 horizontalScrollerValues
		{
			get;
			set;
		}

		public Vector2 verticalScrollerValues
		{
			get;
			set;
		}

		public bool showHorizontal
		{
			get;
			set;
		}

		public bool showVertical
		{
			get;
			set;
		}

		public bool needsHorizontal
		{
			get
			{
				return this.showHorizontal || this.contentView.position.width - base.position.width > 0f;
			}
		}

		public bool needsVertical
		{
			get
			{
				return this.showVertical || this.contentView.position.height - base.position.height > 0f;
			}
		}

		public Vector2 scrollOffset
		{
			get
			{
				return this.m_ScrollOffset;
			}
			set
			{
				this.m_ScrollOffset = value;
				this.UpdateContentViewTransform();
			}
		}

		public VisualContainer contentView
		{
			get;
			private set;
		}

		public VisualContainer contentViewport
		{
			get;
			private set;
		}

		public Scroller horizontalScroller
		{
			get;
			private set;
		}

		public Scroller verticalScroller
		{
			get;
			private set;
		}

		public ScrollView() : this(ScrollView.kDefaultScrollerValues, ScrollView.kDefaultScrollerValues)
		{
		}

		public ScrollView(Vector2 horizontalScrollerValues, Vector2 verticalScrollerValues)
		{
			this.horizontalScrollerValues = horizontalScrollerValues;
			this.verticalScrollerValues = verticalScrollerValues;
			this.contentView = new VisualContainer
			{
				name = "ContentView"
			};
			this.contentViewport = new VisualContainer
			{
				name = "ContentViewport"
			};
			this.contentViewport.clipChildren = true;
			this.contentViewport.AddChild(this.contentView);
			base.AddChild(this.contentViewport);
			this.horizontalScroller = new Scroller(horizontalScrollerValues.x, horizontalScrollerValues.y, delegate(float value)
			{
				this.scrollOffset = new Vector2(value, this.scrollOffset.y);
			}, Slider.Direction.Horizontal)
			{
				name = "HorizontalScroller"
			};
			base.AddChild(this.horizontalScroller);
			this.verticalScroller = new Scroller(verticalScrollerValues.x, verticalScrollerValues.y, delegate(float value)
			{
				this.scrollOffset = new Vector2(this.scrollOffset.x, value);
			}, Slider.Direction.Vertical)
			{
				name = "VerticalScroller"
			};
			base.AddChild(this.verticalScroller);
		}

		private void UpdateContentViewTransform()
		{
			Vector2 scrollOffset = this.m_ScrollOffset;
			scrollOffset.x -= this.horizontalScroller.lowValue;
			scrollOffset.x /= this.horizontalScroller.highValue - this.horizontalScroller.lowValue;
			scrollOffset.y -= this.verticalScroller.lowValue;
			scrollOffset.y /= this.verticalScroller.highValue - this.verticalScroller.lowValue;
			float num = this.contentView.position.width - this.contentViewport.position.width;
			float num2 = this.contentView.position.height - this.contentViewport.position.height;
			Matrix4x4 transform = this.contentView.transform;
			transform.m03 = -(scrollOffset.x * num);
			transform.m13 = -(scrollOffset.y * num2);
			this.contentView.transform = transform;
			base.Dirty(ChangeType.Repaint);
		}

		protected internal override void OnPostLayout(bool hasNewLayout)
		{
			if (hasNewLayout)
			{
				if (this.contentView.position.width > Mathf.Epsilon)
				{
					this.horizontalScroller.Adjust(this.contentViewport.position.width / this.contentView.position.width);
				}
				if (this.contentView.position.height > Mathf.Epsilon)
				{
					this.verticalScroller.Adjust(this.contentViewport.position.height / this.contentView.position.height);
				}
				this.horizontalScroller.enabled = (this.contentView.position.width - base.position.width > 0f);
				this.verticalScroller.enabled = (this.contentView.position.height - base.position.height > 0f);
				this.horizontalScroller.visible = this.needsHorizontal;
				this.verticalScroller.visible = this.needsVertical;
				this.UpdateContentViewTransform();
			}
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			EventType type = evt.type;
			EventPropagation result;
			if (type != EventType.ScrollWheel)
			{
				result = EventPropagation.Continue;
			}
			else
			{
				if (this.contentView.position.height - base.position.height > 0f)
				{
					if (evt.delta.y < 0f)
					{
						this.verticalScroller.ScrollPageUp();
					}
					else if (evt.delta.y > 0f)
					{
						this.verticalScroller.ScrollPageDown();
					}
				}
				result = EventPropagation.Stop;
			}
			return result;
		}
	}
}
