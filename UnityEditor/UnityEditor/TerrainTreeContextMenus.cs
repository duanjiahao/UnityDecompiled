using System;
using UnityEngine;
namespace UnityEditor
{
	internal class TerrainTreeContextMenus
	{
		[MenuItem("CONTEXT/TerrainEngineTrees/Add Tree")]
		internal static void AddTree(MenuCommand item)
		{
			TreeWizard treeWizard = ScriptableWizard.DisplayWizard<TreeWizard>("Add Tree", "Add");
			treeWizard.InitializeDefaults((Terrain)item.context, -1);
		}
		[MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree")]
		internal static void EditTree(MenuCommand item)
		{
			TreeWizard treeWizard = ScriptableWizard.DisplayWizard<TreeWizard>("Edit Tree", "Apply");
			treeWizard.InitializeDefaults((Terrain)item.context, item.userData);
		}
		[MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree", true)]
		internal static bool EditTreeCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.treePrototypes.Length;
		}
		[MenuItem("CONTEXT/TerrainEngineTrees/Remove Tree")]
		internal static void RemoveTree(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			TerrainEditorUtility.RemoveTree(terrain, item.userData);
		}
		[MenuItem("CONTEXT/TerrainEngineTrees/Remove Tree", true)]
		internal static bool RemoveTreeCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.treePrototypes.Length;
		}
	}
}
