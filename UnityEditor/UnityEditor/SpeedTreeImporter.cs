using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	public sealed class SpeedTreeImporter : AssetImporter
	{
		public static readonly string[] windQualityNames = new string[]
		{
			"None",
			"Fastest",
			"Fast",
			"Better",
			"Best",
			"Palm"
		};

		public extern bool hasImported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string materialFolderPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float scaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color mainColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_mainColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_mainColor(ref value);
			}
		}

		[Obsolete("specColor is no longer used and has been deprecated.", true)]
		public Color specColor
		{
			get;
			set;
		}

		[Obsolete("shininess is no longer used and has been deprecated.", true)]
		public float shininess
		{
			get;
			set;
		}

		public Color hueVariation
		{
			get
			{
				Color result;
				this.INTERNAL_get_hueVariation(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_hueVariation(ref value);
			}
		}

		public extern float alphaTestRef
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasBillboard
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool enableSmoothLODTransition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool animateCrossFading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float billboardTransitionCrossFadeWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float fadeOutWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float[] LODHeights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool[] castShadows
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool[] receiveShadows
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool[] useLightProbes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ReflectionProbeUsage[] reflectionProbeUsages
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool[] enableBump
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool[] enableHue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int bestWindQuality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int[] windQualities
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal extern bool materialsShouldBeRegenerated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_mainColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_mainColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_hueVariation(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_hueVariation(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GenerateMaterials();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetMaterialVersionToCurrent();
	}
}
