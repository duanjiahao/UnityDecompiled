using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

		internal static View s_IgnoreDockingForView = null;

		private static DropInfo s_DropInfo = null;

		[SerializeField]
		internal List<EditorWindow> m_Panes = new List<EditorWindow>();

		[SerializeField]
		internal int m_Selected;

		[SerializeField]
		internal int m_LastSelected;

		[NonSerialized]
		internal GUIStyle tabStyle = null;

		private bool m_IsBeingDestroyed;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache1;

		public int selected
		{
			get
			{
				return this.m_Selected;
			}
			set
			{
				if (this.m_Selected != value)
				{
					this.m_LastSelected = this.m_Selected;
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
			if (this.m_Panes != null)
			{
				if (this.m_Panes.Count == 0)
				{
					this.m_Selected = 0;
				}
				else
				{
					this.m_Selected = Math.Min(this.m_Selected, this.m_Panes.Count - 1);
					base.actualView = this.m_Panes[this.m_Selected];
				}
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
			this.selected = idx;
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
			if (num != -1)
			{
				this.m_Panes.Remove(pane);
				if (num == this.m_LastSelected)
				{
					this.m_LastSelected = this.m_Panes.Count - 1;
				}
				else if (num < this.m_LastSelected || this.m_LastSelected == this.m_Panes.Count)
				{
					this.m_LastSelected--;
				}
				this.m_LastSelected = Mathf.Clamp(this.m_LastSelected, 0, this.m_Panes.Count - 1);
				if (num == this.m_Selected)
				{
					this.m_Selected = this.m_LastSelected;
				}
				else
				{
					this.m_Selected = this.m_Panes.IndexOf(this.m_ActualView);
				}
				if (this.m_Selected >= 0 && this.m_Selected < this.m_Panes.Count)
				{
					base.actualView = this.m_Panes[this.m_Selected];
				}
				base.Repaint();
				pane.m_Parent = null;
				if (killIfEmpty)
				{
					this.KillIfEmpty();
				}
				base.RegisterSelectedPane();
			}
		}

		private void KillIfEmpty()
		{
			if (this.m_Panes.Count == 0)
			{
				if (base.parent == null)
				{
					base.window.InternalCloseWindow();
				}
				else
				{
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
			}
		}

		public DropInfo DragOver(EditorWindow window, Vector2 mouseScreenPosition)
		{
			Rect screenPosition = base.screenPosition;
			screenPosition.height = 39f;
			DropInfo result;
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
				result = new DropInfo(this)
				{
					type = DropInfo.Type.Tab,
					rect = new Rect(mousePos.x - tabWidth * 0.25f + rect.x, tabRect.y + rect.y, tabWidth, tabRect.height)
				};
			}
			else
			{
				result = null;
			}
			return result;
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
			if (base.window.rootView.GetType() != typeof(MainView))
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
			Rect rect = this.background.margin.Remove(new Rect(0f, 0f, base.position.width, base.position.height));
			rect.x = (float)this.background.margin.left;
			rect.y = (float)this.background.margin.top;
			Rect windowPosition = base.windowPosition;
			float num2 = 2f;
			if (windowPosition.x == 0f)
			{
				rect.x -= num2;
				rect.width += num2;
			}
			if (windowPosition.xMax == base.window.position.width)
			{
				rect.width += num2;
			}
			if (windowPosition.yMax == base.window.position.height)
			{
				rect.height += ((!flag) ? 2f : 2f);
			}
			GUI.Box(rect, GUIContent.none, this.background);
			if (this.tabStyle == null)
			{
				this.tabStyle = "dragtab";
			}
			if (this.m_Panes.Count > 0)
			{
				HostView.BeginOffsetArea(new Rect(rect.x + 2f, rect.y + 17f, rect.width - 4f, rect.height - 17f - 2f), GUIContent.none, "TabWindowBackground");
				Vector2 vector = GUIUtility.GUIToScreenPoint(Vector2.zero);
				Rect pos = base.borderSize.Remove(base.position);
				pos.x = vector.x;
				pos.y = vector.y;
				this.m_Panes[this.selected].m_Pos = pos;
				HostView.EndOffsetArea();
			}
			this.DragTab(new Rect(rect.x + 1f, rect.y, rect.width - 40f, 17f), this.tabStyle);
			this.tabStyle = "dragtab";
			base.ShowGenericMenu();
			if (this.m_Panes.Count > 0)
			{
				base.InvokeOnGUI(rect);
			}
			EditorGUI.ShowRepaints();
			Highlighter.ControlHighlightGUI(this);
		}

		protected override void SetActualViewPosition(Rect newPos)
		{
			Rect actualViewPosition = base.borderSize.Remove(newPos);
			base.SetActualViewPosition(actualViewPosition);
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

		private bool AllowTabAction()
		{
			int num = 0;
			ContainerWindow containerWindow = ContainerWindow.windows.First((ContainerWindow e) => e.showMode == ShowMode.MainWindow);
			bool result;
			if (containerWindow != null)
			{
				View[] allChildren = containerWindow.rootView.allChildren;
				for (int i = 0; i < allChildren.Length; i++)
				{
					View view = allChildren[i];
					DockArea dockArea = view as DockArea;
					if (dockArea != null)
					{
						num += dockArea.m_Panes.Count;
						if (num > 1)
						{
							result = true;
							return result;
						}
					}
				}
			}
			result = false;
			return result;
		}

		protected override void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
		{
			if (menu.GetItemCount() != 0)
			{
				menu.AddSeparator("");
			}
			if (base.parent.window.showMode == ShowMode.MainWindow)
			{
				menu.AddItem(EditorGUIUtility.TextContent("Maximize"), !(base.parent is SplitView), new GenericMenu.MenuFunction2(this.Maximize), view);
			}
			else
			{
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Maximize"));
			}
			bool flag = base.window.showMode != ShowMode.MainWindow || this.AllowTabAction();
			if (flag)
			{
				menu.AddItem(EditorGUIUtility.TextContent("Close Tab"), false, new GenericMenu.MenuFunction2(this.Close), view);
			}
			else
			{
				menu.AddDisabledItem(EditorGUIUtility.TextContent("Close Tab"));
			}
			menu.AddSeparator("");
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
						int button = current.button;
						if (button != 0)
						{
							if (button == 2)
							{
								this.m_Panes[tabAtMousePos].Close();
								current.Use();
							}
						}
						else
						{
							if (tabAtMousePos != this.selected)
							{
								this.selected = tabAtMousePos;
							}
							GUIUtility.hotControl = controlID;
							DockArea.s_StartDragPosition = current.mousePosition;
							DockArea.s_DragMode = 0;
							current.Use();
						}
					}
				}
				goto IL_741;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 vector = GUIUtility.GUIToScreenPoint(current.mousePosition);
					if (DockArea.s_DragMode != 0)
					{
						DockArea.s_DragMode = 0;
						PaneDragTab.get.Close();
						Delegate arg_49A_0 = EditorApplication.update;
						if (DockArea.<>f__mg$cache1 == null)
						{
							DockArea.<>f__mg$cache1 = new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists);
						}
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(arg_49A_0, DockArea.<>f__mg$cache1);
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
				goto IL_741;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
				IL_6F:
				if (typeForControl != EventType.ContextClick)
				{
					goto IL_741;
				}
				if (pos.Contains(current.mousePosition) && GUIUtility.hotControl == 0)
				{
					int tabAtMousePos2 = this.GetTabAtMousePos(current.mousePosition, pos);
					if (tabAtMousePos2 < this.m_Panes.Count)
					{
						base.PopupGenericMenu(this.m_Panes[tabAtMousePos2], new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f));
					}
				}
				goto IL_741;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 vector2 = current.mousePosition - DockArea.s_StartDragPosition;
					current.Use();
					Rect screenPosition = base.screenPosition;
					bool flag = base.window.showMode != ShowMode.MainWindow || this.AllowTabAction();
					if (DockArea.s_DragMode == 0 && vector2.sqrMagnitude > 99f && flag)
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
						PaneDragTab.get.Show(new Rect(pos.x + screenPosition.x + tabWidth * (float)this.selected, pos.y + screenPosition.y, tabWidth, pos.height), DockArea.s_DragPane.titleContent, base.position.size, GUIUtility.GUIToScreenPoint(current.mousePosition));
						Delegate arg_2F5_0 = EditorApplication.update;
						if (DockArea.<>f__mg$cache0 == null)
						{
							DockArea.<>f__mg$cache0 = new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists);
						}
						EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(arg_2F5_0, DockArea.<>f__mg$cache0);
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
							SplitView rootSplitView = containerWindow.rootSplitView;
							if (rootSplitView != null)
							{
								dropInfo = rootSplitView.DragOverRootView(vector3);
							}
							if (dropInfo == null)
							{
								View[] allChildren = containerWindow.rootView.allChildren;
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
				goto IL_741;
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
				goto IL_741;
			}
			}
			goto IL_6F;
			IL_741:
			this.selected = Mathf.Clamp(this.selected, 0, this.m_Panes.Count - 1);
		}

		protected override RectOffset GetBorderSize()
		{
			RectOffset borderSize;
			if (!base.window)
			{
				borderSize = this.m_BorderSize;
			}
			else
			{
				RectOffset arg_4E_0 = this.m_BorderSize;
				int num = 0;
				this.m_BorderSize.bottom = num;
				num = num;
				this.m_BorderSize.top = num;
				num = num;
				this.m_BorderSize.right = num;
				arg_4E_0.left = num;
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
				borderSize = this.m_BorderSize;
			}
			return borderSize;
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
