using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA
{
	public sealed class WorldAnchor : Component
	{
		public delegate void OnTrackingChangedDelegate(WorldAnchor self, bool located);

		public event WorldAnchor.OnTrackingChangedDelegate OnTrackingChanged
		{
			add
			{
				WorldAnchor.OnTrackingChangedDelegate onTrackingChangedDelegate = this.OnTrackingChanged;
				WorldAnchor.OnTrackingChangedDelegate onTrackingChangedDelegate2;
				do
				{
					onTrackingChangedDelegate2 = onTrackingChangedDelegate;
					onTrackingChangedDelegate = Interlocked.CompareExchange<WorldAnchor.OnTrackingChangedDelegate>(ref this.OnTrackingChanged, (WorldAnchor.OnTrackingChangedDelegate)Delegate.Combine(onTrackingChangedDelegate2, value), onTrackingChangedDelegate);
				}
				while (onTrackingChangedDelegate != onTrackingChangedDelegate2);
			}
			remove
			{
				WorldAnchor.OnTrackingChangedDelegate onTrackingChangedDelegate = this.OnTrackingChanged;
				WorldAnchor.OnTrackingChangedDelegate onTrackingChangedDelegate2;
				do
				{
					onTrackingChangedDelegate2 = onTrackingChangedDelegate;
					onTrackingChangedDelegate = Interlocked.CompareExchange<WorldAnchor.OnTrackingChangedDelegate>(ref this.OnTrackingChanged, (WorldAnchor.OnTrackingChangedDelegate)Delegate.Remove(onTrackingChangedDelegate2, value), onTrackingChangedDelegate);
				}
				while (onTrackingChangedDelegate != onTrackingChangedDelegate2);
			}
		}

		public bool isLocated
		{
			get
			{
				return this.IsLocated_Internal();
			}
		}

		private WorldAnchor()
		{
		}

		private bool IsLocated_Internal()
		{
			return WorldAnchor.INTERNAL_CALL_IsLocated_Internal(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsLocated_Internal(WorldAnchor self);

		[RequiredByNativeCode]
		private static void Internal_TriggerEventOnTrackingLost(WorldAnchor self, bool located)
		{
			if (self != null && self.OnTrackingChanged != null)
			{
				self.OnTrackingChanged(self, located);
			}
		}
	}
}
