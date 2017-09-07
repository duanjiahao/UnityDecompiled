using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditorInternal.VR
{
	public sealed class HolographicEmulation
	{
		private static SimulatedBody s_Body = new SimulatedBody();

		private static SimulatedHead s_Head = new SimulatedHead();

		private static SimulatedHand s_LeftHand = new SimulatedHand(GestureHand.Left);

		private static SimulatedHand s_RightHand = new SimulatedHand(GestureHand.Right);

		public static SimulatedBody simulatedBody
		{
			get
			{
				return HolographicEmulation.s_Body;
			}
		}

		public static SimulatedHead simulatedHead
		{
			get
			{
				return HolographicEmulation.s_Head;
			}
		}

		public static SimulatedHand simulatedLeftHand
		{
			get
			{
				return HolographicEmulation.s_LeftHand;
			}
		}

		public static SimulatedHand simulatedRightHand
		{
			get
			{
				return HolographicEmulation.s_RightHand;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Initialize();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Shutdown();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadRoom(string id);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetEmulationMode(EmulationMode mode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGestureHand(GestureHand hand);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Reset();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PerformGesture_Internal(GestureHand hand, SimulatedGesture gesture);

		internal static Vector3 GetBodyPosition_Internal()
		{
			Vector3 result;
			HolographicEmulation.INTERNAL_CALL_GetBodyPosition_Internal(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetBodyPosition_Internal(out Vector3 value);

		internal static void SetBodyPosition_Internal(Vector3 position)
		{
			HolographicEmulation.INTERNAL_CALL_SetBodyPosition_Internal(ref position);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetBodyPosition_Internal(ref Vector3 position);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetBodyRotation_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBodyRotation_Internal(float degrees);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetBodyHeight_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBodyHeight_Internal(float meters);

		internal static Vector3 GetHeadRotation_Internal()
		{
			Vector3 result;
			HolographicEmulation.INTERNAL_CALL_GetHeadRotation_Internal(out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetHeadRotation_Internal(out Vector3 value);

		internal static void SetHeadRotation_Internal(Vector3 rotation)
		{
			HolographicEmulation.INTERNAL_CALL_SetHeadRotation_Internal(ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetHeadRotation_Internal(ref Vector3 rotation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetHeadDiameter_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetHeadDiameter_Internal(float meters);

		internal static Vector3 GetHandPosition_Internal(GestureHand hand)
		{
			Vector3 result;
			HolographicEmulation.INTERNAL_CALL_GetHandPosition_Internal(hand, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetHandPosition_Internal(GestureHand hand, out Vector3 value);

		internal static void SetHandPosition_Internal(GestureHand hand, Vector3 position)
		{
			HolographicEmulation.INTERNAL_CALL_SetHandPosition_Internal(hand, ref position);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetHandPosition_Internal(GestureHand hand, ref Vector3 position);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetHandActivated_Internal(GestureHand hand);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetHandActivated_Internal(GestureHand hand, bool activated);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetHandVisible_Internal(GestureHand hand);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EnsureHandVisible_Internal(GestureHand hand);
	}
}
