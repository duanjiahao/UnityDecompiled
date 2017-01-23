using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.EventSystems
{
	public class PointerEventData : BaseEventData
	{
		public enum InputButton
		{
			Left,
			Right,
			Middle
		}

		public enum FramePressState
		{
			Pressed,
			Released,
			PressedAndReleased,
			NotChanged
		}

		private GameObject m_PointerPress;

		public List<GameObject> hovered = new List<GameObject>();

		public GameObject pointerEnter
		{
			get;
			set;
		}

		public GameObject lastPress
		{
			get;
			private set;
		}

		public GameObject rawPointerPress
		{
			get;
			set;
		}

		public GameObject pointerDrag
		{
			get;
			set;
		}

		public RaycastResult pointerCurrentRaycast
		{
			get;
			set;
		}

		public RaycastResult pointerPressRaycast
		{
			get;
			set;
		}

		public bool eligibleForClick
		{
			get;
			set;
		}

		public int pointerId
		{
			get;
			set;
		}

		public Vector2 position
		{
			get;
			set;
		}

		public Vector2 delta
		{
			get;
			set;
		}

		public Vector2 pressPosition
		{
			get;
			set;
		}

		[Obsolete("Use either pointerCurrentRaycast.worldPosition or pointerPressRaycast.worldPosition")]
		public Vector3 worldPosition
		{
			get;
			set;
		}

		[Obsolete("Use either pointerCurrentRaycast.worldNormal or pointerPressRaycast.worldNormal")]
		public Vector3 worldNormal
		{
			get;
			set;
		}

		public float clickTime
		{
			get;
			set;
		}

		public int clickCount
		{
			get;
			set;
		}

		public Vector2 scrollDelta
		{
			get;
			set;
		}

		public bool useDragThreshold
		{
			get;
			set;
		}

		public bool dragging
		{
			get;
			set;
		}

		public PointerEventData.InputButton button
		{
			get;
			set;
		}

		public Camera enterEventCamera
		{
			get
			{
				return (!(this.pointerCurrentRaycast.module == null)) ? this.pointerCurrentRaycast.module.eventCamera : null;
			}
		}

		public Camera pressEventCamera
		{
			get
			{
				return (!(this.pointerPressRaycast.module == null)) ? this.pointerPressRaycast.module.eventCamera : null;
			}
		}

		public GameObject pointerPress
		{
			get
			{
				return this.m_PointerPress;
			}
			set
			{
				if (!(this.m_PointerPress == value))
				{
					this.lastPress = this.m_PointerPress;
					this.m_PointerPress = value;
				}
			}
		}

		public PointerEventData(EventSystem eventSystem) : base(eventSystem)
		{
			this.eligibleForClick = false;
			this.pointerId = -1;
			this.position = Vector2.zero;
			this.delta = Vector2.zero;
			this.pressPosition = Vector2.zero;
			this.clickTime = 0f;
			this.clickCount = 0;
			this.scrollDelta = Vector2.zero;
			this.useDragThreshold = true;
			this.dragging = false;
			this.button = PointerEventData.InputButton.Left;
		}

		public bool IsPointerMoving()
		{
			return this.delta.sqrMagnitude > 0f;
		}

		public bool IsScrolling()
		{
			return this.scrollDelta.sqrMagnitude > 0f;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Position</b>: " + this.position);
			stringBuilder.AppendLine("<b>delta</b>: " + this.delta);
			stringBuilder.AppendLine("<b>eligibleForClick</b>: " + this.eligibleForClick);
			stringBuilder.AppendLine("<b>pointerEnter</b>: " + this.pointerEnter);
			stringBuilder.AppendLine("<b>pointerPress</b>: " + this.pointerPress);
			stringBuilder.AppendLine("<b>lastPointerPress</b>: " + this.lastPress);
			stringBuilder.AppendLine("<b>pointerDrag</b>: " + this.pointerDrag);
			stringBuilder.AppendLine("<b>Use Drag Threshold</b>: " + this.useDragThreshold);
			stringBuilder.AppendLine("<b>Current Rayast:</b>");
			stringBuilder.AppendLine(this.pointerCurrentRaycast.ToString());
			stringBuilder.AppendLine("<b>Press Rayast:</b>");
			stringBuilder.AppendLine(this.pointerPressRaycast.ToString());
			return stringBuilder.ToString();
		}
	}
}
