using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LODGroup : Component
	{
		public Vector3 localReferencePoint
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_localReferencePoint(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_localReferencePoint(ref value);
			}
		}

		public extern float size
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int lodCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LODFadeMode fadeMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool animateCrossFading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float crossFadeAnimationDuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_localReferencePoint(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_localReferencePoint(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateBounds();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern LOD[] GetLODs();

		[Obsolete("Use SetLODs instead.")]
		public void SetLODS(LOD[] lods)
		{
			this.SetLODs(lods);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLODs(LOD[] lods);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceLOD(int index);
	}
}
