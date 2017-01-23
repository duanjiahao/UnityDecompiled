using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Connect
{
	internal class UnityAdsSettings
	{
		[ThreadAndSerializationSafe]
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool initializeOnStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool testMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPlatformEnabled(RuntimePlatform platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPlatformEnabled(RuntimePlatform platform, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetGameId(RuntimePlatform platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGameId(RuntimePlatform platform, string gameId);
	}
}
