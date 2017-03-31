using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	internal struct ChangeTrackerHandle
	{
		private IntPtr m_Handle;

		internal static ChangeTrackerHandle AcquireTracker(UnityEngine.Object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("Not a valid unity engine object");
			}
			return ChangeTrackerHandle.Internal_AcquireTracker(obj);
		}

		private static ChangeTrackerHandle Internal_AcquireTracker(UnityEngine.Object o)
		{
			ChangeTrackerHandle result;
			ChangeTrackerHandle.INTERNAL_CALL_Internal_AcquireTracker(o, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_AcquireTracker(UnityEngine.Object o, out ChangeTrackerHandle value);

		internal void ReleaseTracker()
		{
			if (this.m_Handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("Not a valid handle, has it been released already?");
			}
			ChangeTrackerHandle.Internal_ReleaseTracker(this);
			this.m_Handle = IntPtr.Zero;
		}

		private static void Internal_ReleaseTracker(ChangeTrackerHandle h)
		{
			ChangeTrackerHandle.INTERNAL_CALL_Internal_ReleaseTracker(ref h);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_ReleaseTracker(ref ChangeTrackerHandle h);

		internal bool PollForChanges()
		{
			if (this.m_Handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("Not a valid handle, has it been released already?");
			}
			return this.Internal_PollChanges();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_PollChanges();
	}
}
