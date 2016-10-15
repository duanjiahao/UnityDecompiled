using System;

namespace UnityEngine
{
	internal class MeshSubsetCombineUtility
	{
		public struct MeshInstance
		{
			public int meshInstanceID;

			public int rendererInstanceID;

			public int additionalVertexStreamsMeshInstanceID;

			public Matrix4x4 transform;

			public Vector4 lightmapScaleOffset;

			public Vector4 realtimeLightmapScaleOffset;
		}

		public struct SubMeshInstance
		{
			public int meshInstanceID;

			public int vertexOffset;

			public int gameObjectInstanceID;

			public int subMeshIndex;

			public Matrix4x4 transform;
		}
	}
}
