using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public class RenderTexture : Texture
	{
		public override int width
		{
			get
			{
				return RenderTexture.Internal_GetWidth(this);
			}
			set
			{
				RenderTexture.Internal_SetWidth(this, value);
			}
		}

		public override int height
		{
			get
			{
				return RenderTexture.Internal_GetHeight(this);
			}
			set
			{
				RenderTexture.Internal_SetHeight(this, value);
			}
		}

		public extern int depth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isPowerOfTwo
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool sRGB
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern RenderTextureFormat format
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useMipMap
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoGenerateMips
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public override TextureDimension dimension
		{
			get
			{
				return RenderTexture.Internal_GetDimension(this);
			}
			set
			{
				RenderTexture.Internal_SetDimension(this, value);
			}
		}

		[Obsolete("Use RenderTexture.dimension instead.")]
		public extern bool isCubemap
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use RenderTexture.dimension instead.")]
		public extern bool isVolume
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int volumeDepth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern RenderTextureMemoryless memorylessMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int antiAliasing
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableRandomWrite
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public RenderBuffer colorBuffer
		{
			get
			{
				RenderBuffer result;
				this.GetColorBuffer(out result);
				return result;
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				RenderBuffer result;
				this.GetDepthBuffer(out result);
				return result;
			}
		}

		public static extern RenderTexture active
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("RenderTexture.enabled is always now, no need to use it")]
		public static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public RenderTextureDescriptor descriptor
		{
			get
			{
				return this.GetDescriptor();
			}
			set
			{
				RenderTexture.ValidateRenderTextureDesc(value);
				this.SetRenderTextureDescriptor(value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use RenderTexture.autoGenerateMips instead (UnityUpgradable) -> autoGenerateMips", false)]
		public bool generateMips
		{
			get
			{
				return this.autoGenerateMips;
			}
			set
			{
				this.autoGenerateMips = value;
			}
		}

		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			bool sRGB = readWrite == RenderTextureReadWrite.sRGB;
			if (readWrite == RenderTextureReadWrite.Default)
			{
				sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			}
			RenderTexture.Internal_SetSRGBReadWrite(this, sRGB);
		}

		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			RenderTexture.Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		public RenderTexture(int width, int height, int depth)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = RenderTextureFormat.Default;
			RenderTexture.Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		public RenderTexture(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			RenderTexture.Internal_CreateRenderTexture(this);
			this.SetRenderTextureDescriptor(desc);
		}

		public RenderTexture(RenderTexture textureToCopy)
		{
			if (textureToCopy == null)
			{
				throw new ArgumentNullException("textureToCopy");
			}
			RenderTexture.ValidateRenderTextureDesc(textureToCopy.descriptor);
			RenderTexture.Internal_CreateRenderTexture(this);
			this.SetRenderTextureDescriptor(textureToCopy.descriptor);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateRenderTexture([Writable] RenderTexture rt);

		private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
		{
			RenderTexture.INTERNAL_CALL_SetRenderTextureDescriptor(this, ref desc);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRenderTextureDescriptor(RenderTexture self, ref RenderTextureDescriptor desc);

		private RenderTextureDescriptor GetDescriptor()
		{
			RenderTextureDescriptor result;
			RenderTexture.INTERNAL_CALL_GetDescriptor(this, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetDescriptor(RenderTexture self, out RenderTextureDescriptor value);

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None;
			int antiAliasing = 1;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None;
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None;
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat format = RenderTextureFormat.Default;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None;
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat format = RenderTextureFormat.Default;
			int depthBuffer = 0;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode);
		}

		public static RenderTexture GetTemporary(int width, int height, [UnityEngine.Internal.DefaultValue("0")] int depthBuffer, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height)
			{
				depthBufferBits = depthBuffer,
				vrUsage = VRTextureUsage.None,
				colorFormat = format,
				sRGB = (readWrite != RenderTextureReadWrite.Linear),
				msaaSamples = antiAliasing,
				memoryless = memorylessMode
			});
		}

		private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
		{
			return RenderTexture.INTERNAL_CALL_GetTemporary_Internal(ref desc);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTexture INTERNAL_CALL_GetTemporary_Internal(ref RenderTextureDescriptor desc);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleaseTemporary(RenderTexture temp);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetWidth(RenderTexture mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetWidth(RenderTexture mono, int width);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetHeight(RenderTexture mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHeight(RenderTexture mono, int width);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension Internal_GetDimension(RenderTexture rt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetDimension(RenderTexture rt, TextureDimension dim);

		public bool Create()
		{
			return RenderTexture.INTERNAL_CALL_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Create(RenderTexture self);

		public void Release()
		{
			RenderTexture.INTERNAL_CALL_Release(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Release(RenderTexture self);

		public bool IsCreated()
		{
			return RenderTexture.INTERNAL_CALL_IsCreated(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsCreated(RenderTexture self);

		public void DiscardContents()
		{
			RenderTexture.INTERNAL_CALL_DiscardContents(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DiscardContents(bool discardColor, bool discardDepth);

		public void MarkRestoreExpected()
		{
			RenderTexture.INTERNAL_CALL_MarkRestoreExpected(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);

		public void GenerateMips()
		{
			RenderTexture.INTERNAL_CALL_GenerateMips(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GenerateMips(RenderTexture self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColorBuffer(out RenderBuffer res);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetDepthBuffer(out RenderBuffer res);

		public IntPtr GetNativeDepthBufferPtr()
		{
			IntPtr result;
			RenderTexture.INTERNAL_CALL_GetNativeDepthBufferPtr(this, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeDepthBufferPtr(RenderTexture self, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalShaderProperty(string propertyName);

		[Obsolete("GetTexelOffset always returns zero now, no point in using it.")]
		public Vector2 GetTexelOffset()
		{
			return Vector2.zero;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsStencil(RenderTexture rt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VRTextureUsage GetActiveVRUsage();

		[Obsolete("SetBorderColor is no longer supported.", true)]
		public void SetBorderColor(Color color)
		{
		}

		public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			desc.createdFromScript = true;
			return RenderTexture.GetTemporary_Internal(desc);
		}

		private static void ValidateRenderTextureDesc(RenderTextureDescriptor desc)
		{
			if (desc.width <= 0)
			{
				throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
			}
			if (desc.height <= 0)
			{
				throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
			}
			if (desc.volumeDepth <= 0)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
			}
			if (desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8)
			{
				throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
			}
			if (desc.depthBufferBits != 0 && desc.depthBufferBits != 16 && desc.depthBufferBits != 24)
			{
				throw new ArgumentException("RenderTextureDesc depthBufferBits must be 0, 16, or 24.", "desc.depthBufferBits");
			}
		}
	}
}
