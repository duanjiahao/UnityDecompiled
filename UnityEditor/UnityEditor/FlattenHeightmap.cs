using System;

namespace UnityEditor
{
	internal class FlattenHeightmap : TerrainWizard
	{
		public float height = 0f;

		internal override void OnWizardUpdate()
		{
			if (base.terrainData)
			{
				base.helpString = string.Concat(new object[]
				{
					this.height,
					" meters (",
					this.height / base.terrainData.size.y * 100f,
					"%)"
				});
			}
		}

		private void OnWizardCreate()
		{
			Undo.RegisterCompleteObjectUndo(base.terrainData, "Flatten Heightmap");
			HeightmapFilters.Flatten(base.terrainData, this.height / base.terrainData.size.y);
		}
	}
}
