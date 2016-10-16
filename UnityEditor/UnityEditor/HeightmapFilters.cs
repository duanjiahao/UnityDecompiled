using System;
using UnityEngine;

namespace UnityEditor
{
	internal class HeightmapFilters
	{
		private static void WobbleStuff(float[,] heights, TerrainData terrain)
		{
			for (int i = 0; i < heights.GetLength(0); i++)
			{
				for (int j = 0; j < heights.GetLength(1); j++)
				{
					heights[i, j] = (heights[i, j] + 1f) / 2f;
				}
			}
		}

		private static void Noise(float[,] heights, TerrainData terrain)
		{
			for (int i = 0; i < heights.GetLength(0); i++)
			{
				for (int j = 0; j < heights.GetLength(1); j++)
				{
					heights[i, j] += UnityEngine.Random.value * 0.01f;
				}
			}
		}

		public static void Smooth(float[,] heights, TerrainData terrain)
		{
			float[,] array = heights.Clone() as float[,];
			int length = heights.GetLength(1);
			int length2 = heights.GetLength(0);
			for (int i = 1; i < length2 - 1; i++)
			{
				for (int j = 1; j < length - 1; j++)
				{
					float num = 0f;
					num += array[i, j];
					num += array[i, j - 1];
					num += array[i, j + 1];
					num += array[i - 1, j];
					num += array[i + 1, j];
					num /= 5f;
					heights[i, j] = num;
				}
			}
		}

		public static void Smooth(TerrainData terrain)
		{
			int heightmapWidth = terrain.heightmapWidth;
			int heightmapHeight = terrain.heightmapHeight;
			float[,] heights = terrain.GetHeights(0, 0, heightmapWidth, heightmapHeight);
			HeightmapFilters.Smooth(heights, terrain);
			terrain.SetHeights(0, 0, heights);
		}

		public static void Flatten(TerrainData terrain, float height)
		{
			int heightmapWidth = terrain.heightmapWidth;
			int heightmapHeight = terrain.heightmapHeight;
			float[,] array = new float[heightmapHeight, heightmapWidth];
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = height;
				}
			}
			terrain.SetHeights(0, 0, array);
		}
	}
}
