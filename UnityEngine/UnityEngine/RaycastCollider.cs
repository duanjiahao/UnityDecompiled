using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable")]
	public sealed class RaycastCollider : Collider
	{
		public Vector3 center
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_center(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_center(ref value);
			}
		}
		[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable")]
		public extern float length
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable"), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_center(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_center(ref Vector3 value);
	}
}
