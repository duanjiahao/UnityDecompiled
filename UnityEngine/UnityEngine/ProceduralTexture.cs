using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ProceduralTexture : Texture
	{
		public extern bool hasAlpha
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralOutputType GetProceduralOutputType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ProceduralMaterial GetProceduralMaterial();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasBeenGenerated();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight);
	}
}
