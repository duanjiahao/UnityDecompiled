using System;

namespace UnityEngine.U2D.Interface
{
	internal class Texture2D : ITexture2D
	{
		private UnityEngine.Texture2D m_Texture;

		public override int width
		{
			get
			{
				return this.m_Texture.width;
			}
		}

		public override int height
		{
			get
			{
				return this.m_Texture.height;
			}
		}

		public override TextureFormat format
		{
			get
			{
				return this.m_Texture.format;
			}
		}

		public override FilterMode filterMode
		{
			get
			{
				return this.m_Texture.filterMode;
			}
			set
			{
				this.m_Texture.filterMode = value;
			}
		}

		public override float mipMapBias
		{
			get
			{
				return this.m_Texture.mipMapBias;
			}
		}

		public override string name
		{
			get
			{
				return this.m_Texture.name;
			}
		}

		public Texture2D(UnityEngine.Texture2D texture)
		{
			this.m_Texture = texture;
		}

		public override Color32[] GetPixels32()
		{
			return this.m_Texture.GetPixels32();
		}

		public override bool Equals(object other)
		{
			Texture2D texture2D = other as Texture2D;
			bool result;
			if (object.ReferenceEquals(texture2D, null))
			{
				result = (this.m_Texture == null);
			}
			else
			{
				result = (this.m_Texture == texture2D.m_Texture);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.m_Texture.GetHashCode();
		}

		public override void SetPixels(Color[] c)
		{
			this.m_Texture.SetPixels(c);
		}

		public override void Apply()
		{
			this.m_Texture.Apply();
		}

		protected override UnityEngine.Object ToUnityObject()
		{
			return this.m_Texture;
		}

		protected override UnityEngine.Texture2D ToUnityTexture()
		{
			return this.m_Texture;
		}
	}
}
