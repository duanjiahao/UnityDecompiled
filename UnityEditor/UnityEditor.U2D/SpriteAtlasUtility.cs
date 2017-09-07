using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.U2D;

namespace UnityEditor.U2D
{
	internal sealed class SpriteAtlasUtility
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PackAllAtlases(BuildTarget target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void PackAtlases(SpriteAtlas[] atlases, BuildTarget target);
	}
}
