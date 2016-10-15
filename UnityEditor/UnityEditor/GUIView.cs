using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	internal class GUIView : View
	{
		private int m_DepthBufferBits;

		private int m_AntiAlias;

		private bool m_WantsMouseMove;

		private bool m_AutoRepaintOnSceneChange;

		private bool m_BackgroundValid;

		public static extern GUIView current
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GUIView focusedView
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GUIView mouseOverView
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasFocus
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool mouseRayInvisible
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool wantsMouseMove
		{
			get
			{
				return this.m_WantsMouseMove;
			}
			set
			{
				this.m_WantsMouseMove = value;
				this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
			}
		}

		internal bool backgroundValid
		{
			get
			{
				return this.m_BackgroundValid;
			}
			set
			{
				this.m_BackgroundValid = value;
			}
		}

		public bool autoRepaintOnSceneChange
		{
			get
			{
				return this.m_AutoRepaintOnSceneChange;
			}
			set
			{
				this.m_AutoRepaintOnSceneChange = value;
				this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
			}
		}

		public int depthBufferBits
		{
			get
			{
				return this.m_DepthBufferBits;
			}
			set
			{
				this.m_DepthBufferBits = value;
			}
		}

		public int antiAlias
		{
			get
			{
				return this.m_AntiAlias;
			}
			set
			{
				this.m_AntiAlias = value;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTitle(string title);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Init(int depthBits, int antiAlias);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Recreate(int depthBits, int antiAlias);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Close();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_SendEvent(Event e);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddToAuxWindowList();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveFromAuxWindowList();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern void Internal_SetAsActiveWindow();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetWantsMouseMove(bool wantIt);

		public void SetInternalGameViewDimensions(Rect rect, Rect clippedRect, Vector2 targetSize)
		{
			GUIView.INTERNAL_CALL_SetInternalGameViewDimensions(this, ref rect, ref clippedRect, ref targetSize);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInternalGameViewDimensions(GUIView self, ref Rect rect, ref Rect clippedRect, ref Vector2 targetSize);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAsStartView();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearStartView();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetAutoRepaint(bool doit);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetWindow(ContainerWindow win);

		private void Internal_SetPosition(Rect windowPosition)
		{
			GUIView.INTERNAL_CALL_Internal_SetPosition(this, ref windowPosition);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetPosition(GUIView self, ref Rect windowPosition);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Focus();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Repaint();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RepaintImmediately();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CaptureRenderDoc();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MakeVistaDWMHappyDance();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StealMouseCapture();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ClearKeyboardControl();

		internal void GrabPixels(RenderTexture rd, Rect rect)
		{
			GUIView.INTERNAL_CALL_GrabPixels(this, rd, ref rect);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GrabPixels(GUIView self, RenderTexture rd, ref Rect rect);

		internal bool SendEvent(Event e)
		{
			int num = SavedGUIState.Internal_GetGUIDepth();
			bool result;
			if (num > 0)
			{
				SavedGUIState savedGUIState = SavedGUIState.Create();
				result = this.Internal_SendEvent(e);
				savedGUIState.ApplyAndForget();
			}
			else
			{
				result = this.Internal_SendEvent(e);
			}
			return result;
		}

		protected override void SetWindow(ContainerWindow win)
		{
			base.SetWindow(win);
			this.Internal_Init(this.m_DepthBufferBits, this.m_AntiAlias);
			if (win)
			{
				this.Internal_SetWindow(win);
			}
			this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
			this.Internal_SetPosition(base.windowPosition);
			this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
			this.m_BackgroundValid = false;
		}

		internal void RecreateContext()
		{
			this.Internal_Recreate(this.m_DepthBufferBits, this.m_AntiAlias);
			this.m_BackgroundValid = false;
		}

		protected override void SetPosition(Rect newPos)
		{
			Rect windowPosition = base.windowPosition;
			base.SetPosition(newPos);
			if (windowPosition == base.windowPosition)
			{
				this.Internal_SetPosition(base.windowPosition);
				return;
			}
			this.Internal_SetPosition(base.windowPosition);
			this.m_BackgroundValid = false;
			this.Repaint();
		}

		public new void OnDestroy()
		{
			this.Internal_Close();
			base.OnDestroy();
		}

		internal void DoWindowDecorationStart()
		{
			if (base.window != null)
			{
				base.window.HandleWindowDecorationStart(base.windowPosition);
			}
		}

		internal void DoWindowDecorationEnd()
		{
			if (base.window != null)
			{
				base.window.HandleWindowDecorationEnd(base.windowPosition);
			}
		}
	}
}
