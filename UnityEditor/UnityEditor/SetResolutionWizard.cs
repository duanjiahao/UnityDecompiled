using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	internal class SetResolutionWizard : TerrainWizard
	{
		public float m_TerrainWidth = 2000f;
		public float m_TerrainHeight = 600f;
		public float m_TerrainLength = 2000f;
		public int m_HeightmapResolution = 1024;
		public int m_DetailResolution = 1024;
		public int m_DetailResolutionPerPatch = 16;
		public int m_ControlTextureResolution = 1024;
		public int m_BaseTextureResolution = 1024;
		internal new void InitializeDefaults(Terrain terrain)
		{
			this.m_Terrain = terrain;
			this.m_TerrainWidth = base.terrainData.size.x;
			this.m_TerrainHeight = base.terrainData.size.y;
			this.m_TerrainLength = base.terrainData.size.z;
			this.m_HeightmapResolution = base.terrainData.heightmapResolution;
			this.m_DetailResolution = base.terrainData.detailResolution;
			this.m_DetailResolutionPerPatch = base.terrainData.detailResolutionPerPatch;
			this.m_ControlTextureResolution = base.terrainData.alphamapResolution;
			this.m_BaseTextureResolution = base.terrainData.baseMapResolution;
			this.OnWizardUpdate();
		}
		private void OnWizardCreate()
		{
			if (this.m_HeightmapResolution >= 4097)
			{
				base.isValid = false;
				base.errorString = "Heightmaps above 4096x4096 in resolution are not supported";
				Debug.LogError("Heightmaps above 4096x4096 in resolution are not supported");
				return;
			}
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			list.Add(base.terrainData);
			list.AddRange(base.terrainData.alphamapTextures);
			Undo.RegisterCompleteObjectUndo(list.ToArray(), "Set Resolution");
			if (base.terrainData.heightmapResolution != this.m_HeightmapResolution)
			{
				base.terrainData.heightmapResolution = this.m_HeightmapResolution;
			}
			base.terrainData.size = new Vector3(this.m_TerrainWidth, this.m_TerrainHeight, this.m_TerrainLength);
			if (base.terrainData.detailResolution != this.m_DetailResolution || this.m_DetailResolutionPerPatch != base.terrainData.detailResolutionPerPatch)
			{
				SetResolutionWizard.ResizeDetailResolution(base.terrainData, this.m_DetailResolution, this.m_DetailResolutionPerPatch);
			}
			if (base.terrainData.alphamapResolution != this.m_ControlTextureResolution)
			{
				base.terrainData.alphamapResolution = this.m_ControlTextureResolution;
			}
			if (base.terrainData.baseMapResolution != this.m_BaseTextureResolution)
			{
				base.terrainData.baseMapResolution = this.m_BaseTextureResolution;
			}
			base.FlushHeightmapModification();
		}
		private static void ResizeDetailResolution(TerrainData terrainData, int resolution, int resolutionPerPatch)
		{
			if (resolution == terrainData.detailResolution)
			{
				List<int[,]> list = new List<int[,]>();
				for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
				{
					list.Add(terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i));
				}
				terrainData.SetDetailResolution(resolution, resolutionPerPatch);
				for (int j = 0; j < list.Count; j++)
				{
					terrainData.SetDetailLayer(0, 0, j, list[j]);
				}
			}
			else
			{
				terrainData.SetDetailResolution(resolution, resolutionPerPatch);
			}
		}
		internal override void OnWizardUpdate()
		{
			base.helpString = "Please note that modifying the resolution will clear the heightmap, detail map or splatmap.";
			base.OnWizardUpdate();
			if (base.terrainData != null)
			{
				this.m_HeightmapResolution = base.terrainData.GetAdjustedSize(this.m_HeightmapResolution);
			}
			if (this.m_TerrainWidth <= 0f)
			{
				this.m_TerrainWidth = 1f;
			}
			if (this.m_TerrainHeight <= 0f)
			{
				this.m_TerrainHeight = 1f;
			}
			if (this.m_TerrainLength <= 0f)
			{
				this.m_TerrainLength = 1f;
			}
			this.m_ControlTextureResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(this.m_ControlTextureResolution), 16, 2048);
			this.m_BaseTextureResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(this.m_BaseTextureResolution), 16, 2048);
			this.m_DetailResolution = Mathf.Clamp(this.m_DetailResolution, 0, 4048);
			this.m_DetailResolutionPerPatch = Mathf.Clamp(this.m_DetailResolutionPerPatch, 8, 128);
		}
	}
}
