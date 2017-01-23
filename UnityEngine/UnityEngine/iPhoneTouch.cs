using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Obsolete("iPhoneTouch struct is deprecated. Please use Touch instead (UnityUpgradable) -> Touch", true)]
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct iPhoneTouch
	{
		[Obsolete("positionDelta property is deprecated. Please use Touch.deltaPosition instead (UnityUpgradable) -> Touch.deltaPosition", true)]
		public Vector2 positionDelta
		{
			get
			{
				return default(Vector2);
			}
		}

		[Obsolete("timeDelta property is deprecated. Please use Touch.deltaTime instead (UnityUpgradable) -> Touch.deltaTime", true)]
		public float timeDelta
		{
			get
			{
				return 0f;
			}
		}

		public int fingerId
		{
			get
			{
				return 0;
			}
		}

		public Vector2 position
		{
			get
			{
				return default(Vector2);
			}
		}

		public Vector2 deltaPosition
		{
			get
			{
				return default(Vector2);
			}
		}

		public float deltaTime
		{
			get
			{
				return 0f;
			}
		}

		public int tapCount
		{
			get
			{
				return 0;
			}
		}

		public iPhoneTouchPhase phase
		{
			get
			{
				return iPhoneTouchPhase.Began;
			}
		}
	}
}
