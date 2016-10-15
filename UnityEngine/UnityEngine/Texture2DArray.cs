using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Texture2DArray : Texture
	{
		public extern int depth
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

		public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap)
		{
			Texture2DArray.Internal_Create(this, width, height, depth, format, mipmap, false);
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap, bool linear)
		{
			Texture2DArray.Internal_Create(this, width, height, depth, format, mipmap, linear);
		}

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Texture2DArray mono, int width, int height, int depth, TextureFormat format, bool mipmap, bool linear);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, int arrayElement)
		{
			int miplevel = 0;
			this.SetPixels(colors, arrayElement, miplevel);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors, int arrayElement)
		{
			int miplevel = 0;
			this.SetPixels32(colors, arrayElement, miplevel);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(int arrayElement)
		{
			int miplevel = 0;
			return this.GetPixels(arrayElement, miplevel);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32(int arrayElement)
		{
			int miplevel = 0;
			return this.GetPixels32(arrayElement, miplevel);
		}
	}
}
