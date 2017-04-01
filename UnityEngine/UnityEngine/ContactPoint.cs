using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct ContactPoint
	{
		internal Vector3 m_Point;

		internal Vector3 m_Normal;

		internal int m_ThisColliderInstanceID;

		internal int m_OtherColliderInstanceID;

		internal float m_Separation;

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
				return ContactPoint.ColliderFromInstanceId(this.m_ThisColliderInstanceID);
			}
		}

		public Collider otherCollider
		{
			get
			{
				return ContactPoint.ColliderFromInstanceId(this.m_OtherColliderInstanceID);
			}
		}

		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Collider ColliderFromInstanceId(int instanceID);
	}
}
