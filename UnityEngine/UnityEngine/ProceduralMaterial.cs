using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ProceduralMaterial : Material
	{
		public extern ProceduralCacheSize cacheSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int animationUpdateRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isProcessing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isCachedDataAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isLoadTimeGenerated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ProceduralLoadingBehavior loadingBehavior
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ProceduralProcessorUsage substanceProcessorUsage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string preset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isFrozen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal ProceduralMaterial() : base(null)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralPropertyDescription[] GetProceduralPropertyDescriptions();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasProceduralProperty(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetProceduralBoolean(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsProceduralPropertyVisible(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetProceduralBoolean(string inputName, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetProceduralFloat(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetProceduralFloat(string inputName, float value);

		public Vector4 GetProceduralVector(string inputName)
		{
			Vector4 result;
			ProceduralMaterial.INTERNAL_CALL_GetProceduralVector(this, inputName, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetProceduralVector(ProceduralMaterial self, string inputName, out Vector4 value);

		public void SetProceduralVector(string inputName, Vector4 value)
		{
			ProceduralMaterial.INTERNAL_CALL_SetProceduralVector(this, inputName, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetProceduralVector(ProceduralMaterial self, string inputName, ref Vector4 value);

		public Color GetProceduralColor(string inputName)
		{
			Color result;
			ProceduralMaterial.INTERNAL_CALL_GetProceduralColor(this, inputName, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetProceduralColor(ProceduralMaterial self, string inputName, out Color value);

		public void SetProceduralColor(string inputName, Color value)
		{
			ProceduralMaterial.INTERNAL_CALL_SetProceduralColor(this, inputName, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetProceduralColor(ProceduralMaterial self, string inputName, ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetProceduralEnum(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetProceduralEnum(string inputName, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture2D GetProceduralTexture(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetProceduralTexture(string inputName, Texture2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsProceduralPropertyCached(string inputName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CacheProceduralProperty(string inputName, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCache();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RebuildTextures();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RebuildTexturesImmediately();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopRebuilds();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture[] GetGeneratedTextures();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralTexture GetGeneratedTexture(string textureName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void FreezeAndReleaseSourceData();
	}
}
