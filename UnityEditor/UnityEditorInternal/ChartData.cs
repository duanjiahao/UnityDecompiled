using System;

namespace UnityEditorInternal
{
	internal class ChartData
	{
		public ChartSeries[] charts;

		public int[] chartOrder;

		public float[] scale;

		public float[] grid;

		public string[] gridLabels;

		public string[] selectedLabels;

		public int firstFrame;

		public int firstSelectableFrame;

		public bool hasOverlay;

		public float maxValue;

		public int NumberOfFrames
		{
			get
			{
				return this.charts[0].data.Length;
			}
		}

		public void Assign(ChartSeries[] items, int firstFrame, int firstSelectableFrame)
		{
			this.charts = items;
			this.firstFrame = firstFrame;
			this.firstSelectableFrame = firstSelectableFrame;
			if (this.chartOrder == null || this.chartOrder.Length != items.Length)
			{
				this.chartOrder = new int[items.Length];
				for (int i = 0; i < this.chartOrder.Length; i++)
				{
					this.chartOrder[i] = this.chartOrder.Length - 1 - i;
				}
			}
		}

		public void AssignScale(float[] scale)
		{
			this.scale = scale;
		}

		public void SetGrid(float[] grid, string[] labels)
		{
			this.grid = grid;
			this.gridLabels = labels;
		}
	}
}
