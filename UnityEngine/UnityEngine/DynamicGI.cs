using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class DynamicGI
	{
		public static extern float indirectScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float updateThreshold
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool synchronousMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static void SetEmissive(Renderer renderer, Color color)
		{
			DynamicGI.INTERNAL_CALL_SetEmissive(renderer, ref color);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetEmissive(Renderer renderer, ref Color color);

		public static void UpdateMaterials(Renderer renderer)
		{
			DynamicGI.UpdateMaterialsForRenderer(renderer);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateMaterialsForRenderer(Renderer renderer);

		public static void UpdateMaterials(Terrain terrain)
		{
			if (terrain == null)
			{
				throw new ArgumentNullException("terrain");
			}
			if (terrain.terrainData == null)
			{
				throw new ArgumentException("Invalid terrainData.");
			}
			DynamicGI.UpdateMaterialsForTerrain(terrain, new Rect(0f, 0f, 1f, 1f));
		}

		public static void UpdateMaterials(Terrain terrain, int x, int y, int width, int height)
		{
			if (terrain == null)
			{
				throw new ArgumentNullException("terrain");
			}
			if (terrain.terrainData == null)
			{
				throw new ArgumentException("Invalid terrainData.");
			}
			float num = (float)terrain.terrainData.alphamapWidth;
			float num2 = (float)terrain.terrainData.alphamapHeight;
			DynamicGI.UpdateMaterialsForTerrain(terrain, new Rect((float)x / num, (float)y / num2, (float)width / num, (float)height / num2));
		}

		internal static void UpdateMaterialsForTerrain(Terrain terrain, Rect uvBounds)
		{
			DynamicGI.INTERNAL_CALL_UpdateMaterialsForTerrain(terrain, ref uvBounds);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_UpdateMaterialsForTerrain(Terrain terrain, ref Rect uvBounds);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateEnvironment();
	}
}
