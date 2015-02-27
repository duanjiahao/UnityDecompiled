using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WebView : ScriptableObject
	{
		[SerializeField]
		private MonoReloadableIntPtr webViewWrapper;
		internal extern Texture2D texture
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern WebScriptObject windowScriptObject
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool needsRepaint
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public WebView(int width, int height, bool showResizeHandle)
		{
			this.InitWebView(width, height, showResizeHandle);
		}
		public WebView(int width, int height, string url)
		{
			this.InitWebView(width, height, false);
			this.LoadURL(url);
		}
		public void OnDestroy()
		{
			this.DestroyWebView();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyWebView();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWebView(int width, int height, bool showResizeHandle);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadURL(string url);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadFile(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IntPtrIsNull();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDelegateObject(ScriptableObject value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetWebkitControlsGlobalLocation(int x, int y);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Resize(int width, int height);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InjectMouseDown(int x, int y, int button, int clickCount);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Cut();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Copy();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Paste();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SelectAll();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InjectMouseUp(int x, int y, int button, int clickCount);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InjectMouseMove(int x, int y);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InjectMouseDrag(int x, int y, int button);
		internal void InjectScrollWheel(int x, int y, Vector2 delta)
		{
			WebView.INTERNAL_CALL_InjectScrollWheel(this, x, y, ref delta);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InjectScrollWheel(WebView self, int x, int y, ref Vector2 delta);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InjectKeyboardEvent(Event e);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Focus();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnFocus();
		internal bool InjectEvent(Event e, Rect position)
		{
			switch (e.type)
			{
			case EventType.MouseDown:
				this.InjectMouseDown((int)(e.mousePosition.x - position.x), (int)(e.mousePosition.y - position.y), (Application.platform != RuntimePlatform.OSXEditor || !e.control || e.button != 0) ? e.button : 1, e.clickCount);
				goto IL_1D5;
			case EventType.MouseUp:
				this.InjectMouseUp((int)(e.mousePosition.x - position.x), (int)(e.mousePosition.y - position.y), (Application.platform != RuntimePlatform.OSXEditor || !e.control || e.button != 0) ? e.button : 1, e.clickCount);
				goto IL_1D5;
			case EventType.MouseMove:
				this.InjectMouseMove((int)(e.mousePosition.x - position.x), (int)(e.mousePosition.y - position.y));
				goto IL_1D5;
			case EventType.MouseDrag:
				this.InjectMouseDrag((int)(e.mousePosition.x - position.x), (int)(e.mousePosition.y - position.y), e.button);
				goto IL_1D5;
			case EventType.KeyDown:
				break;
			case EventType.KeyUp:
				break;
			case EventType.ScrollWheel:
				this.InjectScrollWheel((int)(e.mousePosition.x - position.x), (int)(e.mousePosition.y - position.y), e.delta);
				goto IL_1D5;
			default:
				return false;
			}
			this.InjectKeyboardEvent(e);
			IL_1D5:
			if (Event.current == null)
			{
				GUIUtility.ExitGUI();
			}
			return true;
		}
		public void DoGUI(Rect position)
		{
			int controlID = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Native, position);
			EventType type = Event.current.type;
			switch (type)
			{
			case EventType.MouseDown:
			{
				Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
				this.SetWebkitControlsGlobalLocation((int)vector.x, (int)vector.y);
				if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
				}
				goto IL_251;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
				}
				goto IL_251;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
			{
				IL_43:
				string commandName;
				if (type == EventType.ValidateCommand)
				{
					commandName = Event.current.commandName;
					if (commandName != null)
					{
						if (WebView.<>f__switch$map1A == null)
						{
							WebView.<>f__switch$map1A = new Dictionary<string, int>(4)
							{

								{
									"Cut",
									0
								},

								{
									"Copy",
									0
								},

								{
									"Paste",
									0
								},

								{
									"SelectAll",
									0
								}
							};
						}
						int num;
						if (WebView.<>f__switch$map1A.TryGetValue(commandName, out num))
						{
							if (num == 0)
							{
								Event.current.Use();
							}
						}
					}
					return;
				}
				if (type != EventType.ExecuteCommand)
				{
					goto IL_251;
				}
				commandName = Event.current.commandName;
				switch (commandName)
				{
				case "Cut":
					this.Cut();
					break;
				case "Copy":
					this.Copy();
					break;
				case "Paste":
					this.Paste();
					break;
				case "SelectAll":
					this.SelectAll();
					break;
				}
				return;
			}
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Event.current.Use();
				}
				goto IL_251;
			case EventType.Repaint:
				GUI.DrawTexture(position, this.texture);
				return;
			case EventType.Layout:
				this.Resize((int)position.width, (int)position.height);
				return;
			}
			goto IL_43;
			IL_251:
			if (this.InjectEvent(Event.current, position))
			{
				Event.current.Use();
			}
			else
			{
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
				}
			}
		}
		public void DoGUILayout(params GUILayoutOption[] options)
		{
			Rect rect = GUILayoutUtility.GetRect(new GUIContent(), EditorStyles.textField, options);
			if (Event.current.type != EventType.Layout && Event.current.type != EventType.MouseUp)
			{
				this.Resize((int)rect.width, (int)rect.height);
				this.DoGUI(rect);
			}
		}
		public void Back()
		{
			this.windowScriptObject.EvalJavaScript("history.back();");
		}
		public void Forward()
		{
			this.windowScriptObject.EvalJavaScript("history.forward();");
		}
		public void Reload()
		{
			this.windowScriptObject.EvalJavaScript("document.location.reload(true);");
		}
		public static implicit operator bool(WebView exists)
		{
			return exists != null && !exists.IntPtrIsNull();
		}
	}
}
