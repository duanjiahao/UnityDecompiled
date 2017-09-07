using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMButton : IMElement
	{
		public bool wasPressed
		{
			get;
			private set;
		}

		public override bool OnGUI(Event evt)
		{
			this.wasPressed = false;
			return base.OnGUI(evt);
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMButton".GetHashCode(), base.focusType, base.position);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id, false);
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

		protected override bool DoMouseMove(MouseEventArgs args)
		{
			return false;
		}

		protected override bool DoMouseUp(MouseEventArgs args)
		{
			bool result;
			if (GUIUtility.hotControl == base.id)
			{
				GUIUtility.hotControl = 0;
				if (base.position.Contains(args.mousePosition))
				{
					GUIUtility.SetChanged(true);
					this.wasPressed = true;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected override bool DoKeyDown(KeyboardEventArgs args)
		{
			bool result;
			if (args.character == ' ' && GUIUtility.keyboardControl == base.id)
			{
				GUIUtility.SetChanged(true);
				this.wasPressed = true;
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
	}
}
