using System;
using UnityEngine;
namespace UnityEditor
{
	internal class PlaceTreeWizard : TerrainWizard
	{
		public int numberOfTrees = 10000;
		public void OnEnable()
		{
			base.minSize = new Vector2(250f, 150f);
		}
		private void OnWizardCreate()
		{
			TreePlacementUtility.PlaceRandomTrees(this.m_Terrain.terrainData, this.numberOfTrees);
			this.m_Terrain.Flush();
		}
	}
}
