using System;
namespace UnityEditor
{
	[Obsolete("Use ModelImporterMaterialName, ModelImporter.materialName and ModelImporter.importMaterials instead")]
	public enum ModelImporterGenerateMaterials
	{
		None,
		PerTexture,
		PerSourceMaterial
	}
}
