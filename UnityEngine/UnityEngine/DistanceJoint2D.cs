using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class DistanceJoint2D : AnchoredJoint2D
	{
		public extern float distance
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool maxDistanceOnly
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 result;
			DistanceJoint2D.DistanceJoint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out result);
			return result;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DistanceJoint2D_CUSTOM_INTERNAL_GetReactionForce(DistanceJoint2D joint, float timeStep, out Vector2 value);
		public float GetReactionTorque(float timeStep)
		{
			return DistanceJoint2D.INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetReactionTorque(DistanceJoint2D self, float timeStep);
	}
}
