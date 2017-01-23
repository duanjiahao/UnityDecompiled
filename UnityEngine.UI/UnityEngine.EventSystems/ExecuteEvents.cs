using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	public static class ExecuteEvents
	{
		public delegate void EventFunction<T1>(T1 handler, BaseEventData eventData);

		private static readonly ExecuteEvents.EventFunction<IPointerEnterHandler> s_PointerEnterHandler;

		private static readonly ExecuteEvents.EventFunction<IPointerExitHandler> s_PointerExitHandler;

		private static readonly ExecuteEvents.EventFunction<IPointerDownHandler> s_PointerDownHandler;

		private static readonly ExecuteEvents.EventFunction<IPointerUpHandler> s_PointerUpHandler;

		private static readonly ExecuteEvents.EventFunction<IPointerClickHandler> s_PointerClickHandler;

		private static readonly ExecuteEvents.EventFunction<IInitializePotentialDragHandler> s_InitializePotentialDragHandler;

		private static readonly ExecuteEvents.EventFunction<IBeginDragHandler> s_BeginDragHandler;

		private static readonly ExecuteEvents.EventFunction<IDragHandler> s_DragHandler;

		private static readonly ExecuteEvents.EventFunction<IEndDragHandler> s_EndDragHandler;

		private static readonly ExecuteEvents.EventFunction<IDropHandler> s_DropHandler;

		private static readonly ExecuteEvents.EventFunction<IScrollHandler> s_ScrollHandler;

		private static readonly ExecuteEvents.EventFunction<IUpdateSelectedHandler> s_UpdateSelectedHandler;

		private static readonly ExecuteEvents.EventFunction<ISelectHandler> s_SelectHandler;

		private static readonly ExecuteEvents.EventFunction<IDeselectHandler> s_DeselectHandler;

		private static readonly ExecuteEvents.EventFunction<IMoveHandler> s_MoveHandler;

		private static readonly ExecuteEvents.EventFunction<ISubmitHandler> s_SubmitHandler;

		private static readonly ExecuteEvents.EventFunction<ICancelHandler> s_CancelHandler;

		private static readonly ObjectPool<List<IEventSystemHandler>> s_HandlerListPool;

		private static readonly List<Transform> s_InternalTransformList;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IPointerEnterHandler> <>f__mg$cache0;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IPointerExitHandler> <>f__mg$cache1;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IPointerDownHandler> <>f__mg$cache2;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IPointerUpHandler> <>f__mg$cache3;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IPointerClickHandler> <>f__mg$cache4;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IInitializePotentialDragHandler> <>f__mg$cache5;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IBeginDragHandler> <>f__mg$cache6;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IDragHandler> <>f__mg$cache7;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IEndDragHandler> <>f__mg$cache8;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IDropHandler> <>f__mg$cache9;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IScrollHandler> <>f__mg$cacheA;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IUpdateSelectedHandler> <>f__mg$cacheB;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<ISelectHandler> <>f__mg$cacheC;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IDeselectHandler> <>f__mg$cacheD;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<IMoveHandler> <>f__mg$cacheE;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<ISubmitHandler> <>f__mg$cacheF;

		[CompilerGenerated]
		private static ExecuteEvents.EventFunction<ICancelHandler> <>f__mg$cache10;

		public static ExecuteEvents.EventFunction<IPointerEnterHandler> pointerEnterHandler
		{
			get
			{
				return ExecuteEvents.s_PointerEnterHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IPointerExitHandler> pointerExitHandler
		{
			get
			{
				return ExecuteEvents.s_PointerExitHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IPointerDownHandler> pointerDownHandler
		{
			get
			{
				return ExecuteEvents.s_PointerDownHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IPointerUpHandler> pointerUpHandler
		{
			get
			{
				return ExecuteEvents.s_PointerUpHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IPointerClickHandler> pointerClickHandler
		{
			get
			{
				return ExecuteEvents.s_PointerClickHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IInitializePotentialDragHandler> initializePotentialDrag
		{
			get
			{
				return ExecuteEvents.s_InitializePotentialDragHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IBeginDragHandler> beginDragHandler
		{
			get
			{
				return ExecuteEvents.s_BeginDragHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IDragHandler> dragHandler
		{
			get
			{
				return ExecuteEvents.s_DragHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IEndDragHandler> endDragHandler
		{
			get
			{
				return ExecuteEvents.s_EndDragHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IDropHandler> dropHandler
		{
			get
			{
				return ExecuteEvents.s_DropHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IScrollHandler> scrollHandler
		{
			get
			{
				return ExecuteEvents.s_ScrollHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IUpdateSelectedHandler> updateSelectedHandler
		{
			get
			{
				return ExecuteEvents.s_UpdateSelectedHandler;
			}
		}

		public static ExecuteEvents.EventFunction<ISelectHandler> selectHandler
		{
			get
			{
				return ExecuteEvents.s_SelectHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IDeselectHandler> deselectHandler
		{
			get
			{
				return ExecuteEvents.s_DeselectHandler;
			}
		}

		public static ExecuteEvents.EventFunction<IMoveHandler> moveHandler
		{
			get
			{
				return ExecuteEvents.s_MoveHandler;
			}
		}

		public static ExecuteEvents.EventFunction<ISubmitHandler> submitHandler
		{
			get
			{
				return ExecuteEvents.s_SubmitHandler;
			}
		}

		public static ExecuteEvents.EventFunction<ICancelHandler> cancelHandler
		{
			get
			{
				return ExecuteEvents.s_CancelHandler;
			}
		}

		public static T ValidateEventData<T>(BaseEventData data) where T : class
		{
			if (!(data is T))
			{
				throw new ArgumentException(string.Format("Invalid type: {0} passed to event expecting {1}", data.GetType(), typeof(T)));
			}
			return data as T;
		}

		private static void Execute(IPointerEnterHandler handler, BaseEventData eventData)
		{
			handler.OnPointerEnter(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IPointerExitHandler handler, BaseEventData eventData)
		{
			handler.OnPointerExit(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IPointerDownHandler handler, BaseEventData eventData)
		{
			handler.OnPointerDown(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IPointerUpHandler handler, BaseEventData eventData)
		{
			handler.OnPointerUp(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IPointerClickHandler handler, BaseEventData eventData)
		{
			handler.OnPointerClick(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IInitializePotentialDragHandler handler, BaseEventData eventData)
		{
			handler.OnInitializePotentialDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IBeginDragHandler handler, BaseEventData eventData)
		{
			handler.OnBeginDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IDragHandler handler, BaseEventData eventData)
		{
			handler.OnDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IEndDragHandler handler, BaseEventData eventData)
		{
			handler.OnEndDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IDropHandler handler, BaseEventData eventData)
		{
			handler.OnDrop(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IScrollHandler handler, BaseEventData eventData)
		{
			handler.OnScroll(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
		}

		private static void Execute(IUpdateSelectedHandler handler, BaseEventData eventData)
		{
			handler.OnUpdateSelected(eventData);
		}

		private static void Execute(ISelectHandler handler, BaseEventData eventData)
		{
			handler.OnSelect(eventData);
		}

		private static void Execute(IDeselectHandler handler, BaseEventData eventData)
		{
			handler.OnDeselect(eventData);
		}

		private static void Execute(IMoveHandler handler, BaseEventData eventData)
		{
			handler.OnMove(ExecuteEvents.ValidateEventData<AxisEventData>(eventData));
		}

		private static void Execute(ISubmitHandler handler, BaseEventData eventData)
		{
			handler.OnSubmit(eventData);
		}

		private static void Execute(ICancelHandler handler, BaseEventData eventData)
		{
			handler.OnCancel(eventData);
		}

		private static void GetEventChain(GameObject root, IList<Transform> eventChain)
		{
			eventChain.Clear();
			if (!(root == null))
			{
				Transform transform = root.transform;
				while (transform != null)
				{
					eventChain.Add(transform);
					transform = transform.parent;
				}
			}
		}

		public static bool Execute<T>(GameObject target, BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
		{
			List<IEventSystemHandler> list = ExecuteEvents.s_HandlerListPool.Get();
			ExecuteEvents.GetEventList<T>(target, list);
			int i = 0;
			while (i < list.Count)
			{
				T handler;
				try
				{
					handler = (T)((object)list[i]);
				}
				catch (Exception innerException)
				{
					IEventSystemHandler eventSystemHandler = list[i];
					Debug.LogException(new Exception(string.Format("Type {0} expected {1} received.", typeof(T).Name, eventSystemHandler.GetType().Name), innerException));
					goto IL_8F;
				}
				goto Block_2;
				IL_8F:
				i++;
				continue;
				Block_2:
				try
				{
					functor(handler, eventData);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				goto IL_8F;
			}
			int count = list.Count;
			ExecuteEvents.s_HandlerListPool.Release(list);
			return count > 0;
		}

		public static GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
		{
			ExecuteEvents.GetEventChain(root, ExecuteEvents.s_InternalTransformList);
			GameObject result;
			for (int i = 0; i < ExecuteEvents.s_InternalTransformList.Count; i++)
			{
				Transform transform = ExecuteEvents.s_InternalTransformList[i];
				if (ExecuteEvents.Execute<T>(transform.gameObject, eventData, callbackFunction))
				{
					result = transform.gameObject;
					return result;
				}
			}
			result = null;
			return result;
		}

		private static bool ShouldSendToComponent<T>(Component component) where T : IEventSystemHandler
		{
			bool result;
			if (!(component is T))
			{
				result = false;
			}
			else
			{
				Behaviour behaviour = component as Behaviour;
				result = (!(behaviour != null) || behaviour.isActiveAndEnabled);
			}
			return result;
		}

		private static void GetEventList<T>(GameObject go, IList<IEventSystemHandler> results) where T : IEventSystemHandler
		{
			if (results == null)
			{
				throw new ArgumentException("Results array is null", "results");
			}
			if (!(go == null) && go.activeInHierarchy)
			{
				List<Component> list = ListPool<Component>.Get();
				go.GetComponents<Component>(list);
				for (int i = 0; i < list.Count; i++)
				{
					if (ExecuteEvents.ShouldSendToComponent<T>(list[i]))
					{
						results.Add(list[i] as IEventSystemHandler);
					}
				}
				ListPool<Component>.Release(list);
			}
		}

		public static bool CanHandleEvent<T>(GameObject go) where T : IEventSystemHandler
		{
			List<IEventSystemHandler> list = ExecuteEvents.s_HandlerListPool.Get();
			ExecuteEvents.GetEventList<T>(go, list);
			int count = list.Count;
			ExecuteEvents.s_HandlerListPool.Release(list);
			return count != 0;
		}

		public static GameObject GetEventHandler<T>(GameObject root) where T : IEventSystemHandler
		{
			GameObject result;
			if (root == null)
			{
				result = null;
			}
			else
			{
				Transform transform = root.transform;
				while (transform != null)
				{
					if (ExecuteEvents.CanHandleEvent<T>(transform.gameObject))
					{
						result = transform.gameObject;
						return result;
					}
					transform = transform.parent;
				}
				result = null;
			}
			return result;
		}

		static ExecuteEvents()
		{
			// Note: this type is marked as 'beforefieldinit'.
			if (ExecuteEvents.<>f__mg$cache0 == null)
			{
				ExecuteEvents.<>f__mg$cache0 = new ExecuteEvents.EventFunction<IPointerEnterHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_PointerEnterHandler = ExecuteEvents.<>f__mg$cache0;
			if (ExecuteEvents.<>f__mg$cache1 == null)
			{
				ExecuteEvents.<>f__mg$cache1 = new ExecuteEvents.EventFunction<IPointerExitHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_PointerExitHandler = ExecuteEvents.<>f__mg$cache1;
			if (ExecuteEvents.<>f__mg$cache2 == null)
			{
				ExecuteEvents.<>f__mg$cache2 = new ExecuteEvents.EventFunction<IPointerDownHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_PointerDownHandler = ExecuteEvents.<>f__mg$cache2;
			if (ExecuteEvents.<>f__mg$cache3 == null)
			{
				ExecuteEvents.<>f__mg$cache3 = new ExecuteEvents.EventFunction<IPointerUpHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_PointerUpHandler = ExecuteEvents.<>f__mg$cache3;
			if (ExecuteEvents.<>f__mg$cache4 == null)
			{
				ExecuteEvents.<>f__mg$cache4 = new ExecuteEvents.EventFunction<IPointerClickHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_PointerClickHandler = ExecuteEvents.<>f__mg$cache4;
			if (ExecuteEvents.<>f__mg$cache5 == null)
			{
				ExecuteEvents.<>f__mg$cache5 = new ExecuteEvents.EventFunction<IInitializePotentialDragHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_InitializePotentialDragHandler = ExecuteEvents.<>f__mg$cache5;
			if (ExecuteEvents.<>f__mg$cache6 == null)
			{
				ExecuteEvents.<>f__mg$cache6 = new ExecuteEvents.EventFunction<IBeginDragHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_BeginDragHandler = ExecuteEvents.<>f__mg$cache6;
			if (ExecuteEvents.<>f__mg$cache7 == null)
			{
				ExecuteEvents.<>f__mg$cache7 = new ExecuteEvents.EventFunction<IDragHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_DragHandler = ExecuteEvents.<>f__mg$cache7;
			if (ExecuteEvents.<>f__mg$cache8 == null)
			{
				ExecuteEvents.<>f__mg$cache8 = new ExecuteEvents.EventFunction<IEndDragHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_EndDragHandler = ExecuteEvents.<>f__mg$cache8;
			if (ExecuteEvents.<>f__mg$cache9 == null)
			{
				ExecuteEvents.<>f__mg$cache9 = new ExecuteEvents.EventFunction<IDropHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_DropHandler = ExecuteEvents.<>f__mg$cache9;
			if (ExecuteEvents.<>f__mg$cacheA == null)
			{
				ExecuteEvents.<>f__mg$cacheA = new ExecuteEvents.EventFunction<IScrollHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_ScrollHandler = ExecuteEvents.<>f__mg$cacheA;
			if (ExecuteEvents.<>f__mg$cacheB == null)
			{
				ExecuteEvents.<>f__mg$cacheB = new ExecuteEvents.EventFunction<IUpdateSelectedHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_UpdateSelectedHandler = ExecuteEvents.<>f__mg$cacheB;
			if (ExecuteEvents.<>f__mg$cacheC == null)
			{
				ExecuteEvents.<>f__mg$cacheC = new ExecuteEvents.EventFunction<ISelectHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_SelectHandler = ExecuteEvents.<>f__mg$cacheC;
			if (ExecuteEvents.<>f__mg$cacheD == null)
			{
				ExecuteEvents.<>f__mg$cacheD = new ExecuteEvents.EventFunction<IDeselectHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_DeselectHandler = ExecuteEvents.<>f__mg$cacheD;
			if (ExecuteEvents.<>f__mg$cacheE == null)
			{
				ExecuteEvents.<>f__mg$cacheE = new ExecuteEvents.EventFunction<IMoveHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_MoveHandler = ExecuteEvents.<>f__mg$cacheE;
			if (ExecuteEvents.<>f__mg$cacheF == null)
			{
				ExecuteEvents.<>f__mg$cacheF = new ExecuteEvents.EventFunction<ISubmitHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_SubmitHandler = ExecuteEvents.<>f__mg$cacheF;
			if (ExecuteEvents.<>f__mg$cache10 == null)
			{
				ExecuteEvents.<>f__mg$cache10 = new ExecuteEvents.EventFunction<ICancelHandler>(ExecuteEvents.Execute);
			}
			ExecuteEvents.s_CancelHandler = ExecuteEvents.<>f__mg$cache10;
			ExecuteEvents.s_HandlerListPool = new ObjectPool<List<IEventSystemHandler>>(null, delegate(List<IEventSystemHandler> l)
			{
				l.Clear();
			});
			ExecuteEvents.s_InternalTransformList = new List<Transform>(30);
		}
	}
}
