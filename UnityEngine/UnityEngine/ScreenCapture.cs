using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public static class ScreenCapture
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);

		[ExcludeFromDocs]
		public static void CaptureScreenshot(string filename)
		{
			int superSize = 0;
			ScreenCapture.CaptureScreenshot(filename, superSize);
		}
	}
}
