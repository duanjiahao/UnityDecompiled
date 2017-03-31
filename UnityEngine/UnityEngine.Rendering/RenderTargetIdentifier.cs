using System;

namespace UnityEngine.Rendering
{
	public struct RenderTargetIdentifier
	{
		private BuiltinRenderTextureType m_Type;

		private int m_NameID;

		private int m_InstanceID;

		public RenderTargetIdentifier(BuiltinRenderTextureType type)
		{
			this.m_Type = type;
			this.m_NameID = -1;
			this.m_InstanceID = 0;
		}

		public RenderTargetIdentifier(string name)
		{
			this.m_Type = BuiltinRenderTextureType.None;
			this.m_NameID = Shader.PropertyToID(name);
			this.m_InstanceID = 0;
		}

		public RenderTargetIdentifier(int nameID)
		{
			this.m_Type = BuiltinRenderTextureType.None;
			this.m_NameID = nameID;
			this.m_InstanceID = 0;
		}

		public RenderTargetIdentifier(Texture tex)
		{
			this.m_Type = ((!(tex == null) && !(tex is RenderTexture)) ? BuiltinRenderTextureType.BindableTexture : BuiltinRenderTextureType.None);
			this.m_NameID = -1;
			this.m_InstanceID = ((!tex) ? 0 : tex.GetInstanceID());
		}

		public static implicit operator RenderTargetIdentifier(BuiltinRenderTextureType type)
		{
			return new RenderTargetIdentifier(type);
		}

		public static implicit operator RenderTargetIdentifier(string name)
		{
			return new RenderTargetIdentifier(name);
		}

		public static implicit operator RenderTargetIdentifier(int nameID)
		{
			return new RenderTargetIdentifier(nameID);
		}

		public static implicit operator RenderTargetIdentifier(Texture tex)
		{
			return new RenderTargetIdentifier(tex);
		}

		public override string ToString()
		{
			return UnityString.Format("Type {0} NameID {1} InstanceID {2}", new object[]
			{
				this.m_Type,
				this.m_NameID,
				this.m_InstanceID
			});
		}

		public override int GetHashCode()
		{
			return (this.m_Type.GetHashCode() * 23 + this.m_NameID.GetHashCode()) * 23 + this.m_InstanceID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (!(obj is RenderTargetIdentifier))
			{
				result = false;
			}
			else
			{
				RenderTargetIdentifier renderTargetIdentifier = (RenderTargetIdentifier)obj;
				result = (this.m_Type == renderTargetIdentifier.m_Type && this.m_NameID == renderTargetIdentifier.m_NameID && this.m_InstanceID == renderTargetIdentifier.m_InstanceID);
			}
			return result;
		}

		public bool Equals(RenderTargetIdentifier rhs)
		{
			return this.m_Type == rhs.m_Type && this.m_NameID == rhs.m_NameID && this.m_InstanceID == rhs.m_InstanceID;
		}

		public static bool operator ==(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs)
		{
			return lhs.m_Type == rhs.m_Type && lhs.m_NameID == rhs.m_NameID && lhs.m_InstanceID == rhs.m_InstanceID;
		}

		public static bool operator !=(RenderTargetIdentifier lhs, RenderTargetIdentifier rhs)
		{
			return lhs.m_Type != rhs.m_Type || lhs.m_NameID != rhs.m_NameID || lhs.m_InstanceID != rhs.m_InstanceID;
		}
	}
}
