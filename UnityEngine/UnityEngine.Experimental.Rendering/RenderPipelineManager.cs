using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	public static class RenderPipelineManager
	{
		private static IRenderPipelineAsset s_CurrentPipelineAsset;

		public static IRenderPipeline currentPipeline
		{
			get;
			private set;
		}

		[RequiredByNativeCode]
		internal static void CleanupRenderPipeline()
		{
			if (RenderPipelineManager.s_CurrentPipelineAsset != null)
			{
				RenderPipelineManager.s_CurrentPipelineAsset.DestroyCreatedInstances();
			}
			RenderPipelineManager.s_CurrentPipelineAsset = null;
			RenderPipelineManager.currentPipeline = null;
		}

		[RequiredByNativeCode]
		private static void DoRenderLoop_Internal(IRenderPipelineAsset pipe, Camera[] cameras, IntPtr loopPtr)
		{
			RenderPipelineManager.PrepareRenderPipeline(pipe);
			if (RenderPipelineManager.currentPipeline != null)
			{
				ScriptableRenderContext renderContext = new ScriptableRenderContext(loopPtr);
				RenderPipelineManager.currentPipeline.Render(renderContext, cameras);
			}
		}

		private static void PrepareRenderPipeline(IRenderPipelineAsset pipe)
		{
			if (RenderPipelineManager.s_CurrentPipelineAsset != pipe)
			{
				if (RenderPipelineManager.s_CurrentPipelineAsset != null)
				{
					RenderPipelineManager.CleanupRenderPipeline();
				}
				RenderPipelineManager.s_CurrentPipelineAsset = pipe;
			}
			if (RenderPipelineManager.s_CurrentPipelineAsset != null && (RenderPipelineManager.currentPipeline == null || RenderPipelineManager.currentPipeline.disposed))
			{
				RenderPipelineManager.currentPipeline = RenderPipelineManager.s_CurrentPipelineAsset.CreatePipeline();
			}
		}
	}
}
