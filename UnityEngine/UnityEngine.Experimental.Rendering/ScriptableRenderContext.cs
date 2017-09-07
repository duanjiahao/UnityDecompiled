using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	public struct ScriptableRenderContext
	{
		private IntPtr m_Ptr;

		internal ScriptableRenderContext(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public void Submit()
		{
			this.CheckValid();
			this.Submit_Internal();
		}

		public void DrawRenderers(ref DrawRendererSettings settings)
		{
			this.CheckValid();
			this.DrawRenderers_Internal(ref settings);
		}

		public void DrawShadows(ref DrawShadowsSettings settings)
		{
			this.CheckValid();
			this.DrawShadows_Internal(ref settings);
		}

		public void ExecuteCommandBuffer(CommandBuffer commandBuffer)
		{
			this.CheckValid();
			this.ExecuteCommandBuffer_Internal(commandBuffer);
		}

		public void SetupCameraProperties(Camera camera)
		{
			this.CheckValid();
			this.SetupCameraProperties_Internal(camera);
		}

		public void DrawSkybox(Camera camera)
		{
			this.CheckValid();
			this.DrawSkybox_Internal(camera);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Submit_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawRenderers_Internal(ref DrawRendererSettings settings);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawShadows_Internal(ref DrawShadowsSettings settings);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ExecuteCommandBuffer_Internal(CommandBuffer commandBuffer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetupCameraProperties_Internal(Camera camera);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawSkybox_Internal(Camera camera);

		internal void CheckValid()
		{
			if (this.m_Ptr.ToInt64() == 0L)
			{
				throw new ArgumentException("Invalid ScriptableRenderContext.  This can be caused by allocating a context in user code.");
			}
		}
	}
}
