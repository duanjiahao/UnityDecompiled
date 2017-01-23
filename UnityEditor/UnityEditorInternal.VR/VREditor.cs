using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace UnityEditorInternal.VR
{
	public sealed class VREditor
	{
		[Obsolete("Use GetVREnabledOnTargetGroup instead.")]
		public static bool GetVREnabled(BuildTargetGroup targetGroup)
		{
			return VREditor.GetVREnabledOnTargetGroup(targetGroup);
		}

		[Obsolete("UseSetVREnabledOnTargetGroup instead.")]
		public static void SetVREnabled(BuildTargetGroup targetGroup, bool value)
		{
			VREditor.SetVREnabledOnTargetGroup(targetGroup, value);
		}

		[Obsolete("Use GetVREnabledDevicesOnTargetGroup instead.")]
		public static string[] GetVREnabledDevices(BuildTargetGroup targetGroup)
		{
			return VREditor.GetVREnabledDevicesOnTargetGroup(targetGroup);
		}

		[Obsolete("Use SetVREnabledDevicesOnTargetGroup instead.")]
		public static void SetVREnabledDevices(BuildTargetGroup targetGroup, string[] devices)
		{
			VREditor.SetVREnabledDevicesOnTargetGroup(targetGroup, devices);
		}

		public static VRDeviceInfoEditor[] GetEnabledVRDeviceInfo(BuildTargetGroup targetGroup)
		{
			string[] enabledVRDevices = VREditor.GetVREnabledDevicesOnTargetGroup(targetGroup);
			return (from d in VREditor.GetAllVRDeviceInfo(targetGroup)
			where enabledVRDevices.Contains(d.deviceNameKey)
			select d).ToArray<VRDeviceInfoEditor>();
		}

		public static VRDeviceInfoEditor[] GetEnabledVRDeviceInfo(BuildTarget target)
		{
			string[] enabledVRDevices = VREditor.GetVREnabledDevicesOnTarget(target);
			return (from d in VREditor.GetAllVRDeviceInfoByTarget(target)
			where enabledVRDevices.Contains(d.deviceNameKey)
			select d).ToArray<VRDeviceInfoEditor>();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VRDeviceInfoEditor[] GetAllVRDeviceInfo(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VRDeviceInfoEditor[] GetAllVRDeviceInfoByTarget(BuildTarget target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetVREnabledOnTargetGroup(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVREnabledOnTargetGroup(BuildTargetGroup targetGroup, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetVREnabledDevicesOnTargetGroup(BuildTargetGroup targetGroup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetVREnabledDevicesOnTarget(BuildTarget target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVREnabledDevicesOnTargetGroup(BuildTargetGroup targetGroup, string[] devices);
	}
}
