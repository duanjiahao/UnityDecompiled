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
	}
}
