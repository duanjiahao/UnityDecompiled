using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal struct RenderBufferHelper
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLoadAction(out RenderBuffer b);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLoadAction(out RenderBuffer b, int a);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetStoreAction(out RenderBuffer b);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetStoreAction(out RenderBuffer b, int a);

		internal static IntPtr GetNativeRenderBufferPtr(IntPtr rb)
		{
			IntPtr result;
			RenderBufferHelper.INTERNAL_CALL_GetNativeRenderBufferPtr(rb, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeRenderBufferPtr(IntPtr rb, out IntPtr value);
	}
}
