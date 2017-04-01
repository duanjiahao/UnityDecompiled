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

		private static string[] s_LocalizedChartNames = null;

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

		private string GetLocalizedChartName()
		{
			if (ProfilerChart.s_LocalizedChartNames == null)
			{
				ProfilerChart.s_LocalizedChartNames = new string[]
				{
					LocalizationDatabase.GetLocalizedString("CPU Usage|Graph out the various CPU areas"),
					LocalizationDatabase.GetLocalizedString("GPU Usage|Graph out the various GPU areas"),
					LocalizationDatabase.GetLocalizedString("Rendering"),
					LocalizationDatabase.GetLocalizedString("Memory|Graph out the various memory usage areas"),
					LocalizationDatabase.GetLocalizedString("Audio"),
					LocalizationDatabase.GetLocalizedString("Video"),
					LocalizationDatabase.GetLocalizedString("Physics"),
					LocalizationDatabase.GetLocalizedString("Physics (2D)"),
					LocalizationDatabase.GetLocalizedString("Network Messages"),
					LocalizationDatabase.GetLocalizedString("Network Operations"),
					LocalizationDatabase.GetLocalizedString("UI"),
					LocalizationDatabase.GetLocalizedString("UI Details")
				};
			}
			return ProfilerChart.s_LocalizedChartNames[(int)this.m_Area];
		}

		public virtual int DoChartGUI(int currentFrame, ProfilerArea currentArea, out Chart.ChartAction action)
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
				string icon = "Profiler." + Enum.GetName(typeof(ProfilerArea), this.m_Area);
				this.m_Icon = EditorGUIUtility.TextContentWithIcon(this.GetLocalizedChartName(), icon);
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
			bool @bool;
			if (this.m_Area == ProfilerArea.GPU)
			{
				@bool = SessionState.GetBool("ProfilerChart" + this.m_Area, false);
			}
			else
			{
				@bool = EditorPrefs.GetBool("ProfilerChart" + this.m_Area, true);
			}
			return @bool;
		}

		private void SaveActiveState()
		{
			if (this.m_Area == ProfilerArea.GPU)
			{
				SessionState.SetBool("ProfilerChart" + this.m_Area, this.m_Active);
			}
			else
			{
				EditorPrefs.SetBool("ProfilerChart" + this.m_Area, this.m_Active);
			}
		}
	}
}
