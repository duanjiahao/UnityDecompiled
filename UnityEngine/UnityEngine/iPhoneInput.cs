using System;
namespace UnityEngine
{
	[Obsolete("iPhoneInput class is deprecated (UnityUpgradable). Please use Input instead", true)]
	public sealed class iPhoneInput
	{
		[Obsolete("orientation property is deprecated (UnityUpgradable). Please use Input.deviceOrientation instead.", true)]
		public static iPhoneOrientation orientation
		{
			get
			{
				return (iPhoneOrientation)0;
			}
		}
		[Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.", true)]
		private static LocationInfo lastLocation
		{
			get
			{
				return default(LocationInfo);
			}
		}
	}
}
