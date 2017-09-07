using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public sealed class BoxCollider : Collider
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

		public Vector3 size
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_size(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_size(ref value);
			}
		}

		[Obsolete("use BoxCollider.size instead.")]
		public Vector3 extents
		{
			get
			{
				return this.size * 0.5f;
			}
			set
			{
				this.size = value * 2f;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_center(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_center(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_size(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_size(ref Vector3 value);
	}
}
