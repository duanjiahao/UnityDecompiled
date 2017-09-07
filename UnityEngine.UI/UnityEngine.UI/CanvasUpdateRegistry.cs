using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
	public class CanvasUpdateRegistry
	{
		private static CanvasUpdateRegistry s_Instance;

		private bool m_PerformingLayoutUpdate;

		private bool m_PerformingGraphicUpdate;

		private readonly IndexedSet<ICanvasElement> m_LayoutRebuildQueue = new IndexedSet<ICanvasElement>();

		private readonly IndexedSet<ICanvasElement> m_GraphicRebuildQueue = new IndexedSet<ICanvasElement>();

		private static readonly Comparison<ICanvasElement> s_SortLayoutFunction;

		[CompilerGenerated]
		private static Comparison<ICanvasElement> <>f__mg$cache0;

		public static CanvasUpdateRegistry instance
		{
			get
			{
				if (CanvasUpdateRegistry.s_Instance == null)
				{
					CanvasUpdateRegistry.s_Instance = new CanvasUpdateRegistry();
				}
				return CanvasUpdateRegistry.s_Instance;
			}
		}

		protected CanvasUpdateRegistry()
		{
			Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.PerformUpdate);
		}

		private bool ObjectValidForUpdate(ICanvasElement element)
		{
			bool result = element != null;
			bool flag = element is UnityEngine.Object;
			if (flag)
			{
				result = (element as UnityEngine.Object != null);
			}
			return result;
		}

		private void CleanInvalidItems()
		{
			for (int i = this.m_LayoutRebuildQueue.Count - 1; i >= 0; i--)
			{
				ICanvasElement canvasElement = this.m_LayoutRebuildQueue[i];
				if (canvasElement == null)
				{
					this.m_LayoutRebuildQueue.RemoveAt(i);
				}
				else if (canvasElement.IsDestroyed())
				{
					this.m_LayoutRebuildQueue.RemoveAt(i);
					canvasElement.LayoutComplete();
				}
			}
			for (int j = this.m_GraphicRebuildQueue.Count - 1; j >= 0; j--)
			{
				ICanvasElement canvasElement2 = this.m_GraphicRebuildQueue[j];
				if (canvasElement2 == null)
				{
					this.m_GraphicRebuildQueue.RemoveAt(j);
				}
				else if (canvasElement2.IsDestroyed())
				{
					this.m_GraphicRebuildQueue.RemoveAt(j);
					canvasElement2.GraphicUpdateComplete();
				}
			}
		}

		private void PerformUpdate()
		{
			UISystemProfilerApi.BeginSample(UISystemProfilerApi.SampleType.Layout);
			this.CleanInvalidItems();
			this.m_PerformingLayoutUpdate = true;
			this.m_LayoutRebuildQueue.Sort(CanvasUpdateRegistry.s_SortLayoutFunction);
			for (int i = 0; i <= 2; i++)
			{
				for (int j = 0; j < this.m_LayoutRebuildQueue.Count; j++)
				{
					ICanvasElement canvasElement = CanvasUpdateRegistry.instance.m_LayoutRebuildQueue[j];
					try
					{
						if (this.ObjectValidForUpdate(canvasElement))
						{
							canvasElement.Rebuild((CanvasUpdate)i);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception, canvasElement.transform);
					}
				}
			}
			for (int k = 0; k < this.m_LayoutRebuildQueue.Count; k++)
			{
				this.m_LayoutRebuildQueue[k].LayoutComplete();
			}
			CanvasUpdateRegistry.instance.m_LayoutRebuildQueue.Clear();
			this.m_PerformingLayoutUpdate = false;
			ClipperRegistry.instance.Cull();
			this.m_PerformingGraphicUpdate = true;
			for (int l = 3; l < 5; l++)
			{
				for (int m = 0; m < CanvasUpdateRegistry.instance.m_GraphicRebuildQueue.Count; m++)
				{
					try
					{
						ICanvasElement canvasElement2 = CanvasUpdateRegistry.instance.m_GraphicRebuildQueue[m];
						if (this.ObjectValidForUpdate(canvasElement2))
						{
							canvasElement2.Rebuild((CanvasUpdate)l);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2, CanvasUpdateRegistry.instance.m_GraphicRebuildQueue[m].transform);
					}
				}
			}
			for (int n = 0; n < this.m_GraphicRebuildQueue.Count; n++)
			{
				this.m_GraphicRebuildQueue[n].GraphicUpdateComplete();
			}
			CanvasUpdateRegistry.instance.m_GraphicRebuildQueue.Clear();
			this.m_PerformingGraphicUpdate = false;
			UISystemProfilerApi.EndSample(UISystemProfilerApi.SampleType.Layout);
		}

		private static int ParentCount(Transform child)
		{
			int result;
			if (child == null)
			{
				result = 0;
			}
			else
			{
				Transform parent = child.parent;
				int num = 0;
				while (parent != null)
				{
					num++;
					parent = parent.parent;
				}
				result = num;
			}
			return result;
		}

		private static int SortLayoutList(ICanvasElement x, ICanvasElement y)
		{
			Transform transform = x.transform;
			Transform transform2 = y.transform;
			return CanvasUpdateRegistry.ParentCount(transform) - CanvasUpdateRegistry.ParentCount(transform2);
		}

		public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			CanvasUpdateRegistry.instance.InternalRegisterCanvasElementForLayoutRebuild(element);
		}

		public static bool TryRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			return CanvasUpdateRegistry.instance.InternalRegisterCanvasElementForLayoutRebuild(element);
		}

		private bool InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			return !this.m_LayoutRebuildQueue.Contains(element) && this.m_LayoutRebuildQueue.AddUnique(element);
		}

		public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			CanvasUpdateRegistry.instance.InternalRegisterCanvasElementForGraphicRebuild(element);
		}

		public static bool TryRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			return CanvasUpdateRegistry.instance.InternalRegisterCanvasElementForGraphicRebuild(element);
		}

		private bool InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			bool result;
			if (this.m_PerformingGraphicUpdate)
			{
				Debug.LogError(string.Format("Trying to add {0} for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported.", element));
				result = false;
			}
			else
			{
				result = this.m_GraphicRebuildQueue.AddUnique(element);
			}
			return result;
		}

		public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
		{
			CanvasUpdateRegistry.instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
			CanvasUpdateRegistry.instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
		}

		private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			if (this.m_PerformingLayoutUpdate)
			{
				Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
			}
			else
			{
				element.LayoutComplete();
				CanvasUpdateRegistry.instance.m_LayoutRebuildQueue.Remove(element);
			}
		}

		private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			if (this.m_PerformingGraphicUpdate)
			{
				Debug.LogError(string.Format("Trying to remove {0} from rebuild list while we are already inside a rebuild loop. This is not supported.", element));
			}
			else
			{
				element.GraphicUpdateComplete();
				CanvasUpdateRegistry.instance.m_GraphicRebuildQueue.Remove(element);
			}
		}

		public static bool IsRebuildingLayout()
		{
			return CanvasUpdateRegistry.instance.m_PerformingLayoutUpdate;
		}

		public static bool IsRebuildingGraphics()
		{
			return CanvasUpdateRegistry.instance.m_PerformingGraphicUpdate;
		}

		static CanvasUpdateRegistry()
		{
			// Note: this type is marked as 'beforefieldinit'.
			if (CanvasUpdateRegistry.<>f__mg$cache0 == null)
			{
				CanvasUpdateRegistry.<>f__mg$cache0 = new Comparison<ICanvasElement>(CanvasUpdateRegistry.SortLayoutList);
			}
			CanvasUpdateRegistry.s_SortLayoutFunction = CanvasUpdateRegistry.<>f__mg$cache0;
		}
	}
}
