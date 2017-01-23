using System;

namespace UnityEngine.EventSystems
{
	public class BaseEventData : AbstractEventData
	{
		private readonly EventSystem m_EventSystem;

		public BaseInputModule currentInputModule
		{
			get
			{
				return this.m_EventSystem.currentInputModule;
			}
		}

		public GameObject selectedObject
		{
			get
			{
				return this.m_EventSystem.currentSelectedGameObject;
			}
			set
			{
				this.m_EventSystem.SetSelectedGameObject(value, this);
			}
		}

		public BaseEventData(EventSystem eventSystem)
		{
			this.m_EventSystem = eventSystem;
		}
	}
}
