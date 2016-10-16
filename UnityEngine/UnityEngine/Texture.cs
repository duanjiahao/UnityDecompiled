using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public class Texture : Object
	{
		public static extern int masterTextureLimit
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern AnisotropicFiltering anisotropicFiltering
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public virtual int width
		{
			get
			{
				return Texture.Internal_GetWidth(this);
			}
			set
			{
				throw new Exception("not implemented");
			}
		}

		public virtual int height
		{
			get
			{
				return Texture.Internal_GetHeight(this);
			}
			set
			{
				throw new Exception("not implemented");
			}
		}

		public virtual TextureDimension dimension
		{
			get
			{
				return Texture.Internal_GetDimension(this);
			}
			set
			{
				throw new Exception("not implemented");
			}
		}

		public extern FilterMode filterMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int anisoLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mipMapBias
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 texelSize
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_texelSize(out result);
				return result;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetWidth(Texture t);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetHeight(Texture t);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension Internal_GetDimension(Texture t);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_texelSize(out Vector2 value);

		public IntPtr GetNativeTexturePtr()
		{
			IntPtr result;
			Texture.INTERNAL_CALL_GetNativeTexturePtr(this, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeTexturePtr(Texture self, out IntPtr value);

		[Obsolete("Use GetNativeTexturePtr instead."), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetNativeTextureID();
	}
}
