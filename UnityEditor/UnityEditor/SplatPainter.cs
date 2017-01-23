using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SplatPainter
	{
		public int size;

		public float strength;

		public Brush brush;

		public float target;

		public TerrainData terrainData;

		public TerrainTool tool;

		private float ApplyBrush(float height, float brushStrength)
		{
			float result;
			if (this.target > height)
			{
				height += brushStrength;
				height = Mathf.Min(height, this.target);
				result = height;
			}
			else
			{
				height -= brushStrength;
				height = Mathf.Max(height, this.target);
				result = height;
			}
			return result;
		}

		private void Normalize(int x, int y, int splatIndex, float[,,] alphamap)
		{
			float num = alphamap[y, x, splatIndex];
			float num2 = 0f;
			int length = alphamap.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				if (i != splatIndex)
				{
					num2 += alphamap[y, x, i];
				}
			}
			if ((double)num2 > 0.01)
			{
				float num3 = (1f - num) / num2;
				for (int j = 0; j < length; j++)
				{
					if (j != splatIndex)
					{
						alphamap[y, x, j] *= num3;
					}
				}
			}
			else
			{
				for (int k = 0; k < length; k++)
				{
					alphamap[y, x, k] = ((k != splatIndex) ? 0f : 1f);
				}
			}
		}

		public void Paint(float xCenterNormalized, float yCenterNormalized, int splatIndex)
		{
			if (splatIndex < this.terrainData.alphamapLayers)
			{
				int num = Mathf.FloorToInt(xCenterNormalized * (float)this.terrainData.alphamapWidth);
				int num2 = Mathf.FloorToInt(yCenterNormalized * (float)this.terrainData.alphamapHeight);
				int num3 = Mathf.RoundToInt((float)this.size) / 2;
				int num4 = Mathf.RoundToInt((float)this.size) % 2;
				int num5 = Mathf.Clamp(num - num3, 0, this.terrainData.alphamapWidth - 1);
				int num6 = Mathf.Clamp(num2 - num3, 0, this.terrainData.alphamapHeight - 1);
				int num7 = Mathf.Clamp(num + num3 + num4, 0, this.terrainData.alphamapWidth);
				int num8 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.alphamapHeight);
				int num9 = num7 - num5;
				int num10 = num8 - num6;
				float[,,] alphamaps = this.terrainData.GetAlphamaps(num5, num6, num9, num10);
				for (int i = 0; i < num10; i++)
				{
					for (int j = 0; j < num9; j++)
					{
						int ix = num5 + j - (num - num3 + num4);
						int iy = num6 + i - (num2 - num3 + num4);
						float strengthInt = this.brush.GetStrengthInt(ix, iy);
						float num11 = this.ApplyBrush(alphamaps[i, j, splatIndex], strengthInt * this.strength);
						alphamaps[i, j, splatIndex] = num11;
						this.Normalize(j, i, splatIndex, alphamaps);
					}
				}
				this.terrainData.SetAlphamaps(num5, num6, alphamaps);
			}
		}
	}
}
