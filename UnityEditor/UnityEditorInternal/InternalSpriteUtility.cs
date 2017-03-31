using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	public sealed class InternalSpriteUtility
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Rect[] GenerateAutomaticSpriteRectangles(Texture2D texture, int minRectSize, int extrudeSize);

		public static Rect[] GenerateGridSpriteRectangles(Texture2D texture, Vector2 offset, Vector2 size, Vector2 padding)
		{
			return InternalSpriteUtility.INTERNAL_CALL_GenerateGridSpriteRectangles(texture, ref offset, ref size, ref padding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Rect[] INTERNAL_CALL_GenerateGridSpriteRectangles(Texture2D texture, ref Vector2 offset, ref Vector2 size, ref Vector2 padding);
	}
}
