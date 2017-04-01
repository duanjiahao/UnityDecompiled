using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Internal.VR
{
	public static class VRTestMock
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Reset();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddTrackedDevice(string deviceName);

		public static void UpdateTrackedDevice(string deviceName, Vector3 position, Quaternion rotation)
		{
			VRTestMock.INTERNAL_CALL_UpdateTrackedDevice(deviceName, ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateTrackedDevice(string deviceName, ref Vector3 position, ref Quaternion rotation);

		public static void UpdateLeftEye(Vector3 position, Quaternion rotation)
		{
			VRTestMock.INTERNAL_CALL_UpdateLeftEye(ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateLeftEye(ref Vector3 position, ref Quaternion rotation);

		public static void UpdateRightEye(Vector3 position, Quaternion rotation)
		{
			VRTestMock.INTERNAL_CALL_UpdateRightEye(ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateRightEye(ref Vector3 position, ref Quaternion rotation);

		public static void UpdateLeftHand(Vector3 position, Quaternion rotation)
		{
			VRTestMock.INTERNAL_CALL_UpdateLeftHand(ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateLeftHand(ref Vector3 position, ref Quaternion rotation);

		public static void UpdateRightHand(Vector3 position, Quaternion rotation)
		{
			VRTestMock.INTERNAL_CALL_UpdateRightHand(ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateRightHand(ref Vector3 position, ref Quaternion rotation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddController(string controllerName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateControllerAxis(string controllerName, int axis, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateControllerButton(string controllerName, int button, bool pressed);
	}
}
