using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	public struct ParticleCollisionEvent
	{
		private Vector3 m_Intersection;

		private Vector3 m_Normal;

		private Vector3 m_Velocity;

		private int m_ColliderInstanceID;

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("collider property is deprecated. Use colliderComponent instead, which supports Collider and Collider2D components (UnityUpgradable) -> colliderComponent", true)]
		public Component collider
		{
			get
			{
				throw new InvalidOperationException("collider property is deprecated. Use colliderComponent instead, which supports Collider and Collider2D components");
			}
		}

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

		public Component colliderComponent
		{
			get
			{
				return ParticleCollisionEvent.InstanceIDToColliderComponent(this.m_ColliderInstanceID);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Component InstanceIDToColliderComponent(int instanceID);
	}
}
