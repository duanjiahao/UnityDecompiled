using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Caching
	{
		[Obsolete("this API is not for public use.")]
		public static extern CacheIndex[] index
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This property is only used for the current cache, use GetSpaceFree() with cache index to get unused bytes per cache.")]
		public static extern long spaceFree
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This property is only used for the current cache, use Cache.maximumAvailableStorageSpace to get/set the maximum available storage space per cache.")]
		public static extern long maximumAvailableDiskSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("This property is only used for the current cache, use Cache.spaceOccupied to get used bytes per cache.")]
		public static extern long spaceOccupied
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Please use Cache.spaceAvailable to get unused bytes per cache.")]
		public static extern int spaceAvailable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Please use use Cache.spaceOccupied to get used bytes per cache")]
		public static extern int spaceUsed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("This property is only used for the current cache, use Cache.expirationDelay to get/set the expiration delay per cache.")]
		public static extern int expirationDelay
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool compressionEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool ready
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int cacheCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Cache defaultCache
		{
			get
			{
				Cache result;
				Caching.INTERNAL_get_defaultCache(out result);
				return result;
			}
		}

		public static Cache currentCacheForWriting
		{
			get
			{
				Cache result;
				Caching.INTERNAL_get_currentCacheForWriting(out result);
				return result;
			}
			set
			{
				Caching.INTERNAL_set_currentCacheForWriting(ref value);
			}
		}

		[Obsolete("This property is only used by web player which is not used any more.", true)]
		public static bool enabled
		{
			get
			{
				return true;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ClearCache();

		[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetVersionFromCache(string url);

		public static bool ClearCachedVersion(string assetBundleName, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_ClearCachedVersion(assetBundleName, ref hash);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ClearCachedVersion(string assetBundleName, ref Hash128 hash);

		public static bool ClearOtherCachedVersions(string assetBundleName, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_ClearOtherCachedVersions(assetBundleName, ref hash);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ClearOtherCachedVersions(string assetBundleName, ref Hash128 hash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ClearAllCachedVersions(string assetBundleName);

		public static void GetCachedVersions(string assetBundleName, List<Hash128> outCachedVersions)
		{
			if (string.IsNullOrEmpty(assetBundleName))
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			if (outCachedVersions == null)
			{
				throw new ArgumentNullException("Input outCachedVersions cannot be null.");
			}
			Caching.GetCachedVersionsInternal(assetBundleName, outCachedVersions);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Hash128[] GetCachedVersions(string assetBundleName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetCachedVersionsInternal(string assetBundleName, object cachedVersions);

		[Obsolete("This function is obsolete. Please use IsVersionCached with Hash128 instead.")]
		public static bool IsVersionCached(string url, int version)
		{
			Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
			return Caching.IsVersionCached(url, "", hash);
		}

		public static bool IsVersionCached(string url, Hash128 hash)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
			}
			return Caching.IsVersionCached(url, "", hash);
		}

		public static bool IsVersionCached(CachedAssetBundle cachedBundle)
		{
			if (string.IsNullOrEmpty(cachedBundle.name))
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.IsVersionCached("", cachedBundle.name, cachedBundle.hash);
		}

		internal static bool IsVersionCached(string url, string name, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_IsVersionCached(url, name, ref hash);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsVersionCached(string url, string name, ref Hash128 hash);

		[Obsolete("This function is obsolete. Please use MarkAsUsed with Hash128 instead.")]
		public static bool MarkAsUsed(string url, int version)
		{
			Hash128 hash = new Hash128(0u, 0u, 0u, (uint)version);
			return Caching.MarkAsUsed(url, "", hash);
		}

		public static bool MarkAsUsed(string url, Hash128 hash)
		{
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Input AssetBundle url cannot be null or empty.");
			}
			return Caching.MarkAsUsed(url, "", hash);
		}

		public static bool MarkAsUsed(CachedAssetBundle cachedBundle)
		{
			if (string.IsNullOrEmpty(cachedBundle.name))
			{
				throw new ArgumentException("Input AssetBundle name cannot be null or empty.");
			}
			return Caching.MarkAsUsed("", cachedBundle.name, cachedBundle.hash);
		}

		internal static bool MarkAsUsed(string url, string name, Hash128 hash)
		{
			return Caching.INTERNAL_CALL_MarkAsUsed(url, name, ref hash);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_MarkAsUsed(string url, string name, ref Hash128 hash);

		[Obsolete("This function is obsolete. Please use SetNoBackupFlag with Hash128 instead.")]
		public static void SetNoBackupFlag(string url, int version)
		{
		}

		public static void SetNoBackupFlag(string url, Hash128 hash)
		{
		}

		public static void SetNoBackupFlag(CachedAssetBundle cachedBundle)
		{
		}

		[Obsolete("This function is obsolete. Please use ResetNoBackupFlag with Hash128 instead.")]
		public static void ResetNoBackupFlag(string url, int version)
		{
		}

		public static void ResetNoBackupFlag(string url, Hash128 hash)
		{
		}

		public static void ResetNoBackupFlag(CachedAssetBundle cachedBundle)
		{
		}

		internal static void SetNoBackupFlag(string url, string name, Hash128 hash, bool enabled)
		{
			Caching.INTERNAL_CALL_SetNoBackupFlag(url, name, ref hash, enabled);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetNoBackupFlag(string url, string name, ref Hash128 hash, bool enabled);

		public static Cache AddCache(string cachePath)
		{
			if (string.IsNullOrEmpty(cachePath))
			{
				throw new ArgumentNullException("Cache path cannot be null or empty.");
			}
			bool isReadonly = false;
			if (cachePath.Replace('\\', '/').StartsWith(Application.streamingAssetsPath))
			{
				isReadonly = true;
			}
			else
			{
				if (!Directory.Exists(cachePath))
				{
					throw new ArgumentException("Cache path '" + cachePath + "' doesn't exist.");
				}
				if ((File.GetAttributes(cachePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					isReadonly = true;
				}
			}
			if (Caching.GetCacheByPath(cachePath).valid)
			{
				throw new InvalidOperationException("Cache with path '" + cachePath + "' has already been added.");
			}
			return Caching.AddCache_Internal(cachePath, isReadonly);
		}

		private static Cache AddCache_Internal(string cachePath, bool isReadonly)
		{
			Cache result;
			Caching.INTERNAL_CALL_AddCache_Internal(cachePath, isReadonly, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddCache_Internal(string cachePath, bool isReadonly, out Cache value);

		public static Cache GetCacheAt(int cacheIndex)
		{
			Cache result;
			Caching.INTERNAL_CALL_GetCacheAt(cacheIndex, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetCacheAt(int cacheIndex, out Cache value);

		public static Cache GetCacheByPath(string cachePath)
		{
			Cache result;
			Caching.INTERNAL_CALL_GetCacheByPath(cachePath, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetCacheByPath(string cachePath, out Cache value);

		public static void GetAllCachePaths(List<string> cachePaths)
		{
			cachePaths.Clear();
			for (int i = 0; i < Caching.cacheCount; i++)
			{
				cachePaths.Add(Caching.GetCacheAt(i).path);
			}
		}

		public static bool RemoveCache(Cache cache)
		{
			return Caching.INTERNAL_CALL_RemoveCache(ref cache);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_RemoveCache(ref Cache cache);

		public static void MoveCacheBefore(Cache src, Cache dst)
		{
			Caching.INTERNAL_CALL_MoveCacheBefore(ref src, ref dst);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveCacheBefore(ref Cache src, ref Cache dst);

		public static void MoveCacheAfter(Cache src, Cache dst)
		{
			Caching.INTERNAL_CALL_MoveCacheAfter(ref src, ref dst);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MoveCacheAfter(ref Cache src, ref Cache dst);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_defaultCache(out Cache value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_currentCacheForWriting(out Cache value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_currentCacheForWriting(ref Cache value);

		[Obsolete("This WebPlayer function is not used any more.", true)]
		public static bool Authorize(string name, string domain, long size, string signature)
		{
			return true;
		}

		[Obsolete("This WebPlayer function is not used any more.", true)]
		public static bool Authorize(string name, string domain, long size, int expiration, string signature)
		{
			return true;
		}

		[Obsolete("This WebPlayer function is not used any more.", true)]
		public static bool Authorize(string name, string domain, int size, int expiration, string signature)
		{
			return true;
		}

		[Obsolete("This WebPlayer function is not used any more.", true)]
		public static bool Authorize(string name, string domain, int size, string signature)
		{
			return true;
		}

		[Obsolete("This function is obsolete. Please use ClearCache.  (UnityUpgradable) -> ClearCache()")]
		public static bool CleanCache()
		{
			return Caching.ClearCache();
		}

		[Obsolete("This API is not for public use.")]
		public static bool CleanNamedCache(string name)
		{
			return false;
		}

		[Obsolete("This function is obsolete and has no effect.")]
		public static bool DeleteFromCache(string url)
		{
			return false;
		}
	}
}
