using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Time
	{
		public static extern float time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float timeSinceLevelLoad
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float deltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float fixedTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float unscaledTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float unscaledDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float fixedDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maximumDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float smoothDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float maximumParticleDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float timeScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int frameCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int renderedFrameCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float realtimeSinceStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int captureFramerate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
