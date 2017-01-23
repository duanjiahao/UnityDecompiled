using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR.WSA.WebCam
{
	public static class WebCam
	{
		public static WebCamMode Mode
		{
			get
			{
				return (WebCamMode)WebCam.GetWebCamModeState_Internal();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetWebCamModeState_Internal();
	}
}
