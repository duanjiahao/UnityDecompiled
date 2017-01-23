using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class TerrainData : Object
	{
		private static readonly int kMaximumResolution = TerrainData.Internal_GetMaximumResolution();

		private static readonly int kMinimumDetailResolutionPerPatch = TerrainData.Internal_GetMinimumDetailResolutionPerPatch();

		private static readonly int kMaximumDetailResolutionPerPatch = TerrainData.Internal_GetMaximumDetailResolutionPerPatch();

		private static readonly int kMaximumDetailPatchCount = TerrainData.Internal_GetMaximumDetailPatchCount();

		private static readonly int kMinimumAlphamapResolution = TerrainData.Internal_GetMinimumAlphamapResolution();

		private static readonly int kMaximumAlphamapResolution = TerrainData.Internal_GetMaximumAlphamapResolution();

		private static readonly int kMinimumBaseMapResolution = TerrainData.Internal_GetMinimumBaseMapResolution();

		private static readonly int kMaximumBaseMapResolution = TerrainData.Internal_GetMaximumBaseMapResolution();

		public extern int heightmapWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int heightmapHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int heightmapResolution
		{
			get
			{
				return this.Internal_heightmapResolution;
			}
			set
			{
				int internal_heightmapResolution = value;
				if (value < 0 || value > TerrainData.kMaximumResolution)
				{
					Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + TerrainData.kMaximumResolution + "].");
					internal_heightmapResolution = Math.Min(TerrainData.kMaximumResolution, Math.Max(value, 0));
				}
				this.Internal_heightmapResolution = internal_heightmapResolution;
			}
		}

		private extern int Internal_heightmapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 heightmapScale
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_heightmapScale(out result);
				return result;
			}
		}

		public Vector3 size
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_size(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_size(ref value);
			}
		}

		public Bounds bounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_bounds(out result);
				return result;
			}
		}

		public extern float thickness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float wavingGrassStrength
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float wavingGrassAmount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float wavingGrassSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color wavingGrassTint
		{
			get
			{
				Color result;
				this.INTERNAL_get_wavingGrassTint(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_wavingGrassTint(ref value);
			}
		}

		public extern int detailWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int detailHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int detailResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern int detailResolutionPerPatch
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern DetailPrototype[] detailPrototypes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TreeInstance[] treeInstances
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int treeInstanceCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TreePrototype[] treePrototypes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int alphamapLayers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int alphamapResolution
		{
			get
			{
				return this.Internal_alphamapResolution;
			}
			set
			{
				int internal_alphamapResolution = value;
				if (value < TerrainData.kMinimumAlphamapResolution || value > TerrainData.kMaximumAlphamapResolution)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"alphamapResolution is clamped to the range of [",
						TerrainData.kMinimumAlphamapResolution,
						", ",
						TerrainData.kMaximumAlphamapResolution,
						"]."
					}));
					internal_alphamapResolution = Math.Min(TerrainData.kMaximumAlphamapResolution, Math.Max(value, TerrainData.kMinimumAlphamapResolution));
				}
				this.Internal_alphamapResolution = internal_alphamapResolution;
			}
		}

		private extern int Internal_alphamapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public int alphamapWidth
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		public int alphamapHeight
		{
			get
			{
				return this.alphamapResolution;
			}
		}

		public int baseMapResolution
		{
			get
			{
				return this.Internal_baseMapResolution;
			}
			set
			{
				int internal_baseMapResolution = value;
				if (value < TerrainData.kMinimumBaseMapResolution || value > TerrainData.kMaximumBaseMapResolution)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"baseMapResolution is clamped to the range of [",
						TerrainData.kMinimumBaseMapResolution,
						", ",
						TerrainData.kMaximumBaseMapResolution,
						"]."
					}));
					internal_baseMapResolution = Math.Min(TerrainData.kMaximumBaseMapResolution, Math.Max(value, TerrainData.kMinimumBaseMapResolution));
				}
				this.Internal_baseMapResolution = internal_baseMapResolution;
			}
		}

		private extern int Internal_baseMapResolution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		private extern int alphamapTextureCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Texture2D[] alphamapTextures
		{
			get
			{
				Texture2D[] array = new Texture2D[this.alphamapTextureCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetAlphamapTexture(i);
				}
				return array;
			}
		}

		public extern SplatPrototype[] splatPrototypes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public TerrainData()
		{
			this.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaximumResolution();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMinimumDetailResolutionPerPatch();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaximumDetailResolutionPerPatch();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaximumDetailPatchCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMinimumAlphamapResolution();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaximumAlphamapResolution();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMinimumBaseMapResolution();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaximumBaseMapResolution();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_Create([Writable] TerrainData terrainData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasUser(GameObject user);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddUser(GameObject user);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveUser(GameObject user);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_heightmapScale(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_size(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_size(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetHeight(int x, int y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetInterpolatedHeight(float x, float y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float[,] GetHeights(int xBase, int yBase, int width, int height);

		public void SetHeights(int xBase, int yBase, float[,] heights)
		{
			if (heights == null)
			{
				throw new NullReferenceException();
			}
			if (xBase + heights.GetLength(1) > this.heightmapWidth || xBase + heights.GetLength(1) < 0 || yBase + heights.GetLength(0) < 0 || xBase < 0 || yBase < 0 || yBase + heights.GetLength(0) > this.heightmapHeight)
			{
				throw new ArgumentException(UnityString.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{3}", new object[]
				{
					xBase + heights.GetLength(1),
					yBase + heights.GetLength(0),
					this.heightmapWidth,
					this.heightmapHeight
				}));
			}
			this.Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights);

		public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
		{
			if (heights == null)
			{
				throw new ArgumentNullException("heights");
			}
			int length = heights.GetLength(0);
			int length2 = heights.GetLength(1);
			if (xBase < 0 || xBase + length2 < 0 || xBase + length2 > this.heightmapWidth)
			{
				throw new ArgumentException(UnityString.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", new object[]
				{
					xBase,
					xBase + length2,
					this.heightmapWidth
				}));
			}
			if (yBase < 0 || yBase + length < 0 || yBase + length > this.heightmapHeight)
			{
				throw new ArgumentException(UnityString.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", new object[]
				{
					yBase,
					yBase + length,
					this.heightmapHeight
				}));
			}
			this.Internal_SetHeightsDelayLOD(xBase, yBase, length2, length, heights);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetSteepness(float x, float y);

		public Vector3 GetInterpolatedNormal(float x, float y)
		{
			Vector3 result;
			TerrainData.INTERNAL_CALL_GetInterpolatedNormal(this, x, y, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetInterpolatedNormal(TerrainData self, float x, float y, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetAdjustedSize(int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_wavingGrassTint(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_wavingGrassTint(ref Color value);

		public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
		{
			if (detailResolution < 0)
			{
				Debug.LogWarning("detailResolution must not be negative.");
				detailResolution = 0;
			}
			if (resolutionPerPatch < TerrainData.kMinimumDetailResolutionPerPatch || resolutionPerPatch > TerrainData.kMaximumDetailResolutionPerPatch)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"resolutionPerPatch is clamped to the range of [",
					TerrainData.kMinimumDetailResolutionPerPatch,
					", ",
					TerrainData.kMaximumDetailResolutionPerPatch,
					"]."
				}));
				resolutionPerPatch = Math.Min(TerrainData.kMaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, TerrainData.kMinimumDetailResolutionPerPatch));
			}
			int num = detailResolution / resolutionPerPatch;
			if (num > TerrainData.kMaximumDetailPatchCount)
			{
				Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + TerrainData.kMaximumDetailPatchCount + "].");
				num = Math.Min(TerrainData.kMaximumDetailPatchCount, Math.Max(num, 0));
			}
			this.Internal_SetDetailResolution(num, resolutionPerPatch);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ResetDirtyDetails();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RefreshPrototypes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer);

		public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
		{
			this.Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data);

		public TreeInstance GetTreeInstance(int index)
		{
			TreeInstance result;
			TerrainData.INTERNAL_CALL_GetTreeInstance(this, index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTreeInstance(TerrainData self, int index, out TreeInstance value);

		public void SetTreeInstance(int index, TreeInstance instance)
		{
			TerrainData.INTERNAL_CALL_SetTreeInstance(this, index, ref instance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTreeInstance(TerrainData self, int index, ref TreeInstance instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveTreePrototype(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RecalculateTreePositions();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveDetailPrototype(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool NeedUpgradeScaledTreePrototypes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void UpgradeScaledTreePrototype();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float[,,] GetAlphamaps(int x, int y, int width, int height);

		public void SetAlphamaps(int x, int y, float[,,] map)
		{
			if (map.GetLength(2) != this.alphamapLayers)
			{
				throw new Exception(UnityString.Format("Float array size wrong (layers should be {0})", new object[]
				{
					this.alphamapLayers
				}));
			}
			this.Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RecalculateBasemapIfDirty();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetBasemapDirty(bool dirty);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Texture2D GetAlphamapTexture(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddTree(out TreeInstance tree);

		internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			return TerrainData.INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_RemoveTrees(TerrainData self, ref Vector2 position, float radius, int prototypeIndex);
	}
}
