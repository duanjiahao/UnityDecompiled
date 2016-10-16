using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public sealed class VREditor
	{
		public static VRDeviceInfoEditor[] GetEnabledVRDeviceInfo(BuildTargetGroup targetGroup)
		{
			string[] enabledVRDevices = VREditor.GetVREnabledDevices(targetGroup);
			return (from d in VREditor.GetAllVRDeviceInfo(targetGroup)
			where enabledVRDevices.Contains(d.deviceNameKey)
			select d).ToArray<VRDeviceInfoEditor>();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VRDeviceInfoEditor[] GetAllVRDeviceInfo(BuildTargetGroup targetGroup);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetVREnabled(BuildTargetGroup targetGroup);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVREnabled(BuildTargetGroup targetGroup, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetVREnabledDevices(BuildTargetGroup targetGroup);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVREnabledDevices(BuildTargetGroup targetGroup, string[] devices);
	}
}
