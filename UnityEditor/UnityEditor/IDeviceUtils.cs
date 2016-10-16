using System;
using UnityEditor.Modules;

namespace UnityEditor
{
	internal static class IDeviceUtils
	{
		internal static RemoteAddress StartRemoteSupport(string deviceId)
		{
			IDevice device = ModuleManager.GetDevice(deviceId);
			return device.StartRemoteSupport();
		}

		internal static void StopRemoteSupport(string deviceId)
		{
			IDevice device = ModuleManager.GetDevice(deviceId);
			device.StopRemoteSupport();
		}

		internal static RemoteAddress StartPlayerConnectionSupport(string deviceId)
		{
			IDevice device = ModuleManager.GetDevice(deviceId);
			return device.StartPlayerConnectionSupport();
		}

		internal static void StopPlayerConnectionSupport(string deviceId)
		{
			IDevice device = ModuleManager.GetDevice(deviceId);
			device.StopPlayerConnectionSupport();
		}
	}
}
