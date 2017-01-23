using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnityEditor.Hardware
{
	public sealed class DevDeviceList
	{
		public delegate void OnChangedHandler();

		public static event DevDeviceList.OnChangedHandler Changed
		{
			add
			{
				DevDeviceList.OnChangedHandler onChangedHandler = DevDeviceList.Changed;
				DevDeviceList.OnChangedHandler onChangedHandler2;
				do
				{
					onChangedHandler2 = onChangedHandler;
					onChangedHandler = Interlocked.CompareExchange<DevDeviceList.OnChangedHandler>(ref DevDeviceList.Changed, (DevDeviceList.OnChangedHandler)Delegate.Combine(onChangedHandler2, value), onChangedHandler);
				}
				while (onChangedHandler != onChangedHandler2);
			}
			remove
			{
				DevDeviceList.OnChangedHandler onChangedHandler = DevDeviceList.Changed;
				DevDeviceList.OnChangedHandler onChangedHandler2;
				do
				{
					onChangedHandler2 = onChangedHandler;
					onChangedHandler = Interlocked.CompareExchange<DevDeviceList.OnChangedHandler>(ref DevDeviceList.Changed, (DevDeviceList.OnChangedHandler)Delegate.Remove(onChangedHandler2, value), onChangedHandler);
				}
				while (onChangedHandler != onChangedHandler2);
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
			bool result;
			for (int i = 0; i < devices.Length; i++)
			{
				DevDevice devDevice = devices[i];
				if (devDevice.id == deviceId)
				{
					device = devDevice;
					result = true;
					return result;
				}
			}
			device = default(DevDevice);
			result = false;
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern DevDevice[] GetDevices();

		internal static void Update(string target, DevDevice[] devices)
		{
			DevDeviceList.UpdateInternal(target, devices);
			DevDeviceList.OnChanged();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateInternal(string target, DevDevice[] devices);
	}
}
