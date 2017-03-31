using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Collision2D
	{
		internal int m_Collider;

		internal int m_OtherCollider;

		internal int m_Rigidbody;

		internal int m_OtherRigidbody;

		internal ContactPoint2D[] m_Contacts;

		internal Vector2 m_RelativeVelocity;

		internal int m_Enabled;

		public Collider2D collider
		{
			get
			{
				return Physics2D.GetColliderFromInstanceID(this.m_Collider);
			}
		}

		public Collider2D otherCollider
		{
			get
			{
				return Physics2D.GetColliderFromInstanceID(this.m_OtherCollider);
			}
		}

		public Rigidbody2D rigidbody
		{
			get
			{
				return Physics2D.GetRigidbodyFromInstanceID(this.m_Rigidbody);
			}
		}

		public Rigidbody2D otherRigidbody
		{
			get
			{
				return Physics2D.GetRigidbodyFromInstanceID(this.m_OtherRigidbody);
			}
		}

		public Transform transform
		{
			get
			{
				return (!(this.rigidbody != null)) ? this.collider.transform : this.rigidbody.transform;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return (!(this.rigidbody != null)) ? this.collider.gameObject : this.rigidbody.gameObject;
			}
		}

		public ContactPoint2D[] contacts
		{
			get
			{
				return this.m_Contacts;
			}
		}

		public Vector2 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_Enabled == 1;
			}
		}
	}
}
