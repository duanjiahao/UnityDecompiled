using System;

namespace UnityEngine.Experimental.UIElements
{
	public class IMGUIContainer : VisualElement
	{
		private struct GUIGlobals
		{
			public Matrix4x4 matrix;

			public Color color;

			public Color contentColor;

			public Color backgroundColor;

			public bool enabled;

			public bool changed;

			public int displayIndex;
		}

		private readonly Action m_OnGUIHandler;

		private GUILayoutUtility.LayoutCache m_Cache = null;

		private IMGUIContainer.GUIGlobals m_GUIGlobals;

		public int executionContext
		{
			get;
			set;
		}

		internal Rect lastWorldClip
		{
			get;
			set;
		}

		private GUILayoutUtility.LayoutCache cache
		{
			get
			{
				if (this.m_Cache == null)
				{
					this.m_Cache = new GUILayoutUtility.LayoutCache();
				}
				return this.m_Cache;
			}
		}

		public ContextType contextType
		{
			get;
			set;
		}

		internal int GUIDepth
		{
			get;
			private set;
		}

		public IMGUIContainer(Action onGUIHandler)
		{
			this.m_OnGUIHandler = onGUIHandler;
			this.contextType = ContextType.Editor;
		}

		internal override void DoRepaint(IStylePainter painter)
		{
			base.DoRepaint(painter);
			this.lastWorldClip = painter.currentWorldClip;
			this.HandleEvent(painter.repaintEvent, this);
		}

		internal override void ChangePanel(BaseVisualElementPanel p)
		{
			if (base.elementPanel != null)
			{
				base.elementPanel.IMGUIContainersCount--;
			}
			base.ChangePanel(p);
			if (base.elementPanel != null)
			{
				base.elementPanel.IMGUIContainersCount++;
			}
		}

		private void SaveGlobals()
		{
			this.m_GUIGlobals.matrix = GUI.matrix;
			this.m_GUIGlobals.color = GUI.color;
			this.m_GUIGlobals.contentColor = GUI.contentColor;
			this.m_GUIGlobals.backgroundColor = GUI.backgroundColor;
			this.m_GUIGlobals.enabled = GUI.enabled;
			this.m_GUIGlobals.changed = GUI.changed;
			this.m_GUIGlobals.displayIndex = Event.current.displayIndex;
		}

		private void RestoreGlobals()
		{
			GUI.matrix = this.m_GUIGlobals.matrix;
			GUI.color = this.m_GUIGlobals.color;
			GUI.contentColor = this.m_GUIGlobals.contentColor;
			GUI.backgroundColor = this.m_GUIGlobals.backgroundColor;
			GUI.enabled = this.m_GUIGlobals.enabled;
			GUI.changed = this.m_GUIGlobals.changed;
			Event.current.displayIndex = this.m_GUIGlobals.displayIndex;
		}

		private bool DoOnGUI(Event evt)
		{
			bool result;
			if (this.m_OnGUIHandler == null || base.panel == null)
			{
				result = false;
			}
			else
			{
				int num = GUIClip.Internal_GetCount();
				this.SaveGlobals();
				int instanceID = (this.executionContext == 0) ? base.elementPanel.instanceID : this.executionContext;
				UIElementsUtility.BeginContainerGUI(this.cache, instanceID, evt, this);
				this.GUIDepth = GUIUtility.Internal_GetGUIDepth();
				EventType type = Event.current.type;
				bool flag = false;
				try
				{
					this.m_OnGUIHandler();
				}
				catch (Exception exception)
				{
					if (type != EventType.Layout)
					{
						throw;
					}
					flag = GUIUtility.IsExitGUIException(exception);
					if (!flag)
					{
						Debug.LogException(exception);
					}
				}
				GUIUtility.CheckForTabEvent(evt);
				EventType type2 = Event.current.type;
				UIElementsUtility.EndContainerGUI();
				this.RestoreGlobals();
				if (!flag)
				{
					if (type2 != EventType.Ignore && type2 != EventType.Used)
					{
						int num2 = GUIClip.Internal_GetCount();
						if (num2 > num)
						{
							Debug.LogError("GUI Error: You are pushing more GUIClips than you are popping. Make sure they are balanced)");
						}
						else if (num2 < num)
						{
							Debug.LogError("GUI Error: You are popping more GUIClips than you are pushing. Make sure they are balanced)");
						}
					}
				}
				while (GUIClip.Internal_GetCount() > num)
				{
					GUIClip.Internal_Pop();
				}
				if (type2 == EventType.Used)
				{
					base.Dirty(ChangeType.Repaint);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public override void OnLostKeyboardFocus()
		{
			GUIUtility.keyboardControl = 0;
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
		{
			EventPropagation result;
			if (this.m_OnGUIHandler == null || base.elementPanel == null || !base.elementPanel.IMGUIEventInterests.WantsEvent(evt.type))
			{
				result = EventPropagation.Continue;
			}
			else
			{
				EventType type = evt.type;
				evt.type = EventType.Layout;
				bool flag = this.DoOnGUI(evt);
				evt.type = type;
				flag |= this.DoOnGUI(evt);
				if (flag)
				{
					result = EventPropagation.Stop;
				}
				else
				{
					if (evt.type == EventType.MouseUp && this.HasCapture())
					{
						GUIUtility.hotControl = 0;
					}
					if (base.elementPanel == null)
					{
						GUIUtility.ExitGUI();
					}
					result = EventPropagation.Continue;
				}
			}
			return result;
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			if (widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly)
			{
				this.DoOnGUI(new Event
				{
					type = EventType.Layout
				});
				num = this.m_Cache.topLevel.minWidth;
				num2 = this.m_Cache.topLevel.minHeight;
			}
			if (widthMode != VisualElement.MeasureMode.Exactly)
			{
				if (widthMode == VisualElement.MeasureMode.AtMost)
				{
					num = Mathf.Min(num, desiredWidth);
				}
			}
			else
			{
				num = desiredWidth;
			}
			if (heightMode != VisualElement.MeasureMode.Exactly)
			{
				if (heightMode == VisualElement.MeasureMode.AtMost)
				{
					num2 = Mathf.Min(num2, desiredHeight);
				}
			}
			else
			{
				num2 = desiredHeight;
			}
			return new Vector2(num, num2);
		}
	}
}
