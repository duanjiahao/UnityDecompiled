using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class GizmoInfo
	{
		[SerializeField]
		private Vector2 m_Point1;

		[SerializeField]
		private Vector2 m_Point2;

		[SerializeField]
		private Vector2 m_Center = new Vector2(0f, 0f);

		[SerializeField]
		private float m_Angle = 0f;

		[SerializeField]
		private float m_Length = 0.2f;

		[SerializeField]
		private Vector4 m_Plane;

		[SerializeField]
		private Vector4 m_PlaneOrtho;

		public Vector2 point1
		{
			get
			{
				return this.m_Point1;
			}
		}

		public Vector2 point2
		{
			get
			{
				return this.m_Point2;
			}
		}

		public Vector2 center
		{
			get
			{
				return this.m_Center;
			}
		}

		public float angle
		{
			get
			{
				return this.m_Angle;
			}
		}

		public float length
		{
			get
			{
				return this.m_Length;
			}
		}

		public Vector4 plane
		{
			get
			{
				return this.m_Plane;
			}
		}

		public Vector4 planeOrtho
		{
			get
			{
				return this.m_PlaneOrtho;
			}
		}

		public GizmoInfo()
		{
			this.Update(this.m_Center, this.m_Length, this.m_Angle);
		}

		private Vector4 Get2DPlane(Vector2 firstPoint, float angle)
		{
			Vector4 a = default(Vector4);
			angle %= 6.28318548f;
			Vector2 a2 = new Vector2(firstPoint.x + Mathf.Sin(angle), firstPoint.y + Mathf.Cos(angle));
			Vector2 vector = a2 - firstPoint;
			if ((double)Mathf.Abs(vector.x) < 1E-05)
			{
				a.Set(-1f, 0f, firstPoint.x, 0f);
				float d = (Mathf.Cos(angle) <= 0f) ? -1f : 1f;
				a *= d;
			}
			else
			{
				float num = vector.y / vector.x;
				a.Set(-num, 1f, -(firstPoint.y - num * firstPoint.x), 0f);
			}
			if (angle > 3.14159274f)
			{
				a = -a;
			}
			float d2 = Mathf.Sqrt(a.x * a.x + a.y * a.y);
			return a / d2;
		}

		public void Update(Vector2 point1, Vector2 point2)
		{
			this.m_Point1 = point1;
			this.m_Point2 = point2;
			this.m_Center = (point1 + point2) * 0.5f;
			this.m_Length = (point2 - point1).magnitude * 0.5f;
			Vector3 rhs = this.Get2DPlane(this.m_Center, 0f);
			float num = Vector3.Dot(new Vector3(point1.x, point1.y, 1f), rhs);
			this.m_Angle = 0.0174532924f * Vector2.Angle(new Vector2(0f, 1f), (point1 - point2).normalized);
			if (num > 0f)
			{
				this.m_Angle = 6.28318548f - this.m_Angle;
			}
			this.m_Plane = this.Get2DPlane(this.m_Center, this.m_Angle);
			this.m_PlaneOrtho = this.Get2DPlane(this.m_Center, this.m_Angle + 1.57079637f);
		}

		public void Update(Vector2 center, float length, float angle)
		{
			this.m_Center = center;
			this.m_Length = length;
			this.m_Angle = angle;
			this.m_Plane = this.Get2DPlane(this.m_Center, this.m_Angle);
			this.m_PlaneOrtho = this.Get2DPlane(this.m_Center, this.m_Angle + 1.57079637f);
			Vector2 a = new Vector2(this.m_PlaneOrtho.x, this.m_PlaneOrtho.y);
			this.m_Point1 = this.m_Center + a * this.m_Length;
			this.m_Point2 = this.m_Center - a * this.m_Length;
		}
	}
}
