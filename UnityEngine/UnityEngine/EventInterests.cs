using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct EventInterests
	{
		public bool wantsMouseMove
		{
			get;
			set;
		}

		public bool wantsMouseEnterLeaveWindow
		{
			get;
			set;
		}

		public bool WantsEvent(EventType type)
		{
			bool result;
			if (type != EventType.MouseEnterWindow && type != EventType.MouseLeaveWindow)
			{
				result = (type != EventType.MouseMove || this.wantsMouseMove);
			}
			else
			{
				result = this.wantsMouseEnterLeaveWindow;
			}
			return result;
		}
	}
}
