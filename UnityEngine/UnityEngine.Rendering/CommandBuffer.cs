using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine.Rendering
{
	public sealed class CommandBuffer : IDisposable
	{
		internal IntPtr m_Ptr;
		public extern string name
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int sizeInBytes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public CommandBuffer()
		{
			this.m_Ptr = IntPtr.Zero;
			CommandBuffer.InitBuffer(this);
		}
		~CommandBuffer()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			this.ReleaseBuffer();
			this.m_Ptr = IntPtr.Zero;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitBuffer(CommandBuffer buf);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseBuffer();
		public void Release()
		{
			this.Dispose();
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			CommandBuffer.INTERNAL_CALL_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}
		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
		{
			MaterialPropertyBlock properties = null;
			CommandBuffer.INTERNAL_CALL_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}
		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
		{
			MaterialPropertyBlock properties = null;
			int shaderPass = -1;
			CommandBuffer.INTERNAL_CALL_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}
		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
		{
			MaterialPropertyBlock properties = null;
			int shaderPass = -1;
			int submeshIndex = 0;
			CommandBuffer.INTERNAL_CALL_DrawMesh(this, mesh, ref matrix, material, submeshIndex, shaderPass, properties);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DrawMesh(CommandBuffer self, Mesh mesh, ref Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass);
		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			int shaderPass = -1;
			this.DrawRenderer(renderer, material, submeshIndex, shaderPass);
		}
		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material)
		{
			int shaderPass = -1;
			int submeshIndex = 0;
			this.DrawRenderer(renderer, material, submeshIndex, shaderPass);
		}
		public void SetRenderTarget(RenderTargetIdentifier rt)
		{
			this.SetRenderTarget_Single(ref rt);
		}
		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
		{
			this.SetRenderTarget_ColDepth(ref color, ref depth);
		}
		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth)
		{
			this.SetRenderTarget_Multiple(colors, ref depth);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTarget_Single(ref RenderTargetIdentifier rt);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTarget_ColDepth(ref RenderTargetIdentifier color, ref RenderTargetIdentifier depth);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTarget_Multiple(RenderTargetIdentifier[] color, ref RenderTargetIdentifier depth);
		public void Blit(Texture source, RenderTargetIdentifier dest)
		{
			this.Blit_Texture(source, ref dest, null, -1);
		}
		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Texture(source, ref dest, mat, -1);
		}
		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Texture(source, ref dest, mat, pass);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass);
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest)
		{
			this.Blit_Identifier(ref source, ref dest, null, -1);
		}
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat)
		{
			this.Blit_Identifier(ref source, ref dest, mat, -1);
		}
		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.Blit_Identifier(ref source, ref dest, mat, pass);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass);
		[ExcludeFromDocs]
		private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat)
		{
			int pass = -1;
			this.Blit_Identifier(ref source, ref dest, mat, pass);
		}
		[ExcludeFromDocs]
		private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest)
		{
			int pass = -1;
			Material mat = null;
			this.Blit_Identifier(ref source, ref dest, mat, pass);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetTemporaryRT(int nameID, int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("FilterMode.Point")] FilterMode filter, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing);
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			int antiAliasing = 1;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
		}
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
		}
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
		{
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat format = RenderTextureFormat.Default;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
		}
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
		{
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat format = RenderTextureFormat.Default;
			FilterMode filter = FilterMode.Point;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
		}
		[ExcludeFromDocs]
		public void GetTemporaryRT(int nameID, int width, int height)
		{
			int antiAliasing = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat format = RenderTextureFormat.Default;
			FilterMode filter = FilterMode.Point;
			int depthBuffer = 0;
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ReleaseTemporaryRT(int nameID);
		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, [DefaultValue("1.0f")] float depth)
		{
			CommandBuffer.INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, depth);
		}
		[ExcludeFromDocs]
		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			float depth = 1f;
			CommandBuffer.INTERNAL_CALL_ClearRenderTarget(this, clearDepth, clearColor, ref backgroundColor, depth);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_ClearRenderTarget(CommandBuffer self, bool clearDepth, bool clearColor, ref Color backgroundColor, float depth);
		public void SetGlobalFloat(string name, float value)
		{
			this.SetGlobalFloat(Shader.PropertyToID(name), value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalFloat(int nameID, float value);
		public void SetGlobalVector(string name, Vector4 value)
		{
			this.SetGlobalVector(Shader.PropertyToID(name), value);
		}
		public void SetGlobalVector(int nameID, Vector4 value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalVector(this, nameID, ref value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalVector(CommandBuffer self, int nameID, ref Vector4 value);
		public void SetGlobalColor(string name, Color value)
		{
			this.SetGlobalColor(Shader.PropertyToID(name), value);
		}
		public void SetGlobalColor(int nameID, Color value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalColor(this, nameID, ref value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalColor(CommandBuffer self, int nameID, ref Color value);
		public void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			this.SetGlobalMatrix(Shader.PropertyToID(name), value);
		}
		public void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			CommandBuffer.INTERNAL_CALL_SetGlobalMatrix(this, nameID, ref value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalMatrix(CommandBuffer self, int nameID, ref Matrix4x4 value);
		public void SetGlobalTexture(string name, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture(Shader.PropertyToID(name), value);
		}
		public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture_Impl(nameID, ref value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalTexture_Impl(int nameID, ref RenderTargetIdentifier rt);
	}
}
