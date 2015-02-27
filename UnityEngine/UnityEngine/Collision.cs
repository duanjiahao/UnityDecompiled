using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public class Collision
	{
		internal Vector3 m_RelativeVelocity;
		internal Rigidbody m_Rigidbody;
		internal Collider m_Collider;
		internal ContactPoint[] m_Contacts;
		public Vector3 relativeVelocity
		{
			get
			{
				return this.m_RelativeVelocity;
			}
		}
		public Rigidbody rigidbody
		{
			get
			{
				return this.m_Rigidbody;
			}
		}
		public Collider collider
		{
			get
			{
				return this.m_Collider;
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
				return (!(this.m_Rigidbody != null)) ? this.m_Collider.gameObject : this.m_Rigidbody.gameObject;
			}
		}
		public ContactPoint[] contacts
		{
			get
			{
				return this.m_Contacts;
			}
		}
		[Obsolete("use Collision.relativeVelocity instead.")]
		public Vector3 impactForceSum
		{
			get
			{
				return this.relativeVelocity;
			}
		}
		[Obsolete("will always return zero.")]
		public Vector3 frictionForceSum
		{
			get
			{
				return Vector3.zero;
			}
		}
		[Obsolete("Please use Collision.rigidbody, Collision.transform or Collision.collider instead")]
		public Component other
		{
			get
			{
				return (!(this.m_Rigidbody != null)) ? this.m_Collider : this.m_Rigidbody;
			}
		}
		public virtual IEnumerator GetEnumerator()
		{
			return this.contacts.GetEnumerator();
		}
	}
}
