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
		private static class Styles
		{
			public static GUIStyle buttonClose = (!ContainerWindow.macEditor) ? "WinBtnClose" : "WinBtnCloseMac";

			public static GUIStyle buttonMin = (!ContainerWindow.macEditor) ? "WinBtnClose" : "WinBtnMinMac";

			public static GUIStyle buttonMax = (!ContainerWindow.macEditor) ? "WinBtnMax" : "WinBtnMaxMac";

			public static GUIStyle buttonInactive = "WinBtnInactiveMac";
		}

		private const float kBorderSize = 4f;

		private const float kTitleHeight = 24f;

		private const float kButtonWidth = 13f;

		private const float kButtonHeight = 13f;

		private const float kButtonSpacing = 3f;

		private const float kButtonTop = 0f;

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

		[SerializeField]
		private SnapEdge m_Left;

		[SerializeField]
		private SnapEdge m_Right;

		[SerializeField]
		private SnapEdge m_Top;

		[SerializeField]
		private SnapEdge m_Bottom;

		private SnapEdge[] m_EdgesCache;

		private int m_ButtonCount;

		private float m_TitleBarWidth;

		private static List<ContainerWindow> s_AllWindows = new List<ContainerWindow>();

		private static Vector2 s_LastDragMousePos;

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

		private IEnumerable<SnapEdge> edges
		{
			get
			{
				if (this.m_EdgesCache == null)
				{
					this.m_EdgesCache = new SnapEdge[]
					{
						this.m_Left,
						this.m_Right,
						this.m_Top,
						this.m_Bottom
					};
				}
				return this.m_EdgesCache;
			}
		}

		internal static bool macEditor
		{
			get
			{
				return Application.platform == RuntimePlatform.OSXEditor;
			}
		}

		internal ShowMode showMode
		{
			get
			{
				return (ShowMode)this.m_ShowMode;
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

		public ContainerWindow()
		{
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
		public extern bool IsZoomed();

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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetTopleftScreenPosition(out Vector2 pos);

		internal Rect FitWindowRectToScreen(Rect r, bool forceCompletelyVisible, bool useMouseScreen)
		{
			Rect result;
			ContainerWindow.INTERNAL_CALL_FitWindowRectToScreen(this, ref r, forceCompletelyVisible, useMouseScreen, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_FitWindowRectToScreen(ContainerWindow self, ref Rect r, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);

		internal static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
		{
			Rect result;
			ContainerWindow.INTERNAL_CALL_FitRectToScreen(ref defaultRect, forceCompletelyVisible, useMouseScreen, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_FitRectToScreen(ref Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);

		private void __internalAwake()
		{
			base.hideFlags = HideFlags.DontSave;
		}

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
			this.position = this.FitWindowRectToScreen(this.m_PixelRect, true, false);
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
				pixelRect.width = Mathf.Min(pixelRect.width, this.m_MaxSize.x);
				pixelRect.height = Mathf.Max(EditorPrefs.GetFloat(str + "h", this.m_PixelRect.height), this.m_MinSize.y);
				pixelRect.height = Mathf.Min(pixelRect.height, this.m_MaxSize.y);
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
			this.Save();
		}

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

		public void HandleWindowDecorationEnd(Rect windowPosition)
		{
		}

		public void HandleWindowDecorationStart(Rect windowPosition)
		{
			if (windowPosition.y != 0f || this.showMode == ShowMode.Utility || this.showMode == ShowMode.PopupMenu)
			{
				return;
			}
			bool flag = Mathf.Abs(windowPosition.xMax - this.position.width) < 2f;
			if (flag)
			{
				GUIStyle style = ContainerWindow.Styles.buttonClose;
				GUIStyle style2 = ContainerWindow.Styles.buttonMin;
				GUIStyle style3 = ContainerWindow.Styles.buttonMax;
				if (ContainerWindow.macEditor && (GUIView.focusedView == null || GUIView.focusedView.window != this))
				{
					style2 = (style = (style3 = ContainerWindow.Styles.buttonInactive));
				}
				this.BeginTitleBarButtons(windowPosition);
				if (this.TitleBarButton(style))
				{
					this.Close();
				}
				if (ContainerWindow.macEditor && this.TitleBarButton(style2))
				{
					this.Minimize();
					GUIUtility.ExitGUI();
				}
				if (this.TitleBarButton(style3))
				{
					this.ToggleMaximize();
				}
			}
			this.HandleTitleBarDrag();
		}

		private void BeginTitleBarButtons(Rect windowPosition)
		{
			this.m_ButtonCount = 0;
			this.m_TitleBarWidth = windowPosition.width;
		}

		private bool TitleBarButton(GUIStyle style)
		{
			Rect position = new Rect(this.m_TitleBarWidth - 13f * (float)(++this.m_ButtonCount) - 4f, 0f, 13f, 13f);
			return GUI.Button(position, GUIContent.none, style);
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

		private void HandleTitleBarDrag()
		{
			this.SetupWindowEdges();
			EditorGUI.BeginChangeCheck();
			this.DragTitleBar(new Rect(0f, 0f, this.position.width, 24f));
			if (EditorGUI.EndChangeCheck())
			{
				Rect rect = new Rect(this.m_Left.pos, this.m_Top.pos, this.m_Right.pos - this.m_Left.pos, this.m_Bottom.pos - this.m_Top.pos);
				if (ContainerWindow.macEditor)
				{
					rect = this.FitWindowRectToScreen(rect, false, false);
				}
				this.position = rect;
			}
		}

		private void DragTitleBar(Rect titleBarRect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (titleBarRect.Contains(current.mousePosition) && GUIUtility.hotControl == 0 && current.button == 0)
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
					ContainerWindow.s_LastDragMousePos = GUIUtility.GUIToScreenPoint(current.mousePosition);
					foreach (SnapEdge current2 in this.edges)
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
					Event.current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Vector2 a = GUIUtility.GUIToScreenPoint(current.mousePosition);
					Vector2 offset = a - ContainerWindow.s_LastDragMousePos;
					ContainerWindow.s_LastDragMousePos = a;
					GUI.changed = true;
					foreach (SnapEdge current3 in this.edges)
					{
						current3.ApplyOffset(offset, true);
					}
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(titleBarRect, MouseCursor.Arrow);
				break;
			}
		}
	}
}
