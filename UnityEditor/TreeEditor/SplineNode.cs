using System;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class SplineNode
	{
		public Vector3 point = Vector3.zero;
		public Quaternion rot = Quaternion.identity;
		public Vector3 normal = Vector3.zero;
		public Vector3 tangent = Vector3.zero;
		public float time;
		public SplineNode(Vector3 p, float t)
		{
			this.point = p;
			this.time = t;
		}
		public SplineNode(SplineNode o)
		{
			this.point = o.point;
			this.rot = o.rot;
			this.normal = o.normal;
			this.tangent = o.tangent;
			this.time = o.time;
		}
	}
}
