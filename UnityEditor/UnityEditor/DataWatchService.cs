using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor
{
	internal class DataWatchService : IDataWatchService
	{
		private struct Spy
		{
			public readonly int handleID;

			public readonly VisualElement watcher;

			public readonly Action onDataChanged;

			public Spy(int handleID, VisualElement watcher, Action onDataChanged)
			{
				this.handleID = handleID;
				this.watcher = watcher;
				this.onDataChanged = onDataChanged;
			}
		}

		private struct Watchers
		{
			public List<DataWatchService.Spy> spyList;

			public ChangeTrackerHandle tracker;
		}

		private HashSet<UnityEngine.Object> m_DirtySet = new HashSet<UnityEngine.Object>();

		private Dictionary<int, DataWatchHandle> m_Handles = new Dictionary<int, DataWatchHandle>();

		private Dictionary<UnityEngine.Object, DataWatchService.Watchers> m_Watched = new Dictionary<UnityEngine.Object, DataWatchService.Watchers>();

		private static int s_WatchID;

		public DataWatchService()
		{
			Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostProcessUndo));
		}

		~DataWatchService()
		{
			Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostProcessUndo));
		}

		public UndoPropertyModification[] PostProcessUndo(UndoPropertyModification[] modifications)
		{
			for (int i = 0; i < modifications.Length; i++)
			{
				UndoPropertyModification undoPropertyModification = modifications[i];
				PropertyModification currentValue = undoPropertyModification.currentValue;
				if (currentValue != null && !(currentValue.target == null))
				{
					if (this.m_Watched.ContainsKey(currentValue.target))
					{
						this.m_DirtySet.Add(currentValue.target);
					}
				}
			}
			return modifications;
		}

		public void PollNativeData()
		{
			foreach (KeyValuePair<UnityEngine.Object, DataWatchService.Watchers> current in this.m_Watched)
			{
				if (current.Key == null || current.Value.tracker.PollForChanges())
				{
					this.m_DirtySet.Add(current.Key);
				}
			}
		}

		public void ProcessNotificationQueue()
		{
			this.PollNativeData();
			HashSet<UnityEngine.Object> dirtySet = this.m_DirtySet;
			this.m_DirtySet = new HashSet<UnityEngine.Object>();
			List<DataWatchService.Spy> list = new List<DataWatchService.Spy>();
			foreach (UnityEngine.Object current in dirtySet)
			{
				DataWatchService.Watchers watchers;
				if (this.m_Watched.TryGetValue(current, out watchers))
				{
					list.Clear();
					list.AddRange(watchers.spyList);
					foreach (DataWatchService.Spy current2 in list)
					{
						if (current2.watcher.panel != null)
						{
							current2.onDataChanged();
						}
						else
						{
							Debug.Log("Leaking Data Spies from element: " + current2.watcher);
						}
					}
				}
			}
			dirtySet.Clear();
		}

		public IDataWatchHandle AddWatch(VisualElement watcher, UnityEngine.Object watched, Action onDataChanged)
		{
			if (watched == null)
			{
				throw new ArgumentException("Object watched cannot be null");
			}
			DataWatchHandle dataWatchHandle = new DataWatchHandle(++DataWatchService.s_WatchID, this, watched);
			this.m_Handles[dataWatchHandle.id] = dataWatchHandle;
			DataWatchService.Watchers value;
			if (!this.m_Watched.TryGetValue(watched, out value))
			{
				value = new DataWatchService.Watchers
				{
					spyList = new List<DataWatchService.Spy>(),
					tracker = ChangeTrackerHandle.AcquireTracker(watched)
				};
				this.m_Watched[watched] = value;
			}
			value.spyList.Add(new DataWatchService.Spy(dataWatchHandle.id, watcher, onDataChanged));
			return dataWatchHandle;
		}

		public void RemoveWatch(IDataWatchHandle handle)
		{
			DataWatchHandle dataWatchHandle = (DataWatchHandle)handle;
			if (this.m_Handles.Remove(dataWatchHandle.id))
			{
				DataWatchService.Watchers watchers;
				if (this.m_Watched.TryGetValue(dataWatchHandle.watched, out watchers))
				{
					List<DataWatchService.Spy> spyList = watchers.spyList;
					for (int i = 0; i < spyList.Count; i++)
					{
						if (spyList[i].handleID == dataWatchHandle.id)
						{
							spyList.RemoveAt(i);
							if (watchers.spyList.Count == 0)
							{
								watchers.tracker.ReleaseTracker();
								this.m_Watched.Remove(dataWatchHandle.watched);
							}
							return;
						}
					}
				}
			}
			throw new ArgumentException("Data watch was not registered");
		}
	}
}
