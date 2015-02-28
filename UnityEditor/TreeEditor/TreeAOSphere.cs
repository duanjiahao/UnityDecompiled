using System;
using UnityEngine;
namespace TreeEditor
{
	public class TreeAOSphere
	{
		public bool flag;
		public float area;
		public float radius;
		public float density = 1f;
		public Vector3 position;
		public TreeAOSphere(Vector3 pos, float radius, float density)
		{
			this.position = pos;
			this.radius = radius;
			this.area = radius * radius;
			this.density = density;
		}
		public float PointOcclusion(Vector3 pos, Vector3 nor)
		{
			Vector3 rhs = this.position - pos;
			float sqrMagnitude = rhs.sqrMagnitude;
			float num = Mathf.Max(0f, sqrMagnitude - this.area);
			if (sqrMagnitude > Mathf.Epsilon)
			{
				rhs.Normalize();
			}
			return (1f - 1f / Mathf.Sqrt(this.area / num + 1f)) * Mathf.Clamp01(4f * Vector3.Dot(nor, rhs));
		}
	}
}
