using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.HolographicEmulation
{
	internal sealed class HolographicEmulation
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Initialize();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Shutdown();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void LoadRoom(string id);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEmulationMode_Internal(EmulationMode mode);

		internal static void SetEmulationMode(EmulationMode mode)
		{
			HolographicEmulation.SetEmulationMode_Internal(mode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGestureHand_Internal(GestureHand hand);

		internal static void SetGestureHand(GestureHand hand)
		{
			HolographicEmulation.SetGestureHand_Internal(hand);
		}
	}
}
