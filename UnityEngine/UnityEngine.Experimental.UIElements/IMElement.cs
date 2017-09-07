using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class IMElement : VisualElement, IOnGUIHandler, IRecyclable
	{
		private GUIStyle m_GUIStyle;

		public const int NonInteractiveControlID = 0;

		public FocusType focusType
		{
			get;
			set;
		}

		public int id
		{
			get;
			protected set;
		}

		public GUIStyle style
		{
			get
			{
				return this.m_GUIStyle;
			}
			set
			{
				this.m_GUIStyle = value;
			}
		}

		public new Rect position
		{
			get;
			set;
		}

		public bool isTrashed
		{
			get;
			set;
		}

		protected IMElement()
		{
			this.style = GUIStyle.none;
			this.focusType = FocusType.Passive;
			this.id = 0;
		}

		public virtual void OnTrash()
		{
		}

		public virtual void OnReuse()
		{
			this.style = GUIStyle.none;
			this.position = new Rect(0f, 0f, 0f, 0f);
			this.enabled = true;
			this.id = 0;
		}

		public virtual bool OnGUI(Event evt)
		{
			bool flag = false;
			switch (evt.GetTypeForControl(this.id))
			{
			case EventType.MouseDown:
				flag = this.DoMouseDown(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.MouseUp:
				flag = this.DoMouseUp(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.MouseMove:
				flag = this.DoMouseMove(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.MouseDrag:
				flag = this.DoMouseDrag(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			case EventType.KeyDown:
				flag = this.DoKeyDown(new KeyboardEventArgs(evt.character, evt.keyCode, evt.modifiers));
				break;
			case EventType.KeyUp:
				flag = this.DoKeyUp(new KeyboardEventArgs(evt.character, evt.keyCode, evt.modifiers));
				break;
			case EventType.Repaint:
				this.DoRepaint(new StylePainter(evt.mousePosition));
				break;
			case EventType.DragUpdated:
				flag = this.DoDragUpdated(new MouseEventArgs(evt.mousePosition, evt.clickCount, evt.modifiers));
				break;
			}
			if (flag)
			{
				evt.Use();
			}
			return flag;
		}

		public void AssignControlID(int id)
		{
			this.id = id;
		}

		public void GenerateControlID()
		{
			this.id = this.DoGenerateControlID();
		}

		protected abstract int DoGenerateControlID();

		protected virtual bool DoMouseDown(MouseEventArgs args)
		{
			return false;
		}

		protected virtual bool DoMouseMove(MouseEventArgs args)
		{
			return false;
		}

		protected virtual bool DoMouseUp(MouseEventArgs args)
		{
			return false;
		}

		protected virtual bool DoKeyDown(KeyboardEventArgs args)
		{
			return false;
		}

		protected virtual bool DoKeyUp(KeyboardEventArgs args)
		{
			return false;
		}

		protected virtual bool DoMouseDrag(MouseEventArgs args)
		{
			return false;
		}

		protected virtual bool DoDragUpdated(MouseEventArgs args)
		{
			return false;
		}
	}
}
