using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public static class OnDemandResources
	{
		public static extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static OnDemandResourcesRequest PreloadAsync(string[] tags)
		{
			return OnDemandResources.PreloadAsyncInternal(tags);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern OnDemandResourcesRequest PreloadAsyncInternal(string[] tags);
	}
}
