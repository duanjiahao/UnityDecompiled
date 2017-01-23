using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

namespace UnityEngine.UI
{
	public class LayoutRebuilder : ICanvasElement
	{
		private RectTransform m_ToRebuild;

		private int m_CachedHashFromTransform;

		private static ObjectPool<LayoutRebuilder> s_Rebuilders;

		[CompilerGenerated]
		private static RectTransform.ReapplyDrivenProperties <>f__mg$cache0;

		public Transform transform
		{
			get
			{
				return this.m_ToRebuild;
			}
		}

		static LayoutRebuilder()
		{
			LayoutRebuilder.s_Rebuilders = new ObjectPool<LayoutRebuilder>(null, delegate(LayoutRebuilder x)
			{
				x.Clear();
			});
			if (LayoutRebuilder.<>f__mg$cache0 == null)
			{
				LayoutRebuilder.<>f__mg$cache0 = new RectTransform.ReapplyDrivenProperties(LayoutRebuilder.ReapplyDrivenProperties);
			}
			RectTransform.reapplyDrivenProperties += LayoutRebuilder.<>f__mg$cache0;
		}

		private void Initialize(RectTransform controller)
		{
			this.m_ToRebuild = controller;
			this.m_CachedHashFromTransform = controller.GetHashCode();
		}

		private void Clear()
		{
			this.m_ToRebuild = null;
			this.m_CachedHashFromTransform = 0;
		}

		private static void ReapplyDrivenProperties(RectTransform driven)
		{
			LayoutRebuilder.MarkLayoutForRebuild(driven);
		}

		public bool IsDestroyed()
		{
			return this.m_ToRebuild == null;
		}

		private static void StripDisabledBehavioursFromList(List<Component> components)
		{
			components.RemoveAll((Component e) => e is Behaviour && !((Behaviour)e).isActiveAndEnabled);
		}

		public static void ForceRebuildLayoutImmediate(RectTransform layoutRoot)
		{
			LayoutRebuilder layoutRebuilder = LayoutRebuilder.s_Rebuilders.Get();
			layoutRebuilder.Initialize(layoutRoot);
			layoutRebuilder.Rebuild(CanvasUpdate.Layout);
			LayoutRebuilder.s_Rebuilders.Release(layoutRebuilder);
		}

		public void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Layout)
			{
				this.PerformLayoutCalculation(this.m_ToRebuild, delegate(Component e)
				{
					(e as ILayoutElement).CalculateLayoutInputHorizontal();
				});
				this.PerformLayoutControl(this.m_ToRebuild, delegate(Component e)
				{
					(e as ILayoutController).SetLayoutHorizontal();
				});
				this.PerformLayoutCalculation(this.m_ToRebuild, delegate(Component e)
				{
					(e as ILayoutElement).CalculateLayoutInputVertical();
				});
				this.PerformLayoutControl(this.m_ToRebuild, delegate(Component e)
				{
					(e as ILayoutController).SetLayoutVertical();
				});
			}
		}

		private void PerformLayoutControl(RectTransform rect, UnityAction<Component> action)
		{
			if (!(rect == null))
			{
				List<Component> list = ListPool<Component>.Get();
				rect.GetComponents(typeof(ILayoutController), list);
				LayoutRebuilder.StripDisabledBehavioursFromList(list);
				if (list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is ILayoutSelfController)
						{
							action(list[i]);
						}
					}
					for (int j = 0; j < list.Count; j++)
					{
						if (!(list[j] is ILayoutSelfController))
						{
							action(list[j]);
						}
					}
					for (int k = 0; k < rect.childCount; k++)
					{
						this.PerformLayoutControl(rect.GetChild(k) as RectTransform, action);
					}
				}
				ListPool<Component>.Release(list);
			}
		}

		private void PerformLayoutCalculation(RectTransform rect, UnityAction<Component> action)
		{
			if (!(rect == null))
			{
				List<Component> list = ListPool<Component>.Get();
				rect.GetComponents(typeof(ILayoutElement), list);
				LayoutRebuilder.StripDisabledBehavioursFromList(list);
				if (list.Count > 0 || rect.GetComponent(typeof(ILayoutGroup)))
				{
					for (int i = 0; i < rect.childCount; i++)
					{
						this.PerformLayoutCalculation(rect.GetChild(i) as RectTransform, action);
					}
					for (int j = 0; j < list.Count; j++)
					{
						action(list[j]);
					}
				}
				ListPool<Component>.Release(list);
			}
		}

		public static void MarkLayoutForRebuild(RectTransform rect)
		{
			if (!(rect == null))
			{
				List<Component> list = ListPool<Component>.Get();
				RectTransform rectTransform = rect;
				while (true)
				{
					RectTransform rectTransform2 = rectTransform.parent as RectTransform;
					if (!LayoutRebuilder.ValidLayoutGroup(rectTransform2, list))
					{
						break;
					}
					rectTransform = rectTransform2;
				}
				if (rectTransform == rect && !LayoutRebuilder.ValidController(rectTransform, list))
				{
					ListPool<Component>.Release(list);
				}
				else
				{
					LayoutRebuilder.MarkLayoutRootForRebuild(rectTransform);
					ListPool<Component>.Release(list);
				}
			}
		}

		private static bool ValidLayoutGroup(RectTransform parent, List<Component> comps)
		{
			bool result;
			if (parent == null)
			{
				result = false;
			}
			else
			{
				parent.GetComponents(typeof(ILayoutGroup), comps);
				LayoutRebuilder.StripDisabledBehavioursFromList(comps);
				bool flag = comps.Count > 0;
				result = flag;
			}
			return result;
		}

		private static bool ValidController(RectTransform layoutRoot, List<Component> comps)
		{
			bool result;
			if (layoutRoot == null)
			{
				result = false;
			}
			else
			{
				layoutRoot.GetComponents(typeof(ILayoutController), comps);
				LayoutRebuilder.StripDisabledBehavioursFromList(comps);
				bool flag = comps.Count > 0;
				result = flag;
			}
			return result;
		}

		private static void MarkLayoutRootForRebuild(RectTransform controller)
		{
			if (!(controller == null))
			{
				LayoutRebuilder layoutRebuilder = LayoutRebuilder.s_Rebuilders.Get();
				layoutRebuilder.Initialize(controller);
				if (!CanvasUpdateRegistry.TryRegisterCanvasElementForLayoutRebuild(layoutRebuilder))
				{
					LayoutRebuilder.s_Rebuilders.Release(layoutRebuilder);
				}
			}
		}

		public void LayoutComplete()
		{
			LayoutRebuilder.s_Rebuilders.Release(this);
		}

		public void GraphicUpdateComplete()
		{
		}

		public override int GetHashCode()
		{
			return this.m_CachedHashFromTransform;
		}

		public override bool Equals(object obj)
		{
			return obj.GetHashCode() == this.GetHashCode();
		}

		public override string ToString()
		{
			return "(Layout Rebuilder for) " + this.m_ToRebuild;
		}
	}
}
