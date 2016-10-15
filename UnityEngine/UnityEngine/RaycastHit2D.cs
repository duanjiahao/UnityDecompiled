using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct RaycastHit2D
	{
		private Vector2 m_Centroid;

		private Vector2 m_Point;

		private Vector2 m_Normal;

		private float m_Distance;

		private float m_Fraction;

		private Collider2D m_Collider;

		public Vector2 centroid
		{
			get
			{
				return this.m_Centroid;
			}
			set
			{
				this.m_Centroid = value;
			}
		}

		public Vector2 point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		public Vector2 normal
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

		public float fraction
		{
			get
			{
				return this.m_Fraction;
			}
			set
			{
				this.m_Fraction = value;
			}
		}

		public Collider2D collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Rigidbody2D rigidbody
		{
			get
			{
				return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
			}
		}

		public Transform transform
		{
			get
			{
				Rigidbody2D rigidbody = this.rigidbody;
				if (rigidbody != null)
				{
					return rigidbody.transform;
				}
				if (this.collider != null)
				{
					return this.collider.transform;
				}
				return null;
			}
		}

		public int CompareTo(RaycastHit2D other)
		{
			if (this.collider == null)
			{
				return 1;
			}
			if (other.collider == null)
			{
				return -1;
			}
			return this.fraction.CompareTo(other.fraction);
		}

		public static implicit operator bool(RaycastHit2D hit)
		{
			return hit.collider != null;
		}
	}
}
