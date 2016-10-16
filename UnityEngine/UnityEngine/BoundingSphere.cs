using System;

namespace UnityEngine
{
	public struct BoundingSphere
	{
		public Vector3 position;

		public float radius;

		public BoundingSphere(Vector3 pos, float rad)
		{
			this.position = pos;
			this.radius = rad;
		}

		public BoundingSphere(Vector4 packedSphere)
		{
			this.position = new Vector3(packedSphere.x, packedSphere.y, packedSphere.z);
			this.radius = packedSphere.w;
		}
	}
}
