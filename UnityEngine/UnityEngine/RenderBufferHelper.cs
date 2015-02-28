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
	}
}
