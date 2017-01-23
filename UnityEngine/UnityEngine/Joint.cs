using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Joint : Component
	{
		public extern Rigidbody connectedBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 axis
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_axis(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_axis(ref value);
			}
		}

		public Vector3 anchor
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_anchor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchor(ref value);
			}
		}

		public Vector3 connectedAnchor
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_connectedAnchor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_connectedAnchor(ref value);
			}
		}

		public extern bool autoConfigureConnectedAnchor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float breakTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableCollision
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enablePreprocessing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 currentForce
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_currentForce(out result);
				return result;
			}
		}

		public Vector3 currentTorque
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_currentTorque(out result);
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_axis(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_axis(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchor(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchor(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_connectedAnchor(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_currentForce(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_currentTorque(out Vector3 value);
	}
}
