using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class UISystemProfilerRenderService : IDisposable
	{
		private class LRUCache
		{
			private int m_Capacity;

			private Dictionary<long, Texture2D> m_Cache;

			private List<long> m_CacheQueue;

			private int m_CacheQueueFront;

			public LRUCache(int capacity)
			{
				if (capacity <= 0)
				{
					capacity = 16;
				}
				this.m_Capacity = capacity;
				this.m_Cache = new Dictionary<long, Texture2D>(this.m_Capacity);
				IEnumerable<long> collection = from value in Enumerable.Repeat<long>(-1L, this.m_Capacity)
				select value;
				this.m_CacheQueue = new List<long>(collection);
				this.m_CacheQueueFront = 0;
			}

			public void Clear()
			{
				foreach (long current in this.m_CacheQueue)
				{
					Texture2D t;
					if (this.m_Cache.TryGetValue(current, out t))
					{
						ProfilerProperty.ReleaseUISystemProfilerRender(t);
					}
				}
				this.m_Cache.Clear();
				this.m_CacheQueue.Clear();
				IEnumerable<long> collection = from value in Enumerable.Repeat<long>(-1L, this.m_Capacity)
				select value;
				this.m_CacheQueue.AddRange(collection);
				this.m_CacheQueueFront = 0;
			}

			public void Add(long key, Texture2D data)
			{
				if (this.Get(key) == null)
				{
					if (this.m_CacheQueue[this.m_CacheQueueFront] != -1L)
					{
						long key2 = this.m_CacheQueue[this.m_CacheQueueFront];
						Texture2D t;
						if (this.m_Cache.TryGetValue(key2, out t))
						{
							this.m_Cache.Remove(key2);
							ProfilerProperty.ReleaseUISystemProfilerRender(t);
						}
					}
					this.m_CacheQueue[this.m_CacheQueueFront] = key;
					this.m_Cache[key] = data;
					this.m_CacheQueueFront++;
					if (this.m_CacheQueueFront == this.m_Capacity)
					{
						this.m_CacheQueueFront = 0;
					}
				}
			}

			public Texture2D Get(long key)
			{
				Texture2D texture2D;
				Texture2D result;
				if (this.m_Cache.TryGetValue(key, out texture2D))
				{
					this.m_CacheQueue[this.m_CacheQueue.IndexOf(key)] = this.m_CacheQueue[this.m_CacheQueueFront];
					this.m_CacheQueue[this.m_CacheQueueFront] = key;
					this.m_CacheQueueFront++;
					if (this.m_CacheQueueFront == this.m_Capacity)
					{
						this.m_CacheQueueFront = 0;
					}
					result = texture2D;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		private UISystemProfilerRenderService.LRUCache m_Cache;

		private bool m_Disposed;

		public UISystemProfilerRenderService()
		{
			this.m_Cache = new UISystemProfilerRenderService.LRUCache(10);
		}

		public void Dispose()
		{
			this.m_Disposed = true;
			this.m_Cache.Clear();
		}

		private Texture2D Generate(int renderDataIndex, int renderDataCount, bool overdraw)
		{
			return (!this.m_Disposed) ? ProfilerProperty.UISystemProfilerRender(renderDataIndex, renderDataCount, overdraw) : null;
		}

		public Texture2D GetThumbnail(int renderDataIndex, int infoRenderDataCount, bool overdraw)
		{
			Texture2D result;
			if (this.m_Disposed)
			{
				result = null;
			}
			else
			{
				long key = (long)((ushort)renderDataIndex) << 32 | (long)((ushort)infoRenderDataCount & 32767) | (long)((!overdraw) ? 0 : 32768);
				Texture2D texture2D = this.m_Cache.Get(key);
				if (texture2D == null)
				{
					texture2D = this.Generate(renderDataIndex, infoRenderDataCount, overdraw);
					if (texture2D != null)
					{
						this.m_Cache.Add(key, texture2D);
					}
				}
				result = texture2D;
			}
			return result;
		}
	}
}
