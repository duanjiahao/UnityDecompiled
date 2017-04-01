using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Input
{
	public sealed class GestureRecognizer : IDisposable
	{
		public delegate void HoldCanceledEventDelegate(InteractionSourceKind source, Ray headRay);

		public delegate void HoldCompletedEventDelegate(InteractionSourceKind source, Ray headRay);

		public delegate void HoldStartedEventDelegate(InteractionSourceKind source, Ray headRay);

		public delegate void TappedEventDelegate(InteractionSourceKind source, int tapCount, Ray headRay);

		public delegate void ManipulationCanceledEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

		public delegate void ManipulationCompletedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

		public delegate void ManipulationStartedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

		public delegate void ManipulationUpdatedEventDelegate(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay);

		public delegate void NavigationCanceledEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

		public delegate void NavigationCompletedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

		public delegate void NavigationStartedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

		public delegate void NavigationUpdatedEventDelegate(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay);

		public delegate void RecognitionEndedEventDelegate(InteractionSourceKind source, Ray headRay);

		public delegate void RecognitionStartedEventDelegate(InteractionSourceKind source, Ray headRay);

		public delegate void GestureErrorDelegate([MarshalAs(UnmanagedType.LPStr)] string error, int hresult);

		private enum GestureEventType
		{
			InteractionDetected,
			HoldCanceled,
			HoldCompleted,
			HoldStarted,
			TapDetected,
			ManipulationCanceled,
			ManipulationCompleted,
			ManipulationStarted,
			ManipulationUpdated,
			NavigationCanceled,
			NavigationCompleted,
			NavigationStarted,
			NavigationUpdated,
			RecognitionStarted,
			RecognitionEnded
		}

		private IntPtr m_Recognizer;

		public event GestureRecognizer.HoldCanceledEventDelegate HoldCanceledEvent
		{
			add
			{
				GestureRecognizer.HoldCanceledEventDelegate holdCanceledEventDelegate = this.HoldCanceledEvent;
				GestureRecognizer.HoldCanceledEventDelegate holdCanceledEventDelegate2;
				do
				{
					holdCanceledEventDelegate2 = holdCanceledEventDelegate;
					holdCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldCanceledEventDelegate>(ref this.HoldCanceledEvent, (GestureRecognizer.HoldCanceledEventDelegate)Delegate.Combine(holdCanceledEventDelegate2, value), holdCanceledEventDelegate);
				}
				while (holdCanceledEventDelegate != holdCanceledEventDelegate2);
			}
			remove
			{
				GestureRecognizer.HoldCanceledEventDelegate holdCanceledEventDelegate = this.HoldCanceledEvent;
				GestureRecognizer.HoldCanceledEventDelegate holdCanceledEventDelegate2;
				do
				{
					holdCanceledEventDelegate2 = holdCanceledEventDelegate;
					holdCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldCanceledEventDelegate>(ref this.HoldCanceledEvent, (GestureRecognizer.HoldCanceledEventDelegate)Delegate.Remove(holdCanceledEventDelegate2, value), holdCanceledEventDelegate);
				}
				while (holdCanceledEventDelegate != holdCanceledEventDelegate2);
			}
		}

		public event GestureRecognizer.HoldCompletedEventDelegate HoldCompletedEvent
		{
			add
			{
				GestureRecognizer.HoldCompletedEventDelegate holdCompletedEventDelegate = this.HoldCompletedEvent;
				GestureRecognizer.HoldCompletedEventDelegate holdCompletedEventDelegate2;
				do
				{
					holdCompletedEventDelegate2 = holdCompletedEventDelegate;
					holdCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldCompletedEventDelegate>(ref this.HoldCompletedEvent, (GestureRecognizer.HoldCompletedEventDelegate)Delegate.Combine(holdCompletedEventDelegate2, value), holdCompletedEventDelegate);
				}
				while (holdCompletedEventDelegate != holdCompletedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.HoldCompletedEventDelegate holdCompletedEventDelegate = this.HoldCompletedEvent;
				GestureRecognizer.HoldCompletedEventDelegate holdCompletedEventDelegate2;
				do
				{
					holdCompletedEventDelegate2 = holdCompletedEventDelegate;
					holdCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldCompletedEventDelegate>(ref this.HoldCompletedEvent, (GestureRecognizer.HoldCompletedEventDelegate)Delegate.Remove(holdCompletedEventDelegate2, value), holdCompletedEventDelegate);
				}
				while (holdCompletedEventDelegate != holdCompletedEventDelegate2);
			}
		}

		public event GestureRecognizer.HoldStartedEventDelegate HoldStartedEvent
		{
			add
			{
				GestureRecognizer.HoldStartedEventDelegate holdStartedEventDelegate = this.HoldStartedEvent;
				GestureRecognizer.HoldStartedEventDelegate holdStartedEventDelegate2;
				do
				{
					holdStartedEventDelegate2 = holdStartedEventDelegate;
					holdStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldStartedEventDelegate>(ref this.HoldStartedEvent, (GestureRecognizer.HoldStartedEventDelegate)Delegate.Combine(holdStartedEventDelegate2, value), holdStartedEventDelegate);
				}
				while (holdStartedEventDelegate != holdStartedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.HoldStartedEventDelegate holdStartedEventDelegate = this.HoldStartedEvent;
				GestureRecognizer.HoldStartedEventDelegate holdStartedEventDelegate2;
				do
				{
					holdStartedEventDelegate2 = holdStartedEventDelegate;
					holdStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.HoldStartedEventDelegate>(ref this.HoldStartedEvent, (GestureRecognizer.HoldStartedEventDelegate)Delegate.Remove(holdStartedEventDelegate2, value), holdStartedEventDelegate);
				}
				while (holdStartedEventDelegate != holdStartedEventDelegate2);
			}
		}

		public event GestureRecognizer.TappedEventDelegate TappedEvent
		{
			add
			{
				GestureRecognizer.TappedEventDelegate tappedEventDelegate = this.TappedEvent;
				GestureRecognizer.TappedEventDelegate tappedEventDelegate2;
				do
				{
					tappedEventDelegate2 = tappedEventDelegate;
					tappedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.TappedEventDelegate>(ref this.TappedEvent, (GestureRecognizer.TappedEventDelegate)Delegate.Combine(tappedEventDelegate2, value), tappedEventDelegate);
				}
				while (tappedEventDelegate != tappedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.TappedEventDelegate tappedEventDelegate = this.TappedEvent;
				GestureRecognizer.TappedEventDelegate tappedEventDelegate2;
				do
				{
					tappedEventDelegate2 = tappedEventDelegate;
					tappedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.TappedEventDelegate>(ref this.TappedEvent, (GestureRecognizer.TappedEventDelegate)Delegate.Remove(tappedEventDelegate2, value), tappedEventDelegate);
				}
				while (tappedEventDelegate != tappedEventDelegate2);
			}
		}

		public event GestureRecognizer.ManipulationCanceledEventDelegate ManipulationCanceledEvent
		{
			add
			{
				GestureRecognizer.ManipulationCanceledEventDelegate manipulationCanceledEventDelegate = this.ManipulationCanceledEvent;
				GestureRecognizer.ManipulationCanceledEventDelegate manipulationCanceledEventDelegate2;
				do
				{
					manipulationCanceledEventDelegate2 = manipulationCanceledEventDelegate;
					manipulationCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationCanceledEventDelegate>(ref this.ManipulationCanceledEvent, (GestureRecognizer.ManipulationCanceledEventDelegate)Delegate.Combine(manipulationCanceledEventDelegate2, value), manipulationCanceledEventDelegate);
				}
				while (manipulationCanceledEventDelegate != manipulationCanceledEventDelegate2);
			}
			remove
			{
				GestureRecognizer.ManipulationCanceledEventDelegate manipulationCanceledEventDelegate = this.ManipulationCanceledEvent;
				GestureRecognizer.ManipulationCanceledEventDelegate manipulationCanceledEventDelegate2;
				do
				{
					manipulationCanceledEventDelegate2 = manipulationCanceledEventDelegate;
					manipulationCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationCanceledEventDelegate>(ref this.ManipulationCanceledEvent, (GestureRecognizer.ManipulationCanceledEventDelegate)Delegate.Remove(manipulationCanceledEventDelegate2, value), manipulationCanceledEventDelegate);
				}
				while (manipulationCanceledEventDelegate != manipulationCanceledEventDelegate2);
			}
		}

		public event GestureRecognizer.ManipulationCompletedEventDelegate ManipulationCompletedEvent
		{
			add
			{
				GestureRecognizer.ManipulationCompletedEventDelegate manipulationCompletedEventDelegate = this.ManipulationCompletedEvent;
				GestureRecognizer.ManipulationCompletedEventDelegate manipulationCompletedEventDelegate2;
				do
				{
					manipulationCompletedEventDelegate2 = manipulationCompletedEventDelegate;
					manipulationCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationCompletedEventDelegate>(ref this.ManipulationCompletedEvent, (GestureRecognizer.ManipulationCompletedEventDelegate)Delegate.Combine(manipulationCompletedEventDelegate2, value), manipulationCompletedEventDelegate);
				}
				while (manipulationCompletedEventDelegate != manipulationCompletedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.ManipulationCompletedEventDelegate manipulationCompletedEventDelegate = this.ManipulationCompletedEvent;
				GestureRecognizer.ManipulationCompletedEventDelegate manipulationCompletedEventDelegate2;
				do
				{
					manipulationCompletedEventDelegate2 = manipulationCompletedEventDelegate;
					manipulationCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationCompletedEventDelegate>(ref this.ManipulationCompletedEvent, (GestureRecognizer.ManipulationCompletedEventDelegate)Delegate.Remove(manipulationCompletedEventDelegate2, value), manipulationCompletedEventDelegate);
				}
				while (manipulationCompletedEventDelegate != manipulationCompletedEventDelegate2);
			}
		}

		public event GestureRecognizer.ManipulationStartedEventDelegate ManipulationStartedEvent
		{
			add
			{
				GestureRecognizer.ManipulationStartedEventDelegate manipulationStartedEventDelegate = this.ManipulationStartedEvent;
				GestureRecognizer.ManipulationStartedEventDelegate manipulationStartedEventDelegate2;
				do
				{
					manipulationStartedEventDelegate2 = manipulationStartedEventDelegate;
					manipulationStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationStartedEventDelegate>(ref this.ManipulationStartedEvent, (GestureRecognizer.ManipulationStartedEventDelegate)Delegate.Combine(manipulationStartedEventDelegate2, value), manipulationStartedEventDelegate);
				}
				while (manipulationStartedEventDelegate != manipulationStartedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.ManipulationStartedEventDelegate manipulationStartedEventDelegate = this.ManipulationStartedEvent;
				GestureRecognizer.ManipulationStartedEventDelegate manipulationStartedEventDelegate2;
				do
				{
					manipulationStartedEventDelegate2 = manipulationStartedEventDelegate;
					manipulationStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationStartedEventDelegate>(ref this.ManipulationStartedEvent, (GestureRecognizer.ManipulationStartedEventDelegate)Delegate.Remove(manipulationStartedEventDelegate2, value), manipulationStartedEventDelegate);
				}
				while (manipulationStartedEventDelegate != manipulationStartedEventDelegate2);
			}
		}

		public event GestureRecognizer.ManipulationUpdatedEventDelegate ManipulationUpdatedEvent
		{
			add
			{
				GestureRecognizer.ManipulationUpdatedEventDelegate manipulationUpdatedEventDelegate = this.ManipulationUpdatedEvent;
				GestureRecognizer.ManipulationUpdatedEventDelegate manipulationUpdatedEventDelegate2;
				do
				{
					manipulationUpdatedEventDelegate2 = manipulationUpdatedEventDelegate;
					manipulationUpdatedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationUpdatedEventDelegate>(ref this.ManipulationUpdatedEvent, (GestureRecognizer.ManipulationUpdatedEventDelegate)Delegate.Combine(manipulationUpdatedEventDelegate2, value), manipulationUpdatedEventDelegate);
				}
				while (manipulationUpdatedEventDelegate != manipulationUpdatedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.ManipulationUpdatedEventDelegate manipulationUpdatedEventDelegate = this.ManipulationUpdatedEvent;
				GestureRecognizer.ManipulationUpdatedEventDelegate manipulationUpdatedEventDelegate2;
				do
				{
					manipulationUpdatedEventDelegate2 = manipulationUpdatedEventDelegate;
					manipulationUpdatedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.ManipulationUpdatedEventDelegate>(ref this.ManipulationUpdatedEvent, (GestureRecognizer.ManipulationUpdatedEventDelegate)Delegate.Remove(manipulationUpdatedEventDelegate2, value), manipulationUpdatedEventDelegate);
				}
				while (manipulationUpdatedEventDelegate != manipulationUpdatedEventDelegate2);
			}
		}

		public event GestureRecognizer.NavigationCanceledEventDelegate NavigationCanceledEvent
		{
			add
			{
				GestureRecognizer.NavigationCanceledEventDelegate navigationCanceledEventDelegate = this.NavigationCanceledEvent;
				GestureRecognizer.NavigationCanceledEventDelegate navigationCanceledEventDelegate2;
				do
				{
					navigationCanceledEventDelegate2 = navigationCanceledEventDelegate;
					navigationCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationCanceledEventDelegate>(ref this.NavigationCanceledEvent, (GestureRecognizer.NavigationCanceledEventDelegate)Delegate.Combine(navigationCanceledEventDelegate2, value), navigationCanceledEventDelegate);
				}
				while (navigationCanceledEventDelegate != navigationCanceledEventDelegate2);
			}
			remove
			{
				GestureRecognizer.NavigationCanceledEventDelegate navigationCanceledEventDelegate = this.NavigationCanceledEvent;
				GestureRecognizer.NavigationCanceledEventDelegate navigationCanceledEventDelegate2;
				do
				{
					navigationCanceledEventDelegate2 = navigationCanceledEventDelegate;
					navigationCanceledEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationCanceledEventDelegate>(ref this.NavigationCanceledEvent, (GestureRecognizer.NavigationCanceledEventDelegate)Delegate.Remove(navigationCanceledEventDelegate2, value), navigationCanceledEventDelegate);
				}
				while (navigationCanceledEventDelegate != navigationCanceledEventDelegate2);
			}
		}

		public event GestureRecognizer.NavigationCompletedEventDelegate NavigationCompletedEvent
		{
			add
			{
				GestureRecognizer.NavigationCompletedEventDelegate navigationCompletedEventDelegate = this.NavigationCompletedEvent;
				GestureRecognizer.NavigationCompletedEventDelegate navigationCompletedEventDelegate2;
				do
				{
					navigationCompletedEventDelegate2 = navigationCompletedEventDelegate;
					navigationCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationCompletedEventDelegate>(ref this.NavigationCompletedEvent, (GestureRecognizer.NavigationCompletedEventDelegate)Delegate.Combine(navigationCompletedEventDelegate2, value), navigationCompletedEventDelegate);
				}
				while (navigationCompletedEventDelegate != navigationCompletedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.NavigationCompletedEventDelegate navigationCompletedEventDelegate = this.NavigationCompletedEvent;
				GestureRecognizer.NavigationCompletedEventDelegate navigationCompletedEventDelegate2;
				do
				{
					navigationCompletedEventDelegate2 = navigationCompletedEventDelegate;
					navigationCompletedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationCompletedEventDelegate>(ref this.NavigationCompletedEvent, (GestureRecognizer.NavigationCompletedEventDelegate)Delegate.Remove(navigationCompletedEventDelegate2, value), navigationCompletedEventDelegate);
				}
				while (navigationCompletedEventDelegate != navigationCompletedEventDelegate2);
			}
		}

		public event GestureRecognizer.NavigationStartedEventDelegate NavigationStartedEvent
		{
			add
			{
				GestureRecognizer.NavigationStartedEventDelegate navigationStartedEventDelegate = this.NavigationStartedEvent;
				GestureRecognizer.NavigationStartedEventDelegate navigationStartedEventDelegate2;
				do
				{
					navigationStartedEventDelegate2 = navigationStartedEventDelegate;
					navigationStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationStartedEventDelegate>(ref this.NavigationStartedEvent, (GestureRecognizer.NavigationStartedEventDelegate)Delegate.Combine(navigationStartedEventDelegate2, value), navigationStartedEventDelegate);
				}
				while (navigationStartedEventDelegate != navigationStartedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.NavigationStartedEventDelegate navigationStartedEventDelegate = this.NavigationStartedEvent;
				GestureRecognizer.NavigationStartedEventDelegate navigationStartedEventDelegate2;
				do
				{
					navigationStartedEventDelegate2 = navigationStartedEventDelegate;
					navigationStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationStartedEventDelegate>(ref this.NavigationStartedEvent, (GestureRecognizer.NavigationStartedEventDelegate)Delegate.Remove(navigationStartedEventDelegate2, value), navigationStartedEventDelegate);
				}
				while (navigationStartedEventDelegate != navigationStartedEventDelegate2);
			}
		}

		public event GestureRecognizer.NavigationUpdatedEventDelegate NavigationUpdatedEvent
		{
			add
			{
				GestureRecognizer.NavigationUpdatedEventDelegate navigationUpdatedEventDelegate = this.NavigationUpdatedEvent;
				GestureRecognizer.NavigationUpdatedEventDelegate navigationUpdatedEventDelegate2;
				do
				{
					navigationUpdatedEventDelegate2 = navigationUpdatedEventDelegate;
					navigationUpdatedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationUpdatedEventDelegate>(ref this.NavigationUpdatedEvent, (GestureRecognizer.NavigationUpdatedEventDelegate)Delegate.Combine(navigationUpdatedEventDelegate2, value), navigationUpdatedEventDelegate);
				}
				while (navigationUpdatedEventDelegate != navigationUpdatedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.NavigationUpdatedEventDelegate navigationUpdatedEventDelegate = this.NavigationUpdatedEvent;
				GestureRecognizer.NavigationUpdatedEventDelegate navigationUpdatedEventDelegate2;
				do
				{
					navigationUpdatedEventDelegate2 = navigationUpdatedEventDelegate;
					navigationUpdatedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.NavigationUpdatedEventDelegate>(ref this.NavigationUpdatedEvent, (GestureRecognizer.NavigationUpdatedEventDelegate)Delegate.Remove(navigationUpdatedEventDelegate2, value), navigationUpdatedEventDelegate);
				}
				while (navigationUpdatedEventDelegate != navigationUpdatedEventDelegate2);
			}
		}

		public event GestureRecognizer.RecognitionEndedEventDelegate RecognitionEndedEvent
		{
			add
			{
				GestureRecognizer.RecognitionEndedEventDelegate recognitionEndedEventDelegate = this.RecognitionEndedEvent;
				GestureRecognizer.RecognitionEndedEventDelegate recognitionEndedEventDelegate2;
				do
				{
					recognitionEndedEventDelegate2 = recognitionEndedEventDelegate;
					recognitionEndedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.RecognitionEndedEventDelegate>(ref this.RecognitionEndedEvent, (GestureRecognizer.RecognitionEndedEventDelegate)Delegate.Combine(recognitionEndedEventDelegate2, value), recognitionEndedEventDelegate);
				}
				while (recognitionEndedEventDelegate != recognitionEndedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.RecognitionEndedEventDelegate recognitionEndedEventDelegate = this.RecognitionEndedEvent;
				GestureRecognizer.RecognitionEndedEventDelegate recognitionEndedEventDelegate2;
				do
				{
					recognitionEndedEventDelegate2 = recognitionEndedEventDelegate;
					recognitionEndedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.RecognitionEndedEventDelegate>(ref this.RecognitionEndedEvent, (GestureRecognizer.RecognitionEndedEventDelegate)Delegate.Remove(recognitionEndedEventDelegate2, value), recognitionEndedEventDelegate);
				}
				while (recognitionEndedEventDelegate != recognitionEndedEventDelegate2);
			}
		}

		public event GestureRecognizer.RecognitionStartedEventDelegate RecognitionStartedEvent
		{
			add
			{
				GestureRecognizer.RecognitionStartedEventDelegate recognitionStartedEventDelegate = this.RecognitionStartedEvent;
				GestureRecognizer.RecognitionStartedEventDelegate recognitionStartedEventDelegate2;
				do
				{
					recognitionStartedEventDelegate2 = recognitionStartedEventDelegate;
					recognitionStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.RecognitionStartedEventDelegate>(ref this.RecognitionStartedEvent, (GestureRecognizer.RecognitionStartedEventDelegate)Delegate.Combine(recognitionStartedEventDelegate2, value), recognitionStartedEventDelegate);
				}
				while (recognitionStartedEventDelegate != recognitionStartedEventDelegate2);
			}
			remove
			{
				GestureRecognizer.RecognitionStartedEventDelegate recognitionStartedEventDelegate = this.RecognitionStartedEvent;
				GestureRecognizer.RecognitionStartedEventDelegate recognitionStartedEventDelegate2;
				do
				{
					recognitionStartedEventDelegate2 = recognitionStartedEventDelegate;
					recognitionStartedEventDelegate = Interlocked.CompareExchange<GestureRecognizer.RecognitionStartedEventDelegate>(ref this.RecognitionStartedEvent, (GestureRecognizer.RecognitionStartedEventDelegate)Delegate.Remove(recognitionStartedEventDelegate2, value), recognitionStartedEventDelegate);
				}
				while (recognitionStartedEventDelegate != recognitionStartedEventDelegate2);
			}
		}

		public event GestureRecognizer.GestureErrorDelegate GestureErrorEvent
		{
			add
			{
				GestureRecognizer.GestureErrorDelegate gestureErrorDelegate = this.GestureErrorEvent;
				GestureRecognizer.GestureErrorDelegate gestureErrorDelegate2;
				do
				{
					gestureErrorDelegate2 = gestureErrorDelegate;
					gestureErrorDelegate = Interlocked.CompareExchange<GestureRecognizer.GestureErrorDelegate>(ref this.GestureErrorEvent, (GestureRecognizer.GestureErrorDelegate)Delegate.Combine(gestureErrorDelegate2, value), gestureErrorDelegate);
				}
				while (gestureErrorDelegate != gestureErrorDelegate2);
			}
			remove
			{
				GestureRecognizer.GestureErrorDelegate gestureErrorDelegate = this.GestureErrorEvent;
				GestureRecognizer.GestureErrorDelegate gestureErrorDelegate2;
				do
				{
					gestureErrorDelegate2 = gestureErrorDelegate;
					gestureErrorDelegate = Interlocked.CompareExchange<GestureRecognizer.GestureErrorDelegate>(ref this.GestureErrorEvent, (GestureRecognizer.GestureErrorDelegate)Delegate.Remove(gestureErrorDelegate2, value), gestureErrorDelegate);
				}
				while (gestureErrorDelegate != gestureErrorDelegate2);
			}
		}

		public GestureRecognizer()
		{
			this.m_Recognizer = this.Internal_Create();
		}

		~GestureRecognizer()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				GestureRecognizer.DestroyThreaded(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				GestureRecognizer.Destroy(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public GestureSettings SetRecognizableGestures(GestureSettings newMaskValue)
		{
			GestureSettings result;
			if (this.m_Recognizer != IntPtr.Zero)
			{
				result = (GestureSettings)this.Internal_SetRecognizableGestures(this.m_Recognizer, (int)newMaskValue);
			}
			else
			{
				result = GestureSettings.None;
			}
			return result;
		}

		public GestureSettings GetRecognizableGestures()
		{
			GestureSettings result;
			if (this.m_Recognizer != IntPtr.Zero)
			{
				result = (GestureSettings)this.Internal_GetRecognizableGestures(this.m_Recognizer);
			}
			else
			{
				result = GestureSettings.None;
			}
			return result;
		}

		public void StartCapturingGestures()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				this.Internal_StartCapturingGestures(this.m_Recognizer);
			}
		}

		public void StopCapturingGestures()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				this.Internal_StopCapturingGestures(this.m_Recognizer);
			}
		}

		public bool IsCapturingGestures()
		{
			return this.m_Recognizer != IntPtr.Zero && this.Internal_IsCapturingGestures(this.m_Recognizer);
		}

		public void CancelGestures()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				this.Internal_CancelGestures(this.m_Recognizer);
			}
		}

		[RequiredByNativeCode]
		private void InvokeHoldEvent(GestureRecognizer.GestureEventType eventType, InteractionSourceKind source, Ray headRay)
		{
			switch (eventType)
			{
			case GestureRecognizer.GestureEventType.HoldCanceled:
			{
				GestureRecognizer.HoldCanceledEventDelegate holdCanceledEvent = this.HoldCanceledEvent;
				if (holdCanceledEvent != null)
				{
					holdCanceledEvent(source, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.HoldCompleted:
			{
				GestureRecognizer.HoldCompletedEventDelegate holdCompletedEvent = this.HoldCompletedEvent;
				if (holdCompletedEvent != null)
				{
					holdCompletedEvent(source, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.HoldStarted:
			{
				GestureRecognizer.HoldStartedEventDelegate holdStartedEvent = this.HoldStartedEvent;
				if (holdStartedEvent != null)
				{
					holdStartedEvent(source, headRay);
				}
				break;
			}
			default:
				throw new ArgumentException("InvokeHoldEvent: Invalid GestureEventType");
			}
		}

		[RequiredByNativeCode]
		private void InvokeTapEvent(InteractionSourceKind source, Ray headRay, int tapCount)
		{
			GestureRecognizer.TappedEventDelegate tappedEvent = this.TappedEvent;
			if (tappedEvent != null)
			{
				tappedEvent(source, tapCount, headRay);
			}
		}

		[RequiredByNativeCode]
		private void InvokeManipulationEvent(GestureRecognizer.GestureEventType eventType, InteractionSourceKind source, Vector3 position, Ray headRay)
		{
			switch (eventType)
			{
			case GestureRecognizer.GestureEventType.ManipulationCanceled:
			{
				GestureRecognizer.ManipulationCanceledEventDelegate manipulationCanceledEvent = this.ManipulationCanceledEvent;
				if (manipulationCanceledEvent != null)
				{
					manipulationCanceledEvent(source, position, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.ManipulationCompleted:
			{
				GestureRecognizer.ManipulationCompletedEventDelegate manipulationCompletedEvent = this.ManipulationCompletedEvent;
				if (manipulationCompletedEvent != null)
				{
					manipulationCompletedEvent(source, position, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.ManipulationStarted:
			{
				GestureRecognizer.ManipulationStartedEventDelegate manipulationStartedEvent = this.ManipulationStartedEvent;
				if (manipulationStartedEvent != null)
				{
					manipulationStartedEvent(source, position, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.ManipulationUpdated:
			{
				GestureRecognizer.ManipulationUpdatedEventDelegate manipulationUpdatedEvent = this.ManipulationUpdatedEvent;
				if (manipulationUpdatedEvent != null)
				{
					manipulationUpdatedEvent(source, position, headRay);
				}
				break;
			}
			default:
				throw new ArgumentException("InvokeManipulationEvent: Invalid GestureEventType");
			}
		}

		[RequiredByNativeCode]
		private void InvokeNavigationEvent(GestureRecognizer.GestureEventType eventType, InteractionSourceKind source, Vector3 relativePosition, Ray headRay)
		{
			switch (eventType)
			{
			case GestureRecognizer.GestureEventType.NavigationCanceled:
			{
				GestureRecognizer.NavigationCanceledEventDelegate navigationCanceledEvent = this.NavigationCanceledEvent;
				if (navigationCanceledEvent != null)
				{
					navigationCanceledEvent(source, relativePosition, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.NavigationCompleted:
			{
				GestureRecognizer.NavigationCompletedEventDelegate navigationCompletedEvent = this.NavigationCompletedEvent;
				if (navigationCompletedEvent != null)
				{
					navigationCompletedEvent(source, relativePosition, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.NavigationStarted:
			{
				GestureRecognizer.NavigationStartedEventDelegate navigationStartedEvent = this.NavigationStartedEvent;
				if (navigationStartedEvent != null)
				{
					navigationStartedEvent(source, relativePosition, headRay);
				}
				break;
			}
			case GestureRecognizer.GestureEventType.NavigationUpdated:
			{
				GestureRecognizer.NavigationUpdatedEventDelegate navigationUpdatedEvent = this.NavigationUpdatedEvent;
				if (navigationUpdatedEvent != null)
				{
					navigationUpdatedEvent(source, relativePosition, headRay);
				}
				break;
			}
			default:
				throw new ArgumentException("InvokeNavigationEvent: Invalid GestureEventType");
			}
		}

		[RequiredByNativeCode]
		private void InvokeRecognitionEvent(GestureRecognizer.GestureEventType eventType, InteractionSourceKind source, Ray headRay)
		{
			if (eventType != GestureRecognizer.GestureEventType.RecognitionEnded)
			{
				if (eventType != GestureRecognizer.GestureEventType.RecognitionStarted)
				{
					throw new ArgumentException("InvokeRecognitionEvent: Invalid GestureEventType");
				}
				GestureRecognizer.RecognitionStartedEventDelegate recognitionStartedEvent = this.RecognitionStartedEvent;
				if (recognitionStartedEvent != null)
				{
					recognitionStartedEvent(source, headRay);
				}
			}
			else
			{
				GestureRecognizer.RecognitionEndedEventDelegate recognitionEndedEvent = this.RecognitionEndedEvent;
				if (recognitionEndedEvent != null)
				{
					recognitionEndedEvent(source, headRay);
				}
			}
		}

		[RequiredByNativeCode]
		private void InvokeErrorEvent(string error, int hresult)
		{
			GestureRecognizer.GestureErrorDelegate gestureErrorEvent = this.GestureErrorEvent;
			if (gestureErrorEvent != null)
			{
				gestureErrorEvent(error, hresult);
			}
		}

		private IntPtr Internal_Create()
		{
			IntPtr result;
			GestureRecognizer.INTERNAL_CALL_Internal_Create(this, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_Create(GestureRecognizer self, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_StartCapturingGestures(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_StopCapturingGestures(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_IsCapturingGestures(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_SetRecognizableGestures(IntPtr recognizer, int newMaskValue);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetRecognizableGestures(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CancelGestures(IntPtr recognizer);
	}
}
