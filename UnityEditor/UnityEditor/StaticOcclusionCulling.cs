using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class StaticOcclusionCulling
	{
		public static extern bool isRunning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float smallestOccluder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float smallestHole
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float backfaceThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool doesSceneHaveManualPortals
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int umbraDataSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Compute();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GenerateInBackground();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InvalidatePrevisualisationData();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Cancel();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Clear();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDefaultOcclusionBakeSettings();
	}
}
