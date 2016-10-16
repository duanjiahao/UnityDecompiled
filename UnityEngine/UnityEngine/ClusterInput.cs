using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ClusterInput
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAxis(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetButton(string name);

		public static Vector3 GetTrackerPosition(string name)
		{
			Vector3 result;
			ClusterInput.INTERNAL_CALL_GetTrackerPosition(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerPosition(string name, out Vector3 value);

		public static Quaternion GetTrackerRotation(string name)
		{
			Quaternion result;
			ClusterInput.INTERNAL_CALL_GetTrackerRotation(name, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerRotation(string name, out Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAxis(string name, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetButton(string name, bool value);

		public static void SetTrackerPosition(string name, Vector3 value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerPosition(name, ref value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerPosition(string name, ref Vector3 value);

		public static void SetTrackerRotation(string name, Quaternion value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerRotation(name, ref value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerRotation(string name, ref Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CheckConnectionToServer(string name);
	}
}
