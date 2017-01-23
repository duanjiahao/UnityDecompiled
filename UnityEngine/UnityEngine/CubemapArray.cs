using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class CubemapArray : Texture
	{
		public extern int cubemapCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public CubemapArray(int faceSize, int cubemapCount, TextureFormat format, bool mipmap)
		{
			CubemapArray.Internal_Create(this, faceSize, cubemapCount, format, mipmap, false);
		}

		public CubemapArray(int faceSize, int cubemapCount, TextureFormat format, bool mipmap, bool linear)
		{
			CubemapArray.Internal_Create(this, faceSize, cubemapCount, format, mipmap, linear);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);

		[ExcludeFromDocs]
		public void Apply(bool updateMipmaps)
		{
			bool makeNoLongerReadable = false;
			this.Apply(updateMipmaps, makeNoLongerReadable);
		}

		[ExcludeFromDocs]
		public void Apply()
		{
			bool makeNoLongerReadable = false;
			bool updateMipmaps = true;
			this.Apply(updateMipmaps, makeNoLongerReadable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] CubemapArray mono, int faceSize, int cubemapCount, TextureFormat format, bool mipmap, bool linear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
		{
			int miplevel = 0;
			this.SetPixels(colors, face, arrayElement, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
		{
			int miplevel = 0;
			this.SetPixels32(colors, face, arrayElement, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(CubemapFace face, int arrayElement)
		{
			int miplevel = 0;
			return this.GetPixels(face, arrayElement, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32(CubemapFace face, int arrayElement)
		{
			int miplevel = 0;
			return this.GetPixels32(face, arrayElement, miplevel);
		}
	}
}
