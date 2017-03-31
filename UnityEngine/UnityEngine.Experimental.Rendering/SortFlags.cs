using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum SortFlags
	{
		None = 0,
		SortingLayer = 1,
		RenderQueue = 2,
		BackToFront = 4,
		QuantizedFrontToBack = 8,
		OptimizeStateChanges = 16,
		CanvasOrder = 32,
		CommonOpaque = 59,
		CommonTransparent = 23
	}
}
