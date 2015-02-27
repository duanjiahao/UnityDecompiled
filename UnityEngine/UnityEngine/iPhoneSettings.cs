using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class iPhoneSettings
	{
		[Obsolete("screenOrientation property is deprecated. Please use Screen.orientation instead.")]
		public static extern iPhoneScreenOrientation screenOrientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("verticalOrientation property is deprecated. Please use Screen.orientation instead.")]
		public static extern bool verticalOrientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("screenCanDarken property is deprecated. Please use Screen.sleepTimeout instead.")]
		public static extern bool screenCanDarken
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("uniqueIdentifier property is deprecated. Please use SystemInfo.deviceUniqueIdentifier instead.")]
		public static extern string uniqueIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("name property is deprecated. Please use SystemInfo.deviceName instead.")]
		public static extern string name
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("model property is deprecated. Please use SystemInfo.deviceModel instead.")]
		public static extern string model
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("systemName property is deprecated. Please use SystemInfo.operatingSystem instead.")]
		public static extern string systemName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("systemVersion property is deprecated. Please use SystemInfo.operatingSystem instead.")]
		public static extern string systemVersion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("internetReachability property is deprecated. Please use Application.internetReachability instead.")]
		public static extern iPhoneNetworkReachability internetReachability
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("generation property is deprecated. Please use iPhone.generation instead.")]
		public static extern iPhoneGeneration generation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("locationServiceStatus property is deprecated. Please use Input.location.status instead.")]
		public static extern LocationServiceStatus locationServiceStatus
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("locationServiceEnabledByUser property is deprecated. Please use Input.location.isEnabledByUser instead.")]
		public static extern bool locationServiceEnabledByUser
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartLocationServiceUpdates(float desiredAccuracyInMeters, float updateDistanceInMeters);
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates(float desiredAccuracyInMeters)
		{
			iPhoneSettings.StartLocationServiceUpdates(desiredAccuracyInMeters, 10f);
		}
		[Obsolete("StartLocationServiceUpdates method is deprecated. Please use Input.location.Start instead.")]
		public static void StartLocationServiceUpdates()
		{
			iPhoneSettings.StartLocationServiceUpdates(10f, 10f);
		}
		[Obsolete("StopLocationServiceUpdates method is deprecated. Please use Input.location.Stop instead."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopLocationServiceUpdates();
	}
}
