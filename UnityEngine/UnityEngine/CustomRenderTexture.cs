using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class CustomRenderTexture : RenderTexture
	{
		public extern Material material
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Material initializationMaterial
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Texture initializationTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CustomRenderTextureInitializationSource initializationSource
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color initializationColor
		{
			get
			{
				Color result;
				this.INTERNAL_get_initializationColor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_initializationColor(ref value);
			}
		}

		public extern CustomRenderTextureUpdateMode updateMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CustomRenderTextureUpdateMode initializationMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern CustomRenderTextureUpdateZoneSpace updateZoneSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int shaderPass
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern uint cubemapFaceMask
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool doubleBuffered
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool wrapUpdateZones
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public CustomRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite) : base(width, height, 0, format, readWrite)
		{
		}

		public CustomRenderTexture(int width, int height, RenderTextureFormat format) : base(width, height, 0, format)
		{
		}

		public CustomRenderTexture(int width, int height) : base(width, height, 0)
		{
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update([DefaultValue("1")] int count);

		[ExcludeFromDocs]
		public void Update()
		{
			int count = 1;
			this.Update(count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Initialize();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearUpdateZones();

		private bool IsCubemapFaceEnabled(CubemapFace face)
		{
			return ((ulong)this.cubemapFaceMask & (ulong)(1L << (int)(face & (CubemapFace)31))) != 0uL;
		}

		private void EnableCubemapFace(CubemapFace face, bool value)
		{
			uint num = this.cubemapFaceMask;
			uint num2 = 1u << (int)face;
			if (value)
			{
				num |= num2;
			}
			else
			{
				num &= ~num2;
			}
			this.cubemapFaceMask = num;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetUpdateZonesInternal(object updateZones);

		public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
		{
			this.GetUpdateZonesInternal(updateZones);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones);

		public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
		{
			if (updateZones == null)
			{
				throw new ArgumentNullException("updateZones");
			}
			this.SetUpdateZonesInternal(updateZones);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_initializationColor(out Color value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_initializationColor(ref Color value);
	}
}
