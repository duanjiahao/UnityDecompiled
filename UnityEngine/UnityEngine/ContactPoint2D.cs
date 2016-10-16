using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct ContactPoint2D
	{
		internal Vector2 m_Point;

		internal Vector2 m_Normal;

		internal Collider2D m_Collider;

		internal Collider2D m_OtherCollider;

		public Vector2 point
		{
			get
			{
				return this.m_Point;
			}
		}

		public Vector2 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public Collider2D collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Collider2D otherCollider
		{
			get
			{
				return this.m_OtherCollider;
			}
		}
	}
}
