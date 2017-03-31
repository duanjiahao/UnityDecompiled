using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[UsedByNativeCode]
	internal struct LightmapConvergence
	{
		public Hash128 cullingHash;

		public int visibleConvergedDirectTexelCount;

		public int visibleConvergedGITexelCount;

		public int visibleTexelCount;

		public int convergedDirectTexelCount;

		public int convergedGITexelCount;

		public int occupiedTexelCount;

		public int minDirectSamples;

		public int minGISamples;

		public int maxDirectSamples;

		public int maxGISamples;

		public int avgDirectSamples;

		public int avgGISamples;

		public float progress;

		public bool IsConverged()
		{
			return this.convergedDirectTexelCount == this.occupiedTexelCount && this.convergedGITexelCount == this.occupiedTexelCount;
		}

		public bool IsValid()
		{
			return -1 != this.visibleConvergedDirectTexelCount;
		}
	}
}
