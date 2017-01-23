using System;

namespace UnityEngine
{
	public struct Plane
	{
		private Vector3 m_Normal;

		private float m_Distance;

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = value;
			}
		}

		public Plane(Vector3 inNormal, Vector3 inPoint)
		{
			this.m_Normal = Vector3.Normalize(inNormal);
			this.m_Distance = -Vector3.Dot(inNormal, inPoint);
		}

		public Plane(Vector3 inNormal, float d)
		{
			this.m_Normal = Vector3.Normalize(inNormal);
			this.m_Distance = d;
		}

		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			this.m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			this.m_Distance = -Vector3.Dot(this.m_Normal, a);
		}

		public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
		{
			this.normal = Vector3.Normalize(inNormal);
			this.distance = -Vector3.Dot(inNormal, inPoint);
		}

		public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
		{
			this.normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			this.distance = -Vector3.Dot(this.normal, a);
		}

		public float GetDistanceToPoint(Vector3 inPt)
		{
			return Vector3.Dot(this.normal, inPt) + this.distance;
		}

		public bool GetSide(Vector3 inPt)
		{
			return Vector3.Dot(this.normal, inPt) + this.distance > 0f;
		}

		public bool SameSide(Vector3 inPt0, Vector3 inPt1)
		{
			float distanceToPoint = this.GetDistanceToPoint(inPt0);
			float distanceToPoint2 = this.GetDistanceToPoint(inPt1);
			return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
		}

		public bool Raycast(Ray ray, out float enter)
		{
			float num = Vector3.Dot(ray.direction, this.normal);
			float num2 = -Vector3.Dot(ray.origin, this.normal) - this.distance;
			bool result;
			if (Mathf.Approximately(num, 0f))
			{
				enter = 0f;
				result = false;
			}
			else
			{
				enter = num2 / num;
				result = (enter > 0f);
			}
			return result;
		}
	}
}
