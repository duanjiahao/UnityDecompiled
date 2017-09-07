using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMScrollView : IMContainer
	{
		private ScrollViewState m_State;

		private Vector2 m_ScrollPosition;

		private GUIStyle m_HorizontalScrollbar;

		private GUIStyle m_VerticalScrollbar;

		private GUIStyle m_Background;

		public Rect viewRect;

		private bool m_NeedsVertical;

		private bool m_NeedsHorizontal;

		private Rect m_ClipRect;

		private readonly IMScroller m_HorizontalScroller;

		private readonly IMScroller m_VerticalScroller;

		public Vector2 scrollPosition
		{
			get
			{
				return this.m_ScrollPosition;
			}
			private set
			{
				this.m_ScrollPosition = value;
			}
		}

		public float scrollPositionHorizontal
		{
			get
			{
				return this.m_ScrollPosition.x;
			}
			set
			{
				this.m_ScrollPosition.x = value;
			}
		}

		public float scrollPositionVertical
		{
			get
			{
				return this.m_ScrollPosition.y;
			}
			set
			{
				this.m_ScrollPosition.y = value;
			}
		}

		public IMScrollView()
		{
			this.m_HorizontalScrollbar = GUIStyle.none;
			this.m_VerticalScrollbar = GUIStyle.none;
			this.m_Background = GUIStyle.none;
			this.m_HorizontalScroller = new IMScroller();
			this.m_VerticalScroller = new IMScroller();
		}

		public void SetProperties(Rect pos, Vector2 scrollPos, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			base.position = pos;
			this.scrollPosition = scrollPos;
			this.viewRect = viewRect;
			this.m_HorizontalScrollbar = horizontalScrollbar;
			this.m_VerticalScrollbar = verticalScrollbar;
			this.m_Background = background;
			this.m_NeedsVertical = alwaysShowVertical;
			this.m_NeedsHorizontal = alwaysShowHorizontal;
			this.CheckState();
			GUIStyle thumb = (this.m_HorizontalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_HorizontalScrollbar.name + "thumb");
			GUIStyle leftButton = (this.m_HorizontalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_HorizontalScrollbar.name + "leftbutton");
			GUIStyle rightButton = (this.m_HorizontalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_HorizontalScrollbar.name + "rightbutton");
			this.m_HorizontalScroller.SetProperties(new Rect(base.position.x, base.position.yMax - this.m_HorizontalScrollbar.fixedHeight, this.m_ClipRect.width, this.m_HorizontalScrollbar.fixedHeight), this.scrollPosition.x, Mathf.Min(this.m_ClipRect.width, this.viewRect.width), 0f, this.viewRect.width, this.m_HorizontalScrollbar, thumb, leftButton, rightButton, true);
			GUIStyle thumb2 = (this.m_VerticalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_VerticalScrollbar.name + "thumb");
			GUIStyle leftButton2 = (this.m_VerticalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_VerticalScrollbar.name + "upbutton");
			GUIStyle rightButton2 = (this.m_VerticalScrollbar == GUIStyle.none) ? GUIStyle.none : GUI.skin.GetStyle(this.m_VerticalScrollbar.name + "downbutton");
			this.m_VerticalScroller.SetProperties(new Rect(this.m_ClipRect.xMax + (float)this.m_VerticalScrollbar.margin.left, this.m_ClipRect.y, this.m_VerticalScrollbar.fixedWidth, this.m_ClipRect.height), this.scrollPosition.y, Mathf.Min(this.m_ClipRect.height, this.viewRect.height), 0f, this.viewRect.height, this.m_VerticalScrollbar, thumb2, leftButton2, rightButton2, false);
		}

		public override void OnReuse()
		{
			base.OnReuse();
			this.m_HorizontalScroller.OnReuse();
			this.m_VerticalScroller.OnReuse();
		}

		public void HandleScrollWheel(Event evt)
		{
			if (this.m_State.position.Contains(evt.mousePosition))
			{
				this.m_State.scrollPosition.x = Mathf.Clamp(this.m_State.scrollPosition.x + evt.delta.x * 20f, 0f, this.m_State.viewRect.width - this.m_State.visibleRect.width);
				this.m_State.scrollPosition.y = Mathf.Clamp(this.m_State.scrollPosition.y + evt.delta.y * 20f, 0f, this.m_State.viewRect.height - this.m_State.visibleRect.height);
				this.m_State.apply = true;
				evt.Use();
			}
		}

		public override bool OnGUI(Event evt)
		{
			bool flag = false;
			switch (evt.GetTypeForControl(base.id))
			{
			case EventType.Layout:
			case EventType.Used:
				goto IL_1F0;
			case EventType.DragUpdated:
				flag = this.DoDragUpdated(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				goto IL_1F0;
			}
			if (evt.type == EventType.Repaint && this.m_Background != GUIStyle.none)
			{
				this.m_Background.Draw(base.position, base.position.Contains(evt.mousePosition), false, this.m_NeedsHorizontal && this.m_NeedsVertical, false);
			}
			if (this.m_NeedsHorizontal && this.m_HorizontalScrollbar != GUIStyle.none)
			{
				flag = this.m_HorizontalScroller.OnGUI(evt);
				this.scrollPositionHorizontal = this.m_HorizontalScroller.value;
			}
			else if (this.m_HorizontalScrollbar != GUIStyle.none)
			{
				this.scrollPositionHorizontal = 0f;
			}
			else
			{
				this.scrollPositionHorizontal = Mathf.Clamp(this.scrollPositionHorizontal, 0f, Mathf.Max(this.viewRect.width - base.position.width, 0f));
			}
			if (this.m_NeedsVertical && this.m_VerticalScrollbar != GUIStyle.none)
			{
				flag = this.m_VerticalScroller.OnGUI(evt);
				this.scrollPositionVertical = this.m_VerticalScroller.value;
			}
			else if (this.m_VerticalScrollbar != GUIStyle.none)
			{
				this.scrollPositionVertical = 0f;
			}
			else
			{
				this.scrollPositionVertical = Mathf.Clamp(this.scrollPositionVertical, 0f, Mathf.Max(this.viewRect.height - base.position.height, 0f));
			}
			IL_1F0:
			if (flag)
			{
				evt.Use();
			}
			GUIClip.Internal_Push(this.m_ClipRect, new Vector2(Mathf.Round(-this.scrollPosition.x - this.viewRect.x), Mathf.Round(-this.scrollPosition.y - this.viewRect.y)), Vector2.zero, false);
			return flag;
		}

		public void ScrollTo(Rect pos)
		{
			this.m_State.ScrollTo(pos);
		}

		public bool ScrollTowards(Rect pos, float maxDelta)
		{
			return this.m_State.ScrollTowards(pos, maxDelta);
		}

		public override void GenerateControlID()
		{
			this.m_HorizontalScroller.GenerateControlID();
			this.m_VerticalScroller.GenerateControlID();
			base.id = GUIUtility.GetControlID("ScrollView".GetHashCode(), FocusType.Passive);
		}

		private void CheckState()
		{
			Debug.Assert(base.id != 0, "Invalid zero control ID");
			this.m_State = (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), base.id);
			if (this.m_State.apply)
			{
				this.scrollPosition = this.m_State.scrollPosition;
				this.m_State.apply = false;
			}
			this.m_State.position = base.position;
			this.m_State.scrollPosition = this.scrollPosition;
			this.m_State.viewRect = this.viewRect;
			this.m_State.visibleRect = this.viewRect;
			this.m_State.visibleRect.width = base.position.width;
			this.m_State.visibleRect.height = base.position.height;
			this.m_ClipRect = new Rect(base.position);
			if (this.m_NeedsHorizontal || this.viewRect.width > this.m_ClipRect.width)
			{
				this.m_State.visibleRect.height = base.position.height - this.m_HorizontalScrollbar.fixedHeight + (float)this.m_HorizontalScrollbar.margin.top;
				this.m_ClipRect.height = this.m_ClipRect.height - (this.m_HorizontalScrollbar.fixedHeight + (float)this.m_HorizontalScrollbar.margin.top);
				this.m_NeedsHorizontal = true;
			}
			if (this.m_NeedsVertical || this.viewRect.height > this.m_ClipRect.height)
			{
				this.m_State.visibleRect.width = base.position.width - this.m_VerticalScrollbar.fixedWidth + (float)this.m_VerticalScrollbar.margin.left;
				this.m_ClipRect.width = this.m_ClipRect.width - (this.m_VerticalScrollbar.fixedWidth + (float)this.m_VerticalScrollbar.margin.left);
				this.m_NeedsVertical = true;
				if (!this.m_NeedsHorizontal && this.viewRect.width > this.m_ClipRect.width)
				{
					this.m_State.visibleRect.height = base.position.height - this.m_HorizontalScrollbar.fixedHeight + (float)this.m_HorizontalScrollbar.margin.top;
					this.m_ClipRect.height = this.m_ClipRect.height - (this.m_HorizontalScrollbar.fixedHeight + (float)this.m_HorizontalScrollbar.margin.top);
					this.m_NeedsHorizontal = true;
				}
			}
		}

		private bool DoDragUpdated(MouseEventArgs args)
		{
			if (base.position.Contains(args.mousePosition))
			{
				if (Mathf.Abs(args.mousePosition.y - base.position.y) < 8f)
				{
					this.scrollPositionVertical -= 16f;
					GUI.InternalRepaintEditorWindow();
				}
				else if (Mathf.Abs(args.mousePosition.y - base.position.yMax) < 8f)
				{
					this.scrollPositionVertical += 16f;
					GUI.InternalRepaintEditorWindow();
				}
			}
			return false;
		}
	}
}
