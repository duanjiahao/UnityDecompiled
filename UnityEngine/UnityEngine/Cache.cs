using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public struct Cache
	{
		private int m_Handle;

		internal int handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public bool valid
		{
			get
			{
				return Cache.IsValidInternal(this.m_Handle);
			}
		}

		public bool readOnly
		{
			get
			{
				return Cache.IsReadonlyInternal(this.m_Handle);
			}
		}

		public string path
		{
			get
			{
				return Cache.GetPathInternal(this.m_Handle);
			}
		}

		public int index
		{
			get
			{
				return Cache.GetIndexInternal(this.m_Handle);
			}
		}

		public long spaceFree
		{
			get
			{
				return Cache.GetSpaceFreeInternal(this.m_Handle);
			}
		}

		public long maximumAvailableStorageSpace
		{
			get
			{
				return Cache.GetMaximumDiskSpaceAvailableInternal(this.m_Handle);
			}
			set
			{
				Cache.SetMaximumDiskSpaceAvailableInternal(this.m_Handle, value);
			}
		}

		public long spaceOccupied
		{
			get
			{
				return Cache.GetCachingDiskSpaceUsedInternal(this.m_Handle);
			}
		}

		public int expirationDelay
		{
			get
			{
				return Cache.GetExpirationDelay(this.m_Handle);
			}
			set
			{
				Cache.SetExpirationDelay(this.m_Handle, value);
			}
		}

		public bool ready
		{
			get
			{
				return Cache.IsReadyInternal(this.m_Handle);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsReadonlyInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetPathInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndexInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetSpaceFreeInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetMaximumDiskSpaceAvailableInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaximumDiskSpaceAvailableInternal(int handle, long value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetCachingDiskSpaceUsedInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetExpirationDelay(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetExpirationDelay(int handle, int expiration);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsReadyInternal(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearCacheInternal(int handle);

		public bool ClearCache()
		{
			return Cache.ClearCacheInternal(this.m_Handle);
		}

		public static bool operator ==(Cache lhs, Cache rhs)
		{
			return lhs.handle == rhs.handle;
		}

		public static bool operator !=(Cache lhs, Cache rhs)
		{
			return lhs.handle != rhs.handle;
		}

		public override int GetHashCode()
		{
			return this.m_Handle;
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is Cache))
			{
				result = false;
			}
			else
			{
				Cache cache = (Cache)other;
				result = (this.handle == cache.handle);
			}
			return result;
		}
	}
}
