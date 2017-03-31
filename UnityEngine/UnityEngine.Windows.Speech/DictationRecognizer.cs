using System;
using System.Runtime.CompilerServices;
using System.Threading;
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
			add
			{
				DictationRecognizer.DictationHypothesisDelegate dictationHypothesisDelegate = this.DictationHypothesis;
				DictationRecognizer.DictationHypothesisDelegate dictationHypothesisDelegate2;
				do
				{
					dictationHypothesisDelegate2 = dictationHypothesisDelegate;
					dictationHypothesisDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationHypothesisDelegate>(ref this.DictationHypothesis, (DictationRecognizer.DictationHypothesisDelegate)Delegate.Combine(dictationHypothesisDelegate2, value), dictationHypothesisDelegate);
				}
				while (dictationHypothesisDelegate != dictationHypothesisDelegate2);
			}
			remove
			{
				DictationRecognizer.DictationHypothesisDelegate dictationHypothesisDelegate = this.DictationHypothesis;
				DictationRecognizer.DictationHypothesisDelegate dictationHypothesisDelegate2;
				do
				{
					dictationHypothesisDelegate2 = dictationHypothesisDelegate;
					dictationHypothesisDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationHypothesisDelegate>(ref this.DictationHypothesis, (DictationRecognizer.DictationHypothesisDelegate)Delegate.Remove(dictationHypothesisDelegate2, value), dictationHypothesisDelegate);
				}
				while (dictationHypothesisDelegate != dictationHypothesisDelegate2);
			}
		}

		public event DictationRecognizer.DictationResultDelegate DictationResult
		{
			add
			{
				DictationRecognizer.DictationResultDelegate dictationResultDelegate = this.DictationResult;
				DictationRecognizer.DictationResultDelegate dictationResultDelegate2;
				do
				{
					dictationResultDelegate2 = dictationResultDelegate;
					dictationResultDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationResultDelegate>(ref this.DictationResult, (DictationRecognizer.DictationResultDelegate)Delegate.Combine(dictationResultDelegate2, value), dictationResultDelegate);
				}
				while (dictationResultDelegate != dictationResultDelegate2);
			}
			remove
			{
				DictationRecognizer.DictationResultDelegate dictationResultDelegate = this.DictationResult;
				DictationRecognizer.DictationResultDelegate dictationResultDelegate2;
				do
				{
					dictationResultDelegate2 = dictationResultDelegate;
					dictationResultDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationResultDelegate>(ref this.DictationResult, (DictationRecognizer.DictationResultDelegate)Delegate.Remove(dictationResultDelegate2, value), dictationResultDelegate);
				}
				while (dictationResultDelegate != dictationResultDelegate2);
			}
		}

		public event DictationRecognizer.DictationCompletedDelegate DictationComplete
		{
			add
			{
				DictationRecognizer.DictationCompletedDelegate dictationCompletedDelegate = this.DictationComplete;
				DictationRecognizer.DictationCompletedDelegate dictationCompletedDelegate2;
				do
				{
					dictationCompletedDelegate2 = dictationCompletedDelegate;
					dictationCompletedDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationCompletedDelegate>(ref this.DictationComplete, (DictationRecognizer.DictationCompletedDelegate)Delegate.Combine(dictationCompletedDelegate2, value), dictationCompletedDelegate);
				}
				while (dictationCompletedDelegate != dictationCompletedDelegate2);
			}
			remove
			{
				DictationRecognizer.DictationCompletedDelegate dictationCompletedDelegate = this.DictationComplete;
				DictationRecognizer.DictationCompletedDelegate dictationCompletedDelegate2;
				do
				{
					dictationCompletedDelegate2 = dictationCompletedDelegate;
					dictationCompletedDelegate = Interlocked.CompareExchange<DictationRecognizer.DictationCompletedDelegate>(ref this.DictationComplete, (DictationRecognizer.DictationCompletedDelegate)Delegate.Remove(dictationCompletedDelegate2, value), dictationCompletedDelegate);
				}
				while (dictationCompletedDelegate != dictationCompletedDelegate2);
			}
		}

		public event DictationRecognizer.DictationErrorHandler DictationError
		{
			add
			{
				DictationRecognizer.DictationErrorHandler dictationErrorHandler = this.DictationError;
				DictationRecognizer.DictationErrorHandler dictationErrorHandler2;
				do
				{
					dictationErrorHandler2 = dictationErrorHandler;
					dictationErrorHandler = Interlocked.CompareExchange<DictationRecognizer.DictationErrorHandler>(ref this.DictationError, (DictationRecognizer.DictationErrorHandler)Delegate.Combine(dictationErrorHandler2, value), dictationErrorHandler);
				}
				while (dictationErrorHandler != dictationErrorHandler2);
			}
			remove
			{
				DictationRecognizer.DictationErrorHandler dictationErrorHandler = this.DictationError;
				DictationRecognizer.DictationErrorHandler dictationErrorHandler2;
				do
				{
					dictationErrorHandler2 = dictationErrorHandler;
					dictationErrorHandler = Interlocked.CompareExchange<DictationRecognizer.DictationErrorHandler>(ref this.DictationError, (DictationRecognizer.DictationErrorHandler)Delegate.Remove(dictationErrorHandler2, value), dictationErrorHandler);
				}
				while (dictationErrorHandler != dictationErrorHandler2);
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
				float result;
				if (this.m_Recognizer == IntPtr.Zero)
				{
					result = 0f;
				}
				else
				{
					result = DictationRecognizer.GetAutoSilenceTimeoutSeconds(this.m_Recognizer);
				}
				return result;
			}
			set
			{
				if (!(this.m_Recognizer == IntPtr.Zero))
				{
					DictationRecognizer.SetAutoSilenceTimeoutSeconds(this.m_Recognizer, value);
				}
			}
		}

		public float InitialSilenceTimeoutSeconds
		{
			get
			{
				float result;
				if (this.m_Recognizer == IntPtr.Zero)
				{
					result = 0f;
				}
				else
				{
					result = DictationRecognizer.GetInitialSilenceTimeoutSeconds(this.m_Recognizer);
				}
				return result;
			}
			set
			{
				if (!(this.m_Recognizer == IntPtr.Zero))
				{
					DictationRecognizer.SetInitialSilenceTimeoutSeconds(this.m_Recognizer, value);
				}
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr self);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpeechSystemStatus GetStatus(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);

		[GeneratedByOldBindingsGenerator]
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
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Start(this.m_Recognizer);
			}
		}

		public void Stop()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Stop(this.m_Recognizer);
			}
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
