using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMGroup : IMContainer
	{
		public override bool OnGUI(Event evt)
		{
			if (!string.IsNullOrEmpty(base.text) || base.style != GUIStyle.none)
			{
				EventType type = evt.type;
				if (type != EventType.Repaint)
				{
					if (base.position.Contains(evt.mousePosition))
					{
						GUIUtility.mouseUsed = true;
					}
				}
				else
				{
					this.DoRepaint(new StylePainter(evt.mousePosition));
				}
			}
			return false;
		}

		public override void GenerateControlID()
		{
			base.id = GUIUtility.GetControlID("IMGroup".GetHashCode(), FocusType.Passive);
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), base.id);
		}
	}
}
