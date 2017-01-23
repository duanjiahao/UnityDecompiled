using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	internal sealed class OSUtil
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetDefaultApps(string fileType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetAppFriendlyName(string app);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDefaultAppPath(string fileType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDefaultCachePath();
	}
}
