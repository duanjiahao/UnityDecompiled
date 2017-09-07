using System;

namespace UnityEngine.Experimental.Rendering
{
	public interface IRenderPipelineAsset
	{
		void DestroyCreatedInstances();

		IRenderPipeline CreatePipeline();

		int GetTerrainBrushPassIndex();
	}
}
