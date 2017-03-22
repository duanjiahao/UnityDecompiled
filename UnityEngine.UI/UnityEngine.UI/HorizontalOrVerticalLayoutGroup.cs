using System;

namespace UnityEngine.UI
{
	public abstract class HorizontalOrVerticalLayoutGroup : LayoutGroup
	{
		[SerializeField]
		protected float m_Spacing = 0f;

		[SerializeField]
		protected bool m_ChildForceExpandWidth = true;

		[SerializeField]
		protected bool m_ChildForceExpandHeight = true;

		[SerializeField]
		protected bool m_ChildControlWidth = true;

		[SerializeField]
		protected bool m_ChildControlHeight = true;

		public float spacing
		{
			get
			{
				return this.m_Spacing;
			}
			set
			{
				base.SetProperty<float>(ref this.m_Spacing, value);
			}
		}

		public bool childForceExpandWidth
		{
			get
			{
				return this.m_ChildForceExpandWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
			}
		}

		public bool childForceExpandHeight
		{
			get
			{
				return this.m_ChildForceExpandHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
			}
		}

		public bool childControlWidth
		{
			get
			{
				return this.m_ChildControlWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildControlWidth, value);
			}
		}

		public bool childControlHeight
		{
			get
			{
				return this.m_ChildControlHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildControlHeight, value);
			}
		}

		protected void CalcAlongAxis(int axis, bool isVertical)
		{
			float num = (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
			bool controlSize = (axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth;
			bool childForceExpand = (axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth;
			float num2 = num;
			float num3 = num;
			float num4 = 0f;
			bool flag = isVertical ^ axis == 1;
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				RectTransform child = base.rectChildren[i];
				float num5;
				float num6;
				float num7;
				this.GetChildSizes(child, axis, controlSize, childForceExpand, out num5, out num6, out num7);
				if (flag)
				{
					num2 = Mathf.Max(num5 + num, num2);
					num3 = Mathf.Max(num6 + num, num3);
					num4 = Mathf.Max(num7, num4);
				}
				else
				{
					num2 += num5 + this.spacing;
					num3 += num6 + this.spacing;
					num4 += num7;
				}
			}
			if (!flag && base.rectChildren.Count > 0)
			{
				num2 -= this.spacing;
				num3 -= this.spacing;
			}
			num3 = Mathf.Max(num2, num3);
			base.SetLayoutInputForAxis(num2, num3, num4, axis);
		}

		protected void SetChildrenAlongAxis(int axis, bool isVertical)
		{
			float num = base.rectTransform.rect.size[axis];
			bool flag = (axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth;
			bool childForceExpand = (axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth;
			float alignmentOnAxis = base.GetAlignmentOnAxis(axis);
			bool flag2 = isVertical ^ axis == 1;
			if (flag2)
			{
				float value = num - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					float min;
					float num2;
					float num3;
					this.GetChildSizes(rectTransform, axis, flag, childForceExpand, out min, out num2, out num3);
					float num4 = Mathf.Clamp(value, min, (num3 <= 0f) ? num2 : num);
					float startOffset = base.GetStartOffset(axis, num4);
					if (flag)
					{
						base.SetChildAlongAxis(rectTransform, axis, startOffset, num4);
					}
					else
					{
						float num5 = (num4 - rectTransform.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxis(rectTransform, axis, startOffset + num5);
					}
				}
			}
			else
			{
				float num6 = (float)((axis != 0) ? base.padding.top : base.padding.left);
				if (base.GetTotalFlexibleSize(axis) == 0f && base.GetTotalPreferredSize(axis) < num)
				{
					num6 = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal));
				}
				float t = 0f;
				if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
				{
					t = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
				}
				float num7 = 0f;
				if (num > base.GetTotalPreferredSize(axis))
				{
					if (base.GetTotalFlexibleSize(axis) > 0f)
					{
						num7 = (num - base.GetTotalPreferredSize(axis)) / base.GetTotalFlexibleSize(axis);
					}
				}
				for (int j = 0; j < base.rectChildren.Count; j++)
				{
					RectTransform rectTransform2 = base.rectChildren[j];
					float a;
					float b;
					float num8;
					this.GetChildSizes(rectTransform2, axis, flag, childForceExpand, out a, out b, out num8);
					float num9 = Mathf.Lerp(a, b, t);
					num9 += num8 * num7;
					if (flag)
					{
						base.SetChildAlongAxis(rectTransform2, axis, num6, num9);
					}
					else
					{
						float num10 = (num9 - rectTransform2.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxis(rectTransform2, axis, num6 + num10);
					}
					num6 += num9 + this.spacing;
				}
			}
		}

		private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
		{
			if (!controlSize)
			{
				min = child.sizeDelta[axis];
				preferred = min;
				flexible = 0f;
			}
			else
			{
				min = LayoutUtility.GetMinSize(child, axis);
				preferred = LayoutUtility.GetPreferredSize(child, axis);
				flexible = LayoutUtility.GetFlexibleSize(child, axis);
			}
			if (childForceExpand)
			{
				flexible = Mathf.Max(flexible, 1f);
			}
		}

		protected override void Reset()
		{
			base.Reset();
			this.m_ChildControlWidth = false;
			this.m_ChildControlHeight = false;
		}
	}
}
