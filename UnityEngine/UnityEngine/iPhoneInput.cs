using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class iPhoneInput
	{
		[Obsolete("accelerationEvents property is deprecated. Please use Input.accelerationEvents instead.")]
		public static iPhoneAccelerationEvent[] accelerationEvents
		{
			get
			{
				int accelerationEventCount = iPhoneInput.accelerationEventCount;
				iPhoneAccelerationEvent[] array = new iPhoneAccelerationEvent[accelerationEventCount];
				for (int i = 0; i < accelerationEventCount; i++)
				{
					array[i] = iPhoneInput.GetAccelerationEvent(i);
				}
				return array;
			}
		}
		[Obsolete("touches property is deprecated. Please use Input.touches instead.")]
		public static iPhoneTouch[] touches
		{
			get
			{
				int touchCount = iPhoneInput.touchCount;
				iPhoneTouch[] array = new iPhoneTouch[touchCount];
				for (int i = 0; i < touchCount; i++)
				{
					array[i] = iPhoneInput.GetTouch(i);
				}
				return array;
			}
		}
		[Obsolete("touchCount property is deprecated. Please use Input.touchCount instead.")]
		public static extern int touchCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("multiTouchEnabled property is deprecated. Please use Input.multiTouchEnabled instead.")]
		public static extern bool multiTouchEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("accelerationEventCount property is deprecated. Please use Input.accelerationEventCount instead.")]
		public static extern int accelerationEventCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("acceleration property is deprecated. Please use Input.acceleration instead.")]
		public static extern Vector3 acceleration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("orientation property is deprecated. Please use Input.deviceOrientation instead.")]
		public static extern iPhoneOrientation orientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.")]
		public static extern LocationInfo lastLocation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[Obsolete("GetTouch method is deprecated. Please use Input.GetTouch instead."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern iPhoneTouch GetTouch(int index);
		[Obsolete("GetAccelerationEvent method is deprecated. Please use Input.GetAccelerationEvent instead."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern iPhoneAccelerationEvent GetAccelerationEvent(int index);
	}
}
