using System;
namespace UnityEngine
{
	public enum AnimatorCullingMode
	{
		AlwaysAnimate,
		CullUpdateTransforms,
		CullCompletely,
		BasedOnRenderers = 1
	}
}
