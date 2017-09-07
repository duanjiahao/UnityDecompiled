using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Toggle", 31), RequireComponent(typeof(RectTransform))]
	public class Toggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement, IEventSystemHandler
	{
		public enum ToggleTransition
		{
			None,
			Fade
		}

		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{
		}

		public Toggle.ToggleTransition toggleTransition = Toggle.ToggleTransition.Fade;

		public Graphic graphic;

		[SerializeField]
		private ToggleGroup m_Group;

		public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

		[FormerlySerializedAs("m_IsActive"), SerializeField, Tooltip("Is the toggle currently on or off?")]
		private bool m_IsOn;

		public ToggleGroup group
		{
			get
			{
				return this.m_Group;
			}
			set
			{
				this.m_Group = value;
				if (Application.isPlaying)
				{
					this.SetToggleGroup(this.m_Group, true);
					this.PlayEffect(true);
				}
			}
		}

		public bool isOn
		{
			get
			{
				return this.m_IsOn;
			}
			set
			{
				this.Set(value);
			}
		}

		protected Toggle()
		{
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			PrefabType prefabType = PrefabUtility.GetPrefabType(this);
			if (prefabType != PrefabType.Prefab && !Application.isPlaying)
			{
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			}
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				this.onValueChanged.Invoke(this.m_IsOn);
			}
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
			this.SetToggleGroup(this.m_Group, false);
			this.PlayEffect(true);
		}

		protected override void OnDisable()
		{
			this.SetToggleGroup(null, false);
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			if (this.graphic != null)
			{
				bool flag = !Mathf.Approximately(this.graphic.canvasRenderer.GetColor().a, 0f);
				if (this.m_IsOn != flag)
				{
					this.m_IsOn = flag;
					this.Set(!flag);
				}
			}
			base.OnDidApplyAnimationProperties();
		}

		private void SetToggleGroup(ToggleGroup newGroup, bool setMemberValue)
		{
			ToggleGroup group = this.m_Group;
			if (this.m_Group != null)
			{
				this.m_Group.UnregisterToggle(this);
			}
			if (setMemberValue)
			{
				this.m_Group = newGroup;
			}
			if (newGroup != null && this.IsActive())
			{
				newGroup.RegisterToggle(this);
			}
			if (newGroup != null && newGroup != group && this.isOn && this.IsActive())
			{
				newGroup.NotifyToggleOn(this);
			}
		}

		private void Set(bool value)
		{
			this.Set(value, true);
		}

		private void Set(bool value, bool sendCallback)
		{
			if (this.m_IsOn != value)
			{
				this.m_IsOn = value;
				if (this.m_Group != null && this.IsActive())
				{
					if (this.m_IsOn || (!this.m_Group.AnyTogglesOn() && !this.m_Group.allowSwitchOff))
					{
						this.m_IsOn = true;
						this.m_Group.NotifyToggleOn(this);
					}
				}
				this.PlayEffect(this.toggleTransition == Toggle.ToggleTransition.None);
				if (sendCallback)
				{
					UISystemProfilerApi.AddMarker("Toggle.value", this);
					this.onValueChanged.Invoke(this.m_IsOn);
				}
			}
		}

		private void PlayEffect(bool instant)
		{
			if (!(this.graphic == null))
			{
				if (!Application.isPlaying)
				{
					this.graphic.canvasRenderer.SetAlpha((!this.m_IsOn) ? 0f : 1f);
				}
				else
				{
					this.graphic.CrossFadeAlpha((!this.m_IsOn) ? 0f : 1f, (!instant) ? 0.1f : 0f, true);
				}
			}
		}

		protected override void Start()
		{
			this.PlayEffect(true);
		}

		private void InternalToggle()
		{
			if (this.IsActive() && this.IsInteractable())
			{
				this.isOn = !this.isOn;
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.InternalToggle();
			}
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.InternalToggle();
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}
	}
}
