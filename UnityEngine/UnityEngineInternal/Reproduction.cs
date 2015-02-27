using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEngineInternal
{
	public sealed class Reproduction
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshot();
	}
}
