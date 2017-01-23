using System;
using System.Threading;

namespace UnityEditor.Hardware
{
	public sealed class Usb
	{
		public delegate void OnDevicesChangedHandler(UsbDevice[] devices);

		public static event Usb.OnDevicesChangedHandler DevicesChanged
		{
			add
			{
				Usb.OnDevicesChangedHandler onDevicesChangedHandler = Usb.DevicesChanged;
				Usb.OnDevicesChangedHandler onDevicesChangedHandler2;
				do
				{
					onDevicesChangedHandler2 = onDevicesChangedHandler;
					onDevicesChangedHandler = Interlocked.CompareExchange<Usb.OnDevicesChangedHandler>(ref Usb.DevicesChanged, (Usb.OnDevicesChangedHandler)Delegate.Combine(onDevicesChangedHandler2, value), onDevicesChangedHandler);
				}
				while (onDevicesChangedHandler != onDevicesChangedHandler2);
			}
			remove
			{
				Usb.OnDevicesChangedHandler onDevicesChangedHandler = Usb.DevicesChanged;
				Usb.OnDevicesChangedHandler onDevicesChangedHandler2;
				do
				{
					onDevicesChangedHandler2 = onDevicesChangedHandler;
					onDevicesChangedHandler = Interlocked.CompareExchange<Usb.OnDevicesChangedHandler>(ref Usb.DevicesChanged, (Usb.OnDevicesChangedHandler)Delegate.Remove(onDevicesChangedHandler2, value), onDevicesChangedHandler);
				}
				while (onDevicesChangedHandler != onDevicesChangedHandler2);
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
