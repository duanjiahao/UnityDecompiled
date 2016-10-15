using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public static class PhraseRecognitionSystem
	{
		public delegate void ErrorDelegate(SpeechError errorCode);

		public delegate void StatusDelegate(SpeechSystemStatus status);

		public static event PhraseRecognitionSystem.ErrorDelegate OnError
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				PhraseRecognitionSystem.OnError = (PhraseRecognitionSystem.ErrorDelegate)Delegate.Combine(PhraseRecognitionSystem.OnError, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				PhraseRecognitionSystem.OnError = (PhraseRecognitionSystem.ErrorDelegate)Delegate.Remove(PhraseRecognitionSystem.OnError, value);
			}
		}

		public static event PhraseRecognitionSystem.StatusDelegate OnStatusChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				PhraseRecognitionSystem.OnStatusChanged = (PhraseRecognitionSystem.StatusDelegate)Delegate.Combine(PhraseRecognitionSystem.OnStatusChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				PhraseRecognitionSystem.OnStatusChanged = (PhraseRecognitionSystem.StatusDelegate)Delegate.Remove(PhraseRecognitionSystem.OnStatusChanged, value);
			}
		}

		[ThreadAndSerializationSafe]
		public static extern bool isSupported
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SpeechSystemStatus Status
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Restart();

		[WrapperlessIcall]
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
