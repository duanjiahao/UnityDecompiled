using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public sealed class Device
	{
		public static extern DeviceGeneration generation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string systemVersion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string vendorIdentifier
		{
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNoBackupFlag(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetNoBackupFlag(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAdvertisingIdentifier();
	}
}
