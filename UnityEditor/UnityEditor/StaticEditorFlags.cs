using System;

namespace UnityEditor
{
	[Flags]
	public enum StaticEditorFlags
	{
		LightmapStatic = 1,
		OccluderStatic = 2,
		OccludeeStatic = 16,
		BatchingStatic = 4,
		NavigationStatic = 8,
		OffMeshLinkGeneration = 32,
		ReflectionProbeStatic = 64
	}
}
