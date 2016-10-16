using System;

namespace UnityEditor
{
	public enum TextureImporterGenerateCubemap
	{
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
