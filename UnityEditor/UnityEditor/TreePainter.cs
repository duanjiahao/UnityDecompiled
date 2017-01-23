using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TreePainter
	{
		public static float brushSize = 40f;

		public static float spacing = 0.8f;

		public static bool lockWidthToHeight = true;

		public static bool randomRotation = true;

		public static bool allowHeightVar = true;

		public static bool allowWidthVar = true;

		public static float treeColorAdjustment = 0.4f;

		public static float treeHeight = 1f;

		public static float treeHeightVariation = 0.1f;

		public static float treeWidth = 1f;

		public static float treeWidthVariation = 0.1f;

		public static int selectedTree = -1;

		private static Color GetTreeColor()
		{
			Color result = Color.white * UnityEngine.Random.Range(1f, 1f - TreePainter.treeColorAdjustment);
			result.a = 1f;
			return result;
		}

		private static float GetTreeHeight()
		{
			float num = (!TreePainter.allowHeightVar) ? 0f : TreePainter.treeHeightVariation;
			return TreePainter.treeHeight * UnityEngine.Random.Range(1f - num, 1f + num);
		}

		private static float GetTreeWidth()
		{
			float num = (!TreePainter.allowWidthVar) ? 0f : TreePainter.treeWidthVariation;
			return TreePainter.treeWidth * UnityEngine.Random.Range(1f - num, 1f + num);
		}

		private static float GetTreeRotation()
		{
			return (!TreePainter.randomRotation) ? 0f : UnityEngine.Random.Range(0f, 6.28318548f);
		}

		public static void PlaceTrees(Terrain terrain, float xBase, float yBase)
		{
			int prototypeCount = TerrainInspectorUtil.GetPrototypeCount(terrain.terrainData);
			if (TreePainter.selectedTree != -1 && TreePainter.selectedTree < prototypeCount)
			{
				if (TerrainInspectorUtil.PrototypeIsRenderable(terrain.terrainData, TreePainter.selectedTree))
				{
					int num = 0;
					TreeInstance instance = default(TreeInstance);
					instance.position = new Vector3(xBase, 0f, yBase);
					instance.color = TreePainter.GetTreeColor();
					instance.lightmapColor = Color.white;
					instance.prototypeIndex = TreePainter.selectedTree;
					instance.heightScale = TreePainter.GetTreeHeight();
					instance.widthScale = ((!TreePainter.lockWidthToHeight) ? TreePainter.GetTreeWidth() : instance.heightScale);
					instance.rotation = TreePainter.GetTreeRotation();
					bool flag = Event.current.type == EventType.MouseDrag || TreePainter.brushSize > 1f;
					if (!flag || TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, instance.position, instance.prototypeIndex, TreePainter.spacing))
					{
						terrain.AddTreeInstance(instance);
						num++;
					}
					Vector3 prototypeExtent = TerrainInspectorUtil.GetPrototypeExtent(terrain.terrainData, TreePainter.selectedTree);
					prototypeExtent.y = 0f;
					float num2 = TreePainter.brushSize / (prototypeExtent.magnitude * TreePainter.spacing * 0.5f);
					int num3 = (int)(num2 * num2 * 0.5f);
					num3 = Mathf.Clamp(num3, 0, 100);
					int num4 = 1;
					while (num4 < num3 && num < num3)
					{
						Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
						insideUnitCircle.x *= TreePainter.brushSize / terrain.terrainData.size.x;
						insideUnitCircle.y *= TreePainter.brushSize / terrain.terrainData.size.z;
						Vector3 position = new Vector3(xBase + insideUnitCircle.x, 0f, yBase + insideUnitCircle.y);
						if (position.x >= 0f && position.x <= 1f && position.z >= 0f && position.z <= 1f && TerrainInspectorUtil.CheckTreeDistance(terrain.terrainData, position, TreePainter.selectedTree, TreePainter.spacing * 0.5f))
						{
							instance = default(TreeInstance);
							instance.position = position;
							instance.color = TreePainter.GetTreeColor();
							instance.lightmapColor = Color.white;
							instance.prototypeIndex = TreePainter.selectedTree;
							instance.heightScale = TreePainter.GetTreeHeight();
							instance.widthScale = ((!TreePainter.lockWidthToHeight) ? TreePainter.GetTreeWidth() : instance.heightScale);
							instance.rotation = TreePainter.GetTreeRotation();
							terrain.AddTreeInstance(instance);
							num++;
						}
						num4++;
					}
				}
			}
		}

		public static void RemoveTrees(Terrain terrain, float xBase, float yBase, bool clearSelectedOnly)
		{
			float radius = TreePainter.brushSize / terrain.terrainData.size.x;
			terrain.RemoveTrees(new Vector2(xBase, yBase), radius, (!clearSelectedOnly) ? -1 : TreePainter.selectedTree);
		}

		public static void MassPlaceTrees(TerrainData terrainData, int numberOfTrees, bool randomTreeColor, bool keepExistingTrees)
		{
			int num = terrainData.treePrototypes.Length;
			if (num == 0)
			{
				Debug.Log("Can't place trees because no prototypes are defined");
			}
			else
			{
				Undo.RegisterCompleteObjectUndo(terrainData, "Mass Place Trees");
				TreeInstance[] array = new TreeInstance[numberOfTrees];
				int i = 0;
				while (i < array.Length)
				{
					TreeInstance treeInstance = default(TreeInstance);
					treeInstance.position = new Vector3(UnityEngine.Random.value, 0f, UnityEngine.Random.value);
					if (terrainData.GetSteepness(treeInstance.position.x, treeInstance.position.z) < 30f)
					{
						treeInstance.color = ((!randomTreeColor) ? Color.white : TreePainter.GetTreeColor());
						treeInstance.lightmapColor = Color.white;
						treeInstance.prototypeIndex = UnityEngine.Random.Range(0, num);
						treeInstance.heightScale = TreePainter.GetTreeHeight();
						treeInstance.widthScale = ((!TreePainter.lockWidthToHeight) ? TreePainter.GetTreeWidth() : treeInstance.heightScale);
						treeInstance.rotation = TreePainter.GetTreeRotation();
						array[i++] = treeInstance;
					}
				}
				if (keepExistingTrees)
				{
					TreeInstance[] treeInstances = terrainData.treeInstances;
					TreeInstance[] array2 = new TreeInstance[treeInstances.Length + array.Length];
					Array.Copy(treeInstances, 0, array2, 0, treeInstances.Length);
					Array.Copy(array, 0, array2, treeInstances.Length, array.Length);
					array = array2;
				}
				terrainData.treeInstances = array;
				terrainData.RecalculateTreePositions();
			}
		}
	}
}
