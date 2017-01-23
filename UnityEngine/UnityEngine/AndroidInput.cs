using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AndroidInput
	{
		public static extern int touchCountSecondary
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool secondaryTouchEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int secondaryTouchWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int secondaryTouchHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private AndroidInput()
		{
		}

		public static Touch GetSecondaryTouch(int index)
		{
			Touch result;
			AndroidInput.INTERNAL_CALL_GetSecondaryTouch(index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSecondaryTouch(int index, out Touch value);
	}
}
