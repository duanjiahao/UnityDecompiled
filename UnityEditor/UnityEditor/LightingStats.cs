using System;

namespace UnityEditor
{
	internal struct LightingStats
	{
		public uint realtimeLightsCount;

		public uint dynamicMeshesCount;

		public uint mixedLightsCount;

		public uint bakedLightsCount;

		public uint staticMeshesCount;

		public uint staticMeshesRealtimeEmissive;

		public uint staticMeshesBakedEmissive;

		public uint lightProbeGroupsCount;

		public uint reflectionProbesCount;

		internal void Reset()
		{
			this.realtimeLightsCount = 0u;
			this.dynamicMeshesCount = 0u;
			this.mixedLightsCount = 0u;
			this.bakedLightsCount = 0u;
			this.staticMeshesCount = 0u;
			this.staticMeshesRealtimeEmissive = 0u;
			this.staticMeshesBakedEmissive = 0u;
			this.lightProbeGroupsCount = 0u;
			this.reflectionProbesCount = 0u;
		}
	}
}
