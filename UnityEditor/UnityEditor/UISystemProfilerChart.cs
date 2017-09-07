using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class UISystemProfilerChart : ProfilerChart
	{
		private EventMarker[] m_Markers;

		private string[] m_MarkerNames;

		public bool showMarkers
		{
			get
			{
				return ((UISystemChart)this.m_Chart).showMarkers;
			}
		}

		public UISystemProfilerChart(Chart.ChartType type, float dataScale, int seriesCount) : base(ProfilerArea.UIDetails, type, dataScale, seriesCount)
		{
			this.m_Chart = new UISystemChart();
		}

		public void Update(int firstFrame, int historyLength)
		{
			int uISystemEventMarkersCount = ProfilerDriver.GetUISystemEventMarkersCount(firstFrame, historyLength);
			if (uISystemEventMarkersCount != 0)
			{
				this.m_Markers = new EventMarker[uISystemEventMarkersCount];
				this.m_MarkerNames = new string[uISystemEventMarkersCount];
				ProfilerDriver.GetUISystemEventMarkersBatch(firstFrame, historyLength, this.m_Markers, this.m_MarkerNames);
			}
		}

		public override int DoChartGUI(int currentFrame, ProfilerArea currentArea, out Chart.ChartAction action)
		{
			int result = base.DoChartGUI(currentFrame, currentArea, out action);
			if (this.m_Markers != null && this.showMarkers)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.xMin += 170f;
				for (int i = 0; i < this.m_Markers.Length; i++)
				{
					EventMarker eventMarker = this.m_Markers[i];
					Color color = ProfilerColors.colors[(int)(checked((IntPtr)(unchecked((ulong)eventMarker.objectInstanceId % (ulong)((long)ProfilerColors.colors.Length)))))];
					Chart.DrawVerticalLine(eventMarker.frame, this.m_Data, lastRect, color.AlphaMultiplied(0.3f), color.AlphaMultiplied(0.4f), 1f);
				}
				this.DrawMarkerLabels(this.m_Data, lastRect, this.m_Markers, this.m_MarkerNames);
			}
			return result;
		}

		private void DrawMarkerLabels(ChartData cdata, Rect r, EventMarker[] markers, string[] markerNames)
		{
			Color contentColor = GUI.contentColor;
			int numberOfFrames = cdata.NumberOfFrames;
			float num = r.width / (float)numberOfFrames;
			int num2 = (int)(r.height / 12f);
			if (num2 != 0)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int i = 0; i < markers.Length; i++)
				{
					int num3 = markers[i].frame;
					int num4;
					if (!dictionary.TryGetValue(markers[i].nameOffset, out num4) || num4 != num3 - 1 || num4 < cdata.firstFrame)
					{
						num3 -= cdata.firstFrame;
						if (num3 >= 0)
						{
							float num5 = r.x + num * (float)num3;
							Color a = ProfilerColors.colors[(int)(checked((IntPtr)(unchecked((ulong)markers[i].objectInstanceId % (ulong)((long)ProfilerColors.colors.Length)))))];
							GUI.contentColor = (a + Color.white) * 0.5f;
							Chart.DoLabel(num5 + -1f, r.y + r.height - (float)((i % num2 + 1) * 12), markerNames[i], 0f);
						}
					}
					dictionary[markers[i].nameOffset] = markers[i].frame;
				}
			}
			GUI.contentColor = contentColor;
		}
	}
}
