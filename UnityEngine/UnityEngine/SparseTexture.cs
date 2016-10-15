using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SparseTexture : Texture
	{
		public extern int tileWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int tileHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isCreated
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount)
		{
			SparseTexture.Internal_Create(this, width, height, format, mipCount, false);
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount, bool linear)
		{
			SparseTexture.Internal_Create(this, width, height, format, mipCount, linear);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, int mipCount, bool linear);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnloadTile(int tileX, int tileY, int miplevel);
	}
}
