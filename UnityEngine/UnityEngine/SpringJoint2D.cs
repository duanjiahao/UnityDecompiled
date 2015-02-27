using System;
using System.Runtime.CompilerServices;
namespace UnityEngine
{
	public sealed class SpringJoint2D : AnchoredJoint2D
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
		public extern float dampingRatio
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float frequency
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
			SpringJoint2D.SpringJoint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out result);
			return result;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SpringJoint2D_CUSTOM_INTERNAL_GetReactionForce(SpringJoint2D joint, float timeStep, out Vector2 value);
		public float GetReactionTorque(float timeStep)
		{
			return SpringJoint2D.INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetReactionTorque(SpringJoint2D self, float timeStep);
	}
}
