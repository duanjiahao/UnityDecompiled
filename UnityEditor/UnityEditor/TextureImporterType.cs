using System;

namespace UnityEditor
{
	public enum TextureImporterType
	{
		Image,
		Bump,
		GUI,
		Sprite = 8,
		Cursor = 7,
		Cubemap = 3,
		[Obsolete("Use Cubemap")]
		Reflection = 3,
		Cookie,
		Lightmap = 6,
		Advanced = 5
	}
}
