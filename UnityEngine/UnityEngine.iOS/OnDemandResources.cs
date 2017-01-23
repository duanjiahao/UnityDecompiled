using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.iOS
{
	public static class OnDemandResources
	{
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static OnDemandResourcesRequest PreloadAsync(string[] tags)
		{
			return OnDemandResources.PreloadAsyncInternal(tags);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern OnDemandResourcesRequest PreloadAsyncInternal(string[] tags);
	}
}
