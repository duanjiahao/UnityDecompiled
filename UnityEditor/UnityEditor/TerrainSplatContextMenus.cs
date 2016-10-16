using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainSplatContextMenus
	{
		[MenuItem("CONTEXT/TerrainEngineSplats/Add Texture...")]
		internal static void AddSplat(MenuCommand item)
		{
			TerrainSplatEditor.ShowTerrainSplatEditor("Add Terrain Texture", "Add", (Terrain)item.context, -1);
		}

		[MenuItem("CONTEXT/TerrainEngineSplats/Edit Texture...")]
		internal static void EditSplat(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			string text = "Edit Terrain Texture";
			switch (terrain.materialType)
			{
			case Terrain.MaterialType.BuiltInStandard:
				text += " (Standard)";
				break;
			case Terrain.MaterialType.BuiltInLegacyDiffuse:
				text += " (Diffuse)";
				break;
			case Terrain.MaterialType.BuiltInLegacySpecular:
				text += " (Specular)";
				break;
			case Terrain.MaterialType.Custom:
				text += " (Custom)";
				break;
			}
			TerrainSplatEditor.ShowTerrainSplatEditor(text, "Apply", (Terrain)item.context, item.userData);
		}

		[MenuItem("CONTEXT/TerrainEngineSplats/Edit Texture...", true)]
		internal static bool EditSplatCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.splatPrototypes.Length;
		}

		[MenuItem("CONTEXT/TerrainEngineSplats/Remove Texture")]
		internal static void RemoveSplat(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			TerrainEditorUtility.RemoveSplatTexture(terrain.terrainData, item.userData);
		}

		[MenuItem("CONTEXT/TerrainEngineSplats/Remove Texture", true)]
		internal static bool RemoveSplatCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.splatPrototypes.Length;
		}
	}
}
