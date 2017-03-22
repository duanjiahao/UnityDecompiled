using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Aspect Ratio Fitter", 142), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
	public class AspectRatioFitter : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		public enum AspectMode
		{
			None,
			WidthControlsHeight,
			HeightControlsWidth,
			FitInParent,
			EnvelopeParent
		}

		[SerializeField]
		private AspectRatioFitter.AspectMode m_AspectMode = AspectRatioFitter.AspectMode.None;

		[SerializeField]
		private float m_AspectRatio = 1f;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		public AspectRatioFitter.AspectMode aspectMode
		{
			get
			{
				return this.m_AspectMode;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<AspectRatioFitter.AspectMode>(ref this.m_AspectMode, value))
				{
					this.SetDirty();
				}
			}
		}

		public float aspectRatio
		{
			get
			{
				return this.m_AspectRatio;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_AspectRatio, value))
				{
					this.SetDirty();
				}
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		protected AspectRatioFitter()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetDirty();
		}

		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			this.UpdateRect();
		}

		private void UpdateRect()
		{
			if (this.IsActive())
			{
				this.m_Tracker.Clear();
				switch (this.m_AspectMode)
				{
				case AspectRatioFitter.AspectMode.None:
					if (!Application.isPlaying)
					{
						this.m_AspectRatio = Mathf.Clamp(this.rectTransform.rect.width / this.rectTransform.rect.height, 0.001f, 1000f);
					}
					break;
				case AspectRatioFitter.AspectMode.WidthControlsHeight:
					this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDeltaY);
					this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.rectTransform.rect.width / this.m_AspectRatio);
					break;
				case AspectRatioFitter.AspectMode.HeightControlsWidth:
					this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.SizeDeltaX);
					this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.rectTransform.rect.height * this.m_AspectRatio);
					break;
				case AspectRatioFitter.AspectMode.FitInParent:
				case AspectRatioFitter.AspectMode.EnvelopeParent:
				{
					this.m_Tracker.Add(this, this.rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
					this.rectTransform.anchorMin = Vector2.zero;
					this.rectTransform.anchorMax = Vector2.one;
					this.rectTransform.anchoredPosition = Vector2.zero;
					Vector2 zero = Vector2.zero;
					Vector2 parentSize = this.GetParentSize();
					if (parentSize.y * this.aspectRatio < parentSize.x ^ this.m_AspectMode == AspectRatioFitter.AspectMode.FitInParent)
					{
						zero.y = this.GetSizeDeltaToProduceSize(parentSize.x / this.aspectRatio, 1);
					}
					else
					{
						zero.x = this.GetSizeDeltaToProduceSize(parentSize.y * this.aspectRatio, 0);
					}
					this.rectTransform.sizeDelta = zero;
					break;
				}
				}
			}
		}

		private float GetSizeDeltaToProduceSize(float size, int axis)
		{
			return size - this.GetParentSize()[axis] * (this.rectTransform.anchorMax[axis] - this.rectTransform.anchorMin[axis]);
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = this.rectTransform.parent as RectTransform;
			Vector2 result;
			if (!rectTransform)
			{
				result = Vector2.zero;
			}
			else
			{
				result = rectTransform.rect.size;
			}
			return result;
		}

		public virtual void SetLayoutHorizontal()
		{
		}

		public virtual void SetLayoutVertical()
		{
		}

		protected void SetDirty()
		{
			this.UpdateRect();
		}

		protected override void OnValidate()
		{
			this.m_AspectRatio = Mathf.Clamp(this.m_AspectRatio, 0.001f, 1000f);
			this.SetDirty();
		}
	}
}
