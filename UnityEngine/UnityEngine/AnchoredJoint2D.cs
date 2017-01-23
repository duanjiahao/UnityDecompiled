using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class AnchoredJoint2D : Joint2D
	{
		public Vector2 anchor
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_anchor(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anchor(ref value);
			}
		}

		public Vector2 connectedAnchor
		{
			get
			{
				Vector2 result;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anchor(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anchor(ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_connectedAnchor(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_connectedAnchor(ref Vector2 value);
	}
}
