using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Rendering
{
	public abstract class RenderPipelineAsset : ScriptableObject, IRenderPipelineAsset
	{
		private readonly List<IRenderPipeline> m_CreatedPipelines = new List<IRenderPipeline>();

		public void DestroyCreatedInstances()
		{
			foreach (IRenderPipeline current in this.m_CreatedPipelines)
			{
				current.Dispose();
			}
			this.m_CreatedPipelines.Clear();
		}

		public IRenderPipeline CreatePipeline()
		{
			IRenderPipeline renderPipeline = this.InternalCreatePipeline();
			if (renderPipeline != null)
			{
				this.m_CreatedPipelines.Add(renderPipeline);
			}
			return renderPipeline;
		}

		protected abstract IRenderPipeline InternalCreatePipeline();

		private void OnValidate()
		{
			this.DestroyCreatedInstances();
		}

		private void OnDisable()
		{
			this.DestroyCreatedInstances();
		}
	}
}
