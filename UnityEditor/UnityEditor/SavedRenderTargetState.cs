using System;
using UnityEngine;

namespace UnityEditor
{
	internal class SavedRenderTargetState
	{
		private RenderTexture renderTexture;

		private Rect viewport;

		private Rect scissor;

		internal SavedRenderTargetState()
		{
			GL.PushMatrix();
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				this.renderTexture = RenderTexture.active;
			}
			this.viewport = ShaderUtil.rawViewportRect;
			this.scissor = ShaderUtil.rawScissorRect;
		}

		internal void Restore()
		{
			if (ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				EditorGUIUtility.SetRenderTextureNoViewport(this.renderTexture);
			}
			ShaderUtil.rawViewportRect = this.viewport;
			ShaderUtil.rawScissorRect = this.scissor;
			GL.PopMatrix();
		}
	}
}
