using System;

namespace UnityEngine.U2D.Interface
{
	internal abstract class ITexture2D
	{
		public abstract int width
		{
			get;
		}

		public abstract int height
		{
			get;
		}

		public abstract TextureFormat format
		{
			get;
		}

		public abstract FilterMode filterMode
		{
			get;
			set;
		}

		public abstract string name
		{
			get;
		}

		public abstract float mipMapBias
		{
			get;
		}

		public abstract Color32[] GetPixels32();

		public abstract void SetPixels(Color[] c);

		public abstract void Apply();

		public static bool operator ==(ITexture2D t1, ITexture2D t2)
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

		public static bool operator !=(ITexture2D t1, ITexture2D t2)
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

		public static implicit operator UnityEngine.Object(ITexture2D t)
		{
			return (!object.ReferenceEquals(t, null)) ? t.ToUnityObject() : null;
		}

		public static implicit operator UnityEngine.Texture2D(ITexture2D t)
		{
			return (!object.ReferenceEquals(t, null)) ? t.ToUnityTexture() : null;
		}

		protected abstract UnityEngine.Object ToUnityObject();

		protected abstract UnityEngine.Texture2D ToUnityTexture();
	}
}
