using System;

namespace UnityEngine
{
	internal interface IGUIUtility
	{
		int hotControl
		{
			get;
			set;
		}

		int keyboardControl
		{
			get;
			set;
		}

		int GetPermanentControlID();

		int GetControlID(int hint, FocusType focus);
	}
}
