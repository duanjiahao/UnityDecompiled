using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public sealed class Device
	{
		public static extern DeviceGeneration generation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string systemVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string vendorIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string advertisingIdentifier
		{
			get
			{
				string advertisingIdentifier = Device.GetAdvertisingIdentifier();
				Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, Device.advertisingTrackingEnabled);
				return advertisingIdentifier;
			}
		}

		public static extern bool advertisingTrackingEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNoBackupFlag(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetNoBackupFlag(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAdvertisingIdentifier();
	}
}
