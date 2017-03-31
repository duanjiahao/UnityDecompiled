using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor.Sprites
{
	public sealed class PackerJob
	{
		internal PackerJob()
		{
		}

		public void AddAtlas(string atlasName, AtlasSettings settings)
		{
			this.AddAtlas_Internal(atlasName, ref settings);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddAtlas_Internal(string atlasName, ref AtlasSettings settings);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AssignToAtlas(string atlasName, Sprite sprite, SpritePackingMode packingMode, SpritePackingRotation packingRotation);
	}
}
