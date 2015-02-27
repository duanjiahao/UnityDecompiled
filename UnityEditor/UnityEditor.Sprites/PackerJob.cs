using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor.Sprites
{
	public sealed class PackerJob
	{
		internal PackerJob()
		{
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddAtlas(string atlasName, AtlasSettings settings);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AssignToAtlas(string atlasName, Sprite sprite, SpritePackingMode packingMode, SpritePackingRotation packingRotation);
	}
}
