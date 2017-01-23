using System;
using System.Collections.Generic;

namespace UnityEditorInternal
{
	internal class AudioProfilerGroupViewBackend
	{
		public delegate void DataUpdateDelegate();

		public AudioProfilerGroupViewBackend.DataUpdateDelegate OnUpdate;

		public AudioProfilerGroupTreeViewState m_TreeViewState;

		public List<AudioProfilerGroupInfoWrapper> items
		{
			get;
			private set;
		}

		public AudioProfilerGroupViewBackend(AudioProfilerGroupTreeViewState state)
		{
			this.m_TreeViewState = state;
			this.items = new List<AudioProfilerGroupInfoWrapper>();
		}

		public void SetData(List<AudioProfilerGroupInfoWrapper> data)
		{
			this.items = data;
			this.UpdateSorting();
		}

		public void UpdateSorting()
		{
			this.items.Sort(new AudioProfilerGroupInfoHelper.AudioProfilerGroupInfoComparer((AudioProfilerGroupInfoHelper.ColumnIndices)this.m_TreeViewState.selectedColumn, (AudioProfilerGroupInfoHelper.ColumnIndices)this.m_TreeViewState.prevSelectedColumn, this.m_TreeViewState.sortByDescendingOrder));
			if (this.OnUpdate != null)
			{
				this.OnUpdate();
			}
		}
	}
}
