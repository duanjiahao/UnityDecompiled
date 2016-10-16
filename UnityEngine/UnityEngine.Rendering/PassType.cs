using System;

namespace UnityEngine.Rendering
{
	public enum PassType
	{
		Normal,
		Vertex,
		VertexLM,
		VertexLMRGBM,
		ForwardBase,
		ForwardAdd,
		LightPrePassBase,
		LightPrePassFinal,
		ShadowCaster,
		Deferred = 10,
		Meta,
		MotionVectors
	}
}
