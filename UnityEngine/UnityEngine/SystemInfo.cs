using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class SystemInfo
	{
		public const string unsupportedIdentifier = "n/a";

		public static extern string operatingSystem
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern OperatingSystemFamily operatingSystemFamily
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string processorType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int processorFrequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int processorCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int systemMemorySize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int graphicsMemorySize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string graphicsDeviceName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string graphicsDeviceVendor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int graphicsDeviceID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int graphicsDeviceVendorID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GraphicsDeviceType graphicsDeviceType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool usesOpenGLTextureCoords
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string graphicsDeviceVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int graphicsShaderLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
		public static int graphicsPixelFillrate
		{
			get
			{
				return -1;
			}
		}

		[Obsolete("Vertex program support is required in Unity 5.0+")]
		public static bool supportsVertexPrograms
		{
			get
			{
				return true;
			}
		}

		public static extern bool graphicsMultiThreaded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsShadows
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsRawShadowDepthSampling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("supportsRenderTextures always returns true, no need to call it")]
		public static extern bool supportsRenderTextures
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsMotionVectors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsRenderToCubemap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsImageEffects
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supports3DTextures
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supports2DArrayTextures
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsCubemapArrayTextures
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern CopyTextureSupport copyTextureSupport
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsComputeShaders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsInstancing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsSparseTextures
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int supportedRenderTargetCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool usesReversedZBuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("supportsStencil always returns true, no need to call it")]
		public static extern int supportsStencil
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern NPOTSupport npotSupport
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string deviceUniqueIdentifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string deviceName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string deviceModel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsAccelerometer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsGyroscope
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsLocationService
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsVibration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool supportsAudio
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern DeviceType deviceType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int maxTextureSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern int maxRenderTextureSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsRenderTextureFormat(RenderTextureFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsTextureFormat(TextureFormat format);
	}
}
