using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class ProfilerChart
	{
		private const string kPrefCharts = "ProfilerChart";
		private bool m_Active;
		public ProfilerArea m_Area;
		public Chart.ChartType m_Type;
		public float m_DataScale;
		public Chart m_Chart;
		public ChartData m_Data;
		public ChartSeries[] m_Series;
		public GUIContent m_Icon;
		public bool active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				if (this.m_Active != value)
				{
					this.m_Active = value;
					this.ApplyActiveState();
					this.SaveActiveState();
				}
			}
		}
		public ProfilerChart(ProfilerArea area, Chart.ChartType type, float dataScale, int seriesCount)
		{
			this.m_Area = area;
			this.m_Type = type;
			this.m_DataScale = dataScale;
			this.m_Chart = new Chart();
			this.m_Data = new ChartData();
			this.m_Series = new ChartSeries[seriesCount];
			this.m_Active = this.ReadActiveState();
			this.ApplyActiveState();
		}
		public int DoChartGUI(int currentFrame, ProfilerArea currentArea, out Chart.ChartAction action)
		{
			if (Event.current.type == EventType.Repaint)
			{
				string[] array = new string[this.m_Series.Length];
				for (int i = 0; i < this.m_Series.Length; i++)
				{
					string propertyName = (!this.m_Data.hasOverlay) ? this.m_Series[i].identifierName : ("Selected" + this.m_Series[i].identifierName);
					int statisticsIdentifier = ProfilerDriver.GetStatisticsIdentifier(propertyName);
					array[i] = ProfilerDriver.GetFormattedStatisticsValue(currentFrame, statisticsIdentifier);
				}
				this.m_Data.selectedLabels = array;
			}
			if (this.m_Icon == null)
			{
				this.m_Icon = EditorGUIUtility.TextContent("Profiler." + Enum.GetName(typeof(ProfilerArea), this.m_Area));
			}
			return this.m_Chart.DoGUI(this.m_Type, currentFrame, this.m_Data, this.m_Area, currentArea == this.m_Area, this.m_Icon, out action);
		}
		public void LoadAndBindSettings()
		{
			this.m_Chart.LoadAndBindSettings("ProfilerChart" + this.m_Area, this.m_Data);
		}
		private void ApplyActiveState()
		{
			if (this.m_Area == ProfilerArea.GPU)
			{
				ProfilerDriver.profileGPU = this.active;
			}
		}
		private bool ReadActiveState()
		{
			if (this.m_Area == ProfilerArea.GPU)
			{
				return InspectorState.GetBool("ProfilerChart" + this.m_Area, false);
			}
			return EditorPrefs.GetBool("ProfilerChart" + this.m_Area, true);
		}
		private void SaveActiveState()
		{
			if (this.m_Area == ProfilerArea.GPU)
			{
				InspectorState.SetBool("ProfilerChart" + this.m_Area, this.m_Active);
			}
			else
			{
				EditorPrefs.SetBool("ProfilerChart" + this.m_Area, this.m_Active);
			}
		}
	}
}
