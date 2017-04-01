using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class LightmapEditorSettings
	{
		public enum GIBakeBackend
		{
			Radiosity,
			PathTracer
		}

		public enum PathTracerSampling
		{
			Auto,
			Fixed
		}

		public enum PathTracerFilterMode
		{
			None,
			Auto,
			Advanced
		}

		public enum PathTracerFilter
		{
			Gaussian,
			ATrous
		}

		public static extern LightmapEditorSettings.GIBakeBackend giBakeBackend
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern LightmapEditorSettings.PathTracerSampling giPathTracerSampling
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern LightmapEditorSettings.PathTracerFilter giPathTracerFilter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int maxAtlasWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int maxAtlasHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float realtimeResolution
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float bakeResolution
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool textureCompression
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ReflectionCubemapCompression reflectionCubemapCompression
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableAmbientOcclusion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float aoMaxDistance
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float aoExponentIndirect
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float aoExponentDirect
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int padding
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("LightmapEditorSettings.aoContrast has been deprecated.", false)]
		public static float aoContrast
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.aoAmount has been deprecated.", false)]
		public static float aoAmount
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.lockAtlas has been deprecated.", false)]
		public static bool lockAtlas
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.skyLightColor has been deprecated.", false)]
		public static Color skyLightColor
		{
			get
			{
				return Color.black;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.skyLightIntensity has been deprecated.", false)]
		public static float skyLightIntensity
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.quality has been deprecated.", false)]
		public static LightmapBakeQuality quality
		{
			get
			{
				return LightmapBakeQuality.High;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.bounceBoost has been deprecated.", false)]
		public static float bounceBoost
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.finalGatherRays has been deprecated.", false)]
		public static int finalGatherRays
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.finalGatherContrastThreshold has been deprecated.", false)]
		public static float finalGatherContrastThreshold
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.finalGatherGradientThreshold has been deprecated.", false)]
		public static float finalGatherGradientThreshold
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.finalGatherInterpolationPoints has been deprecated.", false)]
		public static int finalGatherInterpolationPoints
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.lastUsedResolution has been deprecated.", false)]
		public static float lastUsedResolution
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.bounces has been deprecated.", false)]
		public static int bounces
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("LightmapEditorSettings.bounceIntensity has been deprecated.", false)]
		public static float bounceIntensity
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("resolution is now called realtimeResolution (UnityUpgradable) -> realtimeResolution", false)]
		public static float resolution
		{
			get
			{
				return LightmapEditorSettings.realtimeResolution;
			}
			set
			{
				LightmapEditorSettings.realtimeResolution = value;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reset();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsLightmappedOrDynamicLightmappedForRendering(Renderer renderer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasZeroAreaMesh(Renderer renderer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasClampedResolution(Renderer renderer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetSystemResolution(Renderer renderer, out int width, out int height);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetTerrainSystemResolution(Terrain terrain, out int width, out int height, out int numChunksInX, out int numChunksInY);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetInstanceResolution(Renderer renderer, out int width, out int height);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetInputSystemHash(Renderer renderer, out Hash128 inputSystemHash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetInstanceHash(Renderer renderer, out Hash128 instanceHash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetPVRInstanceHash(int instanceID, out Hash128 instanceHash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetPVRAtlasHash(int instanceID, out Hash128 atlasHash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetPVRAtlasInstanceOffset(int instanceID, out int atlasInstanceOffset);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetGeometryHash(Renderer renderer, out Hash128 geometryHash);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void AnalyzeLighting(out LightingStats enabled, out LightingStats active, out LightingStats inactive);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UnityEngine.Object GetLightmapSettings();
	}
}
