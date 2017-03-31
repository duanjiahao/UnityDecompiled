using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

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

		[SerializeField]
		private MonoReloadableIntPtr m_WindowPtr;

		[SerializeField]
		private Rect m_PixelRect;

		[SerializeField]
		private int m_ShowMode;

		[SerializeField]
		private string m_Title = "";

		[FormerlySerializedAs("m_MainView"), SerializeField]
		private View m_RootView;

		[SerializeField]
		private Vector2 m_MinSize = new Vector2(120f, 80f);

		[SerializeField]
		private Vector2 m_MaxSize = new Vector2(4000f, 4000f);

		internal bool m_DontSaveToLayout = false;

		private const float kBorderSize = 4f;

		private const float kTitleHeight = 24f;

		private int m_ButtonCount;

		private float m_TitleBarWidth;

		private const float kButtonWidth = 13f;

		private const float kButtonHeight = 13f;

		private const float kButtonSpacing = 3f;

		private const float kButtonTop = 0f;

		private static List<ContainerWindow> s_AllWindows = new List<ContainerWindow>();

		private static Vector2 s_LastDragMousePos;

		private static Rect dragPosition;

		public extern bool maximized
		{
			[GeneratedByOldBindingsGenerator]
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

		internal bool isPopup
		{
			get
			{
				return ContainerWindow.IsPopup((ShowMode)this.m_ShowMode);
			}
		}

		private static Color skinBackgroundColor
		{
			get
			{
				return (!EditorGUIUtility.isProSkin) ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied(0.3f).AlphaMultiplied(0.5f);
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

		public View rootView
		{
			get
			{
				return this.m_RootView;
			}
			set
			{
				this.m_RootView = value;
				this.m_RootView.SetWindowRecurse(this);
				this.m_RootView.position = new Rect(0f, 0f, this.position.width, this.position.height);
				this.m_MinSize = value.minSize;
				this.m_MaxSize = value.maxSize;
			}
		}

		public SplitView rootSplitView
		{
			get
			{
				SplitView result;
				if (this.m_ShowMode == 4 && this.rootView && this.rootView.children.Length == 3)
				{
					result = (this.rootView.children[1] as SplitView);
				}
				else
				{
					result = (this.rootView as SplitView);
				}
				return result;
			}
		}

		public ContainerWindow()
		{
			this.m_PixelRect = new Rect(0f, 0f, 400f, 300f);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAlpha(float alpha);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInvisible();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsZoomed();

		private void Internal_SetMinMaxSizes(Vector2 minSize, Vector2 maxSize)
		{
			ContainerWindow.INTERNAL_CALL_Internal_SetMinMaxSizes(this, ref minSize, ref maxSize);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetMinMaxSizes(ContainerWindow self, ref Vector2 minSize, ref Vector2 maxSize);

		private void Internal_Show(Rect r, int showMode, Vector2 minSize, Vector2 maxSize)
		{
			ContainerWindow.INTERNAL_CALL_Internal_Show(this, ref r, showMode, ref minSize, ref maxSize);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Show(ContainerWindow self, ref Rect r, int showMode, ref Vector2 minSize, ref Vector2 maxSize);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_BringLiveAfterCreation(bool displayImmediately, bool setFocus);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFreezeDisplay(bool freeze);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisplayAllViews();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Minimize();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ToggleMaximize();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MoveInFrontOf(ContainerWindow other);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MoveBehindOf(ContainerWindow other);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InternalClose();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnDestroy();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_position(out Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_position(ref Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTitle(string title);

		private void SetBackgroundColor(Color color)
		{
			ContainerWindow.INTERNAL_CALL_SetBackgroundColor(this, ref color);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetBackgroundColor(ContainerWindow self, ref Color color);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetOrderedWindowList();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetTopleftScreenPosition(out Vector2 pos);

		internal Rect FitWindowRectToScreen(Rect r, bool forceCompletelyVisible, bool useMouseScreen)
		{
			Rect result;
			ContainerWindow.INTERNAL_CALL_FitWindowRectToScreen(this, ref r, forceCompletelyVisible, useMouseScreen, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_FitWindowRectToScreen(ContainerWindow self, ref Rect r, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);

		internal static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
		{
			Rect result;
			ContainerWindow.INTERNAL_CALL_FitRectToScreen(ref defaultRect, forceCompletelyVisible, useMouseScreen, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_FitRectToScreen(ref Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);

		private void __internalAwake()
		{
			base.hideFlags = HideFlags.DontSave;
		}

		internal static bool IsPopup(ShowMode mode)
		{
			return mode == ShowMode.PopupMenu || ShowMode.PopupMenuWithKeyboardFocus == mode;
		}

		internal void ShowPopup()
		{
			this.m_ShowMode = 1;
			this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
			if (this.m_RootView)
			{
				this.m_RootView.SetWindowRecurse(this);
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
			if (showMode == ShowMode.Utility || ContainerWindow.IsPopup(showMode))
			{
				this.m_DontSaveToLayout = true;
			}
			this.m_ShowMode = (int)showMode;
			if (!this.isPopup)
			{
				this.Load(loadPosition);
			}
			this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
			if (this.m_RootView)
			{
				this.m_RootView.SetWindowRecurse(this);
			}
			this.Internal_SetTitle(this.m_Title);
			this.SetBackgroundColor(ContainerWindow.skinBackgroundColor);
			this.Internal_BringLiveAfterCreation(displayImmediately, true);
			if (!(this == null))
			{
				this.position = this.FitWindowRectToScreen(this.m_PixelRect, true, false);
				this.rootView.position = new Rect(0f, 0f, this.m_PixelRect.width, this.m_PixelRect.height);
				this.rootView.Reflow();
				this.Save();
			}
		}

		public void OnEnable()
		{
			if (this.m_RootView)
			{
				this.m_RootView.Initialize(this);
			}
			this.SetBackgroundColor(ContainerWindow.skinBackgroundColor);
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
			if (this.m_RootView)
			{
				if (this.m_RootView is GUIView)
				{
					((GUIView)this.m_RootView).RemoveFromAuxWindowList();
				}
				UnityEngine.Object.DestroyImmediate(this.m_RootView, true);
				this.m_RootView = null;
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
			return this.m_ShowMode == 2 || this.m_ShowMode == 5 || (this.rootView as SplitView != null && this.rootView.children.Length == 1 && this.rootView.children.Length == 1 && this.rootView.children[0] is DockArea && ((DockArea)this.rootView.children[0]).m_Panes.Count == 1);
		}

		private string NotDockedWindowID()
		{
			string result;
			if (this.IsNotDocked())
			{
				HostView hostView = this.rootView as HostView;
				if (hostView == null)
				{
					if (!(this.rootView is SplitView))
					{
						result = this.rootView.GetType().ToString();
						return result;
					}
					hostView = (HostView)this.rootView.children[0];
				}
				result = ((this.m_ShowMode != 2 && this.m_ShowMode != 5) ? ((DockArea)this.rootView.children[0]).m_Panes[0].GetType().ToString() : hostView.actualView.GetType().ToString());
			}
			else
			{
				result = null;
			}
			return result;
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
			if (!(this.rootView == null))
			{
				this.rootView.position = new Rect(0f, 0f, this.position.width, this.position.height);
				this.Save();
			}
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
			return this.rootView.DebugHierarchy(0);
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
			if (windowPosition.y == 0f && this.showMode != ShowMode.Utility && !this.isPopup)
			{
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
				this.DragTitleBar(new Rect(0f, 0f, this.position.width, 24f));
			}
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
					ContainerWindow.dragPosition = this.position;
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
					Vector2 vector = a - ContainerWindow.s_LastDragMousePos;
					ContainerWindow.s_LastDragMousePos = a;
					ContainerWindow.dragPosition.x = ContainerWindow.dragPosition.x + vector.x;
					ContainerWindow.dragPosition.y = ContainerWindow.dragPosition.y + vector.y;
					this.position = ContainerWindow.dragPosition;
					GUI.changed = true;
				}
				break;
			case EventType.Repaint:
				EditorGUIUtility.AddCursorRect(titleBarRect, MouseCursor.Arrow);
				break;
			}
		}
	}
}
