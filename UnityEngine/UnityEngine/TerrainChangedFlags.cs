using System;

namespace UnityEngine
{
	[Flags]
	public enum TerrainChangedFlags
	{
		Heightmap = 1,
		TreeInstances = 2,
		DelayedHeightmapUpdate = 4,
		FlushEverythingImmediately = 8,
		RemoveDirtyDetailsImmediately = 16,
		WillBeDestroyed = 256
	}
}
