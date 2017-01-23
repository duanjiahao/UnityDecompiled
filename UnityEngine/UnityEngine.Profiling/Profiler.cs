using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Profiling
{
	[MovedFrom("UnityEngine")]
	public sealed class Profiler
	{
		public static extern bool supported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string logFile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enableBinaryLog
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("maxNumberOfSamplesPerFrame is no longer needed, as the profiler buffers auto expand as needed")]
		public static extern int maxNumberOfSamplesPerFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern uint usedHeapSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Conditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddFramesFromFile(string file);

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name)
		{
			Profiler.BeginSampleOnly(name);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSample(string name, UnityEngine.Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSampleOnly(string name);

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSample();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRuntimeMemorySize(UnityEngine.Object o);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetMonoHeapSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetMonoUsedSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SetTempAllocatorRequestedSize(uint size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTempAllocatorSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTotalAllocatedMemory();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTotalUnusedReservedMemory();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetTotalReservedMemory();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Sampler GetSampler(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetSamplerNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetSamplersEnabled(bool enabled);
	}
}
