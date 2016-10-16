using System;

namespace UnityEditor
{
	public enum ModelImporterMaterialName
	{
		BasedOnTextureName,
		BasedOnMaterialName,
		BasedOnModelNameAndMaterialName,
		[Obsolete("You should use ModelImporterMaterialName.BasedOnTextureName instead, because it it less complicated and behaves in more consistent way.")]
		BasedOnTextureName_Or_ModelNameAndMaterialName
	}
}
