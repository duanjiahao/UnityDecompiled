using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PlaceTreeWizard : TerrainWizard
	{
		private const int kMaxNumberOfTrees = 1000000;

		public int numberOfTrees = 10000;

		public bool keepExistingTrees = true;

		public void OnEnable()
		{
			base.minSize = new Vector2(250f, 150f);
		}

		private void OnWizardCreate()
		{
			if (this.numberOfTrees > 1000000)
			{
				base.isValid = false;
				base.errorString = string.Format("Mass placing more than {0} trees is not supported", 1000000);
				Debug.LogError(base.errorString);
				return;
			}
			TreePainter.MassPlaceTrees(this.m_Terrain.terrainData, this.numberOfTrees, true, this.keepExistingTrees);
			this.m_Terrain.Flush();
		}
	}
}
