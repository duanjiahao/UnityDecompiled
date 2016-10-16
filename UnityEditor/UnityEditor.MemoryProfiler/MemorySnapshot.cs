using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;

namespace UnityEditor.MemoryProfiler
{
	public static class MemorySnapshot
	{
		public static event Action<PackedMemorySnapshot> OnSnapshotReceived
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				MemorySnapshot.OnSnapshotReceived = (Action<PackedMemorySnapshot>)Delegate.Combine(MemorySnapshot.OnSnapshotReceived, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				MemorySnapshot.OnSnapshotReceived = (Action<PackedMemorySnapshot>)Delegate.Remove(MemorySnapshot.OnSnapshotReceived, value);
			}
		}

		public static void RequestNewSnapshot()
		{
			ProfilerDriver.RequestMemorySnapshot();
		}

		private static void DispatchSnapshot(PackedMemorySnapshot snapshot)
		{
			Action<PackedMemorySnapshot> onSnapshotReceived = MemorySnapshot.OnSnapshotReceived;
			if (onSnapshotReceived != null)
			{
				onSnapshotReceived(snapshot);
			}
		}
	}
}
