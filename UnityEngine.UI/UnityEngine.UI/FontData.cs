using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[Serializable]
	public class FontData : ISerializationCallbackReceiver
	{
		[FormerlySerializedAs("font"), SerializeField]
		private Font m_Font;

		[FormerlySerializedAs("fontSize"), SerializeField]
		private int m_FontSize;

		[FormerlySerializedAs("fontStyle"), SerializeField]
		private FontStyle m_FontStyle;

		[SerializeField]
		private bool m_BestFit;

		[SerializeField]
		private int m_MinSize;

		[SerializeField]
		private int m_MaxSize;

		[FormerlySerializedAs("alignment"), SerializeField]
		private TextAnchor m_Alignment;

		[SerializeField]
		private bool m_AlignByGeometry;

		[FormerlySerializedAs("richText"), SerializeField]
		private bool m_RichText;

		[SerializeField]
		private HorizontalWrapMode m_HorizontalOverflow;

		[SerializeField]
		private VerticalWrapMode m_VerticalOverflow;

		[SerializeField]
		private float m_LineSpacing;

		public static FontData defaultFontData
		{
			get
			{
				return new FontData
				{
					m_FontSize = 14,
					m_LineSpacing = 1f,
					m_FontStyle = FontStyle.Normal,
					m_BestFit = false,
					m_MinSize = 10,
					m_MaxSize = 40,
					m_Alignment = TextAnchor.UpperLeft,
					m_HorizontalOverflow = HorizontalWrapMode.Wrap,
					m_VerticalOverflow = VerticalWrapMode.Truncate,
					m_RichText = true,
					m_AlignByGeometry = false
				};
			}
		}

		public Font font
		{
			get
			{
				return this.m_Font;
			}
			set
			{
				this.m_Font = value;
			}
		}

		public int fontSize
		{
			get
			{
				return this.m_FontSize;
			}
			set
			{
				this.m_FontSize = value;
			}
		}

		public FontStyle fontStyle
		{
			get
			{
				return this.m_FontStyle;
			}
			set
			{
				this.m_FontStyle = value;
			}
		}

		public bool bestFit
		{
			get
			{
				return this.m_BestFit;
			}
			set
			{
				this.m_BestFit = value;
			}
		}

		public int minSize
		{
			get
			{
				return this.m_MinSize;
			}
			set
			{
				this.m_MinSize = value;
			}
		}

		public int maxSize
		{
			get
			{
				return this.m_MaxSize;
			}
			set
			{
				this.m_MaxSize = value;
			}
		}

		public TextAnchor alignment
		{
			get
			{
				return this.m_Alignment;
			}
			set
			{
				this.m_Alignment = value;
			}
		}

		public bool alignByGeometry
		{
			get
			{
				return this.m_AlignByGeometry;
			}
			set
			{
				this.m_AlignByGeometry = value;
			}
		}

		public bool richText
		{
			get
			{
				return this.m_RichText;
			}
			set
			{
				this.m_RichText = value;
			}
		}

		public HorizontalWrapMode horizontalOverflow
		{
			get
			{
				return this.m_HorizontalOverflow;
			}
			set
			{
				this.m_HorizontalOverflow = value;
			}
		}

		public VerticalWrapMode verticalOverflow
		{
			get
			{
				return this.m_VerticalOverflow;
			}
			set
			{
				this.m_VerticalOverflow = value;
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_LineSpacing;
			}
			set
			{
				this.m_LineSpacing = value;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_FontSize = Mathf.Clamp(this.m_FontSize, 0, 300);
			this.m_MinSize = Mathf.Clamp(this.m_MinSize, 0, this.m_FontSize);
			this.m_MaxSize = Mathf.Clamp(this.m_MaxSize, this.m_FontSize, 300);
		}
	}
}
