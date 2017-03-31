using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal struct RenderBufferHelper
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLoadAction(out RenderBuffer b);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLoadAction(out RenderBuffer b, int a);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetStoreAction(out RenderBuffer b);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetStoreAction(out RenderBuffer b, int a);

		internal static IntPtr GetNativeRenderBufferPtr(IntPtr rb)
		{
			IntPtr result;
			RenderBufferHelper.INTERNAL_CALL_GetNativeRenderBufferPtr(rb, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeRenderBufferPtr(IntPtr rb, out IntPtr value);
	}
}
