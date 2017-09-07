using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Selectable", 70), DisallowMultipleComponent, ExecuteInEditMode, SelectionBase]
	public class Selectable : UIBehaviour, IMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		public enum Transition
		{
			None,
			ColorTint,
			SpriteSwap,
			Animation
		}

		protected enum SelectionState
		{
			Normal,
			Highlighted,
			Pressed,
			Disabled
		}

		private static List<Selectable> s_List = new List<Selectable>();

		[FormerlySerializedAs("navigation"), SerializeField]
		private Navigation m_Navigation = Navigation.defaultNavigation;

		[FormerlySerializedAs("transition"), SerializeField]
		private Selectable.Transition m_Transition = Selectable.Transition.ColorTint;

		[FormerlySerializedAs("colors"), SerializeField]
		private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

		[FormerlySerializedAs("spriteState"), SerializeField]
		private SpriteState m_SpriteState;

		[FormerlySerializedAs("animationTriggers"), SerializeField]
		private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

		[SerializeField, Tooltip("Can the Selectable be interacted with?")]
		private bool m_Interactable = true;

		[FormerlySerializedAs("highlightGraphic"), FormerlySerializedAs("m_HighlightGraphic"), SerializeField]
		private Graphic m_TargetGraphic;

		private bool m_GroupsAllowInteraction = true;

		private Selectable.SelectionState m_CurrentSelectionState;

		private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();

		public static List<Selectable> allSelectables
		{
			get
			{
				return Selectable.s_List;
			}
		}

		public Navigation navigation
		{
			get
			{
				return this.m_Navigation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Navigation>(ref this.m_Navigation, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public Selectable.Transition transition
		{
			get
			{
				return this.m_Transition;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Selectable.Transition>(ref this.m_Transition, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public ColorBlock colors
		{
			get
			{
				return this.m_Colors;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<ColorBlock>(ref this.m_Colors, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public SpriteState spriteState
		{
			get
			{
				return this.m_SpriteState;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<SpriteState>(ref this.m_SpriteState, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public AnimationTriggers animationTriggers
		{
			get
			{
				return this.m_AnimationTriggers;
			}
			set
			{
				if (SetPropertyUtility.SetClass<AnimationTriggers>(ref this.m_AnimationTriggers, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public Graphic targetGraphic
		{
			get
			{
				return this.m_TargetGraphic;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Graphic>(ref this.m_TargetGraphic, value))
				{
					this.OnSetProperty();
				}
			}
		}

		public bool interactable
		{
			get
			{
				return this.m_Interactable;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_Interactable, value))
				{
					if (!this.m_Interactable && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == base.gameObject)
					{
						EventSystem.current.SetSelectedGameObject(null);
					}
					if (this.m_Interactable)
					{
						this.UpdateSelectionState(null);
					}
					this.OnSetProperty();
				}
			}
		}

		private bool isPointerInside
		{
			get;
			set;
		}

		private bool isPointerDown
		{
			get;
			set;
		}

		private bool hasSelection
		{
			get;
			set;
		}

		public Image image
		{
			get
			{
				return this.m_TargetGraphic as Image;
			}
			set
			{
				this.m_TargetGraphic = value;
			}
		}

		public Animator animator
		{
			get
			{
				return base.GetComponent<Animator>();
			}
		}

		protected Selectable.SelectionState currentSelectionState
		{
			get
			{
				return this.m_CurrentSelectionState;
			}
		}

		protected Selectable()
		{
		}

		protected override void Awake()
		{
			if (this.m_TargetGraphic == null)
			{
				this.m_TargetGraphic = base.GetComponent<Graphic>();
			}
		}

		protected override void OnCanvasGroupChanged()
		{
			bool flag = true;
			Transform transform = base.transform;
			while (transform != null)
			{
				transform.GetComponents<CanvasGroup>(this.m_CanvasGroupCache);
				bool flag2 = false;
				for (int i = 0; i < this.m_CanvasGroupCache.Count; i++)
				{
					if (!this.m_CanvasGroupCache[i].interactable)
					{
						flag = false;
						flag2 = true;
					}
					if (this.m_CanvasGroupCache[i].ignoreParentGroups)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					break;
				}
				transform = transform.parent;
			}
			if (flag != this.m_GroupsAllowInteraction)
			{
				this.m_GroupsAllowInteraction = flag;
				this.OnSetProperty();
			}
		}

		public virtual bool IsInteractable()
		{
			return this.m_GroupsAllowInteraction && this.m_Interactable;
		}

		protected override void OnDidApplyAnimationProperties()
		{
			this.OnSetProperty();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Selectable.s_List.Add(this);
			Selectable.SelectionState currentSelectionState = Selectable.SelectionState.Normal;
			if (this.hasSelection)
			{
				currentSelectionState = Selectable.SelectionState.Highlighted;
			}
			this.m_CurrentSelectionState = currentSelectionState;
			this.InternalEvaluateAndTransitionToSelectionState(true);
		}

		private void OnSetProperty()
		{
			if (!Application.isPlaying)
			{
				this.InternalEvaluateAndTransitionToSelectionState(true);
			}
			else
			{
				this.InternalEvaluateAndTransitionToSelectionState(false);
			}
		}

		protected override void OnDisable()
		{
			Selectable.s_List.Remove(this);
			this.InstantClearState();
			base.OnDisable();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Colors.fadeDuration = Mathf.Max(this.m_Colors.fadeDuration, 0f);
			if (base.isActiveAndEnabled)
			{
				if (!this.interactable && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == base.gameObject)
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
				this.DoSpriteSwap(null);
				this.StartColorTween(Color.white, true);
				this.TriggerAnimation(this.m_AnimationTriggers.normalTrigger);
				this.InternalEvaluateAndTransitionToSelectionState(true);
			}
		}

		protected override void Reset()
		{
			this.m_TargetGraphic = base.GetComponent<Graphic>();
		}

		protected virtual void InstantClearState()
		{
			string normalTrigger = this.m_AnimationTriggers.normalTrigger;
			this.isPointerInside = false;
			this.isPointerDown = false;
			this.hasSelection = false;
			Selectable.Transition transition = this.m_Transition;
			if (transition != Selectable.Transition.ColorTint)
			{
				if (transition != Selectable.Transition.SpriteSwap)
				{
					if (transition == Selectable.Transition.Animation)
					{
						this.TriggerAnimation(normalTrigger);
					}
				}
				else
				{
					this.DoSpriteSwap(null);
				}
			}
			else
			{
				this.StartColorTween(Color.white, true);
			}
		}

		protected virtual void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			Color a;
			Sprite newSprite;
			string triggername;
			switch (state)
			{
			case Selectable.SelectionState.Normal:
				a = this.m_Colors.normalColor;
				newSprite = null;
				triggername = this.m_AnimationTriggers.normalTrigger;
				break;
			case Selectable.SelectionState.Highlighted:
				a = this.m_Colors.highlightedColor;
				newSprite = this.m_SpriteState.highlightedSprite;
				triggername = this.m_AnimationTriggers.highlightedTrigger;
				break;
			case Selectable.SelectionState.Pressed:
				a = this.m_Colors.pressedColor;
				newSprite = this.m_SpriteState.pressedSprite;
				triggername = this.m_AnimationTriggers.pressedTrigger;
				break;
			case Selectable.SelectionState.Disabled:
				a = this.m_Colors.disabledColor;
				newSprite = this.m_SpriteState.disabledSprite;
				triggername = this.m_AnimationTriggers.disabledTrigger;
				break;
			default:
				a = Color.black;
				newSprite = null;
				triggername = string.Empty;
				break;
			}
			if (base.gameObject.activeInHierarchy)
			{
				Selectable.Transition transition = this.m_Transition;
				if (transition != Selectable.Transition.ColorTint)
				{
					if (transition != Selectable.Transition.SpriteSwap)
					{
						if (transition == Selectable.Transition.Animation)
						{
							this.TriggerAnimation(triggername);
						}
					}
					else
					{
						this.DoSpriteSwap(newSprite);
					}
				}
				else
				{
					this.StartColorTween(a * this.m_Colors.colorMultiplier, instant);
				}
			}
		}

		public Selectable FindSelectable(Vector3 dir)
		{
			dir = dir.normalized;
			Vector3 v = Quaternion.Inverse(base.transform.rotation) * dir;
			Vector3 b = base.transform.TransformPoint(Selectable.GetPointOnRectEdge(base.transform as RectTransform, v));
			float num = float.NegativeInfinity;
			Selectable result = null;
			for (int i = 0; i < Selectable.s_List.Count; i++)
			{
				Selectable selectable = Selectable.s_List[i];
				if (!(selectable == this) && !(selectable == null))
				{
					if (selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None)
					{
						RectTransform rectTransform = selectable.transform as RectTransform;
						Vector3 position = (!(rectTransform != null)) ? Vector3.zero : rectTransform.rect.center;
						Vector3 rhs = selectable.transform.TransformPoint(position) - b;
						float num2 = Vector3.Dot(dir, rhs);
						if (num2 > 0f)
						{
							float num3 = num2 / rhs.sqrMagnitude;
							if (num3 > num)
							{
								num = num3;
								result = selectable;
							}
						}
					}
				}
			}
			return result;
		}

		private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
		{
			Vector3 result;
			if (rect == null)
			{
				result = Vector3.zero;
			}
			else
			{
				if (dir != Vector2.zero)
				{
					dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
				}
				dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
				result = dir;
			}
			return result;
		}

		private void Navigate(AxisEventData eventData, Selectable sel)
		{
			if (sel != null && sel.IsActive())
			{
				eventData.selectedObject = sel.gameObject;
			}
		}

		public virtual Selectable FindSelectableOnLeft()
		{
			Selectable result;
			if (this.m_Navigation.mode == Navigation.Mode.Explicit)
			{
				result = this.m_Navigation.selectOnLeft;
			}
			else if ((this.m_Navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
			{
				result = this.FindSelectable(base.transform.rotation * Vector3.left);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public virtual Selectable FindSelectableOnRight()
		{
			Selectable result;
			if (this.m_Navigation.mode == Navigation.Mode.Explicit)
			{
				result = this.m_Navigation.selectOnRight;
			}
			else if ((this.m_Navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
			{
				result = this.FindSelectable(base.transform.rotation * Vector3.right);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public virtual Selectable FindSelectableOnUp()
		{
			Selectable result;
			if (this.m_Navigation.mode == Navigation.Mode.Explicit)
			{
				result = this.m_Navigation.selectOnUp;
			}
			else if ((this.m_Navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
			{
				result = this.FindSelectable(base.transform.rotation * Vector3.up);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public virtual Selectable FindSelectableOnDown()
		{
			Selectable result;
			if (this.m_Navigation.mode == Navigation.Mode.Explicit)
			{
				result = this.m_Navigation.selectOnDown;
			}
			else if ((this.m_Navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
			{
				result = this.FindSelectable(base.transform.rotation * Vector3.down);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public virtual void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
			case MoveDirection.Left:
				this.Navigate(eventData, this.FindSelectableOnLeft());
				break;
			case MoveDirection.Up:
				this.Navigate(eventData, this.FindSelectableOnUp());
				break;
			case MoveDirection.Right:
				this.Navigate(eventData, this.FindSelectableOnRight());
				break;
			case MoveDirection.Down:
				this.Navigate(eventData, this.FindSelectableOnDown());
				break;
			}
		}

		private void StartColorTween(Color targetColor, bool instant)
		{
			if (!(this.m_TargetGraphic == null))
			{
				this.m_TargetGraphic.CrossFadeColor(targetColor, (!instant) ? this.m_Colors.fadeDuration : 0f, true, true);
			}
		}

		private void DoSpriteSwap(Sprite newSprite)
		{
			if (!(this.image == null))
			{
				this.image.overrideSprite = newSprite;
			}
		}

		private void TriggerAnimation(string triggername)
		{
			if (this.transition == Selectable.Transition.Animation && !(this.animator == null) && this.animator.isActiveAndEnabled && this.animator.hasBoundPlayables && !string.IsNullOrEmpty(triggername))
			{
				this.animator.ResetTrigger(this.m_AnimationTriggers.normalTrigger);
				this.animator.ResetTrigger(this.m_AnimationTriggers.pressedTrigger);
				this.animator.ResetTrigger(this.m_AnimationTriggers.highlightedTrigger);
				this.animator.ResetTrigger(this.m_AnimationTriggers.disabledTrigger);
				this.animator.SetTrigger(triggername);
			}
		}

		protected bool IsHighlighted(BaseEventData eventData)
		{
			bool result;
			if (!this.IsActive())
			{
				result = false;
			}
			else if (this.IsPressed())
			{
				result = false;
			}
			else
			{
				bool flag = this.hasSelection;
				if (eventData is PointerEventData)
				{
					PointerEventData pointerEventData = eventData as PointerEventData;
					flag |= ((this.isPointerDown && !this.isPointerInside && pointerEventData.pointerPress == base.gameObject) || (!this.isPointerDown && this.isPointerInside && pointerEventData.pointerPress == base.gameObject) || (!this.isPointerDown && this.isPointerInside && pointerEventData.pointerPress == null));
				}
				else
				{
					flag |= this.isPointerInside;
				}
				result = flag;
			}
			return result;
		}

		[Obsolete("Is Pressed no longer requires eventData", false)]
		protected bool IsPressed(BaseEventData eventData)
		{
			return this.IsPressed();
		}

		protected bool IsPressed()
		{
			return this.IsActive() && this.isPointerInside && this.isPointerDown;
		}

		protected void UpdateSelectionState(BaseEventData eventData)
		{
			if (this.IsPressed())
			{
				this.m_CurrentSelectionState = Selectable.SelectionState.Pressed;
			}
			else if (this.IsHighlighted(eventData))
			{
				this.m_CurrentSelectionState = Selectable.SelectionState.Highlighted;
			}
			else
			{
				this.m_CurrentSelectionState = Selectable.SelectionState.Normal;
			}
		}

		private void EvaluateAndTransitionToSelectionState(BaseEventData eventData)
		{
			if (this.IsActive() && this.IsInteractable())
			{
				this.UpdateSelectionState(eventData);
				this.InternalEvaluateAndTransitionToSelectionState(false);
			}
		}

		private void InternalEvaluateAndTransitionToSelectionState(bool instant)
		{
			Selectable.SelectionState state = this.m_CurrentSelectionState;
			if (this.IsActive() && !this.IsInteractable())
			{
				state = Selectable.SelectionState.Disabled;
			}
			this.DoStateTransition(state, instant);
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (this.IsInteractable() && this.navigation.mode != Navigation.Mode.None && EventSystem.current != null)
				{
					EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
				}
				this.isPointerDown = true;
				this.EvaluateAndTransitionToSelectionState(eventData);
			}
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.isPointerDown = false;
				this.EvaluateAndTransitionToSelectionState(eventData);
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			this.isPointerInside = true;
			this.EvaluateAndTransitionToSelectionState(eventData);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			this.isPointerInside = false;
			this.EvaluateAndTransitionToSelectionState(eventData);
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			this.hasSelection = true;
			this.EvaluateAndTransitionToSelectionState(eventData);
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			this.hasSelection = false;
			this.EvaluateAndTransitionToSelectionState(eventData);
		}

		public virtual void Select()
		{
			if (!(EventSystem.current == null) && !EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
		}
	}
}
