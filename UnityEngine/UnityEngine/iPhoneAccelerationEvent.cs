using System;
namespace UnityEngine
{
	[Obsolete("iPhoneAccelerationEvent struct is deprecated (UnityUpgradable). Please use AccelerationEvent instead.", true)]
	public struct iPhoneAccelerationEvent
	{
		[Obsolete("timeDelta property is deprecated (UnityUpgradable). Please use AccelerationEvent.deltaTime instead.", true)]
		public float timeDelta
		{
			get
			{
				return 0f;
			}
		}
	}
}
