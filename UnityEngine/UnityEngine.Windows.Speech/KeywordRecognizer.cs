using System;
using System.Collections.Generic;

namespace UnityEngine.Windows.Speech
{
	public sealed class KeywordRecognizer : PhraseRecognizer
	{
		public IEnumerable<string> Keywords
		{
			get;
			private set;
		}

		public KeywordRecognizer(string[] keywords) : this(keywords, ConfidenceLevel.Medium)
		{
		}

		public KeywordRecognizer(string[] keywords, ConfidenceLevel minimumConfidence)
		{
			if (keywords == null)
			{
				throw new ArgumentNullException("keywords");
			}
			if (keywords.Length == 0)
			{
				throw new ArgumentException("At least one keyword must be specified.", "keywords");
			}
			this.Keywords = keywords;
			this.m_Recognizer = base.CreateFromKeywords(keywords, minimumConfidence);
		}
	}
}
