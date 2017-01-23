using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public class MaskUtilities
	{
		public static void Notify2DMaskStateChanged(Component mask)
		{
			List<Component> list = ListPool<Component>.Get();
			mask.GetComponentsInChildren<Component>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null) && !(list[i].gameObject == mask.gameObject))
				{
					IClippable clippable = list[i] as IClippable;
					if (clippable != null)
					{
						clippable.RecalculateClipping();
					}
				}
			}
			ListPool<Component>.Release(list);
		}

		public static void NotifyStencilStateChanged(Component mask)
		{
			List<Component> list = ListPool<Component>.Get();
			mask.GetComponentsInChildren<Component>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i] == null) && !(list[i].gameObject == mask.gameObject))
				{
					IMaskable maskable = list[i] as IMaskable;
					if (maskable != null)
					{
						maskable.RecalculateMasking();
					}
				}
			}
			ListPool<Component>.Release(list);
		}

		public static Transform FindRootSortOverrideCanvas(Transform start)
		{
			List<Canvas> list = ListPool<Canvas>.Get();
			start.GetComponentsInParent<Canvas>(false, list);
			Canvas canvas = null;
			for (int i = 0; i < list.Count; i++)
			{
				canvas = list[i];
				if (canvas.overrideSorting)
				{
					break;
				}
			}
			ListPool<Canvas>.Release(list);
			return (!(canvas != null)) ? null : canvas.transform;
		}

		public static int GetStencilDepth(Transform transform, Transform stopAfter)
		{
			int num = 0;
			int result;
			if (transform == stopAfter)
			{
				result = num;
			}
			else
			{
				Transform parent = transform.parent;
				List<Mask> list = ListPool<Mask>.Get();
				while (parent != null)
				{
					parent.GetComponents<Mask>(list);
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != null && list[i].MaskEnabled() && list[i].graphic.IsActive())
						{
							num++;
							break;
						}
					}
					if (parent == stopAfter)
					{
						break;
					}
					parent = parent.parent;
				}
				ListPool<Mask>.Release(list);
				result = num;
			}
			return result;
		}

		public static bool IsDescendantOrSelf(Transform father, Transform child)
		{
			bool result;
			if (father == null || child == null)
			{
				result = false;
			}
			else if (father == child)
			{
				result = true;
			}
			else
			{
				while (child.parent != null)
				{
					if (child.parent == father)
					{
						result = true;
						return result;
					}
					child = child.parent;
				}
				result = false;
			}
			return result;
		}

		public static RectMask2D GetRectMaskForClippable(IClippable clippable)
		{
			List<RectMask2D> list = ListPool<RectMask2D>.Get();
			List<Canvas> list2 = ListPool<Canvas>.Get();
			RectMask2D rectMask2D = null;
			clippable.rectTransform.GetComponentsInParent<RectMask2D>(false, list);
			RectMask2D result;
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					rectMask2D = list[i];
					if (rectMask2D.gameObject == clippable.gameObject)
					{
						rectMask2D = null;
					}
					else
					{
						if (rectMask2D.isActiveAndEnabled)
						{
							clippable.rectTransform.GetComponentsInParent<Canvas>(false, list2);
							for (int j = list2.Count - 1; j >= 0; j--)
							{
								if (!MaskUtilities.IsDescendantOrSelf(list2[j].transform, rectMask2D.transform) && list2[j].overrideSorting)
								{
									rectMask2D = null;
									break;
								}
							}
							result = rectMask2D;
							return result;
						}
						rectMask2D = null;
					}
				}
			}
			ListPool<RectMask2D>.Release(list);
			ListPool<Canvas>.Release(list2);
			result = rectMask2D;
			return result;
		}

		public static void GetRectMasksForClip(RectMask2D clipper, List<RectMask2D> masks)
		{
			masks.Clear();
			List<Canvas> list = ListPool<Canvas>.Get();
			List<RectMask2D> list2 = ListPool<RectMask2D>.Get();
			clipper.transform.GetComponentsInParent<RectMask2D>(false, list2);
			if (list2.Count > 0)
			{
				clipper.transform.GetComponentsInParent<Canvas>(false, list);
				for (int i = list2.Count - 1; i >= 0; i--)
				{
					if (list2[i].IsActive())
					{
						bool flag = true;
						for (int j = list.Count - 1; j >= 0; j--)
						{
							if (!MaskUtilities.IsDescendantOrSelf(list[j].transform, list2[i].transform) && list[j].overrideSorting)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							masks.Add(list2[i]);
						}
					}
				}
			}
			ListPool<RectMask2D>.Release(list2);
			ListPool<Canvas>.Release(list);
		}
	}
}
