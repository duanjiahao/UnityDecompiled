using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class BoxCollider2D : Collider2D
	{
		public Vector2 size
		{
			get
			{
				Vector2 result;
				this.INTERNAL_get_size(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_size(ref value);
			}
		}

		[Obsolete("BoxCollider2D.center has been deprecated. Use BoxCollider2D.offset instead (UnityUpgradable) -> offset", true)]
		public Vector2 center
		{
			get
			{
				return Vector2.zero;
			}
			set
			{
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_size(out Vector2 value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_size(ref Vector2 value);
	}
}
