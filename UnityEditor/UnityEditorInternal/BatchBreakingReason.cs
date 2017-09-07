using System;

namespace UnityEditorInternal
{
	public enum BatchBreakingReason
	{
		NoBreaking,
		NotCoplanarWithCanvas,
		CanvasInjectionIndex,
		DifferentMaterialInstance = 4,
		DifferentRectClipping = 8,
		DifferentTexture = 16,
		DifferentA8TextureUsage = 32,
		DifferentClipRect = 64,
		Unknown = 128
	}
}
