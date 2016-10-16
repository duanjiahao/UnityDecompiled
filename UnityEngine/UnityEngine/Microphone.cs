using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Microphone
	{
		public static extern string[] devices
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void End(string deviceName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsRecording(string deviceName);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPosition(string deviceName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetDeviceCaps(string deviceName, out int minFreq, out int maxFreq);
	}
}
