using System;

namespace UnityEditor
{
	[Obsolete("Use ModelImporterMaterialName, ModelImporter.materialName and ModelImporter.importMaterials instead")]
	public enum ModelImporterGenerateMaterials
	{
		[Obsolete("Use ModelImporter.importMaterials=false instead")]
		None,
		[Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnTextureName instead")]
		PerTexture,
		[Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnModelNameAndMaterialName instead")]
		PerSourceMaterial
	}
}
