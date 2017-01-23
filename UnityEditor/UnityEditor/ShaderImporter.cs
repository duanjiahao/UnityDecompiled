using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class ShaderImporter : AssetImporter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Shader GetShader();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDefaultTextures(string[] name, Texture[] textures);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture GetDefaultTexture(string name);
	}
}
