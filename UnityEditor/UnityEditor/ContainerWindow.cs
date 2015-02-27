using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class ContainerWindow : ScriptableObject
	{
		private const float kButtonWidth = 13f;
		private const float kButtonHeight = 13f;
		private const float kButtonSpacing = 3f;
		private const float kButtonTop = 0f;
		private const float kBorderSize = 4f;
		private const float kTitleHeight = 24f;
		private const float kMacCornerSize = 16f;
		[SerializeField]
		private MonoReloadableIntPtr m_WindowPtr;
		[SerializeField]
		private Rect m_PixelRect;
		[SerializeField]
		private int m_ShowMode;
		[SerializeField]
		private string m_Title = string.Empty;
		[SerializeField]
		private View m_MainView;
		[SerializeField]
		private Vector2 m_MinSize = new Vector2(120f, 80f);
		[SerializeField]
		private Vector2 m_MaxSize = new Vector2(4000f, 4000f);
		internal bool m_DontSaveToLayout;
		private static List<ContainerWindow> s_AllWindows = new List<ContainerWindow>();
		internal static GUIStyle s_ButtonClose;
		internal static GUIStyle s_ButtonMin;
		internal static GUIStyle s_ButtonMax;
		internal static GUIStyle s_ButtonCloseActive;
		internal static GUIStyle s_ButtonMinActive;
		internal static GUIStyle s_ButtonMaxActive;
		internal static GUIStyle s_ButtonInactive;
		internal static GUIStyle s_WindowResize;
		[SerializeField]
		private List<SnapEdge>[] edges = new List<SnapEdge>[9];
		[SerializeField]
		private SnapEdge m_Left;
		[SerializeField]
		private SnapEdge m_Right;
		[SerializeField]
		private SnapEdge m_Top;
		[SerializeField]
		private SnapEdge m_Bottom;
		private static Vector2 s_DragStartMousePos;
		private static Vector2 s_LastDragMousePos;
		internal ShowMode showMode
		{
			get
			{
				return (ShowMode)this.m_ShowMode;
			}
		}
		public extern bool maximized
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public Rect position
		{
			get
			{
				Rect result;
				this.INTERNAL_get_position(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_position(ref value);
			}
		}
		public string title
		{
			get
			{
				return this.m_Title;
			}
			set
			{
				this.m_Title = value;
				this.Internal_SetTitle(value);
			}
		}
		public static ContainerWindow[] windows
		{
			get
			{
				ContainerWindow.s_AllWindows.Clear();
				ContainerWindow.GetOrderedWindowList();
				return ContainerWindow.s_AllWindows.ToArray();
			}
		}
		public View mainView
		{
			get
			{
				return this.m_MainView;
			}
			set
			{
				this.m_MainView = value;
				this.m_MainView.SetWindowRecurse(this);
				this.m_MainView.position = new Rect(0f, 0f, this.position.width, this.position.height);
				this.m_MinSize = value.minSize;
				this.m_MaxSize = value.maxSize;
			}
		}
		internal static bool macEditor
		{
			get
			{
				return Application.platform == RuntimePlatform.OSXEditor;
			}
		}
		public ContainerWindow()
		{
			base.hideFlags = HideFlags.DontSave;
			this.m_PixelRect = new Rect(0f, 0f, 400f, 300f);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAlpha(float alpha);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInvisible();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIsDragging(bool dragging);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsZoomed();
		internal void ShowPopup()
		{
			this.m_ShowMode = 1;
			this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
			if (this.m_MainView)
			{
				this.m_MainView.SetWindowRecurse(this);
			}
			this.Internal_SetTitle(this.m_Title);
			this.Save();
			this.Internal_BringLiveAfterCreation(false, false);
		}
		public void Show(ShowMode showMode, bool loadPosition, bool displayImmediately)
		{
			if (showMode == ShowMode.AuxWindow)
			{
				showMode = ShowMode.Utility;
			}
			if (showMode == ShowMode.Utility || showMode == ShowMode.PopupMenu)
			{
				this.m_DontSaveToLayout = true;
			}
			this.m_ShowMode = (int)showMode;
			if (showMode != ShowMode.PopupMenu)
			{
				this.Load(loadPosition);
			}
			this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
			if (this.m_MainView)
			{
				this.m_MainView.SetWindowRecurse(this);
			}
			this.Internal_SetTitle(this.m_Title);
			this.Internal_BringLiveAfterCreation(displayImmediately, true);
			if (this == null)
			{
				return;
			}
			this.position = this.FitWindowRectToScreen(this.m_PixelRect, false, false);
			this.mainView.position = new Rect(0f, 0f, this.m_PixelRect.width, this.m_PixelRect.height);
			this.mainView.Reflow();
			this.Save();
		}
		public void OnEnable()
		{
			if (this.m_MainView)
			{
				this.m_MainView.Initialize(this);
			}
		}
		public void SetMinMaxSizes(Vector2 min, Vector2 max)
		{
			this.m_MinSize = min;
			this.m_MaxSize = max;
			Rect position = this.position;
			Rect position2 = position;
			position2.width = Mathf.Clamp(position.width, min.x, max.x);
			position2.height = Mathf.Clamp(position.height, min.y, max.y);
			if (position2.width != position.width || position2.height != position.height)
			{
				this.position = position2;
			}
			this.Internal_SetMinMaxSizes(min, max);
		}
		private void Internal_SetMinMaxSizes(Vector2 minSize, Vector2 maxSize)
		{
			ContainerWindow.INTERNAL_CALL_Internal_SetMinMaxSizes(this, ref minSize, ref maxSize);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetMinMaxSizes(ContainerWindow self, ref Vector2 minSize, ref Vector2 maxSize);
		private void Internal_Show(Rect r, int showMode, Vector2 minSize, Vector2 maxSize)
		{
			ContainerWindow.INTERNAL_CALL_Internal_Show(this, ref r, showMode, ref minSize, ref maxSize);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Show(ContainerWindow self, ref Rect r, int showMode, ref Vector2 minSize, ref Vector2 maxSize);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_BringLiveAfterCreation(bool displayImmediately, bool setFocus);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFreezeDisplay(bool freeze);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisplayAllViews();
		internal void InternalCloseWindow()
		{
			this.Save();
			if (this.m_MainView)
			{
				if (this.m_MainView is GUIView)
				{
					((GUIView)this.m_MainView).RemoveFromAuxWindowList();
				}
				UnityEngine.Object.DestroyImmediate(this.m_MainView, true);
				this.m_MainView = null;
			}
			UnityEngine.Object.DestroyImmediate(this, true);
		}
		public void Close()
		{
			this.Save();
			this.InternalClose();
			UnityEngine.Object.DestroyImmediate(this, true);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Minimize();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ToggleMaximize();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MoveInFrontOf(ContainerWindow other);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MoveBehindOf(ContainerWindow other);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InternalClose();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnDestroy();
		internal bool IsNotDocked()
		{
			return this.m_ShowMode == 2 || this.m_ShowMode == 5 || (this.mainView as SplitView != null && this.mainView.children.Length == 1 && this.mainView.children.Length == 1 && this.mainView.children[0] is DockArea && ((DockArea)this.mainView.children[0]).m_Panes.Count == 1);
		}
		private string NotDockedWindowID()
		{
			if (this.IsNotDocked())
			{
				HostView hostView = this.mainView as HostView;
				if (hostView == null)
				{
					if (!(this.mainView is SplitView))
					{
						return this.mainView.GetType().ToString();
					}
					hostView = (HostView)this.mainView.children[0];
				}
				return (this.m_ShowMode != 2 && this.m_ShowMode != 5) ? ((DockArea)this.mainView.children[0]).m_Panes[0].GetType().ToString() : hostView.actualView.GetType().ToString();
			}
			return null;
		}
		public void Save()
		{
			if (this.m_ShowMode != 4 && this.IsNotDocked() && !this.IsZoomed())
			{
				string str = this.NotDockedWindowID();
				EditorPrefs.SetFloat(str + "x", this.m_PixelRect.x);
				EditorPrefs.SetFloat(str + "y", this.m_PixelRect.y);
				EditorPrefs.SetFloat(str + "w", this.m_PixelRect.width);
				EditorPrefs.SetFloat(str + "h", this.m_PixelRect.height);
			}
		}
		private void Load(bool loadPosition)
		{
			if (this.m_ShowMode != 4 && this.IsNotDocked())
			{
				string str = this.NotDockedWindowID();
				Rect pixelRect = this.m_PixelRect;
				if (loadPosition)
				{
					pixelRect.x = EditorPrefs.GetFloat(str + "x", this.m_PixelRect.x);
					pixelRect.y = EditorPrefs.GetFloat(str + "y", this.m_PixelRect.y);
				}
				pixelRect.width = Mathf.Max(EditorPrefs.GetFloat(str + "w", this.m_PixelRect.width), this.m_MinSize.x);
				pixelRect.height = Mathf.Max(EditorPrefs.GetFloat(str + "h", this.m_PixelRect.height), this.m_MinSize.y);
				this.m_PixelRect = pixelRect;
			}
		}
		internal void OnResize()
		{
			if (this.mainView == null)
			{
				return;
			}
			this.mainView.position = new Rect(0f, 0f, this.position.width, this.position.height);
			this.mainView.Reflow();
			this.Save();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_position(out Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_position(ref Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTitle(string title);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetOrderedWindowList();
		internal void AddToWindowList()
		{
			ContainerWindow.s_AllWindows.Add(this);
		}
		public Vector2 WindowToScreenPoint(Vector2 windowPoint)
		{
			Vector2 b;
			this.Internal_GetTopleftScreenPosition(out b);
			return windowPoint + b;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetTopleftScreenPosition(out Vector2 pos);
		internal string DebugHierarchy()
		{
			return this.mainView.DebugHierarchy(0);
		}
		internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
		{
			return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this, locationPriorityOrder);
		}
		internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize)
		{
			return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this);
		}
		internal Rect FitPopupWindowRectToScreen(Rect rect, float minimumHeight)
		{
			float num = 0f;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				num = 10f;
			}
			float b = minimumHeight + num;
			Rect rect2 = rect;
			rect2.height = Mathf.Min(rect2.height, 900f);
			rect2.height += num;
			rect2 = this.FitWindowRectToScreen(rect2, true, true);
			float num2 = Mathf.Max(rect2.yMax - rect.y, b);
			rect2.y = rect2.yMax - num2;
			rect2.height = num2 - num;
			return rect2;
		}
		internal Rect FitWindowRectToScreen(Rect r, bool forceCompletelyVisible, bool useMouseScreen)
		{
			return ContainerWindow.INTERNAL_CALL_FitWindowRectToScreen(this, ref r, forceCompletelyVisible, useMouseScreen);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Rect INTERNAL_CALL_FitWindowRectToScreen(ContainerWindow self, ref Rect r, bool forceCompletelyVisible, bool useMouseScreen);
		internal static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
		{
			return ContainerWindow.INTERNAL_CALL_FitRectToScreen(ref defaultRect, forceCompletelyVisible, useMouseScreen);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Rect INTERNAL_CALL_FitRectToScreen(ref Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen);
		private static void InitIcons()
		{
			if (ContainerWindow.macEditor)
			{
				ContainerWindow.s_ButtonClose = "WinBtnCloseMac";
				ContainerWindow.s_ButtonMin = "WinBtnMinMac";
				ContainerWindow.s_ButtonMax = "WinBtnMaxMac";
				ContainerWindow.s_ButtonCloseActive = "WinBtnCloseActiveMac";
				ContainerWindow.s_ButtonMinActive = "WinBtnMinActiveMac";
				ContainerWindow.s_ButtonMaxActive = "WinBtnMaxActiveMac";
				ContainerWindow.s_ButtonInactive = "WinBtnInactiveMac";
				ContainerWindow.s_WindowResize = "WindowResizeMac";
			}
			else
			{
				ContainerWindow.s_ButtonClose = "WinBtnCloseWin";
				ContainerWindow.s_ButtonMin = "WinBtnMinWin";
				ContainerWindow.s_ButtonMax = "WinBtnMaxWin";
			}
		}
		public void HandleEdgesEnd(Rect windowPosition)
		{
			bool flag = Mathf.Abs(windowPosition.xMax - this.position.width) < 2f;
			bool flag2 = Mathf.Abs(windowPosition.yMax - this.position.height) < 2f;
			if (Event.current.type == EventType.Repaint && ContainerWindow.macEditor && (this.m_MinSize == Vector2.zero || this.m_MinSize != this.m_MaxSize) && flag2 && flag)
			{
				if (ContainerWindow.s_WindowResize == null)
				{
					ContainerWindow.InitIcons();
				}
				ContainerWindow.s_WindowResize.Draw(new Rect(windowPosition.width - ContainerWindow.s_WindowResize.fixedWidth, windowPosition.height - ContainerWindow.s_WindowResize.fixedHeight, ContainerWindow.s_WindowResize.fixedWidth, ContainerWindow.s_WindowResize.fixedHeight), false, false, false, false);
			}
		}
		public void HandleEdgesStart(Rect windowPosition)
		{
			bool left = windowPosition.x == 0f;
			bool flag = windowPosition.y == 0f;
			bool flag2 = Mathf.Abs(windowPosition.xMax - this.position.width) < 2f;
			bool bottom = Mathf.Abs(windowPosition.yMax - this.position.height) < 2f;
			ContainerWindow.InitIcons();
			if (ContainerWindow.macEditor)
			{
				if (flag2 && flag && this.showMode != ShowMode.Utility && this.showMode != ShowMode.PopupMenu)
				{
					GUIView focusedView = GUIView.focusedView;
					GUIStyle style;
					GUIStyle style2;
					GUIStyle style3;
					if (focusedView && focusedView.window == this)
					{
						style = ContainerWindow.s_ButtonClose;
						style2 = ContainerWindow.s_ButtonMin;
						style3 = ContainerWindow.s_ButtonMax;
					}
					else
					{
						style2 = (style = (style3 = ContainerWindow.s_ButtonInactive));
					}
					if (GUI.Button(new Rect(windowPosition.width - 13f - 4f, 0f, 13f, 13f), GUIContent.none, style))
					{
						this.Close();
					}
					if (GUI.Button(new Rect(windowPosition.width - 26f - 3f - 4f, 0f, 13f, 13f), GUIContent.none, style2))
					{
						this.Minimize();
						GUIUtility.ExitGUI();
					}
					if (GUI.Button(new Rect(windowPosition.width - 39f - 6f - 4f, 0f, 13f, 13f), GUIContent.none, style3))
					{
						this.ToggleMaximize();
					}
				}
				this.DragWindowEdgesMac(left, flag, flag2, bottom, windowPosition);
			}
			else
			{
				if (flag2 && flag && this.showMode != ShowMode.Utility && this.showMode != ShowMode.PopupMenu)
				{
					if (GUI.Button(new Rect(windowPosition.width - 13f - 4f, 0f, 13f, 13f), GUIContent.none, ContainerWindow.s_ButtonClose))
					{
						this.Close();
					}
					if (GUI.Button(new Rect(windowPosition.width - 26f - 3f - 4f, 0f, 13f, 13f), GUIContent.none, ContainerWindow.s_ButtonMax))
					{
						this.ToggleMaximize();
					}
				}
				if (!this.maximized)
				{
					this.DragWindowEdgesWin(left, flag, flag2, bottom, windowPosition);
				}
			}
		}
		private void SetupWindowEdges()
		{
			Rect position = this.position;
			if (this.m_Left == null)
			{
				this.m_Left = new SnapEdge(this, SnapEdge.EdgeDir.Left, position.xMin, position.yMin, position.yMax);
				this.m_Right = new SnapEdge(this, SnapEdge.EdgeDir.Right, position.xMax, position.yMin, position.yMax);
				this.m_Top = new SnapEdge(this, SnapEdge.EdgeDir.Up, position.yMin, position.xMin, position.xMax);
				this.m_Bottom = new SnapEdge(this, SnapEdge.EdgeDir.Down, position.yMax, position.xMin, position.xMax);
				for (int i = 0; i < 9; i++)
				{
					this.edges[i] = new List<SnapEdge>();
				}
				this.edges[0].Add(this.m_Top);
				this.edges[0].Add(this.m_Left);
				this.edges[1].Add(this.m_Top);
				this.edges[2].Add(this.m_Top);
				this.edges[2].Add(this.m_Right);
				this.edges[3].Add(this.m_Left);
				this.edges[4].Add(this.m_Left);
				this.edges[4].Add(this.m_Right);
				this.edges[4].Add(this.m_Top);
				this.edges[4].Add(this.m_Bottom);
				this.edges[5].Add(this.m_Right);
				this.edges[6].Add(this.m_Bottom);
				this.edges[6].Add(this.m_Left);
				this.edges[7].Add(this.m_Bottom);
				this.edges[8].Add(this.m_Bottom);
				this.edges[8].Add(this.m_Right);
			}
			this.m_Left.pos = position.xMin;
			this.m_Left.start = position.yMin;
			this.m_Left.end = position.yMax;
			this.m_Right.pos = position.xMax;
			this.m_Right.start = position.yMin;
			this.m_Right.end = position.yMax;
			this.m_Top.pos = position.yMin;
			this.m_Top.start = position.xMin;
			this.m_Top.end = position.xMax;
			this.m_Bottom.pos = position.yMax;
			this.m_Bottom.start = position.xMin;
			this.m_Bottom.end = position.xMax;
		}
		private void DragWindowEdgesMac(bool left, bool top, bool right, bool bottom, Rect windowPosition)
		{
			this.SetupWindowEdges();
			Rect position = this.position;
			GUI.changed = false;
			int changedDirs = 0;
			if (top && this.showMode != ShowMode.Utility && this.showMode != ShowMode.PopupMenu)
			{
				this.DragEdges(new Rect(0f, 0f, windowPosition.width, 24f), this.edges[4], new Vector2(position.xMin, position.yMin), true, true, MouseCursor.Arrow, ref changedDirs, false);
			}
			if (bottom && right)
			{
				this.DragEdges(new Rect(windowPosition.width - 16f, windowPosition.height - 16f, 16f, 16f), this.edges[8], new Vector2(position.xMin, position.yMin), true, true, MouseCursor.Arrow, ref changedDirs, false);
			}
			if (GUI.changed)
			{
				this.ClampEdgeSizes(changedDirs);
				Rect rect = new Rect(this.m_Left.pos, this.m_Top.pos, this.m_Right.pos - this.m_Left.pos, this.m_Bottom.pos - this.m_Top.pos);
				rect = this.FitWindowRectToScreen(rect, false, false);
				this.position = rect;
			}
		}
		private void DragWindowEdgesWin(bool left, bool top, bool right, bool bottom, Rect windowPosition)
		{
			if (top && this.showMode != ShowMode.Utility && this.showMode != ShowMode.PopupMenu)
			{
				this.SetupWindowEdges();
				GUI.changed = false;
				int changedDirs = 0;
				Rect position = this.position;
				this.DragEdges(new Rect(0f, 0f, windowPosition.width, 24f), this.edges[4], new Vector2(position.xMin, position.yMin), true, true, MouseCursor.Arrow, ref changedDirs, true);
				if (GUI.changed)
				{
					this.ClampEdgeSizes(changedDirs);
					this.position = new Rect(this.m_Left.pos, this.m_Top.pos, this.m_Right.pos - this.m_Left.pos, this.m_Bottom.pos - this.m_Top.pos);
				}
			}
		}
		private void ClampEdgeSizes(int changedDirs)
		{
			Vector2 minSize = this.m_MinSize;
			Vector2 maxSize = this.m_MaxSize;
			float num = this.m_Right.pos - this.m_Left.pos;
			float num2 = this.m_Bottom.pos - this.m_Top.pos;
			float num3 = Mathf.Clamp(num, minSize.x, maxSize.x);
			float num4 = Mathf.Clamp(num2, minSize.y, maxSize.y);
			if (num != num3)
			{
				if ((changedDirs & 1) != 0)
				{
					this.m_Left.pos = this.m_Right.pos - num3;
				}
				else
				{
					this.m_Right.pos = this.m_Left.pos + num3;
				}
			}
			if (num2 != num4)
			{
				if ((changedDirs & 8) != 0)
				{
					this.m_Top.pos = this.m_Bottom.pos - num4;
				}
				else
				{
					this.m_Bottom.pos = this.m_Top.pos + num4;
				}
			}
		}
		private void DragEdges(Rect position, IEnumerable<SnapEdge> edges, Vector2 windowPosition, bool allowHorizontal, bool allowVertical, MouseCursor cursor, ref int changedEdges, bool windowMove)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition) && GUIUtility.hotControl == 0 && current.button == 0)
				{
					GUIUtility.hotControl = controlID;
					this.SetIsDragging(true);
					Event.current.Use();
					ContainerWindow.s_LastDragMousePos = GUIUtility.GUIToScreenPoint(current.mousePosition);
					ContainerWindow.s_DragStartMousePos = ContainerWindow.s_LastDragMousePos;
					foreach (SnapEdge current2 in edges)
					{
						current2.startDragPos = current2.pos;
						current2.startDragStart = current2.start;
					}
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					this.SetIsDragging(false);
					Event.current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 a = GUIUtility.GUIToScreenPoint(current.mousePosition);
					Vector2 offset = a - ContainerWindow.s_LastDragMousePos;
					ContainerWindow.s_LastDragMousePos = a;
					if (!windowMove)
					{
						offset = a - ContainerWindow.s_DragStartMousePos;
					}
					if (!allowHorizontal)
					{
						offset.x = 0f;
					}
					if (!allowVertical)
					{
						offset.y = 0f;
					}
					GUI.changed = true;
					foreach (SnapEdge current3 in edges)
					{
						current3.ApplyOffset(offset, ref changedEdges, windowMove);
					}
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(position, cursor);
				break;
			}
		}
	}
}
