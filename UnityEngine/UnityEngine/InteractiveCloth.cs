using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine
{
	public sealed class InteractiveCloth : Cloth
	{
		public extern Mesh mesh
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float friction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float density
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float pressure
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float collisionResponse
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float tearFactor
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float attachmentTearFactor
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float attachmentResponse
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool isTeared
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius, [DefaultValue("ForceMode.Force")] ForceMode mode)
		{
			InteractiveCloth.INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, radius, mode);
		}
		[ExcludeFromDocs]
		public void AddForceAtPosition(Vector3 force, Vector3 position, float radius)
		{
			ForceMode mode = ForceMode.Force;
			InteractiveCloth.INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, radius, mode);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddForceAtPosition(InteractiveCloth self, ref Vector3 force, ref Vector3 position, float radius, ForceMode mode);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AttachToCollider(Collider collider, [DefaultValue("false")] bool tearable, [DefaultValue("false")] bool twoWayInteraction);
		[ExcludeFromDocs]
		public void AttachToCollider(Collider collider, bool tearable)
		{
			bool twoWayInteraction = false;
			this.AttachToCollider(collider, tearable, twoWayInteraction);
		}
		[ExcludeFromDocs]
		public void AttachToCollider(Collider collider)
		{
			bool twoWayInteraction = false;
			bool tearable = false;
			this.AttachToCollider(collider, tearable, twoWayInteraction);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DetachFromCollider(Collider collider);
	}
}
