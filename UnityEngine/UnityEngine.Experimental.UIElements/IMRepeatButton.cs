using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMRepeatButton : IMElement
	{
		public bool isPressed
		{
			get;
			private set;
		}

		public IMRepeatButton()
		{
			base.focusType = FocusType.Keyboard;
		}

		public override bool OnGUI(Event evt)
		{
			this.isPressed = false;
			return base.OnGUI(evt);
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMRepeatButton".GetHashCode(), base.focusType, base.position);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id);
			if (GUIUtility.hotControl == base.id)
			{
				this.isPressed = base.position.Contains(args.mousePosition);
			}
		}

		protected override bool DoMouseDown(MouseEventArgs args)
		{
			bool result;
			if (base.position.Contains(args.mousePosition))
			{
				GUIUtility.hotControl = base.id;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoMouseUp(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				GUIUtility.hotControl = 0;
				this.isPressed = base.position.Contains(args.mousePosition);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoMouseDrag(MouseEventArgs args)
		{
			return GUIUtility.hotControl == base.id;
		}

		protected override bool DoKeyDown(KeyboardEventArgs args)
		{
			bool result;
			if (args.character == ' ' && GUIUtility.keyboardControl == base.id)
			{
				GUIUtility.SetChanged(true);
				this.isPressed = true;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoKeyUp(KeyboardEventArgs args)
		{
			bool result;
			if (args.character == ' ' && GUIUtility.keyboardControl == base.id)
			{
				this.isPressed = false;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
