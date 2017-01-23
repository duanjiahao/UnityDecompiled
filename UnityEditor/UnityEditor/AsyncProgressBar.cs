using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	internal sealed class AsyncProgressBar
	{
		public static extern float progress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string progressInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isShowing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Display(string progressInfo, float progress);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Clear();
	}
}
