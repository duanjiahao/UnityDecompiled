using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.CrashReportHandler
{
	public static class CrashReportHandler
	{
		[ThreadAndSerializationSafe]
		public static extern bool enableCaptureExceptions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
