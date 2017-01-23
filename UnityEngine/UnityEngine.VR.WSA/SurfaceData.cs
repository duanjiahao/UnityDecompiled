using System;

namespace UnityEngine.VR.WSA
{
	public struct SurfaceData
	{
		public SurfaceId id;

		public MeshFilter outputMesh;

		public WorldAnchor outputAnchor;

		public MeshCollider outputCollider;

		public float trianglesPerCubicMeter;

		public bool bakeCollider;

		public SurfaceData(SurfaceId _id, MeshFilter _outputMesh, WorldAnchor _outputAnchor, MeshCollider _outputCollider, float _trianglesPerCubicMeter, bool _bakeCollider)
		{
			this.id = _id;
			this.outputMesh = _outputMesh;
			this.outputAnchor = _outputAnchor;
			this.outputCollider = _outputCollider;
			this.trianglesPerCubicMeter = _trianglesPerCubicMeter;
			this.bakeCollider = _bakeCollider;
		}
	}
}
