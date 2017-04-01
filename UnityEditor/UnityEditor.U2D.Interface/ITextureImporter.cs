using System;
using UnityEngine;

namespace UnityEditor.U2D.Interface
{
	internal abstract class ITextureImporter
	{
		public abstract SpriteImportMode spriteImportMode
		{
			get;
		}

		public abstract Vector4 spriteBorder
		{
			get;
		}

		public abstract Vector2 spritePivot
		{
			get;
		}

		public abstract string assetPath
		{
			get;
		}

		public abstract void GetWidthAndHeight(ref int width, ref int height);

		public static bool operator ==(ITextureImporter t1, ITextureImporter t2)
		{
			bool result;
			if (object.ReferenceEquals(t1, null))
			{
				result = (object.ReferenceEquals(t2, null) || t2 == null);
			}
			else
			{
				result = t1.Equals(t2);
			}
			return result;
		}

		public static bool operator !=(ITextureImporter t1, ITextureImporter t2)
		{
			bool result;
			if (object.ReferenceEquals(t1, null))
			{
				result = (!object.ReferenceEquals(t2, null) && t2 != null);
			}
			else
			{
				result = !t1.Equals(t2);
			}
			return result;
		}

		public override bool Equals(object other)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
