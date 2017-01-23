using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Slider", 33), RequireComponent(typeof(RectTransform))]
	public class Slider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement, IEventSystemHandler
	{
		public enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}

		[Serializable]
		public class SliderEvent : UnityEvent<float>
		{
		}

		private enum Axis
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private RectTransform m_FillRect;

		[SerializeField]
		private RectTransform m_HandleRect;

		[SerializeField, Space]
		private Slider.Direction m_Direction = Slider.Direction.LeftToRight;

		[SerializeField]
		private float m_MinValue = 0f;

		[SerializeField]
		private float m_MaxValue = 1f;

		[SerializeField]
		private bool m_WholeNumbers = false;

		[SerializeField]
		protected float m_Value;

		[SerializeField, Space]
		private Slider.SliderEvent m_OnValueChanged = new Slider.SliderEvent();

		private Image m_FillImage;

		private Transform m_FillTransform;

		private RectTransform m_FillContainerRect;

		private Transform m_HandleTransform;

		private RectTransform m_HandleContainerRect;

		private Vector2 m_Offset = Vector2.zero;

		private DrivenRectTransformTracker m_Tracker;

		public RectTransform fillRect
		{
			get
			{
				return this.m_FillRect;
			}
			set
			{
				if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_FillRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		public RectTransform handleRect
		{
			get
			{
				return this.m_HandleRect;
			}
			set
			{
				if (SetPropertyUtility.SetClass<RectTransform>(ref this.m_HandleRect, value))
				{
					this.UpdateCachedReferences();
					this.UpdateVisuals();
				}
			}
		}

		public Slider.Direction direction
		{
			get
			{
				return this.m_Direction;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Slider.Direction>(ref this.m_Direction, value))
				{
					this.UpdateVisuals();
				}
			}
		}

		public float minValue
		{
			get
			{
				return this.m_MinValue;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_MinValue, value))
				{
					this.Set(this.m_Value);
					this.UpdateVisuals();
				}
			}
		}

		public float maxValue
		{
			get
			{
				return this.m_MaxValue;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_MaxValue, value))
				{
					this.Set(this.m_Value);
					this.UpdateVisuals();
				}
			}
		}

		public bool wholeNumbers
		{
			get
			{
				return this.m_WholeNumbers;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_WholeNumbers, value))
				{
					this.Set(this.m_Value);
					this.UpdateVisuals();
				}
			}
		}

		public virtual float value
		{
			get
			{
				float result;
				if (this.wholeNumbers)
				{
					result = Mathf.Round(this.m_Value);
				}
				else
				{
					result = this.m_Value;
				}
				return result;
			}
			set
			{
				this.Set(value);
			}
		}

		public float normalizedValue
		{
			get
			{
				float result;
				if (Mathf.Approximately(this.minValue, this.maxValue))
				{
					result = 0f;
				}
				else
				{
					result = Mathf.InverseLerp(this.minValue, this.maxValue, this.value);
				}
				return result;
			}
			set
			{
				this.value = Mathf.Lerp(this.minValue, this.maxValue, value);
			}
		}

		public Slider.SliderEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		private float stepSize
		{
			get
			{
				return (!this.wholeNumbers) ? ((this.maxValue - this.minValue) * 0.1f) : 1f;
			}
		}

		private Slider.Axis axis
		{
			get
			{
				return (this.m_Direction != Slider.Direction.LeftToRight && this.m_Direction != Slider.Direction.RightToLeft) ? Slider.Axis.Vertical : Slider.Axis.Horizontal;
			}
		}

		private bool reverseValue
		{
			get
			{
				return this.m_Direction == Slider.Direction.RightToLeft || this.m_Direction == Slider.Direction.TopToBottom;
			}
		}

		protected Slider()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.UpdateCachedReferences();
			this.Set(this.m_Value, false);
			this.UpdateVisuals();
		}

		protected override void OnDisable()
		{
			this.m_Tracker.Clear();
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			this.m_Value = this.ClampValue(this.m_Value);
			float num = this.normalizedValue;
			if (this.m_FillContainerRect != null)
			{
				if (this.m_FillImage != null && this.m_FillImage.type == Image.Type.Filled)
				{
					num = this.m_FillImage.fillAmount;
				}
				else
				{
					num = ((!this.reverseValue) ? this.m_FillRect.anchorMax[(int)this.axis] : (1f - this.m_FillRect.anchorMin[(int)this.axis]));
				}
			}
			else if (this.m_HandleContainerRect != null)
			{
				num = ((!this.reverseValue) ? this.m_HandleRect.anchorMin[(int)this.axis] : (1f - this.m_HandleRect.anchorMin[(int)this.axis]));
			}
			this.UpdateVisuals();
			if (num != this.normalizedValue)
			{
				this.onValueChanged.Invoke(this.m_Value);
			}
		}

		private void UpdateCachedReferences()
		{
			if (this.m_FillRect)
			{
				this.m_FillTransform = this.m_FillRect.transform;
				this.m_FillImage = this.m_FillRect.GetComponent<Image>();
				if (this.m_FillTransform.parent != null)
				{
					this.m_FillContainerRect = this.m_FillTransform.parent.GetComponent<RectTransform>();
				}
			}
			else
			{
				this.m_FillContainerRect = null;
				this.m_FillImage = null;
			}
			if (this.m_HandleRect)
			{
				this.m_HandleTransform = this.m_HandleRect.transform;
				if (this.m_HandleTransform.parent != null)
				{
					this.m_HandleContainerRect = this.m_HandleTransform.parent.GetComponent<RectTransform>();
				}
			}
			else
			{
				this.m_HandleContainerRect = null;
			}
		}

		private float ClampValue(float input)
		{
			float num = Mathf.Clamp(input, this.minValue, this.maxValue);
			if (this.wholeNumbers)
			{
				num = Mathf.Round(num);
			}
			return num;
		}

		private void Set(float input)
		{
			this.Set(input, true);
		}

		protected virtual void Set(float input, bool sendCallback)
		{
			float num = this.ClampValue(input);
			if (this.m_Value != num)
			{
				this.m_Value = num;
				this.UpdateVisuals();
				if (sendCallback)
				{
					this.m_OnValueChanged.Invoke(num);
				}
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			if (this.IsActive())
			{
				this.UpdateVisuals();
			}
		}

		private void UpdateVisuals()
		{
			this.m_Tracker.Clear();
			if (this.m_FillContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_FillRect, DrivenTransformProperties.Anchors);
				Vector2 zero = Vector2.zero;
				Vector2 one = Vector2.one;
				if (this.m_FillImage != null && this.m_FillImage.type == Image.Type.Filled)
				{
					this.m_FillImage.fillAmount = this.normalizedValue;
				}
				else if (this.reverseValue)
				{
					zero[(int)this.axis] = 1f - this.normalizedValue;
				}
				else
				{
					one[(int)this.axis] = this.normalizedValue;
				}
				this.m_FillRect.anchorMin = zero;
				this.m_FillRect.anchorMax = one;
			}
			if (this.m_HandleContainerRect != null)
			{
				this.m_Tracker.Add(this, this.m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 zero2 = Vector2.zero;
				Vector2 one2 = Vector2.one;
				int arg_144_1 = (int)this.axis;
				float value = (!this.reverseValue) ? this.normalizedValue : (1f - this.normalizedValue);
				one2[(int)this.axis] = value;
				zero2[arg_144_1] = value;
				this.m_HandleRect.anchorMin = zero2;
				this.m_HandleRect.anchorMax = one2;
			}
		}

		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform rectTransform = this.m_HandleContainerRect ?? this.m_FillContainerRect;
			if (rectTransform != null && rectTransform.rect.size[(int)this.axis] > 0f)
			{
				Vector2 a;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, cam, out a))
				{
					a -= rectTransform.rect.position;
					float num = Mathf.Clamp01((a - this.m_Offset)[(int)this.axis] / rectTransform.rect.size[(int)this.axis]);
					this.normalizedValue = ((!this.reverseValue) ? num : (1f - num));
				}
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				base.OnPointerDown(eventData);
				this.m_Offset = Vector2.zero;
				if (this.m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(this.m_HandleRect, eventData.position, eventData.enterEventCamera))
				{
					Vector2 offset;
					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_HandleRect, eventData.position, eventData.pressEventCamera, out offset))
					{
						this.m_Offset = offset;
					}
				}
				else
				{
					this.UpdateDrag(eventData, eventData.pressEventCamera);
				}
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (this.MayDrag(eventData))
			{
				this.UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				base.OnMove(eventData);
			}
			else
			{
				switch (eventData.moveDir)
				{
				case MoveDirection.Left:
					if (this.axis == Slider.Axis.Horizontal && this.FindSelectableOnLeft() == null)
					{
						this.Set((!this.reverseValue) ? (this.value - this.stepSize) : (this.value + this.stepSize));
					}
					else
					{
						base.OnMove(eventData);
					}
					break;
				case MoveDirection.Up:
					if (this.axis == Slider.Axis.Vertical && this.FindSelectableOnUp() == null)
					{
						this.Set((!this.reverseValue) ? (this.value + this.stepSize) : (this.value - this.stepSize));
					}
					else
					{
						base.OnMove(eventData);
					}
					break;
				case MoveDirection.Right:
					if (this.axis == Slider.Axis.Horizontal && this.FindSelectableOnRight() == null)
					{
						this.Set((!this.reverseValue) ? (this.value + this.stepSize) : (this.value - this.stepSize));
					}
					else
					{
						base.OnMove(eventData);
					}
					break;
				case MoveDirection.Down:
					if (this.axis == Slider.Axis.Vertical && this.FindSelectableOnDown() == null)
					{
						this.Set((!this.reverseValue) ? (this.value - this.stepSize) : (this.value + this.stepSize));
					}
					else
					{
						base.OnMove(eventData);
					}
					break;
				}
			}
		}

		public override Selectable FindSelectableOnLeft()
		{
			Selectable result;
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == Slider.Axis.Horizontal)
			{
				result = null;
			}
			else
			{
				result = base.FindSelectableOnLeft();
			}
			return result;
		}

		public override Selectable FindSelectableOnRight()
		{
			Selectable result;
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == Slider.Axis.Horizontal)
			{
				result = null;
			}
			else
			{
				result = base.FindSelectableOnRight();
			}
			return result;
		}

		public override Selectable FindSelectableOnUp()
		{
			Selectable result;
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == Slider.Axis.Vertical)
			{
				result = null;
			}
			else
			{
				result = base.FindSelectableOnUp();
			}
			return result;
		}

		public override Selectable FindSelectableOnDown()
		{
			Selectable result;
			if (base.navigation.mode == Navigation.Mode.Automatic && this.axis == Slider.Axis.Vertical)
			{
				result = null;
			}
			else
			{
				result = base.FindSelectableOnDown();
			}
			return result;
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		public void SetDirection(Slider.Direction direction, bool includeRectLayouts)
		{
			Slider.Axis axis = this.axis;
			bool reverseValue = this.reverseValue;
			this.direction = direction;
			if (includeRectLayouts)
			{
				if (this.axis != axis)
				{
					RectTransformUtility.FlipLayoutAxes(base.transform as RectTransform, true, true);
				}
				if (this.reverseValue != reverseValue)
				{
					RectTransformUtility.FlipLayoutOnAxis(base.transform as RectTransform, (int)this.axis, true, true);
				}
			}
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}
	}
}
