using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class ControllerColliderHit
	{
		internal CharacterController m_Controller;

		internal Collider m_Collider;

		internal Vector3 m_Point;

		internal Vector3 m_Normal;

		internal Vector3 m_MoveDirection;

		internal float m_MoveLength;

		internal int m_Push;

		public CharacterController controller
		{
			get
			{
				return this.m_Controller;
			}
		}

		public Collider collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return this.m_Collider.attachedRigidbody;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return this.m_Collider.gameObject;
			}
		}

		public Transform transform
		{
			get
			{
				return this.m_Collider.transform;
			}
		}

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

		public Vector3 moveDirection
		{
			get
			{
				return this.m_MoveDirection;
			}
		}

		public float moveLength
		{
			get
			{
				return this.m_MoveLength;
			}
		}

		private bool push
		{
			get
			{
				return this.m_Push != 0;
			}
			set
			{
				this.m_Push = ((!value) ? 0 : 1);
			}
		}
	}
}
