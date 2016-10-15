using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class DragAndDrop
	{
		private static Hashtable ms_GenericData;

		public static extern UnityEngine.Object[] objectReferences
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string[] paths
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern DragAndDropVisualMode visualMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int activeControlID
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static bool HandleDelayedDrag(Rect position, int id, UnityEngine.Object objectToDrag)
		{
			Event current = Event.current;
			switch (current.GetTypeForControl(id))
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition) && current.clickCount == 1 && current.button == 0 && (Application.platform != RuntimePlatform.OSXEditor || !current.control))
				{
					GUIUtility.hotControl = id;
					DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
					dragAndDropDelay.mouseDownPosition = current.mousePosition;
					return true;
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == id)
				{
					DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
					if (dragAndDropDelay2.CanStartDrag())
					{
						GUIUtility.hotControl = 0;
						DragAndDrop.PrepareStartDrag();
						UnityEngine.Object[] objectReferences = new UnityEngine.Object[]
						{
							objectToDrag
						};
						DragAndDrop.objectReferences = objectReferences;
						DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(objectToDrag));
						return true;
					}
				}
				break;
			}
			return false;
		}

		public static void PrepareStartDrag()
		{
			DragAndDrop.ms_GenericData = null;
			DragAndDrop.PrepareStartDrag_Internal();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PrepareStartDrag_Internal();

		public static void StartDrag(string title)
		{
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
			{
				DragAndDrop.StartDrag_Internal(title);
			}
			else
			{
				Debug.LogError("Drags can only be started from MouseDown or MouseDrag events");
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartDrag_Internal(string title);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AcceptDrag();

		public static object GetGenericData(string type)
		{
			if (DragAndDrop.ms_GenericData != null && DragAndDrop.ms_GenericData.Contains(type))
			{
				return DragAndDrop.ms_GenericData[type];
			}
			return null;
		}

		public static void SetGenericData(string type, object data)
		{
			if (DragAndDrop.ms_GenericData == null)
			{
				DragAndDrop.ms_GenericData = new Hashtable();
			}
			DragAndDrop.ms_GenericData[type] = data;
		}
	}
}
