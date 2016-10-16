using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class GameViewSize
	{
		private const int kMaxBaseTextLength = 40;

		private const int kMinResolution = 10;

		private const int kMinAspect = 0;

		private const int kMaxResolutionOrAspect = 99999;

		[SerializeField]
		private string m_BaseText;

		[SerializeField]
		private GameViewSizeType m_SizeType;

		[SerializeField]
		private int m_Width;

		[SerializeField]
		private int m_Height;

		[NonSerialized]
		private string m_CachedDisplayText;

		public string baseText
		{
			get
			{
				return this.m_BaseText;
			}
			set
			{
				this.m_BaseText = value;
				if (this.m_BaseText.Length > 40)
				{
					this.m_BaseText = this.m_BaseText.Substring(0, 40);
				}
				this.Changed();
			}
		}

		public GameViewSizeType sizeType
		{
			get
			{
				return this.m_SizeType;
			}
			set
			{
				this.m_SizeType = value;
				this.Clamp();
				this.Changed();
			}
		}

		public int width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				this.m_Width = value;
				this.Clamp();
				this.Changed();
			}
		}

		public int height
		{
			get
			{
				return this.m_Height;
			}
			set
			{
				this.m_Height = value;
				this.Clamp();
				this.Changed();
			}
		}

		public bool isFreeAspectRatio
		{
			get
			{
				return this.width == 0;
			}
		}

		public float aspectRatio
		{
			get
			{
				if (this.height == 0)
				{
					return 0f;
				}
				return (float)this.width / (float)this.height;
			}
		}

		public string displayText
		{
			get
			{
				string arg_1C_0;
				if ((arg_1C_0 = this.m_CachedDisplayText) == null)
				{
					arg_1C_0 = (this.m_CachedDisplayText = this.ComposeDisplayString());
				}
				return arg_1C_0;
			}
		}

		private string sizeText
		{
			get
			{
				if (this.sizeType == GameViewSizeType.AspectRatio)
				{
					return string.Format("{0}:{1}", this.width, this.height);
				}
				if (this.sizeType == GameViewSizeType.FixedResolution)
				{
					return string.Format("{0}x{1}", this.width, this.height);
				}
				Debug.LogError("Unhandled game view size type");
				return string.Empty;
			}
		}

		public GameViewSize(GameViewSizeType type, int width, int height, string baseText)
		{
			this.sizeType = type;
			this.width = width;
			this.height = height;
			this.baseText = baseText;
		}

		public GameViewSize(GameViewSize other)
		{
			this.Set(other);
		}

		private void Clamp()
		{
			int width = this.m_Width;
			int height = this.m_Height;
			int min = 0;
			GameViewSizeType sizeType = this.sizeType;
			if (sizeType != GameViewSizeType.AspectRatio)
			{
				if (sizeType != GameViewSizeType.FixedResolution)
				{
					Debug.LogError("Unhandled enum!");
				}
				else
				{
					min = 10;
				}
			}
			else
			{
				min = 0;
			}
			this.m_Width = Mathf.Clamp(this.m_Width, min, 99999);
			this.m_Height = Mathf.Clamp(this.m_Height, min, 99999);
			if (this.m_Width != width || this.m_Height != height)
			{
				this.Changed();
			}
		}

		public void Set(GameViewSize other)
		{
			this.sizeType = other.sizeType;
			this.width = other.width;
			this.height = other.height;
			this.baseText = other.baseText;
			this.Changed();
		}

		private string ComposeDisplayString()
		{
			if (this.width == 0 && this.height == 0)
			{
				return this.baseText;
			}
			if (string.IsNullOrEmpty(this.baseText))
			{
				return this.sizeText;
			}
			return this.baseText + " (" + this.sizeText + ")";
		}

		private void Changed()
		{
			this.m_CachedDisplayText = null;
			ScriptableSingleton<GameViewSizes>.instance.Changed();
		}
	}
}
