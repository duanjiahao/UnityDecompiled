using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ProfilerDetailedObjectsView : ProfilerDetailedView
	{
		private ProfilerHierarchyGUI m_ProfilerHierarchyGUI;

		public ProfilerDetailedObjectsView(ProfilerHierarchyGUI profilerHierarchyGUI, ProfilerHierarchyGUI mainProfilerHierarchyGUI) : base(mainProfilerHierarchyGUI)
		{
			this.m_ProfilerHierarchyGUI = profilerHierarchyGUI;
		}

		public void DoGUI(GUIStyle headerStyle, int frameIndex, ProfilerViewType viewType)
		{
			ProfilerProperty detailedProperty = this.GetDetailedProperty(frameIndex, viewType, this.m_ProfilerHierarchyGUI.sortType);
			if (detailedProperty != null)
			{
				this.m_ProfilerHierarchyGUI.DoGUI(detailedProperty, string.Empty, false);
			}
			else
			{
				base.DrawEmptyPane(headerStyle);
			}
		}

		private ProfilerProperty GetDetailedProperty(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType)
		{
			ProfilerProperty result;
			if (this.m_CachedProfilerPropertyConfig.EqualsTo(frameIndex, viewType, sortType))
			{
				result = this.m_CachedProfilerProperty;
			}
			else
			{
				ProfilerProperty detailedProperty = this.m_MainProfilerHierarchyGUI.GetDetailedProperty();
				if (this.m_CachedProfilerProperty != null)
				{
					this.m_CachedProfilerProperty.Cleanup();
				}
				this.m_CachedProfilerPropertyConfig.Set(frameIndex, viewType, sortType);
				this.m_CachedProfilerProperty = detailedProperty;
				result = detailedProperty;
			}
			return result;
		}
	}
}
