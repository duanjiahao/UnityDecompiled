using System;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public class Collision2D
	{
		internal Rigidbody2D m_Rigidbody;
		internal Collider2D m_Collider;
		internal ContactPoint2D[] m_Contacts;
		internal Vector2 m_RelativeVelocity;
		public Rigidbody2D rigidbody
		{
			get
			{
				return this.m_Rigidbody;
			}
		}
		public Collider2D collider
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
	}
}
