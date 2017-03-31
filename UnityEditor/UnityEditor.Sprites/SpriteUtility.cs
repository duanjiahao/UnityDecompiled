using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.Sprites
{
	public sealed class SpriteUtility
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetSpriteTexture(Sprite sprite, bool getAtlasData);

		[Obsolete("Use Sprite.vertices API instead. This data is the same for packed and unpacked sprites.")]
		public static Vector2[] GetSpriteMesh(Sprite sprite, bool getAtlasData)
		{
			return sprite.vertices;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Vector2[] GetSpriteUVs(Sprite sprite, bool getAtlasData);

		[Obsolete("Use Sprite.triangles API instead. This data is the same for packed and unpacked sprites.")]
		public static ushort[] GetSpriteIndices(Sprite sprite, bool getAtlasData)
		{
			return sprite.triangles;
		}

		internal static void GenerateOutline(Texture2D texture, Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths)
		{
			SpriteUtility.INTERNAL_CALL_GenerateOutline(texture, ref rect, detail, alphaTolerance, holeDetection, out paths);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GenerateOutline(Texture2D texture, ref Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GenerateOutlineFromSprite(Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] GeneratePolygonOutlineVerticesOfSize(int sides, int width, int height);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateSpritePolygonAssetAtPath(string pathName, int sides);
	}
}
