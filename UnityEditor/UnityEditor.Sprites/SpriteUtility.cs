using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Sprites
{
	public sealed class SpriteUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetSpriteTexture(Sprite sprite, bool getAtlasData);

		[Obsolete("Use Sprite.vertices API instead. This data is the same for packed and unpacked sprites.")]
		public static Vector2[] GetSpriteMesh(Sprite sprite, bool getAtlasData)
		{
			return sprite.vertices;
		}

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GenerateOutline(Texture2D texture, ref Rect rect, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GenerateOutlineFromSprite(Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, out Vector2[][] paths);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Vector2[] GeneratePolygonOutlineVerticesOfSize(int sides, int width, int height);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateSpritePolygonAssetAtPath(string pathName, int sides);
	}
}
