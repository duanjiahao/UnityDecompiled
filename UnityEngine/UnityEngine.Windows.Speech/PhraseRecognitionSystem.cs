using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public static class PhraseRecognitionSystem
	{
		public delegate void ErrorDelegate(SpeechError errorCode);

		public delegate void StatusDelegate(SpeechSystemStatus status);

		public static event PhraseRecognitionSystem.ErrorDelegate OnError
		{
			add
			{
				PhraseRecognitionSystem.ErrorDelegate errorDelegate = PhraseRecognitionSystem.OnError;
				PhraseRecognitionSystem.ErrorDelegate errorDelegate2;
				do
				{
					errorDelegate2 = errorDelegate;
					errorDelegate = Interlocked.CompareExchange<PhraseRecognitionSystem.ErrorDelegate>(ref PhraseRecognitionSystem.OnError, (PhraseRecognitionSystem.ErrorDelegate)Delegate.Combine(errorDelegate2, value), errorDelegate);
				}
				while (errorDelegate != errorDelegate2);
			}
			remove
			{
				PhraseRecognitionSystem.ErrorDelegate errorDelegate = PhraseRecognitionSystem.OnError;
				PhraseRecognitionSystem.ErrorDelegate errorDelegate2;
				do
				{
					errorDelegate2 = errorDelegate;
					errorDelegate = Interlocked.CompareExchange<PhraseRecognitionSystem.ErrorDelegate>(ref PhraseRecognitionSystem.OnError, (PhraseRecognitionSystem.ErrorDelegate)Delegate.Remove(errorDelegate2, value), errorDelegate);
				}
				while (errorDelegate != errorDelegate2);
			}
		}

		public static event PhraseRecognitionSystem.StatusDelegate OnStatusChanged
		{
			add
			{
				PhraseRecognitionSystem.StatusDelegate statusDelegate = PhraseRecognitionSystem.OnStatusChanged;
				PhraseRecognitionSystem.StatusDelegate statusDelegate2;
				do
				{
					statusDelegate2 = statusDelegate;
					statusDelegate = Interlocked.CompareExchange<PhraseRecognitionSystem.StatusDelegate>(ref PhraseRecognitionSystem.OnStatusChanged, (PhraseRecognitionSystem.StatusDelegate)Delegate.Combine(statusDelegate2, value), statusDelegate);
				}
				while (statusDelegate != statusDelegate2);
			}
			remove
			{
				PhraseRecognitionSystem.StatusDelegate statusDelegate = PhraseRecognitionSystem.OnStatusChanged;
				PhraseRecognitionSystem.StatusDelegate statusDelegate2;
				do
				{
					statusDelegate2 = statusDelegate;
					statusDelegate = Interlocked.CompareExchange<PhraseRecognitionSystem.StatusDelegate>(ref PhraseRecognitionSystem.OnStatusChanged, (PhraseRecognitionSystem.StatusDelegate)Delegate.Remove(statusDelegate2, value), statusDelegate);
				}
				while (statusDelegate != statusDelegate2);
			}
		}

		[ThreadAndSerializationSafe]
		public static extern bool isSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SpeechSystemStatus Status
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Restart();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Shutdown();

		[RequiredByNativeCode]
		private static void PhraseRecognitionSystem_InvokeErrorEvent(SpeechError errorCode)
		{
			PhraseRecognitionSystem.ErrorDelegate onError = PhraseRecognitionSystem.OnError;
			if (onError != null)
			{
				onError(errorCode);
			}
		}

		[RequiredByNativeCode]
		private static void PhraseRecognitionSystem_InvokeStatusChangedEvent(SpeechSystemStatus status)
		{
			PhraseRecognitionSystem.StatusDelegate onStatusChanged = PhraseRecognitionSystem.OnStatusChanged;
			if (onStatusChanged != null)
			{
				onStatusChanged(status);
			}
		}
	}
}
