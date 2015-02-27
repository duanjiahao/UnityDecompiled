using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class MeshCollider : Collider
	{
		[Obsolete("mesh has been replaced with sharedMesh and will be deprecated")]
		public Mesh mesh
		{
			get
			{
				return this.sharedMesh;
			}
			set
			{
				this.sharedMesh = value;
			}
		}
		public extern Mesh sharedMesh
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool convex
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool smoothSphereCollisions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
