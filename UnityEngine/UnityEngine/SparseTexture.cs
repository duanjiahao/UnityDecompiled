using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SparseTexture : Texture
	{
		public extern int tileWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int tileHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isCreated
		{
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, int mipCount, bool linear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnloadTile(int tileX, int tileY, int miplevel);
	}
}
