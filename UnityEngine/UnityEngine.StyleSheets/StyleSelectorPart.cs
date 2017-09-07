using System;

namespace UnityEngine.StyleSheets
{
	[Serializable]
	internal struct StyleSelectorPart
	{
		[SerializeField]
		private string m_Value;

		[SerializeField]
		private StyleSelectorType m_Type;

		public string value
		{
			get
			{
				return this.m_Value;
			}
			internal set
			{
				this.m_Value = value;
			}
		}

		public StyleSelectorType type
		{
			get
			{
				return this.m_Type;
			}
			internal set
			{
				this.m_Type = value;
			}
		}

		public override string ToString()
		{
			return string.Format("[StyleSelectorPart: value={0}, type={1}]", this.value, this.type);
		}
	}
}
