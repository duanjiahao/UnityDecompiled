using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	[UsedByNativeCode]
	public sealed class CustomSampler : Sampler
	{
		internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();

		internal CustomSampler()
		{
		}

		public static CustomSampler Create(string name)
		{
			CustomSampler customSampler = CustomSampler.CreateInternal(name);
			return customSampler ?? CustomSampler.s_InvalidCustomSampler;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CustomSampler CreateInternal(string name);

		[Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Begin();

		[Conditional("ENABLE_PROFILER")]
		public void Begin(UnityEngine.Object targetObject)
		{
			this.BeginWithObject(targetObject);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void BeginWithObject(UnityEngine.Object targetObject);

		[Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void End();
	}
}
