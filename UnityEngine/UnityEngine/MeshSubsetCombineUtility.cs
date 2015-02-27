using System;
namespace UnityEngine
{
	internal class MeshSubsetCombineUtility
	{
		public struct MeshInstance
		{
			public int meshInstanceID;
			public Matrix4x4 transform;
			public Vector4 lightmapTilingOffset;
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
