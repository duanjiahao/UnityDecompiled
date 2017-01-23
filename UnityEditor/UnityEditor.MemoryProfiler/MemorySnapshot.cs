using System;
using System.Threading;
using UnityEditorInternal;

namespace UnityEditor.MemoryProfiler
{
	public static class MemorySnapshot
	{
		public static event Action<PackedMemorySnapshot> OnSnapshotReceived
		{
			add
			{
				Action<PackedMemorySnapshot> action = MemorySnapshot.OnSnapshotReceived;
				Action<PackedMemorySnapshot> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<PackedMemorySnapshot>>(ref MemorySnapshot.OnSnapshotReceived, (Action<PackedMemorySnapshot>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<PackedMemorySnapshot> action = MemorySnapshot.OnSnapshotReceived;
				Action<PackedMemorySnapshot> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<PackedMemorySnapshot>>(ref MemorySnapshot.OnSnapshotReceived, (Action<PackedMemorySnapshot>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
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
