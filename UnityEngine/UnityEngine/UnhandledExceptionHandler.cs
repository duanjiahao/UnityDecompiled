using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	internal sealed class UnhandledExceptionHandler
	{
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NativeUnhandledExceptionHandler();
	}
}
