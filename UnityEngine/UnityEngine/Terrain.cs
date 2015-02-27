using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	[AddComponentMenu(""), ExecuteInEditMode]
	public sealed class Terrain : MonoBehaviour
	{
		[SerializeField]
		private TerrainData m_TerrainData;
		[SerializeField]
		private float m_TreeDistance = 5000f;
		[SerializeField]
		private float m_TreeBillboardDistance = 50f;
		[SerializeField]
		private float m_TreeCrossFadeLength = 5f;
		[SerializeField]
		private int m_TreeMaximumFullLODCount = 50;
		[SerializeField]
		private float m_DetailObjectDistance = 80f;
		[SerializeField]
		private float m_DetailObjectDensity = 1f;
		[SerializeField]
		private float m_HeightmapPixelError = 5f;
		[SerializeField]
		private float m_SplatMapDistance = 1000f;
		[SerializeField]
		private int m_HeightmapMaximumLOD;
		[SerializeField]
		private bool m_CastShadows = true;
		[SerializeField]
		private int m_LightmapIndex = -1;
		[SerializeField]
		private int m_LightmapSize = 1024;
		[SerializeField]
		private bool m_DrawTreesAndFoliage = true;
		[SerializeField]
		private bool m_CollectDetailPatches = true;
		[SerializeField]
		private Material m_MaterialTemplate;
		[NonSerialized]
		private IntPtr m_TerrainInstance;
		private IntPtr InstanceObject
		{
			get
			{
				this.MakeSureObjectIsAlive();
				if (this.m_TerrainInstance == IntPtr.Zero)
				{
					this.m_TerrainInstance = this.Construct();
					this.Internal_SetTerrainData(this.m_TerrainInstance, this.m_TerrainData);
					this.Internal_SetTreeDistance(this.m_TerrainInstance, this.m_TreeDistance);
					this.Internal_SetTreeBillboardDistance(this.m_TerrainInstance, this.m_TreeBillboardDistance);
					this.Internal_SetTreeCrossFadeLength(this.m_TerrainInstance, this.m_TreeCrossFadeLength);
					this.Internal_SetTreeMaximumFullLODCount(this.m_TerrainInstance, this.m_TreeMaximumFullLODCount);
					this.Internal_SetDetailObjectDistance(this.m_TerrainInstance, this.m_DetailObjectDistance);
					this.Internal_SetDetailObjectDensity(this.m_TerrainInstance, this.m_DetailObjectDensity);
					this.Internal_SetHeightmapPixelError(this.m_TerrainInstance, this.m_HeightmapPixelError);
					this.Internal_SetBasemapDistance(this.m_TerrainInstance, this.m_SplatMapDistance);
					this.Internal_SetHeightmapMaximumLOD(this.m_TerrainInstance, this.m_HeightmapMaximumLOD);
					this.Internal_SetCastShadows(this.m_TerrainInstance, this.m_CastShadows);
					this.Internal_SetLightmapIndex(this.m_TerrainInstance, this.m_LightmapIndex);
					this.Internal_SetLightmapSize(this.m_TerrainInstance, this.m_LightmapSize);
					this.Internal_SetDrawTreesAndFoliage(this.m_TerrainInstance, this.m_DrawTreesAndFoliage);
					this.Internal_SetCollectDetailPatches(this.m_TerrainInstance, this.m_CollectDetailPatches);
					this.Internal_SetMaterialTemplate(this.m_TerrainInstance, this.m_MaterialTemplate);
				}
				return this.m_TerrainInstance;
			}
			set
			{
				this.m_TerrainInstance = value;
			}
		}
		public TerrainRenderFlags editorRenderFlags
		{
			get
			{
				return (TerrainRenderFlags)this.GetEditorRenderFlags(this.InstanceObject);
			}
			set
			{
				this.SetEditorRenderFlags(this.InstanceObject, (int)value);
			}
		}
		public TerrainData terrainData
		{
			get
			{
				if (this.m_TerrainData != this.Internal_GetTerrainData(this.InstanceObject))
				{
					this.Internal_SetTerrainData(this.InstanceObject, this.m_TerrainData);
				}
				return this.m_TerrainData;
			}
			set
			{
				this.m_TerrainData = value;
				this.Internal_SetTerrainData(this.InstanceObject, value);
			}
		}
		public float treeDistance
		{
			get
			{
				if (this.m_TreeDistance != this.Internal_GetTreeDistance(this.InstanceObject))
				{
					this.Internal_SetTreeDistance(this.InstanceObject, this.m_TreeDistance);
				}
				return this.m_TreeDistance;
			}
			set
			{
				this.m_TreeDistance = value;
				this.Internal_SetTreeDistance(this.InstanceObject, value);
			}
		}
		public float treeBillboardDistance
		{
			get
			{
				if (this.m_TreeBillboardDistance != this.Internal_GetTreeBillboardDistance(this.InstanceObject))
				{
					this.Internal_SetTreeBillboardDistance(this.InstanceObject, this.m_TreeBillboardDistance);
				}
				return this.m_TreeBillboardDistance;
			}
			set
			{
				this.m_TreeBillboardDistance = value;
				this.Internal_SetTreeBillboardDistance(this.InstanceObject, value);
			}
		}
		public float treeCrossFadeLength
		{
			get
			{
				if (this.m_TreeCrossFadeLength != this.Internal_GetTreeCrossFadeLength(this.InstanceObject))
				{
					this.Internal_SetTreeCrossFadeLength(this.InstanceObject, this.m_TreeCrossFadeLength);
				}
				return this.m_TreeCrossFadeLength;
			}
			set
			{
				this.m_TreeCrossFadeLength = value;
				this.Internal_SetTreeCrossFadeLength(this.InstanceObject, value);
			}
		}
		public int treeMaximumFullLODCount
		{
			get
			{
				if (this.m_TreeMaximumFullLODCount != this.Internal_GetTreeMaximumFullLODCount(this.InstanceObject))
				{
					this.Internal_SetTreeMaximumFullLODCount(this.InstanceObject, this.m_TreeMaximumFullLODCount);
				}
				return this.m_TreeMaximumFullLODCount;
			}
			set
			{
				this.m_TreeMaximumFullLODCount = value;
				this.Internal_SetTreeMaximumFullLODCount(this.InstanceObject, value);
			}
		}
		public float detailObjectDistance
		{
			get
			{
				if (this.m_DetailObjectDistance != this.Internal_GetDetailObjectDistance(this.InstanceObject))
				{
					this.Internal_SetDetailObjectDistance(this.InstanceObject, this.m_DetailObjectDistance);
				}
				return this.m_DetailObjectDistance;
			}
			set
			{
				this.m_DetailObjectDistance = value;
				this.Internal_SetDetailObjectDistance(this.InstanceObject, value);
			}
		}
		public float detailObjectDensity
		{
			get
			{
				if (this.m_DetailObjectDensity != this.Internal_GetDetailObjectDensity(this.InstanceObject))
				{
					this.Internal_SetDetailObjectDensity(this.InstanceObject, this.m_DetailObjectDensity);
				}
				return this.m_DetailObjectDensity;
			}
			set
			{
				this.m_DetailObjectDensity = value;
				this.Internal_SetDetailObjectDensity(this.InstanceObject, value);
			}
		}
		public float heightmapPixelError
		{
			get
			{
				if (this.m_HeightmapPixelError != this.Internal_GetHeightmapPixelError(this.InstanceObject))
				{
					this.Internal_SetHeightmapPixelError(this.InstanceObject, this.m_HeightmapPixelError);
				}
				return this.m_HeightmapPixelError;
			}
			set
			{
				this.m_HeightmapPixelError = value;
				this.Internal_SetHeightmapPixelError(this.InstanceObject, value);
			}
		}
		public int heightmapMaximumLOD
		{
			get
			{
				if (this.m_HeightmapMaximumLOD != this.Internal_GetHeightmapMaximumLOD(this.InstanceObject))
				{
					this.Internal_SetHeightmapMaximumLOD(this.InstanceObject, this.m_HeightmapMaximumLOD);
				}
				return this.m_HeightmapMaximumLOD;
			}
			set
			{
				this.m_HeightmapMaximumLOD = value;
				this.Internal_SetHeightmapMaximumLOD(this.InstanceObject, value);
			}
		}
		public float basemapDistance
		{
			get
			{
				if (this.m_SplatMapDistance != this.Internal_GetBasemapDistance(this.InstanceObject))
				{
					this.Internal_SetBasemapDistance(this.InstanceObject, this.m_SplatMapDistance);
				}
				return this.m_SplatMapDistance;
			}
			set
			{
				this.m_SplatMapDistance = value;
				this.Internal_SetBasemapDistance(this.InstanceObject, value);
			}
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
		public int lightmapIndex
		{
			get
			{
				if (this.m_LightmapIndex != this.Internal_GetLightmapIndex(this.InstanceObject))
				{
					this.Internal_SetLightmapIndex(this.InstanceObject, this.m_LightmapIndex);
				}
				return this.m_LightmapIndex;
			}
			set
			{
				this.m_LightmapIndex = value;
				this.Internal_SetLightmapIndex(this.InstanceObject, value);
			}
		}
		internal int lightmapSize
		{
			get
			{
				if (this.m_LightmapSize != this.Internal_GetLightmapSize(this.InstanceObject))
				{
					this.Internal_SetLightmapSize(this.InstanceObject, this.m_LightmapSize);
				}
				return this.m_LightmapSize;
			}
			set
			{
				this.m_LightmapSize = value;
				this.Internal_SetLightmapSize(this.InstanceObject, value);
			}
		}
		public bool castShadows
		{
			get
			{
				if (this.m_CastShadows != this.Internal_GetCastShadows(this.InstanceObject))
				{
					this.Internal_SetCastShadows(this.InstanceObject, this.m_CastShadows);
				}
				return this.m_CastShadows;
			}
			set
			{
				this.m_CastShadows = value;
				this.Internal_SetCastShadows(this.InstanceObject, value);
			}
		}
		public Material materialTemplate
		{
			get
			{
				if (this.m_MaterialTemplate != this.Internal_GetMaterialTemplate(this.InstanceObject))
				{
					this.Internal_SetMaterialTemplate(this.InstanceObject, this.m_MaterialTemplate);
				}
				return this.m_MaterialTemplate;
			}
			set
			{
				this.m_MaterialTemplate = value;
				this.Internal_SetMaterialTemplate(this.InstanceObject, value);
			}
		}
		internal bool drawTreesAndFoliage
		{
			get
			{
				if (this.m_DrawTreesAndFoliage != this.Internal_GetDrawTreesAndFoliage(this.InstanceObject))
				{
					this.Internal_SetDrawTreesAndFoliage(this.InstanceObject, this.m_DrawTreesAndFoliage);
				}
				return this.m_DrawTreesAndFoliage;
			}
			set
			{
				this.m_DrawTreesAndFoliage = value;
				this.Internal_SetDrawTreesAndFoliage(this.InstanceObject, value);
			}
		}
		public bool collectDetailPatches
		{
			get
			{
				if (this.m_CollectDetailPatches != this.Internal_GetCollectDetailPatches(this.InstanceObject))
				{
					this.Internal_SetCollectDetailPatches(this.InstanceObject, this.m_CollectDetailPatches);
				}
				return this.m_CollectDetailPatches;
			}
			set
			{
				this.m_CollectDetailPatches = value;
				this.Internal_SetCollectDetailPatches(this.InstanceObject, value);
			}
		}
		public static extern Terrain activeTerrain
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern Terrain[] activeTerrains
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		private void OnDestroy()
		{
			this.OnDisable();
			this.Cleanup(this.m_TerrainInstance);
			this.m_TerrainInstance = IntPtr.Zero;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MakeSureObjectIsAlive();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Cleanup(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetEditorRenderFlags(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetEditorRenderFlags(IntPtr terrainInstance, int flags);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TerrainData Internal_GetTerrainData(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTerrainData(IntPtr terrainInstance, TerrainData value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetTreeDistance(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTreeDistance(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetTreeBillboardDistance(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTreeBillboardDistance(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetTreeCrossFadeLength(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTreeCrossFadeLength(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetTreeMaximumFullLODCount(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetTreeMaximumFullLODCount(IntPtr terrainInstance, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetDetailObjectDistance(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailObjectDistance(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetDetailObjectDensity(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDetailObjectDensity(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetHeightmapPixelError(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeightmapPixelError(IntPtr terrainInstance, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetHeightmapMaximumLOD(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetHeightmapMaximumLOD(IntPtr terrainInstance, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float Internal_GetBasemapDistance(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetBasemapDistance(IntPtr terrainInstance, float value);
		private void SetLightmapIndex(int value)
		{
			this.lightmapIndex = value;
		}
		private void ShiftLightmapIndex(int offset)
		{
			this.lightmapIndex += offset;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetLightmapIndex(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetLightmapIndex(IntPtr terrainInstance, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetLightmapSize(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetLightmapSize(IntPtr terrainInstance, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_GetCastShadows(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetCastShadows(IntPtr terrainInstance, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Material Internal_GetMaterialTemplate(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetMaterialTemplate(IntPtr terrainInstance, Material value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_GetDrawTreesAndFoliage(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetDrawTreesAndFoliage(IntPtr terrainInstance, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_GetCollectDetailPatches(IntPtr terrainInstance);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetCollectDetailPatches(IntPtr terrainInstance, bool value);
		public float SampleHeight(Vector3 worldPosition)
		{
			return this.Internal_SampleHeight(this.InstanceObject, worldPosition);
		}
		private float Internal_SampleHeight(IntPtr terrainInstance, Vector3 worldPosition)
		{
			return Terrain.INTERNAL_CALL_Internal_SampleHeight(this, terrainInstance, ref worldPosition);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_Internal_SampleHeight(Terrain self, IntPtr terrainInstance, ref Vector3 worldPosition);
		internal void ApplyDelayedHeightmapModification()
		{
			this.Internal_ApplyDelayedHeightmapModification(this.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_ApplyDelayedHeightmapModification(IntPtr terrainInstance);
		public void AddTreeInstance(TreeInstance instance)
		{
			this.Internal_AddTreeInstance(this.InstanceObject, instance);
		}
		private void Internal_AddTreeInstance(IntPtr terrainInstance, TreeInstance instance)
		{
			Terrain.INTERNAL_CALL_Internal_AddTreeInstance(this, terrainInstance, ref instance);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_AddTreeInstance(Terrain self, IntPtr terrainInstance, ref TreeInstance instance);
		public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom)
		{
			this.Internal_SetNeighbors(this.InstanceObject, (!(left != null)) ? IntPtr.Zero : left.InstanceObject, (!(top != null)) ? IntPtr.Zero : top.InstanceObject, (!(right != null)) ? IntPtr.Zero : right.InstanceObject, (!(bottom != null)) ? IntPtr.Zero : bottom.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetNeighbors(IntPtr terrainInstance, IntPtr left, IntPtr top, IntPtr right, IntPtr bottom);
		public Vector3 GetPosition()
		{
			return this.Internal_GetPosition(this.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector3 Internal_GetPosition(IntPtr terrainInstance);
		public void Flush()
		{
			this.Internal_Flush(this.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_Flush(IntPtr terrainInstance);
		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			this.Internal_RemoveTrees(this.InstanceObject, position, radius, prototypeIndex);
		}
		private void Internal_RemoveTrees(IntPtr terrainInstance, Vector2 position, float radius, int prototypeIndex)
		{
			Terrain.INTERNAL_CALL_Internal_RemoveTrees(this, terrainInstance, ref position, radius, prototypeIndex);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_RemoveTrees(Terrain self, IntPtr terrainInstance, ref Vector2 position, float radius, int prototypeIndex);
		private void OnTerrainChanged(TerrainChangedFlags flags)
		{
			this.Internal_OnTerrainChanged(this.InstanceObject, flags);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_OnTerrainChanged(IntPtr terrainInstance, TerrainChangedFlags flags);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Construct();
		internal void OnEnable()
		{
			this.Internal_OnEnable(this.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_OnEnable(IntPtr terrainInstance);
		internal void OnDisable()
		{
			this.Internal_OnDisable(this.InstanceObject);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_OnDisable(IntPtr terrainInstance);
		public static GameObject CreateTerrainGameObject(TerrainData assignTerrain)
		{
			GameObject gameObject = new GameObject("Terrain", new Type[]
			{
				typeof(Terrain),
				typeof(TerrainCollider)
			});
			gameObject.isStatic = true;
			Terrain terrain = gameObject.GetComponent(typeof(Terrain)) as Terrain;
			TerrainCollider terrainCollider = gameObject.GetComponent(typeof(TerrainCollider)) as TerrainCollider;
			terrainCollider.terrainData = assignTerrain;
			terrain.terrainData = assignTerrain;
			terrain.OnEnable();
			return gameObject;
		}
		private static void ReconnectTerrainData()
		{
			List<Terrain> list = new List<Terrain>(Terrain.activeTerrains);
			foreach (Terrain current in list)
			{
				if (current.terrainData == null)
				{
					current.OnDisable();
				}
				else
				{
					if (!current.terrainData.HasUser(current.gameObject))
					{
						current.OnDisable();
						current.OnEnable();
					}
				}
			}
		}
	}
}
