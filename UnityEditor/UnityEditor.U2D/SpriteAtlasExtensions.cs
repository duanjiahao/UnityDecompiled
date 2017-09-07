using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.U2D;

namespace UnityEditor.U2D
{
	internal static class SpriteAtlasExtensions
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Add(this SpriteAtlas spriteAtlas, UnityEngine.Object[] objects);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Remove(this SpriteAtlas spriteAtlas, UnityEngine.Object[] objects);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveAt(this SpriteAtlas spriteAtlas, int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object[] GetPackables(this SpriteAtlas spriteAtlas);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyTextureSettingsTo(this SpriteAtlas spriteAtlas, SpriteAtlasTextureSettings dest);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetTextureSettings(this SpriteAtlas spriteAtlas, SpriteAtlasTextureSettings src);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CopyPlatformSettingsIfAvailable(this SpriteAtlas spriteAtlas, string buildTarget, TextureImporterPlatformSettings dest);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPlatformSettings(this SpriteAtlas spriteAtlas, TextureImporterPlatformSettings src);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyPackingParametersTo(this SpriteAtlas spriteAtlas, SpriteAtlasPackingParameters dest);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPackingParameters(this SpriteAtlas spriteAtlas, SpriteAtlasPackingParameters src);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIncludeInBuild(this SpriteAtlas spriteAtlas, bool value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIsVariant(this SpriteAtlas spriteAtlas, bool value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMasterAtlas(this SpriteAtlas spriteAtlas, SpriteAtlas value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyMasterAtlasSettings(this SpriteAtlas spriteAtlas);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVariantMultiplier(this SpriteAtlas spriteAtlas, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetHashString(this SpriteAtlas spriteAtlas);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D[] GetPreviewTextures(this SpriteAtlas spriteAtlas);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern TextureImporterFormat FormatDetermineByAtlasSettings(this SpriteAtlas spriteAtlas, BuildTarget target);
	}
}
