using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMToggle : IMElement
	{
		public bool value
		{
			get;
			set;
		}

		public IMToggle()
		{
			base.focusType = FocusType.Passive;
		}

		public void ForceIdValue(int newId)
		{
			base.id = newId;
		}

		protected override int DoGenerateControlID()
		{
			return GUIUtility.GetControlID("IMToggle".GetHashCode(), base.focusType, base.position);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id, this.value);
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
				if (base.position.Contains(args.mousePosition))
				{
					GUIUtility.SetChanged(true);
					this.value = !this.value;
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
				this.value = !this.value;
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
