using System;

namespace UnityEngine
{
	internal class GUIUtilitySystem : IGUIUtility
	{
		public int hotControl
		{
			get
			{
				return GUIUtility.hotControl;
			}
			set
			{
				GUIUtility.hotControl = value;
			}
		}

		public int keyboardControl
		{
			get
			{
				return GUIUtility.keyboardControl;
			}
			set
			{
				GUIUtility.keyboardControl = value;
			}
		}

		public int GetPermanentControlID()
		{
			return GUIUtility.GetPermanentControlID();
		}

		public int GetControlID(int hint, FocusType focus)
		{
			return GUIUtility.GetControlID(hint, focus);
		}
	}
}
