using System;

namespace UnityEditor
{
	internal struct LODVisualizationInformation
	{
		public int triangleCount;

		public int vertexCount;

		public int rendererCount;

		public int submeshCount;

		public int activeLODLevel;

		public float activeLODFade;

		public float activeDistance;

		public float activeRelativeScreenSize;

		public float activePixelSize;

		public float worldSpaceSize;
	}
}
