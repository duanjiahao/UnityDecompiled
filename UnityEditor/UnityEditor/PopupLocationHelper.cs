using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal static class PopupLocationHelper
	{
		public enum PopupLocation
		{
			Below,
			Above,
			Left,
			Right
		}

		private static float k_SpaceFromBottom
		{
			get
			{
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					return 10f;
				}
				return 0f;
			}
		}

		public static Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow)
		{
			return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, popupContainerWindow, null);
		}

		public static Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			if (locationPriorityOrder == null)
			{
				locationPriorityOrder = new PopupLocationHelper.PopupLocation[]
				{
					PopupLocationHelper.PopupLocation.Below,
					PopupLocationHelper.PopupLocation.Above
				};
			}
			List<Rect> list = new List<Rect>();
			PopupLocationHelper.PopupLocation[] array = locationPriorityOrder;
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case PopupLocationHelper.PopupLocation.Below:
				{
					Rect rect;
					if (PopupLocationHelper.PopupBelow(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
					{
						return rect;
					}
					list.Add(rect);
					break;
				}
				case PopupLocationHelper.PopupLocation.Above:
				{
					Rect rect;
					if (PopupLocationHelper.PopupAbove(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
					{
						return rect;
					}
					list.Add(rect);
					break;
				}
				case PopupLocationHelper.PopupLocation.Left:
				{
					Rect rect;
					if (PopupLocationHelper.PopupLeft(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
					{
						return rect;
					}
					list.Add(rect);
					break;
				}
				case PopupLocationHelper.PopupLocation.Right:
				{
					Rect rect;
					if (PopupLocationHelper.PopupRight(buttonRect, minSize, maxSize, popupContainerWindow, out rect))
					{
						return rect;
					}
					list.Add(rect);
					break;
				}
				}
			}
			return PopupLocationHelper.GetLargestRect(list);
		}

		private static Rect FitRect(Rect rect, ContainerWindow popupContainerWindow)
		{
			if (popupContainerWindow)
			{
				return popupContainerWindow.FitWindowRectToScreen(rect, true, true);
			}
			return ContainerWindow.FitRectToScreen(rect, true, true);
		}

		private static bool PopupRight(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
		{
			Rect rect = new Rect(buttonRect.xMax, buttonRect.y, maxSize.x, maxSize.y);
			float num = 0f;
			rect.xMax += num;
			rect.height += PopupLocationHelper.k_SpaceFromBottom;
			rect = PopupLocationHelper.FitRect(rect, popupContainerWindow);
			float num2 = Mathf.Max(rect.xMax - buttonRect.xMax - num, 0f);
			float width = Mathf.Min(num2, maxSize.x);
			resultRect = new Rect(rect.x, rect.y, width, rect.height - PopupLocationHelper.k_SpaceFromBottom);
			return num2 >= minSize.x;
		}

		private static bool PopupLeft(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
		{
			Rect rect = new Rect(buttonRect.x - maxSize.x, buttonRect.y, maxSize.x, maxSize.y);
			float num = 0f;
			rect.xMin -= num;
			rect.height += PopupLocationHelper.k_SpaceFromBottom;
			rect = PopupLocationHelper.FitRect(rect, popupContainerWindow);
			float num2 = Mathf.Max(buttonRect.x - rect.x - num, 0f);
			float width = Mathf.Min(num2, maxSize.x);
			resultRect = new Rect(rect.x, rect.y, width, rect.height - PopupLocationHelper.k_SpaceFromBottom);
			return num2 >= minSize.x;
		}

		private static bool PopupAbove(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
		{
			Rect rect = new Rect(buttonRect.x, buttonRect.y - maxSize.y, maxSize.x, maxSize.y);
			float num = 0f;
			rect.yMin -= num;
			rect = PopupLocationHelper.FitRect(rect, popupContainerWindow);
			float num2 = Mathf.Max(buttonRect.y - rect.y - num, 0f);
			if (num2 >= minSize.y)
			{
				float num3 = Mathf.Min(num2, maxSize.y);
				resultRect = new Rect(rect.x, buttonRect.y - num3, rect.width, num3);
				return true;
			}
			resultRect = new Rect(rect.x, buttonRect.y - num2, rect.width, num2);
			return false;
		}

		private static bool PopupBelow(Rect buttonRect, Vector2 minSize, Vector2 maxSize, ContainerWindow popupContainerWindow, out Rect resultRect)
		{
			Rect rect = new Rect(buttonRect.x, buttonRect.yMax, maxSize.x, maxSize.y);
			rect.height += PopupLocationHelper.k_SpaceFromBottom;
			rect = PopupLocationHelper.FitRect(rect, popupContainerWindow);
			float num = Mathf.Max(rect.yMax - buttonRect.yMax - PopupLocationHelper.k_SpaceFromBottom, 0f);
			if (num >= minSize.y)
			{
				float height = Mathf.Min(num, maxSize.y);
				resultRect = new Rect(rect.x, buttonRect.yMax, rect.width, height);
				return true;
			}
			resultRect = new Rect(rect.x, buttonRect.yMax, rect.width, num);
			return false;
		}

		private static Rect GetLargestRect(List<Rect> rects)
		{
			Rect result = default(Rect);
			foreach (Rect current in rects)
			{
				if (current.height * current.width > result.height * result.width)
				{
					result = current;
				}
			}
			return result;
		}
	}
}
