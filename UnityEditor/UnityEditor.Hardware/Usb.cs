using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.Hardware
{
	public sealed class Usb
	{
		public delegate void OnDevicesChangedHandler(UsbDevice[] devices);

		public static event Usb.OnDevicesChangedHandler DevicesChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Usb.DevicesChanged = (Usb.OnDevicesChangedHandler)Delegate.Combine(Usb.DevicesChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Usb.DevicesChanged = (Usb.OnDevicesChangedHandler)Delegate.Remove(Usb.DevicesChanged, value);
			}
		}

		public static void OnDevicesChanged(UsbDevice[] devices)
		{
			if (Usb.DevicesChanged != null && devices != null)
			{
				Usb.DevicesChanged(devices);
			}
		}
	}
}
