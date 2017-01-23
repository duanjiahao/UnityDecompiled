using System;

namespace UnityEditor
{
	public enum TextureImporterGenerateCubemap
	{
		[Obsolete("This value is deprecated (use TextureImporter.textureShape instead).")]
		None,
		Spheremap,
		Cylindrical,
		[Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
		SimpleSpheremap,
		[Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
		NiceSpheremap,
		FullCubemap,
		AutoCubemap
	}
}
