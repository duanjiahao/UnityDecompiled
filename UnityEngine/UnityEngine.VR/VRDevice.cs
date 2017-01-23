using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public static class VRDevice
	{
		public static extern bool isPresent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("family is deprecated.  Use VRSettings.loadedDeviceName instead.")]
		public static extern string family
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string model
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float refreshRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static IntPtr GetNativePtr()
		{
			IntPtr result;
			VRDevice.INTERNAL_CALL_GetNativePtr(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);
	}
}
