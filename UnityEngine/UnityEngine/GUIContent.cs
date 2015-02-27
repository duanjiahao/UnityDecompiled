using System;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class GUIContent
	{
		[SerializeField]
		private string m_Text = string.Empty;
		[SerializeField]
		private Texture m_Image;
		[SerializeField]
		private string m_Tooltip = string.Empty;
		public static GUIContent none = new GUIContent(string.Empty);
		private static GUIContent s_Text = new GUIContent();
		private static GUIContent s_Image = new GUIContent();
		private static GUIContent s_TextImage = new GUIContent();
		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				this.m_Text = value;
			}
		}
		public Texture image
		{
			get
			{
				return this.m_Image;
			}
			set
			{
				this.m_Image = value;
			}
		}
		public string tooltip
		{
			get
			{
				return this.m_Tooltip;
			}
			set
			{
				this.m_Tooltip = value;
			}
		}
		internal int hash
		{
			get
			{
				int result = 0;
				if (this.m_Text != null && this.m_Text != string.Empty)
				{
					result = this.m_Text.GetHashCode() * 37;
				}
				return result;
			}
		}
		public GUIContent()
		{
		}
		public GUIContent(string text)
		{
			this.m_Text = text;
		}
		public GUIContent(Texture image)
		{
			this.m_Image = image;
		}
		public GUIContent(string text, Texture image)
		{
			this.m_Text = text;
			this.m_Image = image;
		}
		public GUIContent(string text, string tooltip)
		{
			this.m_Text = text;
			this.m_Tooltip = tooltip;
		}
		public GUIContent(Texture image, string tooltip)
		{
			this.m_Image = image;
			this.m_Tooltip = tooltip;
		}
		public GUIContent(string text, Texture image, string tooltip)
		{
			this.m_Text = text;
			this.m_Image = image;
			this.m_Tooltip = tooltip;
		}
		public GUIContent(GUIContent src)
		{
			this.m_Text = src.m_Text;
			this.m_Image = src.m_Image;
			this.m_Tooltip = src.m_Tooltip;
		}
		internal static GUIContent Temp(string t)
		{
			GUIContent.s_Text.m_Text = t;
			return GUIContent.s_Text;
		}
		internal static GUIContent Temp(Texture i)
		{
			GUIContent.s_Image.m_Image = i;
			return GUIContent.s_Image;
		}
		internal static GUIContent Temp(string t, Texture i)
		{
			GUIContent.s_TextImage.m_Text = t;
			GUIContent.s_TextImage.m_Image = i;
			return GUIContent.s_TextImage;
		}
		internal static void ClearStaticCache()
		{
			GUIContent.s_Text.m_Text = null;
			GUIContent.s_Image.m_Image = null;
			GUIContent.s_TextImage.m_Text = null;
			GUIContent.s_TextImage.m_Image = null;
		}
		internal static GUIContent[] Temp(string[] texts)
		{
			GUIContent[] array = new GUIContent[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				array[i] = new GUIContent(texts[i]);
			}
			return array;
		}
		internal static GUIContent[] Temp(Texture[] images)
		{
			GUIContent[] array = new GUIContent[images.Length];
			for (int i = 0; i < images.Length; i++)
			{
				array[i] = new GUIContent(images[i]);
			}
			return array;
		}
	}
}
