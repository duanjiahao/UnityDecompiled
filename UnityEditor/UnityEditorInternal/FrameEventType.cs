using System;

namespace UnityEditorInternal
{
	internal enum FrameEventType
	{
		ClearNone,
		ClearColor,
		ClearDepth,
		ClearColorDepth,
		ClearStencil,
		ClearColorStencil,
		ClearDepthStencil,
		ClearAll,
		SetRenderTarget,
		ResolveRT,
		ResolveDepth,
		GrabIntoRT,
		StaticBatch,
		DynamicBatch,
		Mesh,
		DynamicGeometry,
		GLDraw,
		SkinOnGPU,
		DrawProcedural,
		ComputeDispatch,
		PluginEvent,
		InstancedMesh
	}
}
