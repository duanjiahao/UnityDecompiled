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
		private int m_DepthBufferBits = 0;

		private bool m_WantsMouseMove = false;

		private bool m_WantsMouseEnterLeaveWindow = false;

		private bool m_AutoRepaintOnSceneChange = false;

		private bool m_BackgroundValid = false;

		public static extern GUIView current
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GUIView focusedView
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GUIView mouseOverView
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasFocus
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool mouseRayInvisible
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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

		public bool wantsMouseEnterLeaveWindow
		{
			get
			{
				return this.m_WantsMouseEnterLeaveWindow;
			}
			set
			{
				this.m_WantsMouseEnterLeaveWindow = value;
				this.Internal_SetWantsMouseEnterLeaveWindow(this.m_WantsMouseEnterLeaveWindow);
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

		[Obsolete("AA is not supported on GUIViews", false)]
		public int antiAlias
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetTitle(string title);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Init(int depthBits);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Recreate(int depthBits);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Close();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_SendEvent(Event e);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddToAuxWindowList();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveFromAuxWindowList();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern void Internal_SetAsActiveWindow();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetWantsMouseMove(bool wantIt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetWantsMouseEnterLeaveWindow(bool wantIt);

		public void SetInternalGameViewDimensions(Rect rect, Rect clippedRect, Vector2 targetSize)
		{
			GUIView.INTERNAL_CALL_SetInternalGameViewDimensions(this, ref rect, ref clippedRect, ref targetSize);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInternalGameViewDimensions(GUIView self, ref Rect rect, ref Rect clippedRect, ref Vector2 targetSize);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAsStartView();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearStartView();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetAutoRepaint(bool doit);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetWindow(ContainerWindow win);

		private void Internal_SetPosition(Rect windowPosition)
		{
			GUIView.INTERNAL_CALL_Internal_SetPosition(this, ref windowPosition);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetPosition(GUIView self, ref Rect windowPosition);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Focus();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Repaint();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RepaintImmediately();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CaptureRenderDoc();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MakeVistaDWMHappyDance();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StealMouseCapture();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ClearKeyboardControl();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetKeyboardControl(int id);

		internal void GrabPixels(RenderTexture rd, Rect rect)
		{
			GUIView.INTERNAL_CALL_GrabPixels(this, rd, ref rect);
		}

		[GeneratedByOldBindingsGenerator]
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
			this.Internal_Init(this.m_DepthBufferBits);
			if (win)
			{
				this.Internal_SetWindow(win);
			}
			this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
			this.Internal_SetPosition(base.windowPosition);
			this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
			this.Internal_SetWantsMouseEnterLeaveWindow(this.m_WantsMouseEnterLeaveWindow);
			this.m_BackgroundValid = false;
		}

		internal void RecreateContext()
		{
			this.Internal_Recreate(this.m_DepthBufferBits);
			this.m_BackgroundValid = false;
		}

		protected override void SetPosition(Rect newPos)
		{
			Rect windowPosition = base.windowPosition;
			base.SetPosition(newPos);
			if (windowPosition == base.windowPosition)
			{
				this.Internal_SetPosition(base.windowPosition);
			}
			else
			{
				this.Internal_SetPosition(base.windowPosition);
				this.m_BackgroundValid = false;
				this.Repaint();
			}
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
