using System;

namespace UnityEditor
{
	public enum DrawCameraMode
	{
		Normal = -1,
		Textured,
		Wireframe,
		TexturedWire,
		ShadowCascades,
		RenderPaths,
		AlphaChannel,
		Overdraw,
		Mipmaps,
		DeferredDiffuse,
		DeferredSpecular,
		DeferredSmoothness,
		DeferredNormal,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeCharting instead. (UnityUpgradable) -> RealtimeCharting", true)]
		Charting = -12,
		RealtimeCharting = 12,
		Systems,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeAlbedo instead. (UnityUpgradable) -> RealtimeAlbedo", true)]
		Albedo = -14,
		RealtimeAlbedo = 14,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeEmissive instead. (UnityUpgradable) -> RealtimeEmissive", true)]
		Emissive = -15,
		RealtimeEmissive = 15,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeIndirect instead. (UnityUpgradable) -> RealtimeIndirect", true)]
		Irradiance = -16,
		RealtimeIndirect = 16,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeDirectionality instead. (UnityUpgradable) -> RealtimeDirectionality", true)]
		Directionality = -17,
		RealtimeDirectionality = 17,
		[Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use BakedLightmap instead. (UnityUpgradable) -> BakedLightmap", true)]
		Baked = -18,
		BakedLightmap = 18,
		Clustering,
		LitClustering,
		ValidateAlbedo,
		ValidateMetalSpecular,
		ShadowMasks,
		LightOverlap,
		BakedAlbedo,
		BakedEmissive,
		BakedDirectionality,
		BakedTexelValidity,
		BakedIndices,
		BakedCharting
	}
}
