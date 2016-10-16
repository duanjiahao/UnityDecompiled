using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainDetailContextMenus
	{
		[MenuItem("CONTEXT/TerrainEngineDetails/Add Grass Texture")]
		internal static void AddDetailTexture(MenuCommand item)
		{
			DetailTextureWizard detailTextureWizard = TerrainWizard.DisplayTerrainWizard<DetailTextureWizard>("Add Grass Texture", "Add");
			detailTextureWizard.m_DetailTexture = null;
			detailTextureWizard.InitializeDefaults((Terrain)item.context, -1);
		}

		[MenuItem("CONTEXT/TerrainEngineDetails/Add Detail Mesh")]
		internal static void AddDetailMesh(MenuCommand item)
		{
			DetailMeshWizard detailMeshWizard = TerrainWizard.DisplayTerrainWizard<DetailMeshWizard>("Add Detail Mesh", "Add");
			detailMeshWizard.m_Detail = null;
			detailMeshWizard.InitializeDefaults((Terrain)item.context, -1);
		}

		[MenuItem("CONTEXT/TerrainEngineDetails/Edit")]
		internal static void EditDetail(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			DetailPrototype detailPrototype = terrain.terrainData.detailPrototypes[item.userData];
			if (detailPrototype.usePrototypeMesh)
			{
				DetailMeshWizard detailMeshWizard = TerrainWizard.DisplayTerrainWizard<DetailMeshWizard>("Edit Detail Mesh", "Apply");
				detailMeshWizard.InitializeDefaults((Terrain)item.context, item.userData);
			}
			else
			{
				DetailTextureWizard detailTextureWizard = TerrainWizard.DisplayTerrainWizard<DetailTextureWizard>("Edit Grass Texture", "Apply");
				detailTextureWizard.InitializeDefaults((Terrain)item.context, item.userData);
			}
		}

		[MenuItem("CONTEXT/TerrainEngineDetails/Edit", true)]
		internal static bool EditDetailCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.detailPrototypes.Length;
		}

		[MenuItem("CONTEXT/TerrainEngineDetails/Remove")]
		internal static void RemoveDetail(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			TerrainEditorUtility.RemoveDetail(terrain, item.userData);
		}

		[MenuItem("CONTEXT/TerrainEngineDetails/Remove", true)]
		internal static bool RemoveDetailCheck(MenuCommand item)
		{
			Terrain terrain = (Terrain)item.context;
			return item.userData >= 0 && item.userData < terrain.terrainData.detailPrototypes.Length;
		}
	}
}
