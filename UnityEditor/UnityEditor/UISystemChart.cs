using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class UISystemChart : Chart
	{
		public bool showMarkers;

		protected internal override void LabelDraggerDrag(int chartControlID, Chart.ChartType chartType, ChartData cdata, Rect r, bool active)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown)
			{
				if (Chart.GetToggleRect(r, cdata.charts.Length).Contains(current.mousePosition))
				{
					this.showMarkers = !this.showMarkers;
					base.SaveChartsSettingsEnabled(cdata);
					current.Use();
				}
			}
			base.LabelDraggerDrag(chartControlID, chartType, cdata, r, active);
		}

		protected override void DrawLabelDragger(Chart.ChartType type, Rect r, ChartData cdata)
		{
			base.DrawLabelDragger(type, r, cdata);
			GUI.backgroundColor = ((!this.showMarkers) ? Color.black : Color.white);
			Chart.DrawSubLabel(r, cdata.charts.Length, "Markers");
			GUI.backgroundColor = Color.white;
		}
	}
}
