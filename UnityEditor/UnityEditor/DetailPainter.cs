using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DetailPainter
	{
		public int size;

		public float opacity;

		public float targetStrength;

		public Brush brush;

		public TerrainData terrainData;

		public TerrainTool tool;

		public bool randomizeDetails;

		public bool clearSelectedOnly;

		public void Paint(float xCenterNormalized, float yCenterNormalized, int detailIndex)
		{
			if (detailIndex >= this.terrainData.detailPrototypes.Length)
			{
				return;
			}
			int num = Mathf.FloorToInt(xCenterNormalized * (float)this.terrainData.detailWidth);
			int num2 = Mathf.FloorToInt(yCenterNormalized * (float)this.terrainData.detailHeight);
			int num3 = Mathf.RoundToInt((float)this.size) / 2;
			int num4 = Mathf.RoundToInt((float)this.size) % 2;
			int num5 = Mathf.Clamp(num - num3, 0, this.terrainData.detailWidth - 1);
			int num6 = Mathf.Clamp(num2 - num3, 0, this.terrainData.detailHeight - 1);
			int num7 = Mathf.Clamp(num + num3 + num4, 0, this.terrainData.detailWidth);
			int num8 = Mathf.Clamp(num2 + num3 + num4, 0, this.terrainData.detailHeight);
			int num9 = num7 - num5;
			int num10 = num8 - num6;
			int[] array = new int[]
			{
				detailIndex
			};
			if (this.targetStrength < 0f && !this.clearSelectedOnly)
			{
				array = this.terrainData.GetSupportedLayers(num5, num6, num9, num10);
			}
			for (int i = 0; i < array.Length; i++)
			{
				int[,] detailLayer = this.terrainData.GetDetailLayer(num5, num6, num9, num10, array[i]);
				for (int j = 0; j < num10; j++)
				{
					for (int k = 0; k < num9; k++)
					{
						int ix = num5 + k - (num - num3 + num4);
						int iy = num6 + j - (num2 - num3 + num4);
						float t = this.opacity * this.brush.GetStrengthInt(ix, iy);
						float b = this.targetStrength;
						float num11 = Mathf.Lerp((float)detailLayer[j, k], b, t);
						detailLayer[j, k] = Mathf.RoundToInt(num11 - 0.5f + UnityEngine.Random.value);
					}
				}
				this.terrainData.SetDetailLayer(num5, num6, array[i], detailLayer);
			}
		}
	}
}
