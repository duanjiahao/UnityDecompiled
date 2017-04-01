using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	internal static class RenderPipelineManager
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
		private static bool DoRenderLoop_Internal(IRenderPipelineAsset pipe, Camera[] cameras, IntPtr loopPtr)
		{
			bool result;
			if (!RenderPipelineManager.PrepareRenderPipeline(pipe))
			{
				result = false;
			}
			else
			{
				ScriptableRenderContext renderContext = default(ScriptableRenderContext);
				renderContext.Initialize(loopPtr);
				RenderPipelineManager.currentPipeline.Render(renderContext, cameras);
				result = true;
			}
			return result;
		}

		private static bool PrepareRenderPipeline(IRenderPipelineAsset pipe)
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
			return RenderPipelineManager.s_CurrentPipelineAsset != null;
		}
	}
}
