using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public static class VRSettings
	{
		public static extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showDeviceView
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float renderScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int eyeTextureWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int eyeTextureHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static float renderViewportScale
		{
			get
			{
				return VRSettings.renderViewportScaleInternal;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
				}
				VRSettings.renderViewportScaleInternal = value;
			}
		}

		internal static extern float renderViewportScaleInternal
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("loadedDevice is deprecated.  Use loadedDeviceName and LoadDeviceByName instead.")]
		public static VRDeviceType loadedDevice
		{
			get
			{
				VRDeviceType result = VRDeviceType.Unknown;
				try
				{
					result = (VRDeviceType)((int)Enum.Parse(typeof(VRDeviceType), VRSettings.loadedDeviceName, true));
				}
				catch (Exception)
				{
				}
				return result;
			}
			set
			{
				VRSettings.LoadDeviceByName(value.ToString());
			}
		}

		public static extern string loadedDeviceName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string[] supportedDevices
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static void LoadDeviceByName(string deviceName)
		{
			VRSettings.LoadDeviceByName(new string[]
			{
				deviceName
			});
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);
	}
}
