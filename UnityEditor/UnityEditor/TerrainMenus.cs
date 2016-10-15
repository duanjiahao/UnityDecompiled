using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainMenus
	{
		[MenuItem("GameObject/3D Object/Terrain", false, 3000)]
		private static void CreateTerrain(MenuCommand menuCommand)
		{
			TerrainData terrainData = new TerrainData();
			terrainData.heightmapResolution = 1025;
			terrainData.size = new Vector3(1000f, 600f, 1000f);
			terrainData.heightmapResolution = 512;
			terrainData.baseMapResolution = 1024;
			terrainData.SetDetailResolution(1024, terrainData.detailResolutionPerPatch);
			AssetDatabase.CreateAsset(terrainData, AssetDatabase.GenerateUniqueAssetPath("Assets/New Terrain.asset"));
			GameObject gameObject = menuCommand.context as GameObject;
			string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling((!(gameObject != null)) ? null : gameObject.transform, "Terrain");
			GameObject gameObject2 = Terrain.CreateTerrainGameObject(terrainData);
			gameObject2.name = uniqueNameForSibling;
			GameObjectUtility.SetParentAndAlign(gameObject2, gameObject);
			Selection.activeObject = gameObject2;
			Undo.RegisterCreatedObjectUndo(gameObject2, "Create terrain");
		}

		internal static void ImportRaw()
		{
			string text = EditorUtility.OpenFilePanel("Import Raw Heightmap", string.Empty, "raw");
			if (text != string.Empty)
			{
				ImportRawHeightmap importRawHeightmap = TerrainWizard.DisplayTerrainWizard<ImportRawHeightmap>("Import Heightmap", "Import");
				importRawHeightmap.InitializeImportRaw(TerrainMenus.GetActiveTerrain(), text);
			}
		}

		internal static void ExportHeightmapRaw()
		{
			ExportRawHeightmap exportRawHeightmap = TerrainWizard.DisplayTerrainWizard<ExportRawHeightmap>("Export Heightmap", "Export");
			exportRawHeightmap.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}

		internal static void MassPlaceTrees()
		{
			PlaceTreeWizard placeTreeWizard = TerrainWizard.DisplayTerrainWizard<PlaceTreeWizard>("Place Trees", "Place");
			placeTreeWizard.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}

		internal static void Flatten()
		{
			FlattenHeightmap flattenHeightmap = TerrainWizard.DisplayTerrainWizard<FlattenHeightmap>("Flatten Heightmap", "Flatten");
			flattenHeightmap.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}

		internal static void RefreshPrototypes()
		{
			TerrainMenus.GetActiveTerrainData().RefreshPrototypes();
			TerrainMenus.GetActiveTerrain().Flush();
			EditorApplication.SetSceneRepaintDirty();
		}

		private static void FlushHeightmapModification()
		{
			TerrainMenus.GetActiveTerrain().Flush();
		}

		private static Terrain GetActiveTerrain()
		{
			UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(Terrain), SelectionMode.Editable);
			if (filtered.Length != 0)
			{
				return filtered[0] as Terrain;
			}
			return Terrain.activeTerrain;
		}

		private static TerrainData GetActiveTerrainData()
		{
			if (TerrainMenus.GetActiveTerrain())
			{
				return TerrainMenus.GetActiveTerrain().terrainData;
			}
			return null;
		}
	}
}
