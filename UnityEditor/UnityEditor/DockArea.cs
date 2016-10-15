using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class DockArea : HostView, IDropArea
	{
		internal const float kTabHeight = 17f;

		internal const float kDockHeight = 39f;

		private const float kSideBorders = 2f;

		private const float kBottomBorders = 2f;

		private const float kWindowButtonsWidth = 40f;

		private static int s_PlaceholderPos;

		private static EditorWindow s_DragPane;

		internal static DockArea s_OriginalDragSource;

		private static Vector2 s_StartDragPosition;

		private static int s_DragMode;

		internal static View s_IgnoreDockingForView;

		private static DropInfo s_DropInfo;

		[SerializeField]
		internal List<EditorWindow> m_Panes = new List<EditorWindow>();

		[SerializeField]
		internal int m_Selected;

		[SerializeField]
		internal int m_LastSelected;

		[NonSerialized]
		internal GUIStyle tabStyle;

		private bool m_IsBeingDestroyed;

		public int selected
		{
			get
			{
				return this.m_Selected;
			}
			set
			{
				if (this.m_Selected == value)
				{
					return;
				}
				this.m_Selected = value;
				if (this.m_Selected >= 0 && this.m_Selected < this.m_Panes.Count)
				{
					base.actualView = this.m_Panes[this.m_Selected];
				}
			}
		}

		private Rect tabRect
		{
			get
			{
				return new Rect(0f, 0f, base.position.width, 17f);
			}
		}

		public DockArea()
		{
			if (this.m_Panes != null && this.m_Panes.Count != 0)
			{
				Debug.LogError("m_Panes is filled in DockArea constructor.");
			}
		}

		private void RemoveNullWindows()
		{
			List<EditorWindow> list = new List<EditorWindow>();
			foreach (EditorWindow current in this.m_Panes)
			{
				if (current != null)
				{
					list.Add(current);
				}
			}
			this.m_Panes = list;
		}

		public new void OnDestroy()
		{
			this.m_IsBeingDestroyed = true;
			if (base.hasFocus)
			{
				base.Invoke("OnLostFocus");
			}
			base.actualView = null;
			foreach (EditorWindow current in this.m_Panes)
			{
				UnityEngine.Object.DestroyImmediate(current, true);
			}
			base.OnDestroy();
		}

		public new void OnEnable()
		{
			if (this.m_Panes != null && this.m_Panes.Count > this.m_Selected)
			{
				base.actualView = this.m_Panes[this.m_Selected];
			}
			base.OnEnable();
		}

		public void AddTab(EditorWindow pane)
		{
			this.AddTab(this.m_Panes.Count, pane);
		}

		public void AddTab(int idx, EditorWindow pane)
		{
			base.DeregisterSelectedPane(true);
			this.m_Panes.Insert(idx, pane);
			this.m_ActualView = pane;
			this.m_Panes[idx] = pane;
			this.m_Selected = idx;
			base.RegisterSelectedPane();
			SplitView splitView = base.parent as SplitView;
			if (splitView)
			{
				splitView.Reflow();
			}
			base.Repaint();
		}

		public void RemoveTab(EditorWindow pane)
		{
			this.RemoveTab(pane, true);
		}

		public void RemoveTab(EditorWindow pane, bool killIfEmpty)
		{
			if (this.m_ActualView == pane)
			{
				base.DeregisterSelectedPane(true);
			}
			int num = this.m_Panes.IndexOf(pane);
			if (num == -1)
			{
				return;
			}
			this.m_Panes.Remove(pane);
			if (num == this.m_Selected)
			{
				if (this.m_LastSelected >= this.m_Panes.Count - 1)
				{
					this.m_LastSelected = this.m_Panes.Count - 1;
				}
				this.m_Selected = this.m_LastSelected;
				if (this.m_Selected > -1)
				{
					this.m_ActualView = this.m_Panes[this.m_Selected];
				}
			}
			else if (num < this.m_Selected)
			{
				this.m_Selected--;
			}
			base.Repaint();
			pane.m_Parent = null;
			if (killIfEmpty)
			{
				this.KillIfEmpty();
			}
			base.RegisterSelectedPane();
		}

		private void KillIfEmpty()
		{
			if (this.m_Panes.Count != 0)
			{
				return;
			}
			if (base.parent == null)
			{
				base.window.InternalCloseWindow();
				return;
			}
			SplitView splitView = base.parent as SplitView;
			ICleanuppable cleanuppable = base.parent as ICleanuppable;
			splitView.RemoveChildNice(this);
			if (!this.m_IsBeingDestroyed)
			{
				UnityEngine.Object.DestroyImmediate(this, true);
			}
			if (cleanuppable != null)
			{
				cleanuppable.Cleanup();
			}
		}

		public DropInfo DragOver(EditorWindow window, Vector2 mouseScreenPosition)
		{
			Rect screenPosition = base.screenPosition;
			screenPosition.height = 39f;
			if (screenPosition.Contains(mouseScreenPosition))
			{
				if (this.background == null)
				{
					this.background = "hostview";
				}
				Rect rect = this.background.margin.Remove(base.screenPosition);
				Vector2 mousePos = mouseScreenPosition - new Vector2(rect.x, rect.y);
				Rect tabRect = this.tabRect;
				int tabAtMousePos = this.GetTabAtMousePos(mousePos, tabRect);
				float tabWidth = this.GetTabWidth(tabRect.width);
				if (DockArea.s_PlaceholderPos != tabAtMousePos)
				{
					base.Repaint();
					DockArea.s_PlaceholderPos = tabAtMousePos;
				}
				return new DropInfo(this)
				{
					type = DropInfo.Type.Tab,
					rect = new Rect(mousePos.x - tabWidth * 0.25f + rect.x, tabRect.y + rect.y, tabWidth, tabRect.height)
				};
			}
			return null;
		}

		public bool PerformDrop(EditorWindow w, DropInfo info, Vector2 screenPos)
		{
			DockArea.s_OriginalDragSource.RemoveTab(w, DockArea.s_OriginalDragSource != this);
			int num = (DockArea.s_PlaceholderPos <= this.m_Panes.Count) ? DockArea.s_PlaceholderPos : this.m_Panes.Count;
			this.AddTab(num, w);
			this.selected = num;
			return true;
		}

		public void OnGUI()
		{
			base.ClearBackground();
			EditorGUIUtility.ResetGUIState();
			SplitView splitView = base.parent as SplitView;
			if (Event.current.type == EventType.Repaint && splitView)
			{
				View child = this;
				while (splitView)
				{
					int controlID = splitView.controlID;
					if (controlID == GUIUtility.hotControl || GUIUtility.hotControl == 0)
					{
						int num = splitView.IndexOfChild(child);
						if (splitView.vertical)
						{
							if (num != 0)
							{
								EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
							}
							if (num != splitView.children.Length - 1)
							{
								EditorGUIUtility.AddCursorRect(new Rect(0f, base.position.height - 5f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
							}
						}
						else
						{
							if (num != 0)
							{
								EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
							}
							if (num != splitView.children.Length - 1)
							{
								EditorGUIUtility.AddCursorRect(new Rect(base.position.width - 5f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
							}
						}
					}
					child = splitView;
					splitView = (splitView.parent as SplitView);
				}
				splitView = (base.parent as SplitView);
			}
			bool flag = false;
			if (base.window.mainView.GetType() != typeof(MainWindow))
			{
				flag = true;
				if (base.windowPosition.y == 0f)
				{
					this.background = "dockareaStandalone";
				}
				else
				{
					this.background = "dockarea";
				}
			}
			else
			{
				this.background = "dockarea";
			}
			if (splitView)
			{
				Event @event = new Event(Event.current);
				@event.mousePosition += new Vector2(base.position.x, base.position.y);
				splitView.SplitGUI(@event);
				if (@event.type == EventType.Used)
				{
					Event.current.Use();
				}
			}
			GUIStyle style = "dockareaoverlay";
			Rect position = this.background.margin.Remove(new Rect(0f, 0f, base.position.width, base.position.height));
			position.x = (float)this.background.margin.left;
			position.y = (float)this.background.margin.top;
			Rect windowPosition = base.windowPosition;
			float num2 = 2f;
			if (windowPosition.x == 0f)
			{
				position.x -= num2;
				position.width += num2;
			}
			if (windowPosition.xMax == base.window.position.width)
			{
				position.width += num2;
			}
			if (windowPosition.yMax == base.window.position.height)
			{
				position.height += ((!flag) ? 2f : 2f);
			}
			GUI.Box(position, GUIContent.none, this.background);
			if (this.tabStyle == null)
			{
				this.tabStyle = "dragtab";
			}
			if (this.m_Panes.Count > 0)
			{
				DockArea.BeginOffsetArea(new Rect(position.x + 2f, position.y + 17f, position.width - 4f, position.height - 17f - 2f), GUIContent.none, "TabWindowBackground");
				Vector2 vector = GUIUtility.GUIToScreenPoint(Vector2.zero);
				Rect pos = base.borderSize.Remove(base.position);
				pos.x = vector.x;
				pos.y = vector.y;
				this.m_Panes[this.selected].m_Pos = pos;
				DockArea.EndOffsetArea();
			}
			this.DragTab(new Rect(position.x + 1f, position.y, position.width - 40f, 17f), this.tabStyle);
			this.tabStyle = "dragtab";
			base.ShowGenericMenu();
			base.DoWindowDecorationStart();
			if (this.m_Panes.Count > 0)
			{
				if (this.m_Panes[this.selected] is GameView)
				{
					GUI.Box(position, GUIContent.none, style);
				}
				DockArea.BeginOffsetArea(new Rect(position.x + 2f, position.y + 17f, position.width - 4f, position.height - 17f - 2f), GUIContent.none, "TabWindowBackground");
				EditorGUIUtility.ResetGUIState();
				try
				{
					base.Invoke("OnGUI");
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				EditorGUIUtility.ResetGUIState();
				if (base.actualView != null && base.actualView.m_FadeoutTime != 0f && Event.current != null && Event.current.type == EventType.Repaint)
				{
					base.actualView.DrawNotification();
				}
				DockArea.EndOffsetArea();
			}
			base.DoWindowDecorationEnd();
			GUI.Box(position, GUIContent.none, style);
			EditorGUI.ShowRepaints();
			Highlighter.ControlHighlightGUI(this);
		}

		private void Maximize(object userData)
		{
			EditorWindow editorWindow = userData as EditorWindow;
			if (editorWindow != null)
			{
				WindowLayout.Maximize(editorWindow);
			}
		}

		internal void Close(object userData)
		{
			EditorWindow editorWindow = userData as EditorWindow;
			if (editorWindow != null)
			{
				editorWindow.Close();
			}
			else
			{
				this.RemoveTab(null, false);
				this.KillIfEmpty();
			}
		}

		protected override void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
		{
			if (menu.GetItemCount() != 0)
			{
				menu.AddSeparator(string.Empty);
			}
			if (base.parent.window.showMode == ShowMode.MainWindow)
			{
				menu.AddItem(EditorGUIUtility.TextContent("Maximize"), !(base.parent is SplitView), new GenericMenu.MenuFunction2(this.Maximize), view);
			}
			else
			{
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Maximize"));
			}
			menu.AddItem(EditorGUIUtility.TextContent("Close Tab"), false, new GenericMenu.MenuFunction2(this.Close), view);
			menu.AddSeparator(string.Empty);
			Type[] paneTypes = base.GetPaneTypes();
			GUIContent gUIContent = EditorGUIUtility.TextContent("Add Tab");
			Type[] array = paneTypes;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (type != null)
				{
					GUIContent gUIContent2 = new GUIContent(EditorWindow.GetLocalizedTitleContentFromType(type));
					gUIContent2.text = gUIContent.text + "/" + gUIContent2.text;
					menu.AddItem(gUIContent2, false, new GenericMenu.MenuFunction2(this.AddTabToHere), type);
				}
			}
		}

		private void AddTabToHere(object userData)
		{
			EditorWindow pane = (EditorWindow)ScriptableObject.CreateInstance((Type)userData);
			this.AddTab(pane);
		}

		public static void EndOffsetArea()
		{
			if (Event.current.type == EventType.Used)
			{
				return;
			}
			GUILayoutUtility.EndLayoutGroup();
			GUI.EndGroup();
		}

		public static void BeginOffsetArea(Rect screenRect, GUIContent content, GUIStyle style)
		{
			GUILayoutGroup gUILayoutGroup = EditorGUILayoutUtilityInternal.BeginLayoutArea(style, typeof(GUILayoutGroup));
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUILayoutGroup.resetCoords = false;
				gUILayoutGroup.minWidth = (gUILayoutGroup.maxWidth = screenRect.width + 1f);
				gUILayoutGroup.minHeight = (gUILayoutGroup.maxHeight = screenRect.height + 2f);
				gUILayoutGroup.rect = Rect.MinMaxRect(-1f, -1f, gUILayoutGroup.rect.xMax, gUILayoutGroup.rect.yMax - 10f);
			}
			GUI.BeginGroup(screenRect, content, style);
		}

		private float GetTabWidth(float width)
		{
			int num = this.m_Panes.Count;
			if (DockArea.s_DropInfo != null && object.ReferenceEquals(DockArea.s_DropInfo.dropArea, this))
			{
				num++;
			}
			if (this.m_Panes.IndexOf(DockArea.s_DragPane) != -1)
			{
				num--;
			}
			return Mathf.Min(width / (float)num, 100f);
		}

		private int GetTabAtMousePos(Vector2 mousePos, Rect position)
		{
			return (int)Mathf.Min((mousePos.x - position.xMin) / this.GetTabWidth(position.width), 100f);
		}

		internal override void Initialize(ContainerWindow win)
		{
			base.Initialize(win);
			this.RemoveNullWindows();
			foreach (EditorWindow current in this.m_Panes)
			{
				current.m_Parent = this;
			}
		}

		private static void CheckDragWindowExists()
		{
			if (DockArea.s_DragMode == 1 && !PaneDragTab.get.m_Window)
			{
				DockArea.s_OriginalDragSource.RemoveTab(DockArea.s_DragPane);
				UnityEngine.Object.DestroyImmediate(DockArea.s_DragPane);
				PaneDragTab.get.Close();
				GUIUtility.hotControl = 0;
				DockArea.ResetDragVars();
			}
		}

		private void DragTab(Rect pos, GUIStyle tabStyle)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			float tabWidth = this.GetTabWidth(pos.width);
			Event current = Event.current;
			if (DockArea.s_DragMode != 0 && GUIUtility.hotControl == 0)
			{
				PaneDragTab.get.Close();
				DockArea.ResetDragVars();
			}
			EventType typeForControl = current.GetTypeForControl(controlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
				if (pos.Contains(current.mousePosition) && GUIUtility.hotControl == 0)
				{
					int tabAtMousePos = this.GetTabAtMousePos(current.mousePosition, pos);
					if (tabAtMousePos < this.m_Panes.Count)
					{
						switch (current.button)
						{
						case 0:
							if (tabAtMousePos != this.selected)
							{
								this.selected = tabAtMousePos;
							}
							GUIUtility.hotControl = controlID;
							DockArea.s_StartDragPosition = current.mousePosition;
							DockArea.s_DragMode = 0;
							current.Use();
							break;
						case 2:
							this.m_Panes[tabAtMousePos].Close();
							current.Use();
							break;
						}
					}
				}
				goto IL_6B9;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 vector = GUIUtility.GUIToScreenPoint(current.mousePosition);
					if (DockArea.s_DragMode != 0)
					{
						DockArea.s_DragMode = 0;
						PaneDragTab.get.Close();
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists));
						if (DockArea.s_DropInfo != null && DockArea.s_DropInfo.dropArea != null)
						{
							DockArea.s_DropInfo.dropArea.PerformDrop(DockArea.s_DragPane, DockArea.s_DropInfo, vector);
						}
						else
						{
							EditorWindow editorWindow = DockArea.s_DragPane;
							DockArea.ResetDragVars();
							this.RemoveTab(editorWindow);
							Rect position = editorWindow.position;
							position.x = vector.x - position.width * 0.5f;
							position.y = vector.y - position.height * 0.5f;
							if (Application.platform == RuntimePlatform.WindowsEditor)
							{
								position.y = Mathf.Max(InternalEditorUtility.GetBoundsOfDesktopAtPoint(vector).y, position.y);
							}
							EditorWindow.CreateNewWindowForEditorWindow(editorWindow, false, false);
							editorWindow.position = editorWindow.m_Parent.window.FitWindowRectToScreen(position, true, true);
							GUIUtility.hotControl = 0;
							GUIUtility.ExitGUI();
						}
						DockArea.ResetDragVars();
					}
					GUIUtility.hotControl = 0;
					current.Use();
				}
				goto IL_6B9;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_6E:
				if (typeForControl != EventType.ContextClick)
				{
					goto IL_6B9;
				}
				if (pos.Contains(current.mousePosition) && GUIUtility.hotControl == 0)
				{
					int tabAtMousePos2 = this.GetTabAtMousePos(current.mousePosition, pos);
					if (tabAtMousePos2 < this.m_Panes.Count)
					{
						base.PopupGenericMenu(this.m_Panes[tabAtMousePos2], new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f));
					}
				}
				goto IL_6B9;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 vector2 = current.mousePosition - DockArea.s_StartDragPosition;
					current.Use();
					Rect screenPosition = base.screenPosition;
					if (DockArea.s_DragMode == 0 && vector2.sqrMagnitude > 99f)
					{
						DockArea.s_DragMode = 1;
						DockArea.s_PlaceholderPos = this.selected;
						DockArea.s_DragPane = this.m_Panes[this.selected];
						if (this.m_Panes.Count == 1)
						{
							DockArea.s_IgnoreDockingForView = this;
						}
						else
						{
							DockArea.s_IgnoreDockingForView = null;
						}
						DockArea.s_OriginalDragSource = this;
						PaneDragTab.get.content = DockArea.s_DragPane.titleContent;
						base.Internal_SetAsActiveWindow();
						PaneDragTab.get.GrabThumbnail();
						PaneDragTab.get.Show(new Rect(pos.x + screenPosition.x + tabWidth * (float)this.selected, pos.y + screenPosition.y, tabWidth, pos.height), GUIUtility.GUIToScreenPoint(current.mousePosition));
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists));
						GUIUtility.ExitGUI();
					}
					if (DockArea.s_DragMode == 1)
					{
						DropInfo dropInfo = null;
						ContainerWindow[] windows = ContainerWindow.windows;
						Vector2 vector3 = GUIUtility.GUIToScreenPoint(current.mousePosition);
						ContainerWindow inFrontOf = null;
						ContainerWindow[] array = windows;
						for (int i = 0; i < array.Length; i++)
						{
							ContainerWindow containerWindow = array[i];
							View[] allChildren = containerWindow.mainView.allChildren;
							for (int j = 0; j < allChildren.Length; j++)
							{
								View view = allChildren[j];
								IDropArea dropArea = view as IDropArea;
								if (dropArea != null)
								{
									dropInfo = dropArea.DragOver(DockArea.s_DragPane, vector3);
								}
								if (dropInfo != null)
								{
									break;
								}
							}
							if (dropInfo != null)
							{
								inFrontOf = containerWindow;
								break;
							}
						}
						if (dropInfo == null)
						{
							dropInfo = new DropInfo(null);
						}
						if (dropInfo.type != DropInfo.Type.Tab)
						{
							DockArea.s_PlaceholderPos = -1;
						}
						DockArea.s_DropInfo = dropInfo;
						if (PaneDragTab.get.m_Window)
						{
							PaneDragTab.get.SetDropInfo(dropInfo, vector3, inFrontOf);
						}
					}
				}
				goto IL_6B9;
			case EventType.Repaint:
			{
				float num = pos.xMin;
				int num2 = 0;
				if (base.actualView)
				{
					for (int k = 0; k < this.m_Panes.Count; k++)
					{
						if (!(DockArea.s_DragPane == this.m_Panes[k]))
						{
							if (DockArea.s_DropInfo != null && object.ReferenceEquals(DockArea.s_DropInfo.dropArea, this) && DockArea.s_PlaceholderPos == num2)
							{
								num += tabWidth;
							}
							Rect rect = new Rect(num, pos.yMin, tabWidth, pos.height);
							float num3 = Mathf.Round(rect.x);
							Rect position2 = new Rect(num3, rect.y, Mathf.Round(rect.x + rect.width) - num3, rect.height);
							tabStyle.Draw(position2, this.m_Panes[k].titleContent, false, false, k == this.selected, base.hasFocus);
							num += tabWidth;
							num2++;
						}
					}
				}
				else
				{
					Rect rect2 = new Rect(num, pos.yMin, tabWidth, pos.height);
					float num4 = Mathf.Round(rect2.x);
					Rect position3 = new Rect(num4, rect2.y, Mathf.Round(rect2.x + rect2.width) - num4, rect2.height);
					tabStyle.Draw(position3, "Failed to load", false, false, true, false);
				}
				goto IL_6B9;
			}
			}
			goto IL_6E;
			IL_6B9:
			this.selected = Mathf.Clamp(this.selected, 0, this.m_Panes.Count - 1);
		}

		protected override RectOffset GetBorderSize()
		{
			if (!base.window)
			{
				return this.m_BorderSize;
			}
			RectOffset arg_48_0 = this.m_BorderSize;
			int num = 0;
			this.m_BorderSize.bottom = num;
			num = num;
			this.m_BorderSize.top = num;
			num = num;
			this.m_BorderSize.right = num;
			arg_48_0.left = num;
			Rect windowPosition = base.windowPosition;
			if (windowPosition.xMin != 0f)
			{
				this.m_BorderSize.left += 2;
			}
			if (windowPosition.xMax != base.window.position.width)
			{
				this.m_BorderSize.right += 2;
			}
			this.m_BorderSize.top = 17;
			bool flag = base.windowPosition.y == 0f;
			bool flag2 = windowPosition.yMax == base.window.position.height;
			this.m_BorderSize.bottom = 4;
			if (flag2)
			{
				this.m_BorderSize.bottom -= 2;
			}
			if (flag)
			{
				this.m_BorderSize.bottom += 3;
			}
			return this.m_BorderSize;
		}

		private static void ResetDragVars()
		{
			DockArea.s_DragPane = null;
			DockArea.s_DropInfo = null;
			DockArea.s_PlaceholderPos = -1;
			DockArea.s_DragMode = 0;
			DockArea.s_OriginalDragSource = null;
		}
	}
}
