using System;
using UnityEngine;
namespace UnityEditor
{
	internal class TreePainter
	{
		public static float brushSize = 40f;
		public static float spacing = 0.8f;
		public static float treeColorAdjustment = 0.4f;
		public static float treeWidth = 1f;
		public static float treeHeight = 1f;
		public static float treeWidthVariation = 0.1f;
		public static float treeHeightVariation = 0.1f;
		public static int selectedTree;
		public static Terrain terrain;
		private static Color GetTreeColor()
		{
			Color result = Color.white * UnityEngine.Random.Range(1f, 1f - TreePainter.treeColorAdjustment);
			result.a = 1f;
			return result;
		}
		private static float GetTreeWidth()
		{
			return TreePainter.treeWidth * UnityEngine.Random.Range(1f - TreePainter.treeWidthVariation, 1f + TreePainter.treeWidthVariation);
		}
		private static float GetTreeHeight()
		{
			return TreePainter.treeHeight * UnityEngine.Random.Range(1f - TreePainter.treeHeightVariation, 1f + TreePainter.treeHeightVariation);
		}
		public static void PlaceTrees(float xBase, float yBase)
		{
			if (TreePainter.terrain.terrainData.treePrototypes.Length == 0)
			{
				return;
			}
			TreePainter.selectedTree = Mathf.Min(TerrainInspectorUtil.GetPrototypeCount(TreePainter.terrain.terrainData) - 1, TreePainter.selectedTree);
			if (!TerrainInspectorUtil.PrototypeHasMaterials(TreePainter.terrain.terrainData, TreePainter.selectedTree))
			{
				return;
			}
			int num = 0;
			TreeInstance instance = default(TreeInstance);
			instance.position = new Vector3(xBase, 0f, yBase);
			instance.color = TreePainter.GetTreeColor();
			instance.lightmapColor = Color.white;
			instance.prototypeIndex = TreePainter.selectedTree;
			instance.widthScale = TreePainter.GetTreeWidth();
			instance.heightScale = TreePainter.GetTreeHeight();
			bool flag = Event.current.type == EventType.MouseDrag || TreePainter.brushSize > 1f;
			if (!flag || TerrainInspectorUtil.CheckTreeDistance(TreePainter.terrain.terrainData, instance.position, instance.prototypeIndex, TreePainter.spacing))
			{
				TreePainter.terrain.AddTreeInstance(instance);
				num++;
			}
			Vector3 prototypeExtent = TerrainInspectorUtil.GetPrototypeExtent(TreePainter.terrain.terrainData, TreePainter.selectedTree);
			prototypeExtent.y = 0f;
			float num2 = TreePainter.brushSize / (prototypeExtent.magnitude * TreePainter.spacing * 0.5f);
			int num3 = (int)(num2 * num2 * 0.5f);
			num3 = Mathf.Clamp(num3, 0, 100);
			int num4 = 1;
			while (num4 < num3 && num < num3)
			{
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				insideUnitCircle.x *= TreePainter.brushSize / TreePainter.terrain.terrainData.size.x;
				insideUnitCircle.y *= TreePainter.brushSize / TreePainter.terrain.terrainData.size.z;
				Vector3 position = new Vector3(xBase + insideUnitCircle.x, 0f, yBase + insideUnitCircle.y);
				if (position.x >= 0f && position.x <= 1f && position.z >= 0f && position.z <= 1f && TerrainInspectorUtil.CheckTreeDistance(TreePainter.terrain.terrainData, position, TreePainter.selectedTree, TreePainter.spacing * 0.5f))
				{
					instance = default(TreeInstance);
					instance.position = position;
					instance.color = TreePainter.GetTreeColor();
					instance.lightmapColor = Color.white;
					instance.prototypeIndex = TreePainter.selectedTree;
					instance.widthScale = TreePainter.GetTreeWidth();
					instance.heightScale = TreePainter.GetTreeHeight();
					TreePainter.terrain.AddTreeInstance(instance);
					num++;
				}
				num4++;
			}
		}
		private float GetTreePlacementSize(float treeCount)
		{
			return TerrainInspectorUtil.GetTreePlacementSize(TreePainter.terrain.terrainData, TreePainter.selectedTree, TreePainter.spacing, treeCount);
		}
		public static void RemoveTrees(float xBase, float yBase, bool clearSelectedOnly)
		{
			float radius = TreePainter.brushSize / TreePainter.terrain.terrainData.size.x;
			TreePainter.terrain.RemoveTrees(new Vector2(xBase, yBase), radius, (!clearSelectedOnly) ? -1 : TreePainter.selectedTree);
		}
	}
}
