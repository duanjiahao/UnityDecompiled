using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Experimental.UIElements
{
	internal class UIElementsUtility
	{
		private static Stack<IMGUIContainer> s_ContainerStack;

		private static Dictionary<int, Panel> s_UIElementsCache;

		private static Event s_EventInstance;

		private static EventDispatcher s_EventDispatcher;

		internal static Action<IMGUIContainer> s_BeginContainerCallback;

		internal static Action<IMGUIContainer> s_EndContainerCallback;

		[CompilerGenerated]
		private static Action <>f__mg$cache0;

		[CompilerGenerated]
		private static Action <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<int, IntPtr, bool> <>f__mg$cache2;

		[CompilerGenerated]
		private static Action <>f__mg$cache3;

		[CompilerGenerated]
		private static Func<Exception, bool> <>f__mg$cache4;

		internal static IDispatcher eventDispatcher
		{
			get
			{
				if (UIElementsUtility.s_EventDispatcher == null)
				{
					UIElementsUtility.s_EventDispatcher = new EventDispatcher();
				}
				return UIElementsUtility.s_EventDispatcher;
			}
		}

		static UIElementsUtility()
		{
			UIElementsUtility.s_ContainerStack = new Stack<IMGUIContainer>();
			UIElementsUtility.s_UIElementsCache = new Dictionary<int, Panel>();
			UIElementsUtility.s_EventInstance = new Event();
			Delegate arg_41_0 = GUIUtility.takeCapture;
			if (UIElementsUtility.<>f__mg$cache0 == null)
			{
				UIElementsUtility.<>f__mg$cache0 = new Action(UIElementsUtility.TakeCapture);
			}
			GUIUtility.takeCapture = (Action)Delegate.Combine(arg_41_0, UIElementsUtility.<>f__mg$cache0);
			Delegate arg_72_0 = GUIUtility.releaseCapture;
			if (UIElementsUtility.<>f__mg$cache1 == null)
			{
				UIElementsUtility.<>f__mg$cache1 = new Action(UIElementsUtility.ReleaseCapture);
			}
			GUIUtility.releaseCapture = (Action)Delegate.Combine(arg_72_0, UIElementsUtility.<>f__mg$cache1);
			Delegate arg_A3_0 = GUIUtility.processEvent;
			if (UIElementsUtility.<>f__mg$cache2 == null)
			{
				UIElementsUtility.<>f__mg$cache2 = new Func<int, IntPtr, bool>(UIElementsUtility.ProcessEvent);
			}
			GUIUtility.processEvent = (Func<int, IntPtr, bool>)Delegate.Combine(arg_A3_0, UIElementsUtility.<>f__mg$cache2);
			Delegate arg_D4_0 = GUIUtility.cleanupRoots;
			if (UIElementsUtility.<>f__mg$cache3 == null)
			{
				UIElementsUtility.<>f__mg$cache3 = new Action(UIElementsUtility.CleanupRoots);
			}
			GUIUtility.cleanupRoots = (Action)Delegate.Combine(arg_D4_0, UIElementsUtility.<>f__mg$cache3);
			Delegate arg_105_0 = GUIUtility.endContainerGUIFromException;
			if (UIElementsUtility.<>f__mg$cache4 == null)
			{
				UIElementsUtility.<>f__mg$cache4 = new Func<Exception, bool>(UIElementsUtility.EndContainerGUIFromException);
			}
			GUIUtility.endContainerGUIFromException = (Func<Exception, bool>)Delegate.Combine(arg_105_0, UIElementsUtility.<>f__mg$cache4);
		}

		private static void TakeCapture()
		{
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				IMGUIContainer iMGUIContainer = UIElementsUtility.s_ContainerStack.Peek();
				if (iMGUIContainer.GUIDepth == GUIUtility.Internal_GetGUIDepth())
				{
					if (UIElementsUtility.eventDispatcher.capture != null && UIElementsUtility.eventDispatcher.capture != iMGUIContainer)
					{
						Debug.Log(string.Format("Should not grab hot control with an active capture (current={0} new={1}", UIElementsUtility.eventDispatcher.capture, iMGUIContainer));
					}
					UIElementsUtility.eventDispatcher.TakeCapture(iMGUIContainer);
				}
			}
		}

		private static void ReleaseCapture()
		{
			UIElementsUtility.eventDispatcher.RemoveCapture();
		}

		private static bool ProcessEvent(int instanceID, IntPtr nativeEventPtr)
		{
			Panel panel;
			bool result;
			if (nativeEventPtr != IntPtr.Zero && UIElementsUtility.s_UIElementsCache.TryGetValue(instanceID, out panel))
			{
				UIElementsUtility.s_EventInstance.CopyFromPtr(nativeEventPtr);
				result = UIElementsUtility.DoDispatch(panel);
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static void CleanupRoots()
		{
			UIElementsUtility.s_EventInstance = null;
			UIElementsUtility.s_EventDispatcher = null;
			UIElementsUtility.s_UIElementsCache = null;
			UIElementsUtility.s_ContainerStack = null;
			UIElementsUtility.s_BeginContainerCallback = null;
			UIElementsUtility.s_EndContainerCallback = null;
		}

		private static bool EndContainerGUIFromException(Exception exception)
		{
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
			return GUIUtility.ShouldRethrowException(exception);
		}

		internal static void BeginContainerGUI(GUILayoutUtility.LayoutCache cache, int instanceID, Event evt, IMGUIContainer container)
		{
			GUIUtility.BeginContainer(instanceID);
			UIElementsUtility.s_ContainerStack.Push(container);
			GUIUtility.s_SkinMode = (int)container.contextType;
			GUIUtility.s_OriginalID = instanceID;
			Event.current = evt;
			if (UIElementsUtility.s_BeginContainerCallback != null)
			{
				UIElementsUtility.s_BeginContainerCallback(container);
			}
			GUILayoutUtility.BeginContainer(cache);
			GUIUtility.ResetGlobalState();
			Rect clipRect = container.lastWorldClip;
			if (clipRect.width == 0f || clipRect.height == 0f)
			{
				clipRect = container.globalBound;
			}
			Matrix4x4 rhs = Matrix4x4.TRS(new Vector3(container.position.x, container.position.y, 0f), Quaternion.identity, Vector3.one);
			GUIClip.SetTransform(container.globalTransform * rhs, clipRect);
		}

		internal static void EndContainerGUI()
		{
			if (Event.current.type == EventType.Layout && UIElementsUtility.s_ContainerStack.Count > 0)
			{
				Rect globalBound = UIElementsUtility.s_ContainerStack.Peek().globalBound;
				GUILayoutUtility.LayoutFromContainer(globalBound.width, globalBound.height);
			}
			GUILayoutUtility.SelectIDList(GUIUtility.s_OriginalID, false);
			GUIContent.ClearStaticCache();
			if (UIElementsUtility.s_ContainerStack.Count > 0)
			{
				IMGUIContainer obj = UIElementsUtility.s_ContainerStack.Peek();
				if (UIElementsUtility.s_EndContainerCallback != null)
				{
					UIElementsUtility.s_EndContainerCallback(obj);
				}
				GUIUtility.EndContainer();
				UIElementsUtility.s_ContainerStack.Pop();
			}
		}

		internal static ContextType GetGUIContextType()
		{
			return (GUIUtility.s_SkinMode != 0) ? ContextType.Editor : ContextType.Player;
		}

		private static bool DoDispatch(BaseVisualElementPanel panel)
		{
			bool result;
			if (UIElementsUtility.s_EventInstance.type == EventType.Repaint)
			{
				panel.Repaint(UIElementsUtility.s_EventInstance);
				result = (panel.IMGUIContainersCount > 0);
			}
			else
			{
				panel.ValidateLayout();
				EventPropagation eventPropagation = UIElementsUtility.s_EventDispatcher.DispatchEvent(UIElementsUtility.s_EventInstance, panel);
				if (eventPropagation == EventPropagation.Stop)
				{
					panel.visualTree.Dirty(ChangeType.Repaint);
				}
				result = (eventPropagation == EventPropagation.Stop);
			}
			return result;
		}

		internal static Dictionary<int, Panel>.Enumerator GetPanelsIterator()
		{
			return UIElementsUtility.s_UIElementsCache.GetEnumerator();
		}

		internal static Panel FindOrCreatePanel(int instanceId, ContextType contextType, IDataWatchService dataWatch = null, LoadResourceFunction loadResourceFunction = null)
		{
			Panel panel;
			if (!UIElementsUtility.s_UIElementsCache.TryGetValue(instanceId, out panel))
			{
				panel = new Panel(instanceId, contextType, loadResourceFunction, dataWatch, UIElementsUtility.eventDispatcher);
				UIElementsUtility.s_UIElementsCache.Add(instanceId, panel);
			}
			else
			{
				Debug.Assert(contextType == panel.contextType, "Context type mismatch");
			}
			return panel;
		}

		internal static Panel FindOrCreatePanel(int instanceId)
		{
			return UIElementsUtility.FindOrCreatePanel(instanceId, UIElementsUtility.GetGUIContextType(), null, null);
		}

		internal static void BeginBuilder(VisualContainer w)
		{
		}
	}
}
