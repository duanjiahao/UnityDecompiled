using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class EventDispatcher : IDispatcher
	{
		private VisualElement m_ElementUnderMouse;

		public IEventHandler capture
		{
			get;
			set;
		}

		private VisualElement elementUnderMouse
		{
			get
			{
				return this.m_ElementUnderMouse;
			}
			set
			{
				if (this.m_ElementUnderMouse != value)
				{
					if (this.m_ElementUnderMouse != null)
					{
						this.m_ElementUnderMouse.pseudoStates = (this.m_ElementUnderMouse.pseudoStates & ~PseudoStates.Hover);
					}
					this.m_ElementUnderMouse = value;
					if (this.m_ElementUnderMouse != null)
					{
						this.m_ElementUnderMouse.pseudoStates = (this.m_ElementUnderMouse.pseudoStates | PseudoStates.Hover);
					}
				}
			}
		}

		public void ReleaseCapture(IEventHandler handler)
		{
			Debug.Assert(handler == this.capture, "Element releasing capture does not have capture");
			this.capture = null;
		}

		public void RemoveCapture()
		{
			if (this.capture != null)
			{
				this.capture.OnLostCapture();
			}
			this.capture = null;
		}

		public void TakeCapture(IEventHandler handler)
		{
			if (this.capture != handler)
			{
				if (GUIUtility.hotControl != 0)
				{
					Debug.Log("Should not be capturing when there is a hotcontrol");
				}
				else
				{
					this.RemoveCapture();
					this.capture = handler;
				}
			}
		}

		public EventPropagation DispatchEvent(Event e, BaseVisualElementPanel panel)
		{
			EventPropagation result;
			if (e.type == EventType.Repaint)
			{
				Debug.Log("Repaint should be handled by Panel before Dispatcher");
				result = EventPropagation.Continue;
			}
			else
			{
				bool flag = false;
				if (panel.panelDebug != null && panel.panelDebug.enabled && panel.panelDebug.interceptEvents != null && panel.panelDebug.interceptEvents(e))
				{
					result = EventPropagation.Stop;
				}
				else
				{
					if (this.capture != null && this.capture.panel == null)
					{
						Debug.Log(string.Format("Capture has no panel, forcing removal (capture={0} eventType={1})", this.capture, e.type));
						this.RemoveCapture();
					}
					if (this.capture != null)
					{
						if (this.capture.panel.contextType != panel.contextType)
						{
							result = EventPropagation.Continue;
							return result;
						}
						VisualElement visualElement = this.capture as VisualElement;
						if (visualElement != null)
						{
							e.mousePosition = visualElement.GlobalToBound(e.mousePosition);
						}
						else
						{
							IManipulator manipulator = this.capture as IManipulator;
							if (manipulator != null)
							{
								e.mousePosition = manipulator.target.GlobalToBound(e.mousePosition);
							}
						}
						flag = true;
						if (this.capture.HandleEvent(e, this.capture as VisualElement) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					if (e.isKey)
					{
						flag = true;
						if (panel.focusedElement != null)
						{
							if (this.PropagateEvent(panel.focusedElement, e) == EventPropagation.Stop)
							{
								result = EventPropagation.Stop;
								return result;
							}
						}
						else if (this.SendEventToIMGUIContainers(panel.visualTree, e, panel.focusedElement) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					else if (e.isMouse || e.isScrollWheel || e.type == EventType.DragUpdated || e.type == EventType.DragPerform || e.type == EventType.DragExited)
					{
						if (e.type == EventType.MouseLeaveWindow)
						{
							this.elementUnderMouse = null;
						}
						else
						{
							this.elementUnderMouse = panel.Pick(e.mousePosition);
						}
						if (e.type == EventType.MouseDown && this.elementUnderMouse != null && this.elementUnderMouse.enabled)
						{
							this.SetFocusedElement(panel, this.elementUnderMouse);
						}
						if (this.elementUnderMouse != null)
						{
							flag = true;
							if (this.PropagateEvent(this.elementUnderMouse, e) == EventPropagation.Stop)
							{
								result = EventPropagation.Stop;
								return result;
							}
						}
						if (e.type == EventType.MouseEnterWindow || e.type == EventType.MouseLeaveWindow)
						{
							flag = true;
							if (this.SendEventToIMGUIContainers(panel.visualTree, e, null) == EventPropagation.Stop)
							{
								result = EventPropagation.Stop;
								return result;
							}
						}
					}
					if (e.type == EventType.ExecuteCommand || e.type == EventType.ValidateCommand)
					{
						flag = true;
						if (panel.focusedElement != null && this.PropagateEvent(panel.focusedElement, e) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
						if (this.SendEventToIMGUIContainers(panel.visualTree, e, panel.focusedElement) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					if (e.type == EventType.Used)
					{
						flag = true;
						if (this.SendEventToIMGUIContainers(panel.visualTree, e, null) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					if (!flag)
					{
						if (this.SendEventToIMGUIContainers(panel.visualTree, e, null) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					result = EventPropagation.Continue;
				}
			}
			return result;
		}

		private EventPropagation SendEventToIMGUIContainers(VisualElement root, Event evt, VisualElement skipElement)
		{
			IMGUIContainer iMGUIContainer = root as IMGUIContainer;
			EventPropagation result;
			if (iMGUIContainer != null && iMGUIContainer != skipElement)
			{
				if (iMGUIContainer.HandleEvent(evt, iMGUIContainer) == EventPropagation.Stop)
				{
					result = EventPropagation.Stop;
				}
				else
				{
					result = EventPropagation.Continue;
				}
			}
			else
			{
				VisualContainer visualContainer = root as VisualContainer;
				if (visualContainer != null)
				{
					for (int i = 0; i < visualContainer.childrenCount; i++)
					{
						if (this.SendEventToIMGUIContainers(visualContainer.GetChildAt(i), evt, skipElement) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
				}
				result = EventPropagation.Continue;
			}
			return result;
		}

		private EventPropagation PropagateEvent(VisualElement target, Event evt)
		{
			List<VisualElement> list = this.BuildPropagationPath(target);
			Vector2 mousePosition = evt.mousePosition;
			EventPropagation result;
			for (int i = 0; i < list.Count; i++)
			{
				VisualElement visualElement = list[i];
				if (visualElement.enabled)
				{
					evt.mousePosition = visualElement.GlobalToBound(mousePosition);
					List<IManipulator>.Enumerator manipulatorsInternal = visualElement.GetManipulatorsInternal();
					while (manipulatorsInternal.MoveNext())
					{
						IManipulator current = manipulatorsInternal.Current;
						if (current.phaseInterest == EventPhase.Capture && current.HandleEvent(evt, target) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
					if (visualElement.phaseInterest != EventPhase.Capture || visualElement.HandleEvent(evt, target) != EventPropagation.Stop)
					{
						goto IL_A2;
					}
					result = EventPropagation.Stop;
					return result;
				}
				IL_A2:;
			}
			if (target.enabled)
			{
				evt.mousePosition = target.GlobalToBound(mousePosition);
				List<IManipulator>.Enumerator manipulatorsInternal2 = target.GetManipulatorsInternal();
				while (manipulatorsInternal2.MoveNext())
				{
					IManipulator current2 = manipulatorsInternal2.Current;
					if (current2.phaseInterest == EventPhase.Capture && current2.HandleEvent(evt, target) == EventPropagation.Stop)
					{
						result = EventPropagation.Stop;
						return result;
					}
				}
				if (target.HandleEvent(evt, target) == EventPropagation.Stop)
				{
					result = EventPropagation.Stop;
					return result;
				}
				manipulatorsInternal2 = target.GetManipulatorsInternal();
				while (manipulatorsInternal2.MoveNext())
				{
					IManipulator current3 = manipulatorsInternal2.Current;
					if (current3.phaseInterest == EventPhase.BubbleUp && current3.HandleEvent(evt, target) == EventPropagation.Stop)
					{
						result = EventPropagation.Stop;
						return result;
					}
				}
			}
			for (int j = list.Count - 1; j >= 0; j--)
			{
				VisualElement visualElement2 = list[j];
				if (visualElement2.enabled)
				{
					evt.mousePosition = visualElement2.GlobalToBound(mousePosition);
					if (visualElement2.phaseInterest == EventPhase.BubbleUp && visualElement2.HandleEvent(evt, target) == EventPropagation.Stop)
					{
						result = EventPropagation.Stop;
						return result;
					}
					List<IManipulator>.Enumerator manipulatorsInternal3 = visualElement2.GetManipulatorsInternal();
					while (manipulatorsInternal3.MoveNext())
					{
						IManipulator current4 = manipulatorsInternal3.Current;
						if (current4.phaseInterest == EventPhase.BubbleUp && current4.HandleEvent(evt, target) == EventPropagation.Stop)
						{
							result = EventPropagation.Stop;
							return result;
						}
					}
				}
			}
			result = EventPropagation.Continue;
			return result;
		}

		private void SetFocusedElement(BaseVisualElementPanel panel, VisualElement element)
		{
			if (panel.focusedElement != element)
			{
				if (panel.focusedElement != null)
				{
					panel.focusedElement.pseudoStates = (panel.focusedElement.pseudoStates & ~PseudoStates.Focus);
					panel.focusedElement.OnLostKeyboardFocus();
				}
				panel.focusedElement = element;
				if (element != null)
				{
					element.pseudoStates |= PseudoStates.Focus;
				}
			}
		}

		private List<VisualElement> BuildPropagationPath(VisualElement elem)
		{
			List<VisualElement> list = new List<VisualElement>();
			List<VisualElement> result;
			if (elem == null)
			{
				result = list;
			}
			else
			{
				while (elem.parent != null)
				{
					list.Insert(0, elem.parent);
					elem = elem.parent;
				}
				result = list;
			}
			return result;
		}
	}
}
