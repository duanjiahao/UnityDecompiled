using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class UnityStats
	{
		public static extern int batches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int drawCalls
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int dynamicBatchedDrawCalls
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int staticBatchedDrawCalls
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int instancedBatchedDrawCalls
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int dynamicBatches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int staticBatches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int instancedBatches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int setPassCalls
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int triangles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int vertices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int shadowCasters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int renderTextureChanges
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float frameTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float renderTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float audioLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float audioClippingAmount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float audioDSPLoad
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float audioStreamLoad
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int renderTextureCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int renderTextureBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int usedTextureMemorySize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int usedTextureCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string screenRes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int screenBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int vboTotal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int vboTotalBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int vboUploads
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int vboUploadBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int ibUploads
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int ibUploadBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int visibleSkinnedMeshes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int visibleAnimations
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNetworkStats(int i);
	}
}
