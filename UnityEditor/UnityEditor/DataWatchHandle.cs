using System;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor
{
	internal class DataWatchHandle : IDataWatchHandle, IDisposable
	{
		public readonly int id;

		public WeakReference service;

		public UnityEngine.Object watched
		{
			get;
			private set;
		}

		public bool disposed
		{
			get
			{
				return object.ReferenceEquals(this.watched, null);
			}
		}

		public DataWatchHandle(int id, DataWatchService service, UnityEngine.Object watched)
		{
			this.id = id;
			this.service = new WeakReference(service);
			this.watched = watched;
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				throw new InvalidOperationException("DataWatchHandle was already disposed of");
			}
			if (this.service != null && this.service.IsAlive)
			{
				(this.service.Target as DataWatchService).RemoveWatch(this);
			}
			this.service = null;
			this.watched = null;
		}
	}
}
