using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class Terrain : Behaviour
	{
		public enum MaterialType
		{
			BuiltInStandard,
			BuiltInLegacyDiffuse,
			BuiltInLegacySpecular,
			Custom
		}

		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float treeDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float treeBillboardDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float treeCrossFadeLength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int treeMaximumFullLODCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float detailObjectDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float detailObjectDensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float heightmapPixelError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int heightmapMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float basemapDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("use basemapDistance", true)]
		public float splatmapDistance
		{
			get
			{
				return this.basemapDistance;
			}
			set
			{
				this.basemapDistance = value;
			}
		}

		public extern int lightmapIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int realtimeLightmapIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector4 lightmapScaleOffset
		{
			get
			{
				Vector4 result;
				this.INTERNAL_get_lightmapScaleOffset(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_lightmapScaleOffset(ref value);
			}
		}

		public Vector4 realtimeLightmapScaleOffset
		{
			get
			{
				Vector4 result;
				this.INTERNAL_get_realtimeLightmapScaleOffset(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_realtimeLightmapScaleOffset(ref value);
			}
		}

		public extern bool castShadows
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ReflectionProbeUsage reflectionProbeUsage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Terrain.MaterialType materialType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Material materialTemplate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color legacySpecular
		{
			get
			{
				Color result;
				this.INTERNAL_get_legacySpecular(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_legacySpecular(ref value);
			}
		}

		public extern float legacyShininess
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool drawHeightmap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool drawTreesAndFoliage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float treeLODBiasMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool collectDetailPatches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TerrainRenderFlags editorRenderFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bakeLightProbesForTrees
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern Terrain activeTerrain
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Terrain[] activeTerrains
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetClosestReflectionProbesInternal(object result);

		public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
		{
			this.GetClosestReflectionProbesInternal(result);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_legacySpecular(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_legacySpecular(ref Color value);

		public float SampleHeight(Vector3 worldPosition)
		{
			return Terrain.INTERNAL_CALL_SampleHeight(this, ref worldPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_SampleHeight(Terrain self, ref Vector3 worldPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ApplyDelayedHeightmapModification();

		public void AddTreeInstance(TreeInstance instance)
		{
			Terrain.INTERNAL_CALL_AddTreeInstance(this, ref instance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddTreeInstance(Terrain self, ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom);

		public Vector3 GetPosition()
		{
			Vector3 result;
			Terrain.INTERNAL_CALL_GetPosition(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPosition(Terrain self, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Flush();

		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			Terrain.INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_RemoveTrees(Terrain self, ref Vector2 position, float radius, int prototypeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetSplatMaterialPropertyBlock(MaterialPropertyBlock properties);

		public void GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			this.Internal_GetSplatMaterialPropertyBlock(dest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest);

		[UsedByNativeCode]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GameObject CreateTerrainGameObject(TerrainData assignTerrain);
	}
}
