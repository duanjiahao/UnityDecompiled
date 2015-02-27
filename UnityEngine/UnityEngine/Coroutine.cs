using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Coroutine : YieldInstruction
	{
		internal IntPtr m_Ptr;
		private Coroutine()
		{
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseCoroutine();
		~Coroutine()
		{
			this.ReleaseCoroutine();
		}
	}
}
