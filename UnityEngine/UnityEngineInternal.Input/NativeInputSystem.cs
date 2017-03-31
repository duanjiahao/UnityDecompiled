using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Input
{
	public sealed class NativeInputSystem
	{
		public static NativeUpdateCallback onUpdate;

		public static NativeEventCallback onEvents;

		public static NativeDeviceDiscoveredCallback onDeviceDiscovered;

		internal static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern double zeroEventTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[RequiredByNativeCode]
		internal static void NotifyUpdate(NativeInputUpdateType updateType)
		{
			NativeUpdateCallback nativeUpdateCallback = NativeInputSystem.onUpdate;
			if (nativeUpdateCallback != null)
			{
				nativeUpdateCallback(updateType);
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyEvents(int eventCount, IntPtr eventData)
		{
			NativeEventCallback nativeEventCallback = NativeInputSystem.onEvents;
			if (nativeEventCallback != null)
			{
				nativeEventCallback(eventCount, eventData);
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyDeviceDiscovered(NativeInputDeviceInfo deviceInfo)
		{
			NativeDeviceDiscoveredCallback nativeDeviceDiscoveredCallback = NativeInputSystem.onDeviceDiscovered;
			if (nativeDeviceDiscoveredCallback != null)
			{
				nativeDeviceDiscoveredCallback(deviceInfo);
			}
		}

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SendInput(ref NativeInputEvent inputEvent);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDeviceConfiguration(int deviceId);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetControlConfiguration(int deviceId, int controlIndex);

		public static void SetPollingFrequency(float hertz)
		{
			if (hertz < 1f)
			{
				throw new ArgumentException("Polling frequency cannot be less than 1Hz");
			}
			NativeInputSystem.SetPollingFrequencyInternal(hertz);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPollingFrequencyInternal(float hertz);
	}
}
