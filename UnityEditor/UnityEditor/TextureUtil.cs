using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class TextureUtil
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStorageMemorySize(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRuntimeMemorySize(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsNonPowerOfTwo(Texture2D t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TextureUsageMode GetUsageMode(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetBytesFromTextureFormat(TextureFormat inFormat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRowBytesFromWidthAndFormat(int width, TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidTextureFormat(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsCompressedTextureFormat(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TextureFormat GetTextureFormat(Texture texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAlphaOnlyTextureFormat(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAlphaTextureFormat(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTextureFormatString(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetTextureColorSpaceString(Texture texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TextureFormat ConvertToAlphaTextureFormat(TextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDepthRTFormat(RenderTextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasMipMap(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGPUWidth(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGPUHeight(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetMipmapCount(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetLinearSampled(Texture t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDefaultCompressionQuality();

		public static Vector4 GetTexelSizeVector(Texture t)
		{
			Vector4 result;
			TextureUtil.INTERNAL_CALL_GetTexelSizeVector(t, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTexelSizeVector(Texture t, out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetSourceTexture(Cubemap cubemapRef, CubemapFace face);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSourceTexture(Cubemap cubemapRef, CubemapFace face, Texture2D tex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyTextureIntoCubemapFace(Texture2D textureRef, Cubemap cubemapRef, CubemapFace face);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyCubemapFaceIntoTexture(Cubemap cubemapRef, CubemapFace face, Texture2D textureRef);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ReformatCubemap(ref Cubemap cubemap, int width, int height, TextureFormat textureFormat, bool useMipmap, bool linear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ReformatTexture(ref Texture2D texture, int width, int height, TextureFormat textureFormat, bool useMipmap, bool linear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnisoLevelNoDirty(Texture tex, int level);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetWrapModeNoDirty(Texture tex, TextureWrapMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMipMapBiasNoDirty(Texture tex, float bias);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFilterModeNoDirty(Texture tex, FilterMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DoesTextureStillNeedToBeCompressed(string assetPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsCubemapReadable(Cubemap cubemapRef);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MarkCubemapReadable(Cubemap cubemapRef, bool readable);
	}
}
