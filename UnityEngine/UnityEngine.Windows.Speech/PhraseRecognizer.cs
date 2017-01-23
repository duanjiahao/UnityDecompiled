using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public abstract class PhraseRecognizer : IDisposable
	{
		public delegate void PhraseRecognizedDelegate(PhraseRecognizedEventArgs args);

		protected IntPtr m_Recognizer;

		public event PhraseRecognizer.PhraseRecognizedDelegate OnPhraseRecognized
		{
			add
			{
				PhraseRecognizer.PhraseRecognizedDelegate phraseRecognizedDelegate = this.OnPhraseRecognized;
				PhraseRecognizer.PhraseRecognizedDelegate phraseRecognizedDelegate2;
				do
				{
					phraseRecognizedDelegate2 = phraseRecognizedDelegate;
					phraseRecognizedDelegate = Interlocked.CompareExchange<PhraseRecognizer.PhraseRecognizedDelegate>(ref this.OnPhraseRecognized, (PhraseRecognizer.PhraseRecognizedDelegate)Delegate.Combine(phraseRecognizedDelegate2, value), phraseRecognizedDelegate);
				}
				while (phraseRecognizedDelegate != phraseRecognizedDelegate2);
			}
			remove
			{
				PhraseRecognizer.PhraseRecognizedDelegate phraseRecognizedDelegate = this.OnPhraseRecognized;
				PhraseRecognizer.PhraseRecognizedDelegate phraseRecognizedDelegate2;
				do
				{
					phraseRecognizedDelegate2 = phraseRecognizedDelegate;
					phraseRecognizedDelegate = Interlocked.CompareExchange<PhraseRecognizer.PhraseRecognizedDelegate>(ref this.OnPhraseRecognized, (PhraseRecognizer.PhraseRecognizedDelegate)Delegate.Remove(phraseRecognizedDelegate2, value), phraseRecognizedDelegate);
				}
				while (phraseRecognizedDelegate != phraseRecognizedDelegate2);
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.m_Recognizer != IntPtr.Zero && PhraseRecognizer.IsRunning_Internal(this.m_Recognizer);
			}
		}

		internal PhraseRecognizer()
		{
		}

		protected IntPtr CreateFromKeywords(string[] keywords, ConfidenceLevel minimumConfidence)
		{
			IntPtr result;
			PhraseRecognizer.INTERNAL_CALL_CreateFromKeywords(this, keywords, minimumConfidence, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateFromKeywords(PhraseRecognizer self, string[] keywords, ConfidenceLevel minimumConfidence, out IntPtr value);

		protected IntPtr CreateFromGrammarFile(string grammarFilePath, ConfidenceLevel minimumConfidence)
		{
			IntPtr result;
			PhraseRecognizer.INTERNAL_CALL_CreateFromGrammarFile(this, grammarFilePath, minimumConfidence, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateFromGrammarFile(PhraseRecognizer self, string grammarFilePath, ConfidenceLevel minimumConfidence, out IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start_Internal(IntPtr recognizer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Internal(IntPtr recognizer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRunning_Internal(IntPtr recognizer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr recognizer);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr recognizer);

		~PhraseRecognizer()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				PhraseRecognizer.DestroyThreaded(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public void Start()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				PhraseRecognizer.Start_Internal(this.m_Recognizer);
			}
		}

		public void Stop()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				PhraseRecognizer.Stop_Internal(this.m_Recognizer);
			}
		}

		public void Dispose()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				PhraseRecognizer.Destroy(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[RequiredByNativeCode]
		private void InvokePhraseRecognizedEvent(string text, ConfidenceLevel confidence, SemanticMeaning[] semanticMeanings, long phraseStartFileTime, long phraseDurationTicks)
		{
			PhraseRecognizer.PhraseRecognizedDelegate onPhraseRecognized = this.OnPhraseRecognized;
			if (onPhraseRecognized != null)
			{
				onPhraseRecognized(new PhraseRecognizedEventArgs(text, confidence, semanticMeanings, DateTime.FromFileTime(phraseStartFileTime), TimeSpan.FromTicks(phraseDurationTicks)));
			}
		}

		[RequiredByNativeCode]
		private unsafe static SemanticMeaning[] MarshalSemanticMeaning(IntPtr keys, IntPtr values, IntPtr valueSizes, int valueCount)
		{
			SemanticMeaning[] array = new SemanticMeaning[valueCount];
			int num = 0;
			for (int i = 0; i < valueCount; i++)
			{
				uint num2 = *(uint*)((byte*)((void*)valueSizes) + (IntPtr)i * 4);
				SemanticMeaning semanticMeaning = new SemanticMeaning
				{
					key = new string(*(IntPtr*)((byte*)((void*)keys) + (IntPtr)i * (IntPtr)sizeof(char*))),
					values = new string[num2]
				};
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num2))
				{
					semanticMeaning.values[num3] = new string(*(IntPtr*)((byte*)((void*)values) + (IntPtr)(num + num3) * (IntPtr)sizeof(char*)));
					num3++;
				}
				array[i] = semanticMeaning;
				num += (int)num2;
			}
			return array;
		}
	}
}
