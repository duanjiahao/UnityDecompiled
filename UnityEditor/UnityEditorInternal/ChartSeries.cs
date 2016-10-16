using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class ChartSeries
	{
		public string identifierName;

		public string name;

		public float[] data;

		public float[] overlayData;

		public Color color;

		public bool enabled;

		public ChartSeries(string name, int len, Color clr)
		{
			this.name = name;
			this.identifierName = name;
			this.data = new float[len];
			this.overlayData = null;
			this.color = clr;
			this.enabled = true;
		}

		public void CreateOverlayData()
		{
			this.overlayData = new float[this.data.Length];
		}
	}
}
