using System;

namespace UnityEngine
{
	public enum AnimatorCullingMode
	{
		AlwaysAnimate,
		CullUpdateTransforms,
		CullCompletely,
		[Obsolete("Enum member AnimatorCullingMode.BasedOnRenderers has been deprecated. Use AnimatorCullingMode.CullUpdateTransforms instead (UnityUpgradable) -> CullUpdateTransforms", true)]
		BasedOnRenderers = 1
	}
}
