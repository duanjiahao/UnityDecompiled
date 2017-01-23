using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Caching
	{
		[Obsolete("this API is not for public use.")]
		public static extern CacheIndex[] index
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern long spaceFree
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern long maximumAvailableDiskSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern long spaceOccupied
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Please use Caching.spaceFree instead")]
		public static extern int spaceAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Please use Caching.spaceOccupied instead")]
		public static extern int spaceUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int expirationDelay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool compressionEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool ready
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool Authorize(string name, string domain, long size, string signature)
		{
			return Caching.Authorize(name, domain, size, -1, signature);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Authorize(string name, string domain, long size, int expiration, string signature);

		[Obsolete("Size is now specified as a long")]
		public static bool Authorize(string name, string domain, int size, int expiration, string signature)
		{
			return Caching.Authorize(name, domain, (long)size, expiration, signature);
		}

		[Obsolete("Size is now specified as a long")]
		public static bool Authorize(string name, string domain, int size, string signature)
		{
			return Caching.Authorize(name, domain, (long)size, signature);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CleanCache();

		[Obsolete("this API is not for public use.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CleanNamedCache(string name);

		[Obsolete("This function is obsolete and has no effect.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DeleteFromCache(string url);

		[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetVersionFromCache(string url);

		public static bool IsVersionCached(string url, int version)
		{
			Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
			return Caching.IsVersionCached(url, hash);
		}

		public static bool IsVersionCached(string url, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_IsVersionCached(url, ref hash);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsVersionCached(string url, ref Hash128 hash);

		public static bool MarkAsUsed(string url, int version)
		{
			Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
			return Caching.MarkAsUsed(url, hash);
		}

		public static bool MarkAsUsed(string url, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_MarkAsUsed(url, ref hash);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_MarkAsUsed(string url, ref Hash128 hash);

		public static void SetNoBackupFlag(string url, int version)
		{
		}

		public static void SetNoBackupFlag(string url, Hash128 hash)
		{
			Caching.INTERNAL_CALL_SetNoBackupFlag(url, ref hash);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetNoBackupFlag(string url, ref Hash128 hash);

		public static void ResetNoBackupFlag(string url, int version)
		{
		}

		public static void ResetNoBackupFlag(string url, Hash128 hash)
		{
			Caching.INTERNAL_CALL_ResetNoBackupFlag(url, ref hash);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ResetNoBackupFlag(string url, ref Hash128 hash);
	}
}
