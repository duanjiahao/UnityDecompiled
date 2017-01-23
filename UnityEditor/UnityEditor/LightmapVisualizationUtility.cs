using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

namespace UnityEditor
{
	internal sealed class LightmapVisualizationUtility
	{
		public static Vector4 GetLightmapTilingOffset(LightmapType lightmapType)
		{
			Vector4 result;
			LightmapVisualizationUtility.INTERNAL_CALL_GetLightmapTilingOffset(lightmapType, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLightmapTilingOffset(LightmapType lightmapType, out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetGITexture(GITextureType textureType);

		public static void DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, Rect drawableArea, Rect position, GITextureType textureType, bool drawSpecularUV)
		{
			LightmapVisualizationUtility.INTERNAL_CALL_DrawTextureWithUVOverlay(texture, gameObject, ref drawableArea, ref position, textureType, drawSpecularUV);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawTextureWithUVOverlay(Texture2D texture, GameObject gameObject, ref Rect drawableArea, ref Rect position, GITextureType textureType, bool drawSpecularUV);
	}
}
