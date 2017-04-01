using System;

namespace UnityEngine.Collections
{
	public static class NativeLeakDetection
	{
		private static int s_NativeLeakDetectionMode;

		public static NativeLeakDetectionMode Mode
		{
			get
			{
				return (NativeLeakDetectionMode)NativeLeakDetection.s_NativeLeakDetectionMode;
			}
			set
			{
				NativeLeakDetection.s_NativeLeakDetectionMode = (int)value;
			}
		}
	}
}
