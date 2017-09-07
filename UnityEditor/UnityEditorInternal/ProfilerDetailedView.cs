using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal abstract class ProfilerDetailedView
	{
		protected struct CachedProfilerPropertyConfig
		{
			public string propertyPath;

			public int frameIndex;

			public ProfilerColumn sortType;

			public ProfilerViewType viewType;

			public bool EqualsTo(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType)
			{
				return this.frameIndex == frameIndex && this.sortType == sortType && this.viewType == viewType && this.propertyPath == ProfilerDriver.selectedPropertyPath;
			}

			public void Set(int frameIndex, ProfilerViewType viewType, ProfilerColumn sortType)
			{
				this.frameIndex = frameIndex;
				this.sortType = sortType;
				this.viewType = viewType;
				this.propertyPath = ProfilerDriver.selectedPropertyPath;
			}
		}

		private static class Styles
		{
			public static GUIContent emptyText = new GUIContent("");

			public static GUIContent selectLineText = new GUIContent("Select Line for the detailed information");
		}

		protected readonly ProfilerHierarchyGUI m_MainProfilerHierarchyGUI;

		protected ProfilerDetailedView.CachedProfilerPropertyConfig m_CachedProfilerPropertyConfig;

		protected ProfilerProperty m_CachedProfilerProperty;

		protected ProfilerDetailedView(ProfilerHierarchyGUI mainProfilerHierarchyGUI)
		{
			this.m_MainProfilerHierarchyGUI = mainProfilerHierarchyGUI;
		}

		public void ResetCachedProfilerProperty()
		{
			if (this.m_CachedProfilerProperty != null)
			{
				this.m_CachedProfilerProperty.Cleanup();
				this.m_CachedProfilerProperty = null;
			}
			this.m_CachedProfilerPropertyConfig.frameIndex = -1;
		}

		protected void DrawEmptyPane(GUIStyle headerStyle)
		{
			GUILayout.Box(ProfilerDetailedView.Styles.emptyText, headerStyle, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(ProfilerDetailedView.Styles.selectLineText, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
