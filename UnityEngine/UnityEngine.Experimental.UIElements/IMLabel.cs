using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class IMLabel : IMElement
	{
		public void ShowTooltip(Vector2 tooltipPos)
		{
			if (!string.IsNullOrEmpty(base.tooltip))
			{
				if (base.position.Contains(tooltipPos))
				{
					GUIStyle.SetMouseTooltip(base.tooltip, base.position);
				}
			}
		}

		protected override int DoGenerateControlID()
		{
			return 0;
		}

		internal override void DoRepaint(IStylePainter args)
		{
			base.style.Draw(base.position, GUIContent.Temp(base.text), false, false, false, false);
			this.ShowTooltip(args.mousePosition);
		}
	}
}
