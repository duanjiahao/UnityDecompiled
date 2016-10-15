using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public static class VRDevice
	{
		public static extern bool isPresent
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("family is deprecated.  Use VRSettings.loadedDeviceName instead.")]
		public static extern string family
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string model
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float refreshRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static IntPtr GetNativePtr()
		{
			IntPtr result;
			VRDevice.INTERNAL_CALL_GetNativePtr(out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);
	}
}
