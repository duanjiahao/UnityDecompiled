using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public static class LayoutUtility
	{
		public static float GetMinSize(RectTransform rect, int axis)
		{
			float result;
			if (axis == 0)
			{
				result = LayoutUtility.GetMinWidth(rect);
			}
			else
			{
				result = LayoutUtility.GetMinHeight(rect);
			}
			return result;
		}

		public static float GetPreferredSize(RectTransform rect, int axis)
		{
			float result;
			if (axis == 0)
			{
				result = LayoutUtility.GetPreferredWidth(rect);
			}
			else
			{
				result = LayoutUtility.GetPreferredHeight(rect);
			}
			return result;
		}

		public static float GetFlexibleSize(RectTransform rect, int axis)
		{
			float result;
			if (axis == 0)
			{
				result = LayoutUtility.GetFlexibleWidth(rect);
			}
			else
			{
				result = LayoutUtility.GetFlexibleHeight(rect);
			}
			return result;
		}

		public static float GetMinWidth(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f);
		}

		public static float GetPreferredWidth(RectTransform rect)
		{
			return Mathf.Max(LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f), LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.preferredWidth, 0f));
		}

		public static float GetFlexibleWidth(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.flexibleWidth, 0f);
		}

		public static float GetMinHeight(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f);
		}

		public static float GetPreferredHeight(RectTransform rect)
		{
			return Mathf.Max(LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f), LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.preferredHeight, 0f));
		}

		public static float GetFlexibleHeight(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.flexibleHeight, 0f);
		}

		public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue)
		{
			ILayoutElement layoutElement;
			return LayoutUtility.GetLayoutProperty(rect, property, defaultValue, out layoutElement);
		}

		public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source)
		{
			source = null;
			float result;
			if (rect == null)
			{
				result = 0f;
			}
			else
			{
				float num = defaultValue;
				int num2 = -2147483648;
				List<Component> list = ListPool<Component>.Get();
				rect.GetComponents(typeof(ILayoutElement), list);
				for (int i = 0; i < list.Count; i++)
				{
					ILayoutElement layoutElement = list[i] as ILayoutElement;
					if (!(layoutElement is Behaviour) || ((Behaviour)layoutElement).isActiveAndEnabled)
					{
						int layoutPriority = layoutElement.layoutPriority;
						if (layoutPriority >= num2)
						{
							float num3 = property(layoutElement);
							if (num3 >= 0f)
							{
								if (layoutPriority > num2)
								{
									num = num3;
									num2 = layoutPriority;
									source = layoutElement;
								}
								else if (num3 > num)
								{
									num = num3;
									source = layoutElement;
								}
							}
						}
					}
				}
				ListPool<Component>.Release(list);
				result = num;
			}
			return result;
		}
	}
}
