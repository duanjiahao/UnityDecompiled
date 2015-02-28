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
			GameObject gameObject = Terrain.CreateTerrainGameObject(terrainData);
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			Selection.activeObject = gameObject;
			Undo.RegisterCreatedObjectUndo(gameObject, "Create terrain");
		}
		internal static void ImportRaw()
		{
			string text = EditorUtility.OpenFilePanel("Import Raw Heightmap", string.Empty, "raw");
			if (text != string.Empty)
			{
				ImportRawHeightmap importRawHeightmap = ScriptableWizard.DisplayWizard<ImportRawHeightmap>("Import Heightmap", "Import");
				importRawHeightmap.InitializeImportRaw(TerrainMenus.GetActiveTerrain(), text);
			}
		}
		internal static void ExportHeightmapRaw()
		{
			ExportRawHeightmap exportRawHeightmap = ScriptableWizard.DisplayWizard<ExportRawHeightmap>("Export Heightmap", "Export");
			exportRawHeightmap.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}
		internal static void SetHeightmapResolution()
		{
			SetResolutionWizard setResolutionWizard = ScriptableWizard.DisplayWizard<SetResolutionWizard>("Set Heightmap resolution", "Set Resolution");
			setResolutionWizard.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}
		internal static void MassPlaceTrees()
		{
			PlaceTreeWizard placeTreeWizard = ScriptableWizard.DisplayWizard<PlaceTreeWizard>("Place Trees", "Place");
			placeTreeWizard.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}
		internal static void Flatten()
		{
			FlattenHeightmap flattenHeightmap = ScriptableWizard.DisplayWizard<FlattenHeightmap>("Flatten Heightmap", "Flatten");
			flattenHeightmap.InitializeDefaults(TerrainMenus.GetActiveTerrain());
		}
		internal static void RefreshPrototypes()
		{
			TerrainMenus.GetActiveTerrainData().RefreshPrototypes();
			TerrainMenus.GetActiveTerrain().Flush();
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
