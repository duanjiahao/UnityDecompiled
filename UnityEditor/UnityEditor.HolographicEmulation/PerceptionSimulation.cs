using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.HolographicEmulation
{
	internal sealed class PerceptionSimulation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Initialize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Shutdown();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadRoom(string id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEmulationMode_Internal(EmulationMode mode);

		internal static void SetEmulationMode(EmulationMode mode)
		{
			PerceptionSimulation.SetEmulationMode_Internal(mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGestureHand_Internal(GestureHand hand);

		internal static void SetGestureHand(GestureHand hand)
		{
			PerceptionSimulation.SetGestureHand_Internal(hand);
		}
	}
}
