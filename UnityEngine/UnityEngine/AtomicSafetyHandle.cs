using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	internal struct AtomicSafetyHandle
	{
		internal IntPtr versionNode;

		internal AtomicSafetyHandleVersionMask version;

		internal static AtomicSafetyHandle Create()
		{
			AtomicSafetyHandle result;
			AtomicSafetyHandle.Create_Injected(out result);
			return result;
		}

		internal static void Release(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandle.Release_Injected(ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PrepareUndisposable(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UseSecondaryVersion(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void BumpSecondaryVersion(ref AtomicSafetyHandle handle);

		internal static void EnforceAllBufferJobsHaveCompletedAndRelease(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandle.EnforceAllBufferJobsHaveCompletedAndRelease_Injected(ref handle);
		}

		internal static void CheckReadAndThrowNoEarlyOut(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandle.CheckReadAndThrowNoEarlyOut_Injected(ref handle);
		}

		internal static void CheckWriteAndThrowNoEarlyOut(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandle.CheckWriteAndThrowNoEarlyOut_Injected(ref handle);
		}

		internal static void CheckDeallocateAndThrow(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandle.CheckDeallocateAndThrow_Injected(ref handle);
		}

		internal unsafe static void CheckReadAndThrow(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandleVersionMask* ptr = (AtomicSafetyHandleVersionMask*)((void*)handle.versionNode);
			if ((handle.version & AtomicSafetyHandleVersionMask.Read) == (AtomicSafetyHandleVersionMask)0 && handle.version != (*ptr & AtomicSafetyHandleVersionMask.ReadInv))
			{
				AtomicSafetyHandle.CheckReadAndThrowNoEarlyOut(handle);
			}
		}

		internal unsafe static void CheckWriteAndThrow(AtomicSafetyHandle handle)
		{
			AtomicSafetyHandleVersionMask* ptr = (AtomicSafetyHandleVersionMask*)((void*)handle.versionNode);
			if ((handle.version & AtomicSafetyHandleVersionMask.Write) == (AtomicSafetyHandleVersionMask)0 && handle.version != (*ptr & AtomicSafetyHandleVersionMask.WriteInv))
			{
				AtomicSafetyHandle.CheckWriteAndThrowNoEarlyOut(handle);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Create_Injected(out AtomicSafetyHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Release_Injected(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnforceAllBufferJobsHaveCompletedAndRelease_Injected(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckReadAndThrowNoEarlyOut_Injected(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckWriteAndThrowNoEarlyOut_Injected(ref AtomicSafetyHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckDeallocateAndThrow_Injected(ref AtomicSafetyHandle handle);
	}
}
