using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ProceduralTexture : Texture
	{
		public extern bool hasAlpha
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TextureFormat format
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralOutputType GetProceduralOutputType();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ProceduralMaterial GetProceduralMaterial();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasBeenGenerated();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight);
	}
}
