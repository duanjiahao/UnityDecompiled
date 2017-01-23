using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA
{
	public sealed class WorldManager
	{
		public delegate void OnPositionalLocatorStateChangedDelegate(PositionalLocatorState oldState, PositionalLocatorState newState);

		public static event WorldManager.OnPositionalLocatorStateChangedDelegate OnPositionalLocatorStateChanged
		{
			add
			{
				WorldManager.OnPositionalLocatorStateChangedDelegate onPositionalLocatorStateChangedDelegate = WorldManager.OnPositionalLocatorStateChanged;
				WorldManager.OnPositionalLocatorStateChangedDelegate onPositionalLocatorStateChangedDelegate2;
				do
				{
					onPositionalLocatorStateChangedDelegate2 = onPositionalLocatorStateChangedDelegate;
					onPositionalLocatorStateChangedDelegate = Interlocked.CompareExchange<WorldManager.OnPositionalLocatorStateChangedDelegate>(ref WorldManager.OnPositionalLocatorStateChanged, (WorldManager.OnPositionalLocatorStateChangedDelegate)Delegate.Combine(onPositionalLocatorStateChangedDelegate2, value), onPositionalLocatorStateChangedDelegate);
				}
				while (onPositionalLocatorStateChangedDelegate != onPositionalLocatorStateChangedDelegate2);
			}
			remove
			{
				WorldManager.OnPositionalLocatorStateChangedDelegate onPositionalLocatorStateChangedDelegate = WorldManager.OnPositionalLocatorStateChanged;
				WorldManager.OnPositionalLocatorStateChangedDelegate onPositionalLocatorStateChangedDelegate2;
				do
				{
					onPositionalLocatorStateChangedDelegate2 = onPositionalLocatorStateChangedDelegate;
					onPositionalLocatorStateChangedDelegate = Interlocked.CompareExchange<WorldManager.OnPositionalLocatorStateChangedDelegate>(ref WorldManager.OnPositionalLocatorStateChanged, (WorldManager.OnPositionalLocatorStateChangedDelegate)Delegate.Remove(onPositionalLocatorStateChangedDelegate2, value), onPositionalLocatorStateChangedDelegate);
				}
				while (onPositionalLocatorStateChangedDelegate != onPositionalLocatorStateChangedDelegate2);
			}
		}

		public static extern PositionalLocatorState state
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[RequiredByNativeCode]
		private static void Internal_TriggerPositionalLocatorStateChanged(PositionalLocatorState oldState, PositionalLocatorState newState)
		{
			if (WorldManager.OnPositionalLocatorStateChanged != null)
			{
				WorldManager.OnPositionalLocatorStateChanged(oldState, newState);
			}
		}

		public static IntPtr GetNativeISpatialCoordinateSystemPtr()
		{
			IntPtr result;
			WorldManager.INTERNAL_CALL_GetNativeISpatialCoordinateSystemPtr(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeISpatialCoordinateSystemPtr(out IntPtr value);
	}
}
