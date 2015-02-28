using System;
namespace UnityEngine
{
	[Obsolete("iPhoneTouch struct is deprecated (UnityUpgradable). Please use Touch instead.", true)]
	public struct iPhoneTouch
	{
		[Obsolete("positionDelta property is deprecated (UnityUpgradable). Please use Touch.deltaPosition instead.", true)]
		public Vector2 positionDelta
		{
			get
			{
				return default(Vector2);
			}
		}
		[Obsolete("timeDelta property is deprecated (UnityUpgradable). Please use Touch.deltaTime instead.", true)]
		public float timeDelta
		{
			get
			{
				return 0f;
			}
		}
	}
}
