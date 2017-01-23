using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ClusterInput
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAxis(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetButton(string name);

		public static Vector3 GetTrackerPosition(string name)
		{
			Vector3 result;
			ClusterInput.INTERNAL_CALL_GetTrackerPosition(name, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerPosition(string name, out Vector3 value);

		public static Quaternion GetTrackerRotation(string name)
		{
			Quaternion result;
			ClusterInput.INTERNAL_CALL_GetTrackerRotation(name, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerRotation(string name, out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAxis(string name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetButton(string name, bool value);

		public static void SetTrackerPosition(string name, Vector3 value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerPosition(name, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerPosition(string name, ref Vector3 value);

		public static void SetTrackerRotation(string name, Quaternion value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerRotation(name, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerRotation(string name, ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CheckConnectionToServer(string name);
	}
}
