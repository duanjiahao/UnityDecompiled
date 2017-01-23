using System;

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Horizontal Layout Group", 150)]
	public class HorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
	{
		protected HorizontalLayoutGroup()
		{
		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			base.CalcAlongAxis(0, false);
		}

		public override void CalculateLayoutInputVertical()
		{
			base.CalcAlongAxis(1, false);
		}

		public override void SetLayoutHorizontal()
		{
			base.SetChildrenAlongAxis(0, false);
		}

		public override void SetLayoutVertical()
		{
			base.SetChildrenAlongAxis(1, false);
		}
	}
}
