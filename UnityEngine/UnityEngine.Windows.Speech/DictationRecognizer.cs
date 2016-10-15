using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public sealed class DictationRecognizer : IDisposable
	{
		public delegate void DictationHypothesisDelegate(string text);

		public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);

		public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

		public delegate void DictationErrorHandler(string error, int hresult);

		private IntPtr m_Recognizer;

		public event DictationRecognizer.DictationHypothesisDelegate DictationHypothesis
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.DictationHypothesis = (DictationRecognizer.DictationHypothesisDelegate)Delegate.Combine(this.DictationHypothesis, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.DictationHypothesis = (DictationRecognizer.DictationHypothesisDelegate)Delegate.Remove(this.DictationHypothesis, value);
			}
		}

		public event DictationRecognizer.DictationResultDelegate DictationResult
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.DictationResult = (DictationRecognizer.DictationResultDelegate)Delegate.Combine(this.DictationResult, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.DictationResult = (DictationRecognizer.DictationResultDelegate)Delegate.Remove(this.DictationResult, value);
			}
		}

		public event DictationRecognizer.DictationCompletedDelegate DictationComplete
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.DictationComplete = (DictationRecognizer.DictationCompletedDelegate)Delegate.Combine(this.DictationComplete, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.DictationComplete = (DictationRecognizer.DictationCompletedDelegate)Delegate.Remove(this.DictationComplete, value);
			}
		}

		public event DictationRecognizer.DictationErrorHandler DictationError
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.DictationError = (DictationRecognizer.DictationErrorHandler)Delegate.Combine(this.DictationError, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.DictationError = (DictationRecognizer.DictationErrorHandler)Delegate.Remove(this.DictationError, value);
			}
		}

		public SpeechSystemStatus Status
		{
			get
			{
				return (!(this.m_Recognizer != IntPtr.Zero)) ? SpeechSystemStatus.Stopped : DictationRecognizer.GetStatus(this.m_Recognizer);
			}
		}

		public float AutoSilenceTimeoutSeconds
		{
			get
			{
				if (this.m_Recognizer == IntPtr.Zero)
				{
					return 0f;
				}
				return DictationRecognizer.GetAutoSilenceTimeoutSeconds(this.m_Recognizer);
			}
			set
			{
				if (this.m_Recognizer == IntPtr.Zero)
				{
					return;
				}
				DictationRecognizer.SetAutoSilenceTimeoutSeconds(this.m_Recognizer, value);
			}
		}

		public float InitialSilenceTimeoutSeconds
		{
			get
			{
				if (this.m_Recognizer == IntPtr.Zero)
				{
					return 0f;
				}
				return DictationRecognizer.GetInitialSilenceTimeoutSeconds(this.m_Recognizer);
			}
			set
			{
				if (this.m_Recognizer == IntPtr.Zero)
				{
					return;
				}
				DictationRecognizer.SetInitialSilenceTimeoutSeconds(this.m_Recognizer, value);
			}
		}

		public DictationRecognizer() : this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
		{
		}

		public DictationRecognizer(ConfidenceLevel confidenceLevel) : this(confidenceLevel, DictationTopicConstraint.Dictation)
		{
		}

		public DictationRecognizer(DictationTopicConstraint topic) : this(ConfidenceLevel.Medium, topic)
		{
		}

		public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
		{
			this.m_Recognizer = this.Create(minimumConfidence, topic);
		}

		private IntPtr Create(ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint)
		{
			IntPtr result;
			DictationRecognizer.INTERNAL_CALL_Create(this, minimumConfidence, topicConstraint, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr self);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpeechSystemStatus GetStatus(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInitialSilenceTimeoutSeconds(IntPtr self, float value);

		~DictationRecognizer()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				DictationRecognizer.DestroyThreaded(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public void Start()
		{
			if (this.m_Recognizer == IntPtr.Zero)
			{
				return;
			}
			DictationRecognizer.Start(this.m_Recognizer);
		}

		public void Stop()
		{
			if (this.m_Recognizer == IntPtr.Zero)
			{
				return;
			}
			DictationRecognizer.Stop(this.m_Recognizer);
		}

		public void Dispose()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				DictationRecognizer.Destroy(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeHypothesisGeneratedEvent(string keyword)
		{
			DictationRecognizer.DictationHypothesisDelegate dictationHypothesis = this.DictationHypothesis;
			if (dictationHypothesis != null)
			{
				dictationHypothesis(keyword);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeResultGeneratedEvent(string keyword, ConfidenceLevel minimumConfidence)
		{
			DictationRecognizer.DictationResultDelegate dictationResult = this.DictationResult;
			if (dictationResult != null)
			{
				dictationResult(keyword, minimumConfidence);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeCompletedEvent(DictationCompletionCause cause)
		{
			DictationRecognizer.DictationCompletedDelegate dictationComplete = this.DictationComplete;
			if (dictationComplete != null)
			{
				dictationComplete(cause);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeErrorEvent(string error, int hresult)
		{
			DictationRecognizer.DictationErrorHandler dictationError = this.DictationError;
			if (dictationError != null)
			{
				dictationError(error, hresult);
			}
		}
	}
}
