using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Button", 30)]
	public class Button : Selectable, IPointerClickHandler, ISubmitHandler, IEventSystemHandler
	{
		[Serializable]
		public class ButtonClickedEvent : UnityEvent
		{
		}

		[FormerlySerializedAs("onClick"), SerializeField]
		private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();

		public Button.ButtonClickedEvent onClick
		{
			get
			{
				return this.m_OnClick;
			}
			set
			{
				this.m_OnClick = value;
			}
		}

		protected Button()
		{
		}

		private void Press()
		{
			if (this.IsActive() && this.IsInteractable())
			{
				UISystemProfilerApi.AddMarker("Button.onClick", this);
				this.m_OnClick.Invoke();
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.Press();
			}
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Press();
			if (this.IsActive() && this.IsInteractable())
			{
				this.DoStateTransition(Selectable.SelectionState.Pressed, false);
				base.StartCoroutine(this.OnFinishSubmit());
			}
		}

		[DebuggerHidden]
		private IEnumerator OnFinishSubmit()
		{
			Button.<OnFinishSubmit>c__Iterator0 <OnFinishSubmit>c__Iterator = new Button.<OnFinishSubmit>c__Iterator0();
			<OnFinishSubmit>c__Iterator.$this = this;
			return <OnFinishSubmit>c__Iterator;
		}
	}
}
