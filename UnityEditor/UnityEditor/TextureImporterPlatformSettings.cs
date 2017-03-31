using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[Serializable]
	public sealed class TextureImporterPlatformSettings
	{
		[SerializeField]
		private string m_Name = TextureImporterInspector.s_DefaultPlatformName;

		[SerializeField]
		private int m_Overridden = 0;

		[SerializeField]
		private int m_MaxTextureSize = 2048;

		[SerializeField]
		private int m_TextureFormat = -1;

		[SerializeField]
		private int m_TextureCompression = 1;

		[SerializeField]
		private int m_CompressionQuality = 50;

		[SerializeField]
		private int m_CrunchedCompression = 0;

		[SerializeField]
		private int m_AllowsAlphaSplitting = 0;

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public bool overridden
		{
			get
			{
				return this.m_Overridden != 0;
			}
			set
			{
				this.m_Overridden = ((!value) ? 0 : 1);
			}
		}

		public int maxTextureSize
		{
			get
			{
				return this.m_MaxTextureSize;
			}
			set
			{
				this.m_MaxTextureSize = value;
			}
		}

		public TextureImporterFormat format
		{
			get
			{
				return (TextureImporterFormat)this.m_TextureFormat;
			}
			set
			{
				this.m_TextureFormat = (int)value;
			}
		}

		public TextureImporterCompression textureCompression
		{
			get
			{
				return (TextureImporterCompression)this.m_TextureCompression;
			}
			set
			{
				this.m_TextureCompression = (int)value;
			}
		}

		public int compressionQuality
		{
			get
			{
				return this.m_CompressionQuality;
			}
			set
			{
				this.m_CompressionQuality = value;
			}
		}

		public bool crunchedCompression
		{
			get
			{
				return this.m_CrunchedCompression != 0;
			}
			set
			{
				this.m_CrunchedCompression = ((!value) ? 0 : 1);
			}
		}

		public bool allowsAlphaSplitting
		{
			get
			{
				return this.m_AllowsAlphaSplitting != 0;
			}
			set
			{
				this.m_AllowsAlphaSplitting = ((!value) ? 0 : 1);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyTo(TextureImporterPlatformSettings target);
	}
}
