using System;
using UnityEngine;
namespace UnityEditor
{
	internal class TreePlacementUtility
	{
		public static void PlaceRandomTrees(TerrainData terrainData, int treeCount)
		{
			int num = terrainData.treePrototypes.Length;
			if (num == 0)
			{
				Debug.Log("Can't place trees because no prototypes are defined");
				return;
			}
			Undo.RegisterCompleteObjectUndo(terrainData, "Mass Place Trees");
			TreeInstance[] array = new TreeInstance[treeCount];
			int i = 0;
			while (i < array.Length)
			{
				TreeInstance treeInstance = default(TreeInstance);
				treeInstance.position = new Vector3(UnityEngine.Random.value, 0f, UnityEngine.Random.value);
				if (terrainData.GetSteepness(treeInstance.position.x, treeInstance.position.z) < 30f)
				{
					Color color = Color.Lerp(Color.white, Color.gray * 0.7f, UnityEngine.Random.value);
					color.a = 1f;
					treeInstance.color = color;
					treeInstance.lightmapColor = Color.white;
					treeInstance.prototypeIndex = UnityEngine.Random.Range(0, num);
					treeInstance.widthScale = 1f;
					treeInstance.heightScale = 1f;
					array[i] = treeInstance;
					i++;
				}
			}
			terrainData.treeInstances = array;
			terrainData.RecalculateTreePositions();
		}
	}
}
