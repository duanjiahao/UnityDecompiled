using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class DragAndDrop
	{
		private static Hashtable ms_GenericData;

		public static extern UnityEngine.Object[] objectReferences
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string[] paths
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern DragAndDropVisualMode visualMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int activeControlID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static bool HandleDelayedDrag(Rect position, int id, UnityEngine.Object objectToDrag)
		{
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(id);
			bool result;
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl == EventType.MouseDrag)
				{
					if (GUIUtility.hotControl == id)
					{
						DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
						if (dragAndDropDelay.CanStartDrag())
						{
							GUIUtility.hotControl = 0;
							DragAndDrop.PrepareStartDrag();
							UnityEngine.Object[] objectReferences = new UnityEngine.Object[]
							{
								objectToDrag
							};
							DragAndDrop.objectReferences = objectReferences;
							DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(objectToDrag));
							result = true;
							return result;
						}
					}
				}
			}
			else if (position.Contains(current.mousePosition) && current.clickCount == 1)
			{
				if (current.button == 0 && (Application.platform != RuntimePlatform.OSXEditor || !current.control))
				{
					GUIUtility.hotControl = id;
					DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), id);
					dragAndDropDelay2.mouseDownPosition = current.mousePosition;
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static void PrepareStartDrag()
		{
			DragAndDrop.ms_GenericData = null;
			DragAndDrop.PrepareStartDrag_Internal();
		}

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartDrag_Internal(string title);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AcceptDrag();

		[RequiredByNativeCode]
		private static bool HasGenericDragData()
		{
			return DragAndDrop.ms_GenericData != null;
		}

		public static object GetGenericData(string type)
		{
			object result;
			if (DragAndDrop.ms_GenericData != null && DragAndDrop.ms_GenericData.Contains(type))
			{
				result = DragAndDrop.ms_GenericData[type];
			}
			else
			{
				result = null;
			}
			return result;
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
