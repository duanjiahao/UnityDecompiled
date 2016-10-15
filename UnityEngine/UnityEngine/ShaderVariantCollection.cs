using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class ShaderVariantCollection : Object
	{
		public struct ShaderVariant
		{
			public Shader shader;

			public PassType passType;

			public string[] keywords;

			public ShaderVariant(Shader shader, PassType passType, params string[] keywords)
			{
				this.shader = shader;
				this.passType = passType;
				this.keywords = keywords;
				ShaderVariantCollection.ShaderVariant.Internal_CheckVariant(shader, passType, keywords);
			}

			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void Internal_CheckVariant(Shader shader, PassType passType, string[] keywords);
		}

		public extern int shaderCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int variantCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isWarmedUp
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public ShaderVariantCollection()
		{
			ShaderVariantCollection.Internal_Create(this);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ShaderVariantCollection mono);

		public bool Add(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.AddInternal(variant.shader, variant.passType, variant.keywords);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool AddInternal(Shader shader, PassType passType, string[] keywords);

		public bool Remove(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.RemoveInternal(variant.shader, variant.passType, variant.keywords);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool RemoveInternal(Shader shader, PassType passType, string[] keywords);

		public bool Contains(ShaderVariantCollection.ShaderVariant variant)
		{
			return this.ContainsInternal(variant.shader, variant.passType, variant.keywords);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ContainsInternal(Shader shader, PassType passType, string[] keywords);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WarmUp();
	}
}
