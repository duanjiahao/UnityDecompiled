using System;
using System.Runtime.CompilerServices;

namespace UnityEngineInternal
{
	public sealed class GIDebugVisualisation
	{
		public static extern bool cycleMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool pauseCycleMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern GITextureType texType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetRuntimeInputTextures();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PlayCycleMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PauseCycleMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopCycleMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CycleSkipInstances(int skip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CycleSkipSystems(int skip);
	}
}
