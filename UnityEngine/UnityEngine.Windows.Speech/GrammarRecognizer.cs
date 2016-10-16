using System;

namespace UnityEngine.Windows.Speech
{
	public sealed class GrammarRecognizer : PhraseRecognizer
	{
		public string GrammarFilePath
		{
			get;
			private set;
		}

		public GrammarRecognizer(string grammarFilePath) : this(grammarFilePath, ConfidenceLevel.Medium)
		{
		}

		public GrammarRecognizer(string grammarFilePath, ConfidenceLevel minimumConfidence)
		{
			this.GrammarFilePath = grammarFilePath;
			this.m_Recognizer = base.CreateFromGrammarFile(grammarFilePath, minimumConfidence);
		}
	}
}
