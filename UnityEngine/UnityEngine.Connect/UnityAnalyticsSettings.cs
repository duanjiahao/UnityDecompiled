using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Connect
{
	internal class UnityAnalyticsSettings
	{
		public static extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
