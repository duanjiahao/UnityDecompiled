using System;

namespace UnityEditor
{
	public enum TextureImporterType
	{
		Default,
		[Obsolete("Use Default (UnityUpgradable) -> Default")]
		Image = 0,
		NormalMap,
		[Obsolete("Use NormalMap (UnityUpgradable) -> NormalMap")]
		Bump = 1,
		GUI,
		Sprite = 8,
		Cursor = 7,
		[Obsolete("Use importer.textureShape = TextureImporterShape.TextureCube")]
		Cubemap = 3,
		[Obsolete("Use a texture setup as a cubemap with glossy reflection instead")]
		Reflection = 3,
		Cookie,
		Lightmap = 6,
		[Obsolete("HDRI is not supported anymore")]
		HDRI = 9,
		[Obsolete("Use Default instead. All texture types now have an Advanced foldout (UnityUpgradable) -> Default")]
		Advanced = 5,
		SingleChannel = 10
	}
}
