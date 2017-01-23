using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Serialization;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Event System")]
	public class EventSystem : UIBehaviour
	{
		private List<BaseInputModule> m_SystemInputModules = new List<BaseInputModule>();

		private BaseInputModule m_CurrentInputModule;

		[FormerlySerializedAs("m_Selected"), SerializeField]
		private GameObject m_FirstSelected;

		[SerializeField]
		private bool m_sendNavigationEvents = true;

		[SerializeField]
		private int m_DragThreshold = 5;

		private GameObject m_CurrentSelected;

		private bool m_Paused;

		private bool m_SelectionGuard;

		private BaseEventData m_DummyData;

		private static readonly Comparison<RaycastResult> s_RaycastComparer;

		[CompilerGenerated]
		private static Comparison<RaycastResult> <>f__mg$cache0;

		public static EventSystem current
		{
			get;
			set;
		}

		public bool sendNavigationEvents
		{
			get
			{
				return this.m_sendNavigationEvents;
			}
			set
			{
				this.m_sendNavigationEvents = value;
			}
		}

		public int pixelDragThreshold
		{
			get
			{
				return this.m_DragThreshold;
			}
			set
			{
				this.m_DragThreshold = value;
			}
		}

		public BaseInputModule currentInputModule
		{
			get
			{
				return this.m_CurrentInputModule;
			}
		}

		public GameObject firstSelectedGameObject
		{
			get
			{
				return this.m_FirstSelected;
			}
			set
			{
				this.m_FirstSelected = value;
			}
		}

		public GameObject currentSelectedGameObject
		{
			get
			{
				return this.m_CurrentSelected;
			}
		}

		[Obsolete("lastSelectedGameObject is no longer supported")]
		public GameObject lastSelectedGameObject
		{
			get
			{
				return null;
			}
		}

		public bool alreadySelecting
		{
			get
			{
				return this.m_SelectionGuard;
			}
		}

		private BaseEventData baseEventDataCache
		{
			get
			{
				if (this.m_DummyData == null)
				{
					this.m_DummyData = new BaseEventData(this);
				}
				return this.m_DummyData;
			}
		}

		protected EventSystem()
		{
		}

		public void UpdateModules()
		{
			base.GetComponents<BaseInputModule>(this.m_SystemInputModules);
			for (int i = this.m_SystemInputModules.Count - 1; i >= 0; i--)
			{
				if (!this.m_SystemInputModules[i] || !this.m_SystemInputModules[i].IsActive())
				{
					this.m_SystemInputModules.RemoveAt(i);
				}
			}
		}

		public void SetSelectedGameObject(GameObject selected, BaseEventData pointer)
		{
			if (this.m_SelectionGuard)
			{
				Debug.LogError("Attempting to select " + selected + "while already selecting an object.");
			}
			else
			{
				this.m_SelectionGuard = true;
				if (selected == this.m_CurrentSelected)
				{
					this.m_SelectionGuard = false;
				}
				else
				{
					ExecuteEvents.Execute<IDeselectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.deselectHandler);
					this.m_CurrentSelected = selected;
					ExecuteEvents.Execute<ISelectHandler>(this.m_CurrentSelected, pointer, ExecuteEvents.selectHandler);
					this.m_SelectionGuard = false;
				}
			}
		}

		public void SetSelectedGameObject(GameObject selected)
		{
			this.SetSelectedGameObject(selected, this.baseEventDataCache);
		}

		private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
		{
			int result;
			if (lhs.module != rhs.module)
			{
				if (lhs.module.eventCamera != null && rhs.module.eventCamera != null && lhs.module.eventCamera.depth != rhs.module.eventCamera.depth)
				{
					if (lhs.module.eventCamera.depth < rhs.module.eventCamera.depth)
					{
						result = 1;
						return result;
					}
					if (lhs.module.eventCamera.depth == rhs.module.eventCamera.depth)
					{
						result = 0;
						return result;
					}
					result = -1;
					return result;
				}
				else
				{
					if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
					{
						result = rhs.module.sortOrderPriority.CompareTo(lhs.module.sortOrderPriority);
						return result;
					}
					if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
					{
						result = rhs.module.renderOrderPriority.CompareTo(lhs.module.renderOrderPriority);
						return result;
					}
				}
			}
			if (lhs.sortingLayer != rhs.sortingLayer)
			{
				int layerValueFromID = SortingLayer.GetLayerValueFromID(rhs.sortingLayer);
				int layerValueFromID2 = SortingLayer.GetLayerValueFromID(lhs.sortingLayer);
				result = layerValueFromID.CompareTo(layerValueFromID2);
			}
			else if (lhs.sortingOrder != rhs.sortingOrder)
			{
				result = rhs.sortingOrder.CompareTo(lhs.sortingOrder);
			}
			else if (lhs.depth != rhs.depth)
			{
				result = rhs.depth.CompareTo(lhs.depth);
			}
			else if (lhs.distance != rhs.distance)
			{
				result = lhs.distance.CompareTo(rhs.distance);
			}
			else
			{
				result = lhs.index.CompareTo(rhs.index);
			}
			return result;
		}

		public void RaycastAll(PointerEventData eventData, List<RaycastResult> raycastResults)
		{
			raycastResults.Clear();
			List<BaseRaycaster> raycasters = RaycasterManager.GetRaycasters();
			for (int i = 0; i < raycasters.Count; i++)
			{
				BaseRaycaster baseRaycaster = raycasters[i];
				if (!(baseRaycaster == null) && baseRaycaster.IsActive())
				{
					baseRaycaster.Raycast(eventData, raycastResults);
				}
			}
			raycastResults.Sort(EventSystem.s_RaycastComparer);
		}

		public bool IsPointerOverGameObject()
		{
			return this.IsPointerOverGameObject(-1);
		}

		public bool IsPointerOverGameObject(int pointerId)
		{
			return !(this.m_CurrentInputModule == null) && this.m_CurrentInputModule.IsPointerOverGameObject(pointerId);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (EventSystem.current == null)
			{
				EventSystem.current = this;
			}
		}

		protected override void OnDisable()
		{
			if (this.m_CurrentInputModule != null)
			{
				this.m_CurrentInputModule.DeactivateModule();
				this.m_CurrentInputModule = null;
			}
			if (EventSystem.current == this)
			{
				EventSystem.current = null;
			}
			base.OnDisable();
		}

		private void TickModules()
		{
			for (int i = 0; i < this.m_SystemInputModules.Count; i++)
			{
				if (this.m_SystemInputModules[i] != null)
				{
					this.m_SystemInputModules[i].UpdateModule();
				}
			}
		}

		protected virtual void OnApplicationFocus(bool hasFocus)
		{
			if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows || SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux || SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
			{
				this.m_Paused = !hasFocus;
			}
		}

		protected virtual void Update()
		{
			if (!(EventSystem.current != this) && !this.m_Paused)
			{
				this.TickModules();
				bool flag = false;
				for (int i = 0; i < this.m_SystemInputModules.Count; i++)
				{
					BaseInputModule baseInputModule = this.m_SystemInputModules[i];
					if (baseInputModule.IsModuleSupported() && baseInputModule.ShouldActivateModule())
					{
						if (this.m_CurrentInputModule != baseInputModule)
						{
							this.ChangeEventModule(baseInputModule);
							flag = true;
						}
						break;
					}
				}
				if (this.m_CurrentInputModule == null)
				{
					for (int j = 0; j < this.m_SystemInputModules.Count; j++)
					{
						BaseInputModule baseInputModule2 = this.m_SystemInputModules[j];
						if (baseInputModule2.IsModuleSupported())
						{
							this.ChangeEventModule(baseInputModule2);
							flag = true;
							break;
						}
					}
				}
				if (!flag && this.m_CurrentInputModule != null)
				{
					this.m_CurrentInputModule.Process();
				}
			}
		}

		private void ChangeEventModule(BaseInputModule module)
		{
			if (!(this.m_CurrentInputModule == module))
			{
				if (this.m_CurrentInputModule != null)
				{
					this.m_CurrentInputModule.DeactivateModule();
				}
				if (module != null)
				{
					module.ActivateModule();
				}
				this.m_CurrentInputModule = module;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Selected:</b>" + this.currentSelectedGameObject);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine((!(this.m_CurrentInputModule != null)) ? "No module" : this.m_CurrentInputModule.ToString());
			return stringBuilder.ToString();
		}

		static EventSystem()
		{
			// Note: this type is marked as 'beforefieldinit'.
			if (EventSystem.<>f__mg$cache0 == null)
			{
				EventSystem.<>f__mg$cache0 = new Comparison<RaycastResult>(EventSystem.RaycastComparer);
			}
			EventSystem.s_RaycastComparer = EventSystem.<>f__mg$cache0;
		}
	}
}
