using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal sealed class GUIClip
	{
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Rect topmostRect
		{
			get
			{
				Rect result;
				GUIClip.INTERNAL_get_topmostRect(out result);
				return result;
			}
		}

		public static Rect visibleRect
		{
			get
			{
				Rect result;
				GUIClip.INTERNAL_get_visibleRect(out result);
				return result;
			}
		}

		public static Vector2 Unclip(Vector2 pos)
		{
			GUIClip.Unclip_Vector2(ref pos);
			return pos;
		}

		public static Rect Unclip(Rect rect)
		{
			GUIClip.Unclip_Rect(ref rect);
			return rect;
		}

		public static Vector2 Clip(Vector2 absolutePos)
		{
			GUIClip.Clip_Vector2(ref absolutePos);
			return absolutePos;
		}

		public static Rect Clip(Rect absoluteRect)
		{
			GUIClip.Internal_Clip_Rect(ref absoluteRect);
			return absoluteRect;
		}

		public static Vector2 GetAbsoluteMousePosition()
		{
			Vector2 result;
			GUIClip.Internal_GetAbsoluteMousePosition(out result);
			return result;
		}

		internal static void Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
		{
			GUIClip.INTERNAL_CALL_Push(ref screenRect, ref scrollOffset, ref renderOffset, resetOffset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Push(ref Rect screenRect, ref Vector2 scrollOffset, ref Vector2 renderOffset, bool resetOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Pop();

		internal static Rect GetTopRect()
		{
			Rect result;
			GUIClip.INTERNAL_CALL_GetTopRect(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTopRect(out Rect value);

		private static void Unclip_Vector2(ref Vector2 pos)
		{
			GUIClip.INTERNAL_CALL_Unclip_Vector2(ref pos);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Unclip_Vector2(ref Vector2 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_topmostRect(out Rect value);

		private static void Unclip_Rect(ref Rect rect)
		{
			GUIClip.INTERNAL_CALL_Unclip_Rect(ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Unclip_Rect(ref Rect rect);

		private static void Clip_Vector2(ref Vector2 absolutePos)
		{
			GUIClip.INTERNAL_CALL_Clip_Vector2(ref absolutePos);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Clip_Vector2(ref Vector2 absolutePos);

		private static void Internal_Clip_Rect(ref Rect absoluteRect)
		{
			GUIClip.INTERNAL_CALL_Internal_Clip_Rect(ref absoluteRect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Clip_Rect(ref Rect absoluteRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reapply();

		internal static Matrix4x4 GetMatrix()
		{
			Matrix4x4 result;
			GUIClip.INTERNAL_CALL_GetMatrix(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetMatrix(out Matrix4x4 value);

		internal static void SetMatrix(Matrix4x4 m)
		{
			GUIClip.INTERNAL_CALL_SetMatrix(ref m);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetMatrix(ref Matrix4x4 m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_visibleRect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetAbsoluteMousePosition(out Vector2 output);
	}
}
