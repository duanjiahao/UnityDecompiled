using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Standalone Input Module")]
	public class StandaloneInputModule : PointerInputModule
	{
		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public enum InputMode
		{
			Mouse,
			Buttons
		}

		private float m_PrevActionTime;

		private Vector2 m_LastMoveVector;

		private int m_ConsecutiveMoveCount = 0;

		private Vector2 m_LastMousePosition;

		private Vector2 m_MousePosition;

		private GameObject m_CurrentFocusedGameObject;

		[SerializeField]
		private string m_HorizontalAxis = "Horizontal";

		[SerializeField]
		private string m_VerticalAxis = "Vertical";

		[SerializeField]
		private string m_SubmitButton = "Submit";

		[SerializeField]
		private string m_CancelButton = "Cancel";

		[SerializeField]
		private float m_InputActionsPerSecond = 10f;

		[SerializeField]
		private float m_RepeatDelay = 0.5f;

		[FormerlySerializedAs("m_AllowActivationOnMobileDevice"), SerializeField]
		private bool m_ForceModuleActive;

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public StandaloneInputModule.InputMode inputMode
		{
			get
			{
				return StandaloneInputModule.InputMode.Mouse;
			}
		}

		[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
		public bool allowActivationOnMobileDevice
		{
			get
			{
				return this.m_ForceModuleActive;
			}
			set
			{
				this.m_ForceModuleActive = value;
			}
		}

		public bool forceModuleActive
		{
			get
			{
				return this.m_ForceModuleActive;
			}
			set
			{
				this.m_ForceModuleActive = value;
			}
		}

		public float inputActionsPerSecond
		{
			get
			{
				return this.m_InputActionsPerSecond;
			}
			set
			{
				this.m_InputActionsPerSecond = value;
			}
		}

		public float repeatDelay
		{
			get
			{
				return this.m_RepeatDelay;
			}
			set
			{
				this.m_RepeatDelay = value;
			}
		}

		public string horizontalAxis
		{
			get
			{
				return this.m_HorizontalAxis;
			}
			set
			{
				this.m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return this.m_VerticalAxis;
			}
			set
			{
				this.m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return this.m_SubmitButton;
			}
			set
			{
				this.m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return this.m_CancelButton;
			}
			set
			{
				this.m_CancelButton = value;
			}
		}

		protected StandaloneInputModule()
		{
		}

		private bool ShouldIgnoreEventsOnNoFocus()
		{
			bool result;
			switch (SystemInfo.operatingSystemFamily)
			{
			case OperatingSystemFamily.MacOSX:
			case OperatingSystemFamily.Windows:
			case OperatingSystemFamily.Linux:
				result = !EditorApplication.isRemoteConnected;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		public override void UpdateModule()
		{
			if (base.eventSystem.isFocused || !this.ShouldIgnoreEventsOnNoFocus())
			{
				this.m_LastMousePosition = this.m_MousePosition;
				this.m_MousePosition = base.input.mousePosition;
			}
		}

		public override bool IsModuleSupported()
		{
			return this.m_ForceModuleActive || base.input.mousePresent || base.input.touchSupported;
		}

		public override bool ShouldActivateModule()
		{
			bool result;
			if (!base.ShouldActivateModule())
			{
				result = false;
			}
			else
			{
				bool flag = this.m_ForceModuleActive;
				flag |= base.input.GetButtonDown(this.m_SubmitButton);
				flag |= base.input.GetButtonDown(this.m_CancelButton);
				flag |= !Mathf.Approximately(base.input.GetAxisRaw(this.m_HorizontalAxis), 0f);
				flag |= !Mathf.Approximately(base.input.GetAxisRaw(this.m_VerticalAxis), 0f);
				flag |= ((this.m_MousePosition - this.m_LastMousePosition).sqrMagnitude > 0f);
				flag |= base.input.GetMouseButtonDown(0);
				if (base.input.touchCount > 0)
				{
					flag = true;
				}
				result = flag;
			}
			return result;
		}

		public override void ActivateModule()
		{
			if (base.eventSystem.isFocused || !this.ShouldIgnoreEventsOnNoFocus())
			{
				base.ActivateModule();
				this.m_MousePosition = base.input.mousePosition;
				this.m_LastMousePosition = base.input.mousePosition;
				GameObject gameObject = base.eventSystem.currentSelectedGameObject;
				if (gameObject == null)
				{
					gameObject = base.eventSystem.firstSelectedGameObject;
				}
				base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
			}
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			base.ClearSelection();
		}

		public override void Process()
		{
			if (base.eventSystem.isFocused || !this.ShouldIgnoreEventsOnNoFocus())
			{
				bool flag = this.SendUpdateEventToSelectedObject();
				if (base.eventSystem.sendNavigationEvents)
				{
					if (!flag)
					{
						flag |= this.SendMoveEventToSelectedObject();
					}
					if (!flag)
					{
						this.SendSubmitEventToSelectedObject();
					}
				}
				if (!this.ProcessTouchEvents() && base.input.mousePresent)
				{
					this.ProcessMouseEvent();
				}
			}
		}

		private bool ProcessTouchEvents()
		{
			for (int i = 0; i < base.input.touchCount; i++)
			{
				Touch touch = base.input.GetTouch(i);
				if (touch.type != TouchType.Indirect)
				{
					bool pressed;
					bool flag;
					PointerEventData touchPointerEventData = base.GetTouchPointerEventData(touch, out pressed, out flag);
					this.ProcessTouchPress(touchPointerEventData, pressed, flag);
					if (!flag)
					{
						this.ProcessMove(touchPointerEventData);
						this.ProcessDrag(touchPointerEventData);
					}
					else
					{
						base.RemovePointerData(touchPointerEventData);
					}
				}
			}
			return base.input.touchCount > 0;
		}

		protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
		{
			GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					base.HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress)
				{
					float num = unscaledTime - pointerEvent.clickTime;
					if (num < 0.3f)
					{
						pointerEvent.clickCount++;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (released)
			{
				ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
				}
				else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
				}
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.dragging = false;
				pointerEvent.pointerDrag = null;
				ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
				pointerEvent.pointerEnter = null;
			}
		}

		protected bool SendSubmitEventToSelectedObject()
		{
			bool result;
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				result = false;
			}
			else
			{
				BaseEventData baseEventData = this.GetBaseEventData();
				if (base.input.GetButtonDown(this.m_SubmitButton))
				{
					ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
				}
				if (base.input.GetButtonDown(this.m_CancelButton))
				{
					ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
				}
				result = baseEventData.used;
			}
			return result;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = base.input.GetAxisRaw(this.m_HorizontalAxis);
			zero.y = base.input.GetAxisRaw(this.m_VerticalAxis);
			if (base.input.GetButtonDown(this.m_HorizontalAxis))
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (base.input.GetButtonDown(this.m_VerticalAxis))
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		protected bool SendMoveEventToSelectedObject()
		{
			float unscaledTime = Time.unscaledTime;
			Vector2 rawMoveVector = this.GetRawMoveVector();
			bool result;
			if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
			{
				this.m_ConsecutiveMoveCount = 0;
				result = false;
			}
			else
			{
				bool flag = base.input.GetButtonDown(this.m_HorizontalAxis) || base.input.GetButtonDown(this.m_VerticalAxis);
				bool flag2 = Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0f;
				if (!flag)
				{
					if (flag2 && this.m_ConsecutiveMoveCount == 1)
					{
						flag = (unscaledTime > this.m_PrevActionTime + this.m_RepeatDelay);
					}
					else
					{
						flag = (unscaledTime > this.m_PrevActionTime + 1f / this.m_InputActionsPerSecond);
					}
				}
				if (!flag)
				{
					result = false;
				}
				else
				{
					AxisEventData axisEventData = this.GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
					if (axisEventData.moveDir != MoveDirection.None)
					{
						ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
						if (!flag2)
						{
							this.m_ConsecutiveMoveCount = 0;
						}
						this.m_ConsecutiveMoveCount++;
						this.m_PrevActionTime = unscaledTime;
						this.m_LastMoveVector = rawMoveVector;
					}
					else
					{
						this.m_ConsecutiveMoveCount = 0;
					}
					result = axisEventData.used;
				}
			}
			return result;
		}

		protected void ProcessMouseEvent()
		{
			this.ProcessMouseEvent(0);
		}

		[Obsolete("This method is no longer checked, overriding it with return true does nothing!")]
		protected virtual bool ForceAutoSelect()
		{
			return false;
		}

		protected void ProcessMouseEvent(int id)
		{
			PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
			PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			this.m_CurrentFocusedGameObject = eventData.buttonData.pointerCurrentRaycast.gameObject;
			this.ProcessMousePress(eventData);
			this.ProcessMove(eventData.buttonData);
			this.ProcessDrag(eventData.buttonData);
			this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject);
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, eventData.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			bool result;
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				result = false;
			}
			else
			{
				BaseEventData baseEventData = this.GetBaseEventData();
				ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
				result = baseEventData.used;
			}
			return result;
		}

		protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					float num = unscaledTime - buttonData.clickTime;
					if (num < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					base.HandlePointerExitAndEnter(buttonData, null);
					base.HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		protected GameObject GetCurrentFocusedGameObject()
		{
			return this.m_CurrentFocusedGameObject;
		}
	}
}
