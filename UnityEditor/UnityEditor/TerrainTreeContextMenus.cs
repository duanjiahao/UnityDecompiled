using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainTreeContextMenus
	{
		[MenuItem("CONTEXT/TerrainEngineTrees/Add Tree")]
		internal static void AddTree(MenuCommand item)
		{
			TreeWizard treeWizard = TerrainWizard.DisplayTerrainWizard<TreeWizard>("Add Tree", "Add");
			treeWizard.InitializeDefaults((Terrain)item.context, -1);
		}

		[MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree")]
		internal static void EditTree(MenuCommand item)
		{
			TreeWizard treeWizard = TerrainWizard.DisplayTerrainWizard<TreeWizard>("Edit Tree", "Apply");
			treeWizard.InitializeDefaults((Terrain)item.context, item.userData);
		}

		[MenuItem("CONTEXT/TerrainEngineTrees/Edit Tree", true)]
		internal static bool EditTreeCheck(MenuCommand item)
		{
			return TreePainter.selectedTree >= 0;
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
			return TreePainter.selectedTree >= 0;
		}
	}
}
