using System;

namespace UnityEngine.Experimental.Rendering
{
	public interface IRenderPipeline : IDisposable
	{
		bool disposed
		{
			get;
		}

		void Render(ScriptableRenderContext renderContext, Camera[] cameras);
	}
}
