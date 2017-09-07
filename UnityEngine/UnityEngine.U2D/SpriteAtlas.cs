using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.U2D
{
	public sealed class SpriteAtlas : UnityEngine.Object
	{
		public extern bool isVariant
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string tag
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int spriteCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal SpriteAtlas()
		{
			SpriteAtlas.Internal_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SpriteAtlas mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetSprites(Sprite[] sprites);

		public int GetSprites(Sprite[] sprites, string name)
		{
			return this.GetSpritesByName(sprites, name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetSpritesByName(Sprite[] sprites, string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Sprite GetSprite(string name);
	}
}
