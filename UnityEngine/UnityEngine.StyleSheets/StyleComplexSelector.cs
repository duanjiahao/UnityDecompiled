using System;

namespace UnityEngine.StyleSheets
{
	[Serializable]
	internal class StyleComplexSelector
	{
		[SerializeField]
		private int m_Specificity;

		[SerializeField]
		private StyleSelector[] m_Selectors;

		[SerializeField]
		internal int ruleIndex;

		public int specificity
		{
			get
			{
				return this.m_Specificity;
			}
			internal set
			{
				this.m_Specificity = value;
			}
		}

		public StyleRule rule
		{
			get;
			internal set;
		}

		public bool isSimple
		{
			get
			{
				return this.selectors.Length == 1;
			}
		}

		public StyleSelector[] selectors
		{
			get
			{
				return this.m_Selectors;
			}
			internal set
			{
				this.m_Selectors = value;
			}
		}
	}
}
