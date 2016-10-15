using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainEditorUtility
	{
		internal static void RemoveSplatTexture(TerrainData terrainData, int index)
		{
			Undo.RegisterCompleteObjectUndo(terrainData, "Remove texture");
			int alphamapWidth = terrainData.alphamapWidth;
			int alphamapHeight = terrainData.alphamapHeight;
			float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
			int length = alphamaps.GetLength(2);
			int num = length - 1;
			float[,,] array = new float[alphamapHeight, alphamapWidth, num];
			for (int i = 0; i < alphamapHeight; i++)
			{
				for (int j = 0; j < alphamapWidth; j++)
				{
					for (int k = 0; k < index; k++)
					{
						array[i, j, k] = alphamaps[i, j, k];
					}
					for (int l = index + 1; l < length; l++)
					{
						array[i, j, l - 1] = alphamaps[i, j, l];
					}
				}
			}
			for (int m = 0; m < alphamapHeight; m++)
			{
				for (int n = 0; n < alphamapWidth; n++)
				{
					float num2 = 0f;
					for (int num3 = 0; num3 < num; num3++)
					{
						num2 += array[m, n, num3];
					}
					if ((double)num2 >= 0.01)
					{
						float num4 = 1f / num2;
						for (int num5 = 0; num5 < num; num5++)
						{
							array[m, n, num5] *= num4;
						}
					}
					else
					{
						for (int num6 = 0; num6 < num; num6++)
						{
							array[m, n, num6] = ((num6 != 0) ? 0f : 1f);
						}
					}
				}
			}
			SplatPrototype[] splatPrototypes = terrainData.splatPrototypes;
			SplatPrototype[] array2 = new SplatPrototype[splatPrototypes.Length - 1];
			for (int num7 = 0; num7 < index; num7++)
			{
				array2[num7] = splatPrototypes[num7];
			}
			for (int num8 = index + 1; num8 < length; num8++)
			{
				array2[num8 - 1] = splatPrototypes[num8];
			}
			terrainData.splatPrototypes = array2;
			terrainData.SetAlphamaps(0, 0, array);
		}

		internal static void RemoveTree(Terrain terrain, int index)
		{
			TerrainData terrainData = terrain.terrainData;
			if (terrainData == null)
			{
				return;
			}
			Undo.RegisterCompleteObjectUndo(terrainData, "Remove tree");
			terrainData.RemoveTreePrototype(index);
		}

		internal static void RemoveDetail(Terrain terrain, int index)
		{
			TerrainData terrainData = terrain.terrainData;
			if (terrainData == null)
			{
				return;
			}
			Undo.RegisterCompleteObjectUndo(terrainData, "Remove detail object");
			terrainData.RemoveDetailPrototype(index);
		}

		internal static bool IsLODTreePrototype(GameObject prefab)
		{
			return prefab != null && prefab.GetComponent<LODGroup>() != null;
		}
	}
}
