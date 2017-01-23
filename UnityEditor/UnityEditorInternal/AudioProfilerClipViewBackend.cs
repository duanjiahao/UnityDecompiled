using System;
using System.Collections.Generic;

namespace UnityEditorInternal
{
	internal class AudioProfilerClipViewBackend
	{
		public delegate void DataUpdateDelegate();

		public AudioProfilerClipViewBackend.DataUpdateDelegate OnUpdate;

		public AudioProfilerClipTreeViewState m_TreeViewState;

		public List<AudioProfilerClipInfoWrapper> items
		{
			get;
			private set;
		}

		public AudioProfilerClipViewBackend(AudioProfilerClipTreeViewState state)
		{
			this.m_TreeViewState = state;
			this.items = new List<AudioProfilerClipInfoWrapper>();
		}

		public void SetData(List<AudioProfilerClipInfoWrapper> data)
		{
			this.items = data;
			this.UpdateSorting();
		}

		public void UpdateSorting()
		{
			this.items.Sort(new AudioProfilerClipInfoHelper.AudioProfilerClipInfoComparer((AudioProfilerClipInfoHelper.ColumnIndices)this.m_TreeViewState.selectedColumn, (AudioProfilerClipInfoHelper.ColumnIndices)this.m_TreeViewState.prevSelectedColumn, this.m_TreeViewState.sortByDescendingOrder));
			if (this.OnUpdate != null)
			{
				this.OnUpdate();
			}
		}
	}
}
