using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class UnhandledExceptionHandler
	{
		[CompilerGenerated]
		private static UnhandledExceptionEventHandler <>f__mg$cache0;

		[RequiredByNativeCode]
		private static void RegisterUECatcher()
		{
			AppDomain arg_23_0 = AppDomain.CurrentDomain;
			if (UnhandledExceptionHandler.<>f__mg$cache0 == null)
			{
				UnhandledExceptionHandler.<>f__mg$cache0 = new UnhandledExceptionEventHandler(UnhandledExceptionHandler.HandleUnhandledException);
			}
			arg_23_0.UnhandledException += UnhandledExceptionHandler.<>f__mg$cache0;
		}

		private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = args.ExceptionObject as Exception;
			if (ex != null)
			{
				UnhandledExceptionHandler.PrintException("Unhandled Exception: ", ex);
			}
			UnhandledExceptionHandler.NativeUnhandledExceptionHandler();
		}

		private static void PrintException(string title, Exception e)
		{
			Debug.LogException(e);
			if (e.InnerException != null)
			{
				UnhandledExceptionHandler.PrintException("Inner Exception: ", e.InnerException);
			}
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeUnhandledExceptionHandler();
	}
}
