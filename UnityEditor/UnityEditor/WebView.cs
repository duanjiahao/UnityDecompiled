using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class WebView : ScriptableObject
	{
		[SerializeField]
		private MonoReloadableIntPtr WebViewWindow;

		public void OnDestroy()
		{
			this.DestroyWebView();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyWebView();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWebView(GUIView host, int x, int y, int width, int height, bool showResizeHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ExecuteJavascript(string scriptCode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadURL(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LoadFile(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool DefineScriptObject(string path, ScriptableObject obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDelegateObject(ScriptableObject value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetHostView(GUIView view);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetSizeAndPosition(int x, int y, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetFocus(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasApplicationFocus();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetApplicationFocus(bool applicationFocus);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Show();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Hide();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Back();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Forward();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SendOnEvent(string jsonStr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Reload();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AllowRightClickMenu(bool allowRightClickMenu);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowDevTools();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ToggleMaximize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OnDomainReload();

		public static implicit operator bool(WebView exists)
		{
			return exists != null && !exists.IntPtrIsNull();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IntPtrIsNull();
	}
}
