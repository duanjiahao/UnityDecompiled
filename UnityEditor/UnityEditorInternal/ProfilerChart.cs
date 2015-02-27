using System;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class ProfilerChart
	{
		public ProfilerArea m_Area;
		public Chart.ChartType m_Type;
		public float m_DataScale;
		public Chart m_Chart;
		public ChartData m_Data;
		public ChartSeries[] m_Series;
		public bool m_Active;
		public GUIContent m_Icon;
		public ProfilerChart(ProfilerArea area, Chart.ChartType type, float dataScale, int seriesCount)
		{
			this.m_Area = area;
			this.m_Type = type;
			this.m_DataScale = dataScale;
			this.m_Chart = new Chart();
			this.m_Data = new ChartData();
			this.m_Series = new ChartSeries[seriesCount];
			this.m_Active = EditorPrefs.GetBool("ProfilerChart" + area, true);
			this.m_Icon = EditorGUIUtility.TextContent("Profiler." + Enum.GetName(typeof(ProfilerArea), area));
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
			return this.m_Chart.DoGUI(this.m_Type, currentFrame, this.m_Data, currentArea == this.m_Area, this.m_Icon, out action);
		}
		public void LoadAndBindSettings()
		{
			this.m_Chart.LoadAndBindSettings("ProfilerChart" + this.m_Area, this.m_Data);
		}
	}
}
