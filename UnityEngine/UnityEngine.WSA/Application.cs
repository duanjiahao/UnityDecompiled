using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnityEngine.WSA
{
	public sealed class Application
	{
		public static event WindowSizeChanged windowSizeChanged
		{
			add
			{
				WindowSizeChanged windowSizeChanged = Application.windowSizeChanged;
				WindowSizeChanged windowSizeChanged2;
				do
				{
					windowSizeChanged2 = windowSizeChanged;
					windowSizeChanged = Interlocked.CompareExchange<WindowSizeChanged>(ref Application.windowSizeChanged, (WindowSizeChanged)Delegate.Combine(windowSizeChanged2, value), windowSizeChanged);
				}
				while (windowSizeChanged != windowSizeChanged2);
			}
			remove
			{
				WindowSizeChanged windowSizeChanged = Application.windowSizeChanged;
				WindowSizeChanged windowSizeChanged2;
				do
				{
					windowSizeChanged2 = windowSizeChanged;
					windowSizeChanged = Interlocked.CompareExchange<WindowSizeChanged>(ref Application.windowSizeChanged, (WindowSizeChanged)Delegate.Remove(windowSizeChanged2, value), windowSizeChanged);
				}
				while (windowSizeChanged != windowSizeChanged2);
			}
		}

		public static event WindowActivated windowActivated
		{
			add
			{
				WindowActivated windowActivated = Application.windowActivated;
				WindowActivated windowActivated2;
				do
				{
					windowActivated2 = windowActivated;
					windowActivated = Interlocked.CompareExchange<WindowActivated>(ref Application.windowActivated, (WindowActivated)Delegate.Combine(windowActivated2, value), windowActivated);
				}
				while (windowActivated != windowActivated2);
			}
			remove
			{
				WindowActivated windowActivated = Application.windowActivated;
				WindowActivated windowActivated2;
				do
				{
					windowActivated2 = windowActivated;
					windowActivated = Interlocked.CompareExchange<WindowActivated>(ref Application.windowActivated, (WindowActivated)Delegate.Remove(windowActivated2, value), windowActivated);
				}
				while (windowActivated != windowActivated2);
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetAdvertisingIdentifier();

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

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalTryInvokeOnAppThread(AppCallbackItem item, bool waitUntilDone);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalTryInvokeOnUIThread(AppCallbackItem item, bool waitUntilDone);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RunningOnAppThread();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RunningOnUIThread();
	}
}
