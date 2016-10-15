using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.WSA
{
	public sealed class Application
	{
		public static event WindowSizeChanged windowSizeChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Application.windowSizeChanged = (WindowSizeChanged)Delegate.Combine(Application.windowSizeChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Application.windowSizeChanged = (WindowSizeChanged)Delegate.Remove(Application.windowSizeChanged, value);
			}
		}

		public static event WindowActivated windowActivated
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				Application.windowActivated = (WindowActivated)Delegate.Combine(Application.windowActivated, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				Application.windowActivated = (WindowActivated)Delegate.Remove(Application.windowActivated, value);
			}
		}

		public static string arguments
		{
			get
			{
				return Application.GetAppArguments();
			}
		}

		public static string advertisingIdentifier
		{
			get
			{
				string advertisingIdentifier = Application.GetAdvertisingIdentifier();
				UnityEngine.Application.InvokeOnAdvertisingIdentifierCallback(advertisingIdentifier, true);
				return advertisingIdentifier;
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAdvertisingIdentifier();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAppArguments();

		internal static void InvokeWindowSizeChangedEvent(int width, int height)
		{
			if (Application.windowSizeChanged != null)
			{
				Application.windowSizeChanged(width, height);
			}
		}

		internal static void InvokeWindowActivatedEvent(WindowActivationState state)
		{
			if (Application.windowActivated != null)
			{
				Application.windowActivated(state);
			}
		}

		public static void InvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
		{
			item();
		}

		public static void InvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
		{
			item();
		}

		[Obsolete("TryInvokeOnAppThread is deprecated, use InvokeOnAppThread")]
		public static bool TryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone)
		{
			item();
			return true;
		}

		[Obsolete("TryInvokeOnUIThread is deprecated, use InvokeOnUIThread")]
		public static bool TryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone)
		{
			item();
			return true;
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalTryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalTryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RunningOnAppThread();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RunningOnUIThread();
	}
}
