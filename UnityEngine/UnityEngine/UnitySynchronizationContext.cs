using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class UnitySynchronizationContext : SynchronizationContext
	{
		private struct WorkRequest
		{
			private readonly SendOrPostCallback m_DelagateCallback;

			private readonly object m_DelagateState;

			public WorkRequest(SendOrPostCallback callback, object state)
			{
				this.m_DelagateCallback = callback;
				this.m_DelagateState = state;
			}

			public void Invoke()
			{
				this.m_DelagateCallback(this.m_DelagateState);
			}
		}

		private const int kAwqInitialCapacity = 20;

		private readonly Queue<UnitySynchronizationContext.WorkRequest> m_AsyncWorkQueue = new Queue<UnitySynchronizationContext.WorkRequest>(20);

		public override void Send(SendOrPostCallback callback, object state)
		{
			callback(state);
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			object asyncWorkQueue = this.m_AsyncWorkQueue;
			lock (asyncWorkQueue)
			{
				this.m_AsyncWorkQueue.Enqueue(new UnitySynchronizationContext.WorkRequest(callback, state));
			}
		}

		private void Exec()
		{
			object asyncWorkQueue = this.m_AsyncWorkQueue;
			lock (asyncWorkQueue)
			{
				while (this.m_AsyncWorkQueue.Count > 0)
				{
					this.m_AsyncWorkQueue.Dequeue().Invoke();
				}
			}
		}

		[RequiredByNativeCode]
		private static void InitializeSynchronizationContext()
		{
			if (SynchronizationContext.Current == null)
			{
				SynchronizationContext.SetSynchronizationContext(new UnitySynchronizationContext());
			}
		}

		[RequiredByNativeCode]
		private static void ExecuteTasks()
		{
			UnitySynchronizationContext unitySynchronizationContext = SynchronizationContext.Current as UnitySynchronizationContext;
			if (unitySynchronizationContext != null)
			{
				unitySynchronizationContext.Exec();
			}
		}
	}
}
