using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AndroidInput
	{
		public static extern int touchCountSecondary
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool secondaryTouchEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int secondaryTouchWidth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int secondaryTouchHeight
		{
			[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSecondaryTouch(int index, out Touch value);
	}
}
