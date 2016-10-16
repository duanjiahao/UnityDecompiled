using System;

namespace UnityEngine
{
	[Flags]
	internal enum TerrainChangedFlags
	{
		NoChange = 0,
		Heightmap = 1,
		TreeInstances = 2,
		DelayedHeightmapUpdate = 4,
		FlushEverythingImmediately = 8,
		RemoveDirtyDetailsImmediately = 16,
		WillBeDestroyed = 256
	}
}
