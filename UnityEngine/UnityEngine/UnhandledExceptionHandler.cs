using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class UnhandledExceptionHandler
	{
		[RequiredByNativeCode]
		private static void RegisterUECatcher()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler.HandleUnhandledException);
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
			Debug.LogError(title + e.ToString());
			if (e.InnerException != null)
			{
				UnhandledExceptionHandler.PrintException("Inner Exception: ", e.InnerException);
			}
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeUnhandledExceptionHandler();
	}
}
