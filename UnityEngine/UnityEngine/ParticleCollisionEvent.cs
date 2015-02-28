using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public struct ParticleCollisionEvent
	{
		private Vector3 m_Intersection;
		private Vector3 m_Normal;
		private Vector3 m_Velocity;
		private int m_ColliderInstanceID;
		public Vector3 intersection
		{
			get
			{
				return this.m_Intersection;
			}
		}
		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}
		public Vector3 velocity
		{
			get
			{
				return this.m_Velocity;
			}
		}
		public Collider collider
		{
			get
			{
				return ParticleCollisionEvent.InstanceIDToCollider(this.m_ColliderInstanceID);
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider InstanceIDToCollider(int instanceID);
	}
}
