using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum DrawRendererFlags
	{
		None = 0,
		EnableDynamicBatching = 1,
		EnableInstancing = 2
	}
}
