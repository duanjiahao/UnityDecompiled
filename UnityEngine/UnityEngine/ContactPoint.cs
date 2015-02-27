using System;
namespace UnityEngine
{
	public struct ContactPoint
	{
		internal Vector3 m_Point;
		internal Vector3 m_Normal;
		internal Collider m_ThisCollider;
		internal Collider m_OtherCollider;
		public Vector3 point
		{
			get
			{
				return this.m_Point;
			}
		}
		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}
		public Collider thisCollider
		{
			get
			{
				return this.m_ThisCollider;
			}
		}
		public Collider otherCollider
		{
			get
			{
				return this.m_OtherCollider;
			}
		}
	}
}
