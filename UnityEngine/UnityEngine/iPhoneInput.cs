using System;

namespace UnityEngine
{
	[Obsolete("iPhoneInput class is deprecated. Please use Input instead (UnityUpgradable) -> Input", true)]
	public class iPhoneInput
	{
		[Obsolete("orientation property is deprecated. Please use Input.deviceOrientation instead (UnityUpgradable) -> Input.deviceOrientation", true)]
		public static iPhoneOrientation orientation
		{
			get
			{
				return iPhoneOrientation.Unknown;
			}
		}

		[Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.", true)]
		public static LocationInfo lastLocation
		{
			get
			{
				return default(LocationInfo);
			}
		}

		public static iPhoneAccelerationEvent[] accelerationEvents
		{
			get
			{
				return null;
			}
		}

		public static iPhoneTouch[] touches
		{
			get
			{
				return null;
			}
		}

		public static int touchCount
		{
			get
			{
				return 0;
			}
		}

		public static bool multiTouchEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public static int accelerationEventCount
		{
			get
			{
				return 0;
			}
		}

		public static Vector3 acceleration
		{
			get
			{
				return default(Vector3);
			}
		}

		public static iPhoneTouch GetTouch(int index)
		{
			return default(iPhoneTouch);
		}

		public static iPhoneAccelerationEvent GetAccelerationEvent(int index)
		{
			return default(iPhoneAccelerationEvent);
		}
	}
}
