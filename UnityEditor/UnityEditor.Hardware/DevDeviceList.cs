using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Hardware
{
	public sealed class DevDeviceList
	{
		public delegate void OnChangedHandler();

		public static event DevDeviceList.OnChangedHandler Changed
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				DevDeviceList.Changed = (DevDeviceList.OnChangedHandler)Delegate.Combine(DevDeviceList.Changed, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				DevDeviceList.Changed = (DevDeviceList.OnChangedHandler)Delegate.Remove(DevDeviceList.Changed, value);
			}
		}

		public static void OnChanged()
		{
			if (DevDeviceList.Changed != null)
			{
				DevDeviceList.Changed();
			}
		}

		public static bool FindDevice(string deviceId, out DevDevice device)
		{
			DevDevice[] devices = DevDeviceList.GetDevices();
			for (int i = 0; i < devices.Length; i++)
			{
				DevDevice devDevice = devices[i];
				if (devDevice.id == deviceId)
				{
					device = devDevice;
					return true;
				}
			}
			device = default(DevDevice);
			return false;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DevDevice[] GetDevices();

		internal static void Update(string target, DevDevice[] devices)
		{
			DevDeviceList.UpdateInternal(target, devices);
			DevDeviceList.OnChanged();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateInternal(string target, DevDevice[] devices);
	}
}
