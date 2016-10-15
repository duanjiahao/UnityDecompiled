using System;
using UnityEngine;

namespace UnityEditor
{
	internal class HeightmapPainter
	{
		public int size;

		public float strength;

		public float targetHeight;

		public TerrainTool tool;

		public Brush brush;

		public TerrainData terrainData;

		private float Smooth(int x, int y)
		{
			float num = 0f;
			float num2 = 1f / this.terrainData.size.y;
			num += this.terrainData.GetHeight(x, y) * num2;
			num += this.terrainData.GetHeight(x + 1, y) * num2;
			num += this.terrainData.GetHeight(x - 1, y) * num2;
			num += this.terrainData.GetHeight(x + 1, y + 1) * num2 * 0.75f;
			num += this.terrainData.GetHeight(x - 1, y + 1) * num2 * 0.75f;
			num += this.terrainData.GetHeight(x + 1, y - 1) * num2 * 0.75f;
			num += this.terrainData.GetHeight(x - 1, y - 1) * num2 * 0.75f;
			num += this.terrainData.GetHeight(x, y + 1) * num2;
			num += this.terrainData.GetHeight(x, y - 1) * num2;
			return num / 8f;
		}

		private float ApplyBrush(float height, float brushStrength, int x, int y)
		{
			if (this.tool == TerrainTool.PaintHeight)
			{
				return height + brushStrength;
			}
			if (this.tool == TerrainTool.SetHeight)
			{
				if (this.targetHeight > height)
				{
					height += brushStrength;
					height = Mathf.Min(height, this.targetHeight);
					return height;
				}
				height -= brushStrength;
				height = Mathf.Max(height, this.targetHeight);
				return height;
			}
			else
			{
				if (this.tool == TerrainTool.SmoothHeight)
				{
					return Mathf.Lerp(height, this.Smooth(x, y), brushStrength);
				}
				return height;
			}
		}

		public void PaintHeight(float xCenterNormalized, float yCenterNormalized)
		{
			int num;
			int num2;
			if (this.size % 2 == 0)
			{
				num = Mathf.CeilToInt(xCenterNormalized * (float)(this.terrainData.heightmapWidth - 1));
				num2 = Mathf.CeilToInt(yCenterNormalized * (float)(this.terrainData.heightmapHeight - 1));
			}
			else
			{
				num = Mathf.RoundToInt(xCenterNormalized * (float)(this.terrainData.heightmapWidth - 1));
				num2 = Mathf.RoundToInt(yCenterNormalized * (float)(this.terrainData.heightmapHeight - 1));
			}
			int num3 = this.size / 2;
			int num4 = this.size % 2;
			int num5 = Mathf.Clamp(num - num3, 0, this.terrainData.heightmapWidth - 1);
			int num6 = Mathf.Clamp(num2 - num3, 0, this.terrainData.heightmapHeight - 1);
			int num7 = Mathf.Clamp(num + num3 + num4, 0, this.terrainData.heightmapWidth);
			int num8 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.heightmapHeight);
			int num9 = num7 - num5;
			int num10 = num8 - num6;
			float[,] heights = this.terrainData.GetHeights(num5, num6, num9, num10);
			for (int i = 0; i < num10; i++)
			{
				for (int j = 0; j < num9; j++)
				{
					int ix = num5 + j - (num - num3);
					int iy = num6 + i - (num2 - num3);
					float strengthInt = this.brush.GetStrengthInt(ix, iy);
					float num11 = heights[i, j];
					num11 = this.ApplyBrush(num11, strengthInt * this.strength, j + num5, i + num6);
					heights[i, j] = num11;
				}
			}
			this.terrainData.SetHeightsDelayLOD(num5, num6, heights);
		}
	}
}
