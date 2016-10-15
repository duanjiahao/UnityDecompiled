using System;

namespace UnityEngine
{
	[Obsolete("iPhoneKeyboard class is deprecated. Please use TouchScreenKeyboard instead (UnityUpgradable) -> TouchScreenKeyboard", true)]
	public class iPhoneKeyboard
	{
		public string text
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public static bool hideInput
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool active
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool done
		{
			get
			{
				return false;
			}
		}

		public static Rect area
		{
			get
			{
				return default(Rect);
			}
		}

		public static bool visible
		{
			get
			{
				return false;
			}
		}
	}
}
