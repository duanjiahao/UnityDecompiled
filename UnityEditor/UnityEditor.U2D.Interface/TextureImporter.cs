using System;
using UnityEngine;

namespace UnityEditor.U2D.Interface
{
	internal class TextureImporter : ITextureImporter
	{
		protected AssetImporter m_AssetImporter;

		public override string assetPath
		{
			get
			{
				return this.m_AssetImporter.assetPath;
			}
		}

		public override SpriteImportMode spriteImportMode
		{
			get
			{
				return ((UnityEditor.TextureImporter)this.m_AssetImporter).spriteImportMode;
			}
		}

		public override Vector4 spriteBorder
		{
			get
			{
				return ((UnityEditor.TextureImporter)this.m_AssetImporter).spriteBorder;
			}
		}

		public override Vector2 spritePivot
		{
			get
			{
				return ((UnityEditor.TextureImporter)this.m_AssetImporter).spritePivot;
			}
		}

		public TextureImporter(UnityEditor.TextureImporter textureImporter)
		{
			this.m_AssetImporter = textureImporter;
		}

		public override bool Equals(object other)
		{
			TextureImporter textureImporter = other as TextureImporter;
			bool result;
			if (object.ReferenceEquals(textureImporter, null))
			{
				result = (this.m_AssetImporter == null);
			}
			else
			{
				result = (this.m_AssetImporter == textureImporter.m_AssetImporter);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.m_AssetImporter.GetHashCode();
		}

		public override void GetWidthAndHeight(ref int width, ref int height)
		{
			((UnityEditor.TextureImporter)this.m_AssetImporter).GetWidthAndHeight(ref width, ref height);
		}
	}
}
