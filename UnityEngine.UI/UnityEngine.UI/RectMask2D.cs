using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Rect Mask 2D", 13), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
	public class RectMask2D : UIBehaviour, IClipper, ICanvasRaycastFilter
	{
		[NonSerialized]
		private readonly RectangularVertexClipper m_VertexClipper = new RectangularVertexClipper();

		[NonSerialized]
		private RectTransform m_RectTransform;

		[NonSerialized]
		private HashSet<IClippable> m_ClipTargets = new HashSet<IClippable>();

		[NonSerialized]
		private bool m_ShouldRecalculateClipRects;

		[NonSerialized]
		private List<RectMask2D> m_Clippers = new List<RectMask2D>();

		[NonSerialized]
		private Rect m_LastClipRectCanvasSpace;

		[NonSerialized]
		private bool m_LastValidClipRect;

		[NonSerialized]
		private bool m_ForceClip;

		public Rect canvasRect
		{
			get
			{
				Canvas c = null;
				List<Canvas> list = ListPool<Canvas>.Get();
				base.gameObject.GetComponentsInParent<Canvas>(false, list);
				if (list.Count > 0)
				{
					c = list[list.Count - 1];
				}
				ListPool<Canvas>.Release(list);
				return this.m_VertexClipper.GetCanvasRect(this.rectTransform, c);
			}
		}

		public RectTransform rectTransform
		{
			get
			{
				RectTransform arg_1D_0;
				if ((arg_1D_0 = this.m_RectTransform) == null)
				{
					arg_1D_0 = (this.m_RectTransform = base.GetComponent<RectTransform>());
				}
				return arg_1D_0;
			}
		}

		protected RectMask2D()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_ShouldRecalculateClipRects = true;
			ClipperRegistry.Register(this);
			MaskUtilities.Notify2DMaskStateChanged(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_ClipTargets.Clear();
			this.m_Clippers.Clear();
			ClipperRegistry.Unregister(this);
			MaskUtilities.Notify2DMaskStateChanged(this);
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_ShouldRecalculateClipRects = true;
			if (this.IsActive())
			{
				MaskUtilities.Notify2DMaskStateChanged(this);
			}
		}

		public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return !base.isActiveAndEnabled || RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera);
		}

		public virtual void PerformClipping()
		{
			if (this.m_ShouldRecalculateClipRects)
			{
				MaskUtilities.GetRectMasksForClip(this, this.m_Clippers);
				this.m_ShouldRecalculateClipRects = false;
			}
			bool flag = true;
			Rect rect = Clipping.FindCullAndClipWorldRect(this.m_Clippers, out flag);
			bool flag2 = rect != this.m_LastClipRectCanvasSpace;
			if (flag2 || this.m_ForceClip)
			{
				foreach (IClippable current in this.m_ClipTargets)
				{
					current.SetClipRect(rect, flag);
				}
				this.m_LastClipRectCanvasSpace = rect;
				this.m_LastValidClipRect = flag;
			}
			foreach (IClippable current2 in this.m_ClipTargets)
			{
				MaskableGraphic maskableGraphic = current2 as MaskableGraphic;
				if (!(maskableGraphic != null) || maskableGraphic.canvasRenderer.hasMoved || flag2)
				{
					current2.Cull(this.m_LastClipRectCanvasSpace, this.m_LastValidClipRect);
				}
			}
		}

		public void AddClippable(IClippable clippable)
		{
			if (clippable != null)
			{
				this.m_ShouldRecalculateClipRects = true;
				if (!this.m_ClipTargets.Contains(clippable))
				{
					this.m_ClipTargets.Add(clippable);
				}
				this.m_ForceClip = true;
			}
		}

		public void RemoveClippable(IClippable clippable)
		{
			if (clippable != null)
			{
				this.m_ShouldRecalculateClipRects = true;
				clippable.SetClipRect(default(Rect), false);
				this.m_ClipTargets.Remove(clippable);
				this.m_ForceClip = true;
			}
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.m_ShouldRecalculateClipRects = true;
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this.m_ShouldRecalculateClipRects = true;
		}
	}
}
