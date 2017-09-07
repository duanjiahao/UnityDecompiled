using System;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
	public abstract class MaskableGraphic : Graphic, IClippable, IMaskable, IMaterialModifier
	{
		[Serializable]
		public class CullStateChangedEvent : UnityEvent<bool>
		{
		}

		[NonSerialized]
		protected bool m_ShouldRecalculateStencil = true;

		[NonSerialized]
		protected Material m_MaskMaterial;

		[NonSerialized]
		private RectMask2D m_ParentMask;

		[NonSerialized]
		private bool m_Maskable = true;

		[Obsolete("Not used anymore.", true)]
		[NonSerialized]
		protected bool m_IncludeForMasking = false;

		[SerializeField]
		private MaskableGraphic.CullStateChangedEvent m_OnCullStateChanged = new MaskableGraphic.CullStateChangedEvent();

		[Obsolete("Not used anymore", true)]
		[NonSerialized]
		protected bool m_ShouldRecalculate = true;

		[NonSerialized]
		protected int m_StencilValue;

		private readonly Vector3[] m_Corners = new Vector3[4];

		public MaskableGraphic.CullStateChangedEvent onCullStateChanged
		{
			get
			{
				return this.m_OnCullStateChanged;
			}
			set
			{
				this.m_OnCullStateChanged = value;
			}
		}

		public bool maskable
		{
			get
			{
				return this.m_Maskable;
			}
			set
			{
				if (value != this.m_Maskable)
				{
					this.m_Maskable = value;
					this.m_ShouldRecalculateStencil = true;
					this.SetMaterialDirty();
				}
			}
		}

		private Rect rootCanvasRect
		{
			get
			{
				base.rectTransform.GetWorldCorners(this.m_Corners);
				if (base.canvas)
				{
					Canvas rootCanvas = base.canvas.rootCanvas;
					for (int i = 0; i < 4; i++)
					{
						this.m_Corners[i] = rootCanvas.transform.InverseTransformPoint(this.m_Corners[i]);
					}
				}
				return new Rect(this.m_Corners[0].x, this.m_Corners[0].y, this.m_Corners[2].x - this.m_Corners[0].x, this.m_Corners[2].y - this.m_Corners[0].y);
			}
		}

		public virtual Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (this.m_ShouldRecalculateStencil)
			{
				Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
				this.m_StencilValue = ((!this.maskable) ? 0 : MaskUtilities.GetStencilDepth(base.transform, stopAfter));
				this.m_ShouldRecalculateStencil = false;
			}
			Mask component = base.GetComponent<Mask>();
			if (this.m_StencilValue > 0 && (component == null || !component.IsActive()))
			{
				Material maskMaterial = StencilMaterial.Add(material, (1 << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << this.m_StencilValue) - 1, 0);
				StencilMaterial.Remove(this.m_MaskMaterial);
				this.m_MaskMaterial = maskMaterial;
				material = this.m_MaskMaterial;
			}
			return material;
		}

		public virtual void Cull(Rect clipRect, bool validRect)
		{
			bool cull = !validRect || !clipRect.Overlaps(this.rootCanvasRect, true);
			this.UpdateCull(cull);
		}

		private void UpdateCull(bool cull)
		{
			bool flag = base.canvasRenderer.cull != cull;
			base.canvasRenderer.cull = cull;
			if (flag)
			{
				UISystemProfilerApi.AddMarker("MaskableGraphic.cullingChanged", this);
				this.m_OnCullStateChanged.Invoke(cull);
				this.SetVerticesDirty();
			}
		}

		public virtual void SetClipRect(Rect clipRect, bool validRect)
		{
			if (validRect)
			{
				base.canvasRenderer.EnableRectClipping(clipRect);
			}
			else
			{
				base.canvasRenderer.DisableRectClipping();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_ShouldRecalculateStencil = true;
			this.UpdateClipParent();
			this.SetMaterialDirty();
			if (base.GetComponent<Mask>() != null)
			{
				MaskUtilities.NotifyStencilStateChanged(this);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_ShouldRecalculateStencil = true;
			this.SetMaterialDirty();
			this.UpdateClipParent();
			StencilMaterial.Remove(this.m_MaskMaterial);
			this.m_MaskMaterial = null;
			if (base.GetComponent<Mask>() != null)
			{
				MaskUtilities.NotifyStencilStateChanged(this);
			}
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_ShouldRecalculateStencil = true;
			this.UpdateClipParent();
			this.SetMaterialDirty();
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			if (base.isActiveAndEnabled)
			{
				this.m_ShouldRecalculateStencil = true;
				this.UpdateClipParent();
				this.SetMaterialDirty();
			}
		}

		[Obsolete("Not used anymore.", true)]
		public virtual void ParentMaskStateChanged()
		{
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			if (base.isActiveAndEnabled)
			{
				this.m_ShouldRecalculateStencil = true;
				this.UpdateClipParent();
				this.SetMaterialDirty();
			}
		}

		private void UpdateClipParent()
		{
			RectMask2D rectMask2D = (!this.maskable || !this.IsActive()) ? null : MaskUtilities.GetRectMaskForClippable(this);
			if (this.m_ParentMask != null && (rectMask2D != this.m_ParentMask || !rectMask2D.IsActive()))
			{
				this.m_ParentMask.RemoveClippable(this);
				this.UpdateCull(false);
			}
			if (rectMask2D != null && rectMask2D.IsActive())
			{
				rectMask2D.AddClippable(this);
			}
			this.m_ParentMask = rectMask2D;
		}

		public virtual void RecalculateClipping()
		{
			this.UpdateClipParent();
		}

		public virtual void RecalculateMasking()
		{
			this.m_ShouldRecalculateStencil = true;
			this.SetMaterialDirty();
		}

		GameObject IClippable.get_gameObject()
		{
			return base.gameObject;
		}
	}
}
