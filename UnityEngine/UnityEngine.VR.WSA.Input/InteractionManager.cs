using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace UnityEngine.VR.WSA.Input
{
	public sealed class InteractionManager
	{
		public delegate void SourceEventHandler(InteractionSourceState state);

		private enum EventType
		{
			SourceDetected,
			SourceLost,
			SourceUpdated,
			SourcePressed,
			SourceReleased
		}

		private delegate void InternalSourceEventHandler(InteractionManager.EventType eventType, InteractionSourceState state);

		private static InteractionManager.InternalSourceEventHandler m_OnSourceEventHandler;

		[CompilerGenerated]
		private static InteractionManager.InternalSourceEventHandler <>f__mg$cache0;

		public static event InteractionManager.SourceEventHandler SourceDetected
		{
			add
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceDetected;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceDetected, (InteractionManager.SourceEventHandler)Delegate.Combine(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
			remove
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceDetected;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceDetected, (InteractionManager.SourceEventHandler)Delegate.Remove(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
		}

		public static event InteractionManager.SourceEventHandler SourceLost
		{
			add
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceLost;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceLost, (InteractionManager.SourceEventHandler)Delegate.Combine(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
			remove
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceLost;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceLost, (InteractionManager.SourceEventHandler)Delegate.Remove(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
		}

		public static event InteractionManager.SourceEventHandler SourcePressed
		{
			add
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourcePressed;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourcePressed, (InteractionManager.SourceEventHandler)Delegate.Combine(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
			remove
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourcePressed;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourcePressed, (InteractionManager.SourceEventHandler)Delegate.Remove(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
		}

		public static event InteractionManager.SourceEventHandler SourceReleased
		{
			add
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceReleased;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceReleased, (InteractionManager.SourceEventHandler)Delegate.Combine(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
			remove
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceReleased;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceReleased, (InteractionManager.SourceEventHandler)Delegate.Remove(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
		}

		public static event InteractionManager.SourceEventHandler SourceUpdated
		{
			add
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceUpdated;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceUpdated, (InteractionManager.SourceEventHandler)Delegate.Combine(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
			remove
			{
				InteractionManager.SourceEventHandler sourceEventHandler = InteractionManager.SourceUpdated;
				InteractionManager.SourceEventHandler sourceEventHandler2;
				do
				{
					sourceEventHandler2 = sourceEventHandler;
					sourceEventHandler = Interlocked.CompareExchange<InteractionManager.SourceEventHandler>(ref InteractionManager.SourceUpdated, (InteractionManager.SourceEventHandler)Delegate.Remove(sourceEventHandler2, value), sourceEventHandler);
				}
				while (sourceEventHandler != sourceEventHandler2);
			}
		}

		public static extern int numSourceStates
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		static InteractionManager()
		{
			if (InteractionManager.<>f__mg$cache0 == null)
			{
				InteractionManager.<>f__mg$cache0 = new InteractionManager.InternalSourceEventHandler(InteractionManager.OnSourceEvent);
			}
			InteractionManager.m_OnSourceEventHandler = InteractionManager.<>f__mg$cache0;
			InteractionManager.Initialize(Marshal.GetFunctionPointerForDelegate(InteractionManager.m_OnSourceEventHandler));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCurrentReading_Internal(InteractionSourceState[] sourceStates);

		public static int GetCurrentReading(InteractionSourceState[] sourceStates)
		{
			if (sourceStates == null)
			{
				throw new ArgumentNullException("sourceStates");
			}
			int result;
			if (sourceStates.Length > 0)
			{
				result = InteractionManager.GetCurrentReading_Internal(sourceStates);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static InteractionSourceState[] GetCurrentReading()
		{
			InteractionSourceState[] array = new InteractionSourceState[InteractionManager.numSourceStates];
			if (array.Length > 0)
			{
				InteractionManager.GetCurrentReading_Internal(array);
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Initialize(IntPtr internalSourceEventHandler);

		private static void OnSourceEvent(InteractionManager.EventType eventType, InteractionSourceState state)
		{
			switch (eventType)
			{
			case InteractionManager.EventType.SourceDetected:
			{
				InteractionManager.SourceEventHandler sourceDetected = InteractionManager.SourceDetected;
				if (sourceDetected != null)
				{
					sourceDetected(state);
				}
				break;
			}
			case InteractionManager.EventType.SourceLost:
			{
				InteractionManager.SourceEventHandler sourceLost = InteractionManager.SourceLost;
				if (sourceLost != null)
				{
					sourceLost(state);
				}
				break;
			}
			case InteractionManager.EventType.SourceUpdated:
			{
				InteractionManager.SourceEventHandler sourceUpdated = InteractionManager.SourceUpdated;
				if (sourceUpdated != null)
				{
					sourceUpdated(state);
				}
				break;
			}
			case InteractionManager.EventType.SourcePressed:
			{
				InteractionManager.SourceEventHandler sourcePressed = InteractionManager.SourcePressed;
				if (sourcePressed != null)
				{
					sourcePressed(state);
				}
				break;
			}
			case InteractionManager.EventType.SourceReleased:
			{
				InteractionManager.SourceEventHandler sourceReleased = InteractionManager.SourceReleased;
				if (sourceReleased != null)
				{
					sourceReleased(state);
				}
				break;
			}
			default:
				throw new ArgumentException("OnSourceEvent: Invalid EventType");
			}
		}
	}
}
