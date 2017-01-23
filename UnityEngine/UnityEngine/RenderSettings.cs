using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class RenderSettings : Object
	{
		public static extern bool fog
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern FogMode fogMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Color fogColor
		{
			get
			{
				Color result;
				RenderSettings.INTERNAL_get_fogColor(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_fogColor(ref value);
			}
		}

		public static extern float fogDensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float fogStartDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float fogEndDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern AmbientMode ambientMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static Color ambientSkyColor
		{
			get
			{
				Color result;
				RenderSettings.INTERNAL_get_ambientSkyColor(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_ambientSkyColor(ref value);
			}
		}

		public static Color ambientEquatorColor
		{
			get
			{
				Color result;
				RenderSettings.INTERNAL_get_ambientEquatorColor(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_ambientEquatorColor(ref value);
			}
		}

		public static Color ambientGroundColor
		{
			get
			{
				Color result;
				RenderSettings.INTERNAL_get_ambientGroundColor(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_ambientGroundColor(ref value);
			}
		}

		public static Color ambientLight
		{
			get
			{
				Color result;
				RenderSettings.INTERNAL_get_ambientLight(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_ambientLight(ref value);
			}
		}

		public static extern float ambientIntensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static SphericalHarmonicsL2 ambientProbe
		{
			get
			{
				SphericalHarmonicsL2 result;
				RenderSettings.INTERNAL_get_ambientProbe(out result);
				return result;
			}
			set
			{
				RenderSettings.INTERNAL_set_ambientProbe(ref value);
			}
		}

		public static extern float reflectionIntensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int reflectionBounces
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float haloStrength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float flareStrength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float flareFadeSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Material skybox
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Light sun
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern DefaultReflectionMode defaultReflectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int defaultReflectionResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Cubemap customReflection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use RenderSettings.ambientIntensity instead (UnityUpgradable) -> ambientIntensity", false)]
		public static float ambientSkyboxAmount
		{
			get
			{
				return RenderSettings.ambientIntensity;
			}
			set
			{
				RenderSettings.ambientIntensity = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_fogColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_fogColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_ambientSkyColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_ambientSkyColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_ambientEquatorColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_ambientEquatorColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_ambientGroundColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_ambientGroundColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_ambientLight(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_ambientLight(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_ambientProbe(out SphericalHarmonicsL2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_ambientProbe(ref SphericalHarmonicsL2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Reset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object GetRenderSettings();
	}
}
