using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public sealed class SplashScreen
	{
		public static extern bool isFinished
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Begin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Draw();
	}
}
